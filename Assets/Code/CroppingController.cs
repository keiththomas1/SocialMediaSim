using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroppingController : MonoBehaviour {
    private ScrollController _scrollController;

    private List<GameObject> _movableObjects;
    private GameObject _currentObject = null;
    private float _currentObjectDepth;
    private bool _currentlyDragging = false;
    private Vector3 _dragStartMouseDifference;

    private GameObject _leftArrow;
    private GameObject _rightArrow;
    private GameObject _upArrow;
    private GameObject _downArrow;

    // Use this for initialization
    void Start () {
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
        if (currentTouchCount == 2)
        {
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
            Debug.Log(this._currentObject.transform.localRotation.eulerAngles.z);
            Debug.Log(this._currentObject.transform.rotation.eulerAngles.z);

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = touchDeltaMag - prevTouchDeltaMag;

            var scaleAmount = deltaMagnitudeDiff / 400;
            if (this._currentObject.transform.localScale.x <= 0.2 && scaleAmount < 0)
            {
                // Don't do anything
            } else {
                this._currentObject.transform.localScale = new Vector3(
                this._currentObject.transform.localScale.x + scaleAmount,
                this._currentObject.transform.localScale.y + scaleAmount,
                this._currentObject.transform.localScale.z);
            }   
        }
        // If left touch or tap
        if (Input.GetMouseButtonDown(0))
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
                        this._currentObjectDepth = this._currentObject.transform.position.z;
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
                            }
                        }

                        this._currentlyDragging = true;
                        this._dragStartMouseDifference =
                            Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._currentObject.transform.position;
                        if (this._scrollController)
                        {
                            this._scrollController.CanScroll = false;
                        }

                        break;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            if (this._currentObject)
            {
                this._currentlyDragging = false;
                if (this._scrollController)
                {
                    this._scrollController.CanScroll = true;
                }
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
            var newAvatarPosition =
                Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._dragStartMouseDifference;
            newAvatarPosition.z = this._currentObjectDepth;
            this._currentObject.transform.position = newAvatarPosition;

            var currentColor = this._leftArrow.GetComponent<SpriteRenderer>().color;
            if (currentColor.a > 0.0f)
            {
                currentColor.a -= 0.03f;
                this._leftArrow.GetComponent<SpriteRenderer>().color = currentColor;
                this._rightArrow.GetComponent<SpriteRenderer>().color = currentColor;
                this._upArrow.GetComponent<SpriteRenderer>().color = currentColor;
                this._downArrow.GetComponent<SpriteRenderer>().color = currentColor;
            }
        } else {
            if (this._currentObject)
            {
                var currentColor = this._leftArrow.GetComponent<SpriteRenderer>().color;
                if (currentColor.a < 1.0f)
                {
                    currentColor.a += 0.03f;
                    this._leftArrow.GetComponent<SpriteRenderer>().color = currentColor;
                    this._rightArrow.GetComponent<SpriteRenderer>().color = currentColor;
                    this._upArrow.GetComponent<SpriteRenderer>().color = currentColor;
                    this._downArrow.GetComponent<SpriteRenderer>().color = currentColor;
                }
            }
        }
    }

    public void SetMovableItems(List<GameObject> items)
    {
        this._movableObjects = items;
    }

    public void SetScrollController(ScrollController scrollController)
    {
        this._scrollController = scrollController;
    }
}
