using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class NotificationScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject _notificationPopup;
    private Transform _notificationPanel;

    private UserSerializer _userSerializer;
    private NotificationSerializer _notificationSerializer;
    private NotificationRequester _notificationRequester;
    private PostHelper _postHelper;

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

        this._notificationRequester.RequestAllNotificationsForUser(
            this._userSerializer.PlayerId,
            (NotificationArrayJson notifications, bool success) => {
                if (success)
                {
                    this.PopulatePopupWithNotifications();
                }
            }
        );
    }

    public bool BackOut()
    {
        return true;
    }

    public void DestroyPage()
    {
        this._notificationPopup.SetActive(false);
    }

    private void PopulatePopupWithNotifications()
    {
        // Destroy all of the old notifications
        foreach(Transform child in this._notificationPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        var notifications = this._notificationSerializer.Notifications;
        // Sort the notifications by timestamp
        foreach(var notification in notifications)
        {
            if (notification.liked)
            {
                var notificationObject = GameObject.Instantiate(Resources.Load("UI/NotificationMessage") as GameObject);
                notificationObject.transform.SetParent(this._notificationPanel.transform);
                notificationObject.transform.localScale = new Vector3(1f, 1f, 1f);

                var nameText = notificationObject.transform.Find("NameText");
                nameText.GetComponent<TextMeshProUGUI>().text = notification.otherUserId;

                var timeText = notificationObject.transform.Find("TimeText");
                var timestamp = PostRequester.ParseDateTimeFromServer(notification.createdDate);
                var timeSincePost = DateTime.Now - timestamp;
                timeText.GetComponent<TextMeshProUGUI>().text = PostRequester.GetPostTimeFromTimeSpan(timeSincePost);

                var post = this._userSerializer.FindPost(notification.pictureId);
                if (post != null)
                {
                    Debug.Log("Found id .. trying to setup post");
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
    }
}
