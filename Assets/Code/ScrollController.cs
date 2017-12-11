using UnityEngine;

public class ScrollController : MonoBehaviour
{
    private GameObject scrollObject;
    private float scrollAreaTop;
    private float scrollAreaBottom;
    private bool scrollInitialized = false;
    private bool canScroll = true;

    private bool isScrolling;
    private float maxScrollTimer;
    private float scrollTimer;
    private float currentScrollSpeed;

    // For mouse position handling
    private float prevMouseY, mouseY;

	// Use this for initialization
	void Start () {
        maxScrollTimer = 0.2f;
        scrollTimer = maxScrollTimer;
	}

    public void UpdateScrollArea(GameObject scrollArea, float top, float bottom)
    {
        scrollObject = scrollArea;
        scrollAreaTop = top;
        scrollAreaBottom = bottom;

        scrollInitialized = true;
    }

    public void ScrollToPosition(float yPosition)
    {
        this.currentScrollSpeed = (transform.position.y - yPosition) * 4;
    }
	
	// Update is called once per frame
    void Update()
    {
        if (scrollInitialized && canScroll)
        {
            prevMouseY = mouseY;
            mouseY = Input.mousePosition.y;

            if (!isScrolling && Input.GetMouseButton(0))
            {
                scrollTimer -= Time.deltaTime;
                if (scrollTimer <= 0.0f)
                {
                    isScrolling = true;
                }
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

            if (Input.GetMouseButtonUp(0))
            {
                scrollTimer = maxScrollTimer;
                isScrolling = false;
            }

            if (isScrolling) {
                this.currentScrollSpeed = (prevMouseY - mouseY) / 4.0f;
            } else {
                this.currentScrollSpeed = this.currentScrollSpeed * 0.97f;
            }

            if ((this.currentScrollSpeed <= 0.0f && transform.localPosition.y > scrollAreaBottom) ||
                (this.currentScrollSpeed >= 0.0f && transform.localPosition.y < scrollAreaTop))
            {
                this.currentScrollSpeed = 0.0f;
            }

            scrollObject.transform.Translate(0.0f, -1 * Time.deltaTime * currentScrollSpeed, 0.0f);
        }
	}

    public bool CanScroll
    {
        get { return this.canScroll; }
        set { this.canScroll = value; }
    }
   
}
