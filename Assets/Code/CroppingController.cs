using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using TMPro;

public class CroppingController : MonoBehaviour {
    public UnityEvent OnAvatarMovedDecentDistance;
    public UnityEvent OnAvatarResizedAndRotated;

    private List<GameObject> _movableObjects;
    private GameObject _currentObject = null;
    private bool _currentlyDragging = false;
    private Vector3 _dragStartMouseDifference;
    private Vector3 _currentObjectStartPosition;
    private float _currentObjectStartRotation;
    private Vector3 _currentObjectStartScale;

    private GameObject _leftArrow;
    private GameObject _rightArrow;
    private GameObject _upArrow;
    private GameObject _downArrow;
    private GameObject _topLeftArrow;
    private GameObject _topRightArrow;
    private GameObject _bottomLeftArrow;
    private GameObject _bottomRightArrow;

    private const float HangDelay = 1f;
    private float _hangingTimer = 0f;

    private List<string> _poseStateNames = new List<string>()
    {
        "Standing",
        "RobotDancing",
        "WaveDancing"
    };
    private int _currentPoseIndex = 0;
    private Tweener _poseTextFadeTweener = null;

    // Use this for initialization
    void Start () {
        foreach(GameObject obj in this._movableObjects)
        {
            var components = obj.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var component in components)
            {
                switch (component.name)
                {
                    case "LeftArrow":
                    case "RightArrow":
                    case "TopArrow":
                    case "BottomArrow":
                    case "TopLeftArrow":
                    case "TopRightArrow":
                    case "BottomLeftArrow":
                    case "BottomRightArrow":
                        component.gameObject.SetActive(true);
                        break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        var currentTouchCount = 0;
        foreach (Touch touch in Input.touches)
        {
            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
            {
                currentTouchCount++;
            }
        }
        if (this._currentObject != null && currentTouchCount == 2)
        {
            this.HandleMultiTouch();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            this.HandleClick();
        }
        if (currentTouchCount == 0 && Input.GetMouseButtonUp(0))
        {
            this.HandleRelease();
        }
        if (this._currentlyDragging)
        {
            this.HandleDrag();
        }

        this.TickTimers();
    }

    public void SetMovableItems(List<GameObject> items)
    {
        this._movableObjects = items;
    }

    private void HandleMultiTouch()
    {
        this._currentlyDragging = false;

        // Store both touches.
        Touch touchOne = Input.GetTouch(0);
        Touch touchTwo = Input.GetTouch(1);

        // Find the position in the previous frame of each touch.
        Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
        Vector2 touchTwoPrevPos = touchTwo.position - touchTwo.deltaPosition;

        // Find angle and see if angle has changed
        float previousAngle = Mathf.Atan2(
            touchOnePrevPos.y - touchTwoPrevPos.y,
            touchOnePrevPos.x - touchTwoPrevPos.x);
        float newAngle = Mathf.Atan2(
            touchOne.position.y - touchTwo.position.y,
            touchOne.position.x - touchTwo.position.x);
        var rotation = Mathf.Rad2Deg * (newAngle - previousAngle);
        this._currentObject.transform.Rotate(0, 0, rotation);

        // Find the magnitude of the vector (the distance) between the touches in each frame.
        float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
        float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

        // Find the difference in the distances between each frame.
        float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

        var scaleAmount = deltaMagnitudeDiff / 450;
        if (this._currentObject.transform.localScale.x <= 0.2 && scaleAmount < 0)
        {
            // Don't do anything
        }
        else
        {
            this._currentObject.transform.localScale = new Vector3(
            this._currentObject.transform.localScale.x + scaleAmount,
            this._currentObject.transform.localScale.y + scaleAmount,
            this._currentObject.transform.localScale.z);
        }

        if (Mathf.Abs(Mathf.DeltaAngle(this._currentObject.transform.rotation.eulerAngles.z, this._currentObjectStartRotation)) > 10.0f
            && Vector2.Distance(this._currentObject.transform.localScale, this._currentObjectStartScale) > 0.2f)
        {
            this.OnAvatarResizedAndRotated.Invoke();
        }
    }

    private void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.name == "ApartmentCarpet")
            {
                hit.collider.GetComponent<SpriteRenderer>().color =
                    CharacterRandomization.GetRandomColor();
            }
            foreach (GameObject movable in this._movableObjects)
            {
                if (hit.collider.gameObject == movable)
                {
                    this._currentObject = hit.collider.gameObject;
                    this._currentObjectStartPosition = this._currentObject.transform.position;
                    this._currentObjectStartRotation = this._currentObject.transform.rotation.eulerAngles.z;
                    this._currentObjectStartScale = this._currentObject.transform.localScale;
                    this._currentObject.transform.SetAsLastSibling();

                    if (this._currentObject.GetComponent<Animator>())
                    {
                        this._hangingTimer = HangDelay;
                    }
                    else
                    {
                        var animators = this._currentObject.GetComponentsInChildren<Animator>();
                        if (animators.Length > 0)
                        {
                            foreach (Animator animator in animators)
                            {
                                animator.enabled = false;
                            }
                        }
                    }

                    this.RegisterArrowReferences(this._currentObject);

                    this._currentlyDragging = true;
                    this._dragStartMouseDifference =
                        Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._currentObject.transform.position;

                    break;
                }
            }
        }
    }

    private void HandleRelease()
    {
        if (this._currentObject)
        {
            this._currentlyDragging = false;
            if (this._currentObject.GetComponent<Animator>())
            {
                // If within a certain threshold, change pose state
                if (this._hangingTimer > (HangDelay - 0.2f))
                {
                    var oldPoseName = this._poseStateNames[this._currentPoseIndex];
                    this._currentPoseIndex =
                        ((this._currentPoseIndex + 1) >= this._poseStateNames.Count) ?
                        0 : this._currentPoseIndex + 1;

                    this._currentObject.GetComponent<Animator>().SetBool(oldPoseName, false);

                    // Set pose text to something and tween a fade out?
                    var poseTextTransform = this._currentObject.transform.Find("PoseText");
                    poseTextTransform.gameObject.SetActive(true);
                    var poseText = poseTextTransform.GetComponent<TextMeshPro>();
                    poseText.text = string.Format("{0}/{1}",
                        this._currentPoseIndex + 1,
                        this._poseStateNames.Count);
                    if (this._poseTextFadeTweener != null && this._poseTextFadeTweener.IsPlaying())
                    {
                        this._poseTextFadeTweener.Kill(false);
                    }
                    var currentColor = new Color32(0, 0, 0, 255);
                    poseText.color = currentColor;
                    currentColor.a = 0;
                    this._poseTextFadeTweener = ShortcutExtensionsTextMeshPro
                        .DOColor(poseText, currentColor, 2f)
                        .SetEase(Ease.OutQuad);
                }

                var currentPoseName = this._poseStateNames[this._currentPoseIndex];
                this._currentObject.GetComponent<Animator>().SetBool(currentPoseName, true);

                this._hangingTimer = 0.0f;
                this._currentObject.GetComponent<Animator>().SetBool("Hanging", false);
            }
            else
            {
                var animators = this._currentObject.GetComponentsInChildren<Animator>();
                if (animators.Length > 0)
                {
                    foreach (Animator animator in animators)
                    {
                        animator.enabled = true;
                    }
                }
            }
        }
    }

    private void HandleDrag()
    {
        var newPosition =
            Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._dragStartMouseDifference;
        this._currentObject.transform.position = newPosition;

        var directionColor = this._leftArrow.GetComponent<SpriteRenderer>().color;
        var scaleColor = this._topLeftArrow.GetComponent<SpriteRenderer>().color;
        if (directionColor.a > 0.0f)
        {
            directionColor.a -= 0.03f;
            scaleColor.a -= 0.03f;
            this._leftArrow.GetComponent<SpriteRenderer>().color = directionColor;
            this._rightArrow.GetComponent<SpriteRenderer>().color = directionColor;
            this._upArrow.GetComponent<SpriteRenderer>().color = directionColor;
            this._downArrow.GetComponent<SpriteRenderer>().color = directionColor;
            this._topLeftArrow.GetComponent<SpriteRenderer>().color = scaleColor;
            this._topRightArrow.GetComponent<SpriteRenderer>().color = scaleColor;
            this._bottomLeftArrow.GetComponent<SpriteRenderer>().color = scaleColor;
            this._bottomRightArrow.GetComponent<SpriteRenderer>().color = scaleColor;
        }

        if (Vector3.Distance(newPosition, this._currentObjectStartPosition) > 0.5f)
        {
            this.OnAvatarMovedDecentDistance.Invoke();
        }
    }

    private void RegisterArrowReferences(GameObject arrowParent)
    {
        var components = arrowParent.GetComponentsInChildren<SpriteRenderer>(true);
        foreach (var component in components)
        {
            switch (component.name)
            {
                case "LeftArrow":
                    this._leftArrow = component.gameObject;
                    break;
                case "RightArrow":
                    this._rightArrow = component.gameObject;
                    break;
                case "TopArrow":
                    this._upArrow = component.gameObject;
                    break;
                case "BottomArrow":
                    this._downArrow = component.gameObject;
                    break;
                case "TopLeftArrow":
                    this._topLeftArrow = component.gameObject;
                    break;
                case "TopRightArrow":
                    this._topRightArrow = component.gameObject;
                    break;
                case "BottomLeftArrow":
                    this._bottomLeftArrow = component.gameObject;
                    break;
                case "BottomRightArrow":
                    this._bottomRightArrow = component.gameObject;
                    break;
            }
        }
    }

    private void TickTimers()
    {
        if (this._hangingTimer > 0.0f)
        {
            this._hangingTimer -= Time.deltaTime;
            if (this._hangingTimer <= 0.0f)
            {
                var currentPoseName = this._poseStateNames[this._currentPoseIndex];
                this._currentObject.GetComponent<Animator>().SetBool(currentPoseName, false);
                // this._currentObject.GetComponent<Animator>().SetBool("Standing", false);
                this._currentObject.GetComponent<Animator>().SetBool("Hanging", true);
            }
        }
    }
}
