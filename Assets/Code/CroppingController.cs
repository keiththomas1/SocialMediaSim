using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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
        // If there are two touches on the device...
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
            } else {
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
        } else if (Input.GetMouseButtonDown(0)) // If left touch or tap
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                foreach(GameObject movable in this._movableObjects)
                {
                    if (hit.collider.gameObject == movable)
                    {
                        this._currentObject = hit.collider.gameObject;
                        this._currentObjectStartPosition = this._currentObject.transform.position;
                        this._currentObjectStartRotation = this._currentObject.transform.rotation.eulerAngles.z;
                        this._currentObjectStartScale = this._currentObject.transform.localScale;
                        this._currentObject.transform.SetAsLastSibling();
                        var animators = this._currentObject.GetComponentsInChildren<Animator>();
                        if (animators.Length > 0)
                        {
                            foreach(Animator animator in animators)
                            {
                                animator.enabled = false;
                            }
                        }

                        var components = this._currentObject.GetComponentsInChildren<SpriteRenderer>(true);
                        foreach (var component in components)
                        {
                            switch(component.name)
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

                        this._currentlyDragging = true;
                        this._dragStartMouseDifference =
                            Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._currentObject.transform.position;

                        break;
                    }
                }
            }
        }
        if (currentTouchCount == 0 && Input.GetMouseButtonUp(0))
        {
            if (this._currentObject)
            {
                this._currentlyDragging = false;
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

        if (this._currentlyDragging)
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
        } else {
            // If you want to fade the arrows back in
            /*if (this._currentObject)
            {
                var directionColor = this._leftArrow.GetComponent<SpriteRenderer>().color;
                var scaleColor = this._topLeftArrow.GetComponent<SpriteRenderer>().color;
                if (directionColor.a < 1.0f)
                {
                    directionColor.a += 0.03f;
                    scaleColor.a += 0.03f;
                    this._leftArrow.GetComponent<SpriteRenderer>().color = directionColor;
                    this._rightArrow.GetComponent<SpriteRenderer>().color = directionColor;
                    this._upArrow.GetComponent<SpriteRenderer>().color = directionColor;
                    this._downArrow.GetComponent<SpriteRenderer>().color = directionColor;
                    this._topLeftArrow.GetComponent<SpriteRenderer>().color = scaleColor;
                    this._topRightArrow.GetComponent<SpriteRenderer>().color = scaleColor;
                    this._bottomLeftArrow.GetComponent<SpriteRenderer>().color = scaleColor;
                    this._bottomRightArrow.GetComponent<SpriteRenderer>().color = scaleColor;
                }
            }*/
        }
    }

    public void SetMovableItems(List<GameObject> items)
    {
        this._movableObjects = items;
    }
}
