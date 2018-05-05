using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.Events;
using UnityEngine.UI;

public class NotificationScreenController : MonoBehaviour
{
    public class NotificationEvent : UnityEvent<int>
    {
    }
    public NotificationEvent NewNotificationsPulled = new NotificationEvent();

    [SerializeField]
    private GameObject _notificationPopup;
    private Transform _notificationPanel;

    private UserSerializer _userSerializer;
    private NotificationSerializer _notificationSerializer;
    private NotificationRequester _notificationRequester;
    private PostHelper _postHelper;

    private const float PullFrequencyInSeconds = 30.0f;
    private float _pullTimer = 0.0f;

    // Use this for initialization
    void Start()
    {
        this._userSerializer = UserSerializer.Instance;
        this._notificationSerializer = NotificationSerializer.Instance;
        this._notificationRequester = new NotificationRequester();
        this._postHelper = new PostHelper();

        var viewport = this._notificationPopup.transform.Find("Viewport");
        this._notificationPanel = viewport.transform.Find("NotificationPanel");
    }

    void Update()
    {
        if (this._pullTimer > 0.0f)
        {
            this._pullTimer -= Time.deltaTime;
            if (this._pullTimer <= 0.0f)
            {
                this.PullNotificationsAndSendEvent();
                this._pullTimer = PullFrequencyInSeconds;
            }
        }
    }

    public void HandleClick(string colliderName)
    {
        switch (colliderName)
        {
            default:
                break;
        }
    }

    public void EnterScreen()
    {
        this._notificationPopup.SetActive(true);
        this.PopulatePopupWithNotifications();
    }

    public void StartGatheringNotifications()
    {
        this._pullTimer = 1.0f;
    }

    public bool BackOut()
    {
        return true;
    }

    public void DestroyPage()
    {
        this._notificationPopup.SetActive(false);
    }

    private async void PullNotificationsAndSendEvent()
    {
        await this._notificationRequester.RequestAllNotificationsForUser(
            this._userSerializer.PlayerId,
            (NotificationArrayJson notifications, bool success) => {
                var newCount = this._notificationSerializer.GetNewNotificationCount();
                this.NewNotificationsPulled.Invoke(newCount);
            }
        );
    }

    private void PopulatePopupWithNotifications()
    {
        // Destroy all of the old notifications
        foreach(Transform child in this._notificationPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var notificationPairs = this._notificationSerializer.Notifications;
        // Sort the notifications by timestamp
        for(int i = (notificationPairs.Count - 1); i>=0; i--)
        {
            var notification = notificationPairs[i].Item1;
            if (notification.liked)
            {
                var notificationObject = GameObject.Instantiate(Resources.Load("UI/NotificationMessage") as GameObject);
                notificationObject.transform.SetParent(this._notificationPanel.transform);
                notificationObject.transform.localScale = new Vector3(1f, 1f, 1f);
                if (notificationPairs[i].Item2 == false)
                {
                    notificationObject.GetComponent<Image>().color = new Color(94f / 255f, 255f / 255f, 188f / 255f, 116f / 255f);
                }

                var nameText = notificationObject.transform.Find("NameText");
                nameText.GetComponent<TextMeshProUGUI>().text = notification.otherUserId;

                var timeText = notificationObject.transform.Find("TimeText");
                var timestamp = PostRequester.ParseDateTimeFromServer(notification.createdDate);
                var timeSincePost = DateTime.Now - timestamp;
                timeText.GetComponent<TextMeshProUGUI>().text = PostRequester.GetPostTimeFromTimeSpan(timeSincePost);

                var post = this._userSerializer.FindPost(notification.pictureId);
                if (post != null)
                {
                    var newPost = notificationObject.transform.Find("NewPost");
                    this._postHelper.SetPostDetails(newPost.gameObject, post, false, true);
                    this._postHelper.PopulatePostFromData(newPost.gameObject, post);
                }
                else
                {
                    // Show some default post
                }
            }
        }
        this._notificationSerializer.SetNotificationsViewed(notificationPairs);
    }
}
