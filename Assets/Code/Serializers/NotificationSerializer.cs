using UnityEngine;
using System;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class NotificationSerializer
{
    private static NotificationSerializer instance;
    private NotificationSaveVariables _currentSave;
    private string _savePath;
    private bool _hasBeenLoaded = false;

    public static NotificationSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NotificationSerializer();
            }
            return instance;
        }
    }

    private NotificationSerializer()
    {
        this._savePath = Application.persistentDataPath + "/NotificationInfo.dat";
    }

    public List<Tuple<NotificationModelJsonReceive, bool>> Notifications
    {
        get { return this._currentSave.notifications; }
        set
        {
            this._currentSave.notifications = value;
            this.SaveGame();
        }
    }
    public void AddNotification(NotificationModelJsonReceive newNotification, bool viewed)
    {
        this._currentSave.notifications.Add(
            new Tuple<NotificationModelJsonReceive, bool>(newNotification, viewed));
    }
    public void SetNotificationsViewed(List<Tuple<NotificationModelJsonReceive, bool>> viewedNotifications)
    {
        var currentNotifications = this.Notifications;
        for (int i=0; i < currentNotifications.Count; i++)
        {
            if (viewedNotifications.Contains(currentNotifications[i]))
            {
                currentNotifications[i] =
                    new Tuple<NotificationModelJsonReceive, bool>(
                        currentNotifications[i].Item1,
                        true);
            }
        }
        this.Notifications = currentNotifications;
    }
    public int GetNewNotificationCount()
    {
        var newCount = 0;
        foreach(var notification in this.Notifications)
        {
            if (notification.Item2 == false)
            {
                newCount++;
            }
        }
        return newCount;
    }

    public void SaveGame()
    {
        Thread oThread = new Thread(new ThreadStart(this.SaveGameThread));
        oThread.Start();
    }

    public void SaveGameThread()
    {
        FileStream file = File.Open(this._savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            this._currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, this._currentSave);
            Debug.Log("Saved notifications file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }

    public bool LoadGame()
    {
        if (this._hasBeenLoaded)
        {
            return true;
        }

        bool fileLoaded = false;
        if (File.Exists(this._savePath))
        {
            FileStream file = File.Open(this._savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                this._currentSave = (NotificationSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this._savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            this._currentSave = new NotificationSaveVariables();
            this._currentSave.lastUpdate = DateTime.Now;
            this._currentSave.notifications = new List<Tuple<NotificationModelJsonReceive, bool>>();
            this.SaveGame();
        }

        this._hasBeenLoaded = true;
        return fileLoaded;
    }
}

[Serializable]
public struct NotificationSaveVariables
{
    public DateTime lastUpdate;
    public List<Tuple<NotificationModelJsonReceive, bool>> notifications;
}