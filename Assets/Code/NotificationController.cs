using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    [SerializeField]
    private GameObject _messagesNotificationBubble;
    // [SerializeField]
    // private GameObject _followersNotificationBubble;

    private UIController _uiController;

    private UserSerializer _userSerializer;
    private int _oldFollowerAmount = 0;
    private int _currentFollowerCount = 0;

    enum NotificationType
    {
        Message,
        Follower,
        Heart
    }

    // Use this for initialization
    void Start () {
        this._uiController = GetComponent<UIController>();

        this._userSerializer = UserSerializer.Instance;
        this._userSerializer.RegisterFollowersListener(this);
        this._oldFollowerAmount = _userSerializer.Followers;

        this._messagesNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
        // this._followersNotificationBubble.GetComponent<CanvasGroup>().alpha = 0.0f;
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
                this._currentFollowerCount = 0;
                break;
        }
    }

    public void OnFollowersUpdated(int newFollowerAmount)
    {
        CheckEvents(newFollowerAmount);

        this._oldFollowerAmount = newFollowerAmount;
    }

    public void NewPostEvent(DelayGramPost post)
    {
        // Could do things depending on post location, time, etc
        this.CreateNotificationBubble(NotificationType.Message, 1);
    }

    private void CheckEvents(int newFollowerAmount)
    {
        if (this._oldFollowerAmount < 5 && newFollowerAmount >= 5)
        {
            // this._messagesController.SpawnBoobJob1Message();
            if (this._uiController.GetCurrentPage() != Page.Messages)
            {
                this.CreateNotificationBubble(NotificationType.Message, 1);
            }
        }

        var followerDifference = newFollowerAmount - this._oldFollowerAmount;
        if (followerDifference > 0)
        {
            this._currentFollowerCount += followerDifference;
            this.CreateNotificationBubble(NotificationType.Follower, _currentFollowerCount);
        }
    }

    private void CreateNotificationBubble(NotificationType notificationType, int count)
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
