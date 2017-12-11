using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum NotificationType
{
    Message,
    Follower,
    Heart
}

public class NotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject _messagesNotificationBubble;

    private UserSerializer _userSerializer;

    // Use this for initialization
    void Start () {
        this._userSerializer = UserSerializer.Instance;
        this._userSerializer.RegisterFollowersListener(this);

        this._messagesNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    // Update is called once per frame
    void Update () {
    }

    void OnDestroy()
    {
        _userSerializer.UnregisterFollowersListener(this);
    }

    public void ClearNotifications(Page currentPage)
    {
        switch (currentPage)
        {
            case Page.Messages:
                this._messagesNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
                break;
            case Page.Profile:
                //this._followersNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
                break;
        }
    }

    public void CreateNotificationBubble(NotificationType notificationType, int count)
    {
        switch (notificationType)
        {
            case NotificationType.Message:
                this._messagesNotificationBubble.GetComponent<CanvasGroup>().alpha = 1.0f;
                var messageCountText = this._messagesNotificationBubble.transform.Find("Count");
                messageCountText.GetComponent<TextMeshProUGUI>().text = count.ToString();
                break;
            case NotificationType.Follower:
                //this._followersNotificationBubble.GetComponent<CanvasGroup>().alpha = 1.0f;
                //var followerCountText = this._followersNotificationBubble.transform.Find("Count");
                //followerCountText.GetComponent<TextMeshProUGUI>().text = count.ToString();
                break;
        }
    }
}
