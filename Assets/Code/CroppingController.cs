using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CroppingController : MonoBehaviour {
    private CharacterSerializer _characterSerializer;
    private ScrollController _scrollController;

    private GameObject _avatar;
    private float _avatarDepth;
    private bool _avatarBeingDragged = false;
    private Vector3 _dragStartMouseDifference;

	// Use this for initialization
	void Start () {
        this._characterSerializer = CharacterSerializer.Instance;
        switch (this._characterSerializer.Gender)
        {
            case Gender.Male:
                this._avatar = this.transform.Find("MaleAvatar").gameObject;
                this.transform.Find("FemaleAvatar").gameObject.SetActive(false);
                break;
            case Gender.Female:
            default:
                this._avatar = this.transform.Find("FemaleAvatar").gameObject;
                this.transform.Find("MaleAvatar").gameObject.SetActive(false);
                break;
        }

        this._avatarDepth = this._avatar.transform.position.z;
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

            // Find the magnitude of the vector (the distance) between the touches in each frame.
            float prevTouchDeltaMag = (touchOnePrevPos - touchTwoPrevPos).magnitude;
            float touchDeltaMag = (touchOne.position - touchTwo.position).magnitude;

            // Find the difference in the distances between each frame.
            float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

            this._avatar.transform.localScale = new Vector3(
                this._avatar.transform.localScale.x + (deltaMagnitudeDiff * 0.1f),
                this._avatar.transform.localScale.y + (deltaMagnitudeDiff * 0.1f),
                this._avatar.transform.localScale.z);
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this._avatar)
                {
                    this._avatarBeingDragged = true;
                    this._dragStartMouseDifference =
                        Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._avatar.transform.position;
                    if (this._scrollController)
                    {
                        this._scrollController.CanScroll = false;
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            this._avatarBeingDragged = false;
            if (this._scrollController)
            {
                this._scrollController.CanScroll = true;
            }
        }

        if (this._avatarBeingDragged)
        {
            var newAvatarPosition =
                Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._dragStartMouseDifference;
            newAvatarPosition.z = this._avatarDepth;
            this._avatar.transform.position = newAvatarPosition;
        }
    }

    public void SetScrollController(ScrollController scrollController)
    {
        this._scrollController = scrollController;
    }
}
