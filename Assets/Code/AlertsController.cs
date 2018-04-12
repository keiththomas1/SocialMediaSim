using TMPro;
using UnityEngine;
using DG.Tweening;

public enum NotificationType
{
    Message,
    Follower,
    Heart
}

public class AlertsController : MonoBehaviour
{
    [SerializeField]
    private GameObject _messagesNotificationBubble;
    [SerializeField]
    private TextMeshProUGUI _notificationText;

    // Use this for initialization
    void Start () {
        this._messagesNotificationBubble.SetActive(false);
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
                this._messagesNotificationBubble.SetActive(false);
                break;
            case Page.Profile:
                //this._followersNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
                break;
            case Page.Notifications:
                this._notificationText.text = "0";
                break;
        }
    }

    public void CreateNotificationBubble(NotificationType notificationType, int count)
    {
        switch (notificationType)
        {
            case NotificationType.Message:
                this._messagesNotificationBubble.SetActive(true);
                this._messagesNotificationBubble.transform
                    .DOPunchScale(new Vector3(1.1f, 1.1f), 1.0f, 5, 0.5f)
                    .OnComplete(() => this.StartMessageNotificationAnimation());
                var messageCountText = this._messagesNotificationBubble.transform.Find("Count");
                messageCountText.GetComponent<TextMeshProUGUI>().text = count.ToString();
                break;
        }
    }

    private void StartMessageNotificationAnimation()
    {
        if (this._messagesNotificationBubble.activeSelf)
        {
            // Weird graphical bug happens with this tween where the tween is not smooth
            // and then at completion (at 1.1f scale) the background of the notification disappears
            /*this._messagesNotificationBubble.transform
                .DOScale(new Vector3(1.1f, 1.1f), 1.5f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                    this._messagesNotificationBubble.transform
                        .DOScale(new Vector3(0.9f, 0.9f), 1.5f)
                        .SetEase(Ease.InSine)
                        .OnComplete(() => this.StartMessageNotificationAnimation() )); */
        }
    }
}
