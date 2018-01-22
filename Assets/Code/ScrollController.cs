using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    private GameObject scrollObject;
    private float scrollAreaTop;
    private float scrollAreaBottom;
    private bool scrollInitialized = false;
    private bool canScroll = true;

    private bool isScrolling;
    private const float SCROLL_DELAY = 0.2f;
    private float currentScrollSpeed;

    // For mouse position handling
    private float _previousMouseY;
    private float _currentMouseY;

    public delegate void ScrollCallback();

	// Use this for initialization
	void Start () {
	}

    public void UpdateScrollArea(GameObject scrollArea, float top, float bottom)
    {
        scrollObject = scrollArea;
        scrollAreaTop = top;
        scrollAreaBottom = bottom;

        scrollInitialized = true;
    }

    public void ScrollToPosition(float yPosition, ScrollCallback callback = null)
    {
        transform
            .DOLocalMoveY(yPosition, 0.8f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { callback(); });
    }
	
	// Update is called once per frame
    void Update()
    {
        if (scrollInitialized && canScroll)
        {
            this._previousMouseY = this._currentMouseY;
            this._currentMouseY = Input.mousePosition.y;

            // For case where user is clicking in whole new area, don't jump to it
            if (Input.GetMouseButtonDown(0))
            {
                this._previousMouseY = this._currentMouseY;
            }

            if (!isScrolling && Input.GetMouseButton(0))
            {
                isScrolling = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isScrolling = false;
            }

            // Only for development use
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scroll up
            {
                scrollObject.transform.Translate(new Vector2(0.0f, -20.0f * Time.deltaTime));
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll down
            {
                scrollObject.transform.Translate(new Vector2(0.0f, 20.0f * Time.deltaTime));
            }

            if (isScrolling) {
                var mouseDistance = this._previousMouseY - this._currentMouseY;
                var relativeMouseDistance = mouseDistance / Screen.height;
                this.currentScrollSpeed = relativeMouseDistance * 80.0f;
            } else { // Slow down the scroll speed incrementally
                this.currentScrollSpeed = this.currentScrollSpeed * 0.93f;
            }

            var finalScrollSpeed = -1 * Time.deltaTime * currentScrollSpeed;
            // If at the borders of the scroll range, reset, else continue scrolling
            if (this.currentScrollSpeed <= 0.0f && (transform.localPosition.y + finalScrollSpeed) > scrollAreaBottom)
            {
                var newPosition = transform.localPosition;
                newPosition.y = scrollAreaBottom;
                this.transform.localPosition = newPosition;
                this.currentScrollSpeed = 0.0f;
            }
            else if (this.currentScrollSpeed >= 0.0f && (transform.localPosition.y + finalScrollSpeed) < scrollAreaTop)
            {
                var newPosition = transform.localPosition;
                newPosition.y = scrollAreaTop;
                this.transform.localPosition = newPosition;
                this.currentScrollSpeed = 0.0f;
            }
            else
            {
                scrollObject.transform.Translate(0.0f, finalScrollSpeed, 0.0f);
            }
        }
	}

    public bool CanScroll
    {
        get { return this.canScroll; }
        set { this.canScroll = value; }
    }
   
}
