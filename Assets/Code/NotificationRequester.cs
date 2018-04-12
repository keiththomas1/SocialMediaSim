using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
public class NotificationArrayJson
{
    public NotificationModelJsonReceive[] notificationModels;
}
[Serializable]
public struct NotificationModelJsonReceive
{
    public string _id;
    public string pictureId;
    public string otherUserId;
    public string userId;
    public bool liked;
    // public CharacterPropertiesModelJson characterProperties;
    public string createdDate;
}

public delegate void GetNotificationsCallback(NotificationArrayJson notifications, bool success);

public class NotificationRequester
{
#if UNITY_EDITOR
    public static readonly string SERVER_URL = "http://localhost:3000";
#else
    public static readonly string SERVER_URL = "http://13.59.159.27";
#endif

    private NotificationSerializer _notificationSerializer;

    public NotificationRequester()
    {
        this._notificationSerializer = NotificationSerializer.Instance;
    }

    public void RequestAllNotificationsForUser(string userId, GetNotificationsCallback finishCallback)
    {
        var route = String.Format(@"{0}/notifications/{1}", SERVER_URL, userId);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);

        this.MakeNotificationRequest(request, finishCallback);
    }

    private async Task<NotificationArrayJson> MakeNotificationRequest(HttpWebRequest request, GetNotificationsCallback finishCallback)
    {
        HttpWebResponse response = null;
        var sendRequest = new Action(() => {
            try
            {
                response = (HttpWebResponse)request.GetResponse();
            }
            catch (Exception exception)
            {
                Debug.Log("Bad/No response from server? Exception making web request:" + exception.ToString());
            }
        });
        try
        {
            await Task.Run(sendRequest);
        }
        catch (Exception exception)
        {
            Debug.Log("No internet? Exception making web request:" + exception.ToString());
        }

        if (response != null && response.StatusCode == HttpStatusCode.OK)
        {
            try
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                var responseBody = reader.ReadToEnd();

                string JSONToParse = "{\"notificationModels\":" + responseBody + "}";
                NotificationArrayJson notifications = JsonUtility.FromJson<NotificationArrayJson>(JSONToParse);
                if (notifications.notificationModels == null)
                {
                    notifications.notificationModels = new NotificationModelJsonReceive[0];
                }
                else
                {
                    // Add notifications to the save file
                    foreach (var notification in notifications.notificationModels)
                    {
                        this._notificationSerializer.AddNotification(notification);
                    }
                    this._notificationSerializer.SaveGame();
                }

                finishCallback(notifications, true);
                return notifications;
            }
            catch (Exception exception)
            {
                Debug.Log("Error reading/deserializing response stream: " + exception.ToString());
            }
        }
        else
        {
            var blankArray = new NotificationArrayJson();
            blankArray.notificationModels = new NotificationModelJsonReceive[0];
            finishCallback(blankArray, false);
        }

        return null;
    }
}
