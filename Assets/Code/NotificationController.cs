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

    // Use this for initialization
    void Start () {
        this._messagesNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
    }

    // Update is called once per frame
    void Update () {
    }

    void OnDestroy()
    {
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
        }
    }
}
