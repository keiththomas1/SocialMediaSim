using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ScrollController : MonoBehaviour
{
    private const float SCREEN_SIZE = 5.0f;
    private float _scrollAreaBottom;
    private float _scrollAreaHeight;
    private bool _scrollInitialized = false;
    private bool _canScroll = true;

    private const float SCROLL_DELAY = 0.2f;
    private bool _isScrolling;
    private float _currentScrollSpeed;

    // For mouse position handling
    private float _previousMouseY;
    private float _currentMouseY;

    public delegate void ScrollCallback();

    // Use this for initialization
    private void Start()
    {
        // The current position is the "bottom" because we have to move in
        // the opposite direction to view the content
        this._scrollAreaBottom = this.transform.localPosition.y;
    }

    public void UpdateScrollArea(float height)
    {
        // We only want to scroll if the scroll height is larger than the screen size
        if (height <= SCREEN_SIZE)
        {
            this._scrollAreaHeight = 0.1f;
        }
        else
        {
            this._scrollAreaHeight = height - SCREEN_SIZE;
        }

        this._scrollInitialized = true;
    }

    public void ScrollToPosition(float yPosition, ScrollCallback callback = null)
    {
        // Since we scroll the parent object we have to scroll it to the opposite of the
        // content position to get to it.
        var scrollPosition = yPosition * -1;
        transform
            .DOLocalMoveY(scrollPosition, 0.8f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { callback(); });
    }

    public void ScrollToBottom(ScrollCallback callback = null)
    {
        var scrollPosition = this._scrollAreaBottom + this._scrollAreaHeight;
        transform
            .DOLocalMoveY(scrollPosition, 0.8f)
            .SetEase(Ease.OutSine)
            .OnComplete(() => { callback(); });
    }

    // Update is called once per frame
    void Update()
    {
        if (this._scrollInitialized && this._canScroll)
        {
            this._previousMouseY = this._currentMouseY;
            this._currentMouseY = Input.mousePosition.y;

            // For case where user is clicking in whole new area, don't jump to it
            if (Input.GetMouseButtonDown(0))
            {
                this._previousMouseY = this._currentMouseY;
            }

            if (!this._isScrolling && Input.GetMouseButton(0))
            {
                this._isScrolling = true;
            }

            if (Input.GetMouseButtonUp(0))
            {
                this._isScrolling = false;
            }

            // Only for development use
            if (Input.GetAxis("Mouse ScrollWheel") > 0) // Scroll up
            {
                this.transform.Translate(new Vector2(0.0f, -20.0f * Time.deltaTime));
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0) // Scroll down
            {
                this.transform.Translate(new Vector2(0.0f, 20.0f * Time.deltaTime));
            }

            if (this._isScrolling) {
                var mouseDistance = this._previousMouseY - this._currentMouseY;
                var relativeMouseDistance = mouseDistance / Screen.height;
                this._currentScrollSpeed = relativeMouseDistance * 80.0f;
            } else { // Slow down the scroll speed incrementally
                if (!Mathf.Approximately(this._currentScrollSpeed, 0.00f))
                {
                    this._currentScrollSpeed = this._currentScrollSpeed * 0.93f;
                }
            }

            var finalScrollSpeed = -1 * Time.deltaTime * this._currentScrollSpeed;
            var upcomingPosition = transform.localPosition.y + finalScrollSpeed;
            // The "top" is not original position of scrollController because we
            // are scrolling it in the opposite direction to get to view the content
            var scrollAreaTop = this._scrollAreaBottom + this._scrollAreaHeight;

            if (finalScrollSpeed < 0.0f && (upcomingPosition < this._scrollAreaBottom))
            {   // If we are scrolling and would scroll past the BOTTOM of the page, hard-set position
                var newPosition = transform.localPosition;
                newPosition.y = this._scrollAreaBottom;
                this.transform.localPosition = newPosition;
                this._currentScrollSpeed = 0.0f;
            }
            else if (finalScrollSpeed > 0.0f && (upcomingPosition > scrollAreaTop))
            {   // If we are scrolling and would scroll past the TOP of the page, hard-set position
                var newPosition = transform.localPosition;
                newPosition.y = scrollAreaTop;
                this.transform.localPosition = newPosition;
                this._currentScrollSpeed = 0.0f;
            }
            else
            {
                this.transform.Translate(0.0f, finalScrollSpeed, 0.0f);
            }
        }
	}

    public bool CanScroll
    {
        get { return this._canScroll; }
        set { this._canScroll = value; }
    }
   
}
