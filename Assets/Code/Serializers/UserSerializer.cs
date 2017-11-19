using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UserSerializer
{
    private List<MonoBehaviour> followerListeners = new List<MonoBehaviour>();
    private static UserSerializer instance;
    private UserSaveVariables currentSave;

    // For saving/loading
    private string savePath;

    public static UserSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new UserSerializer();
            }
            return instance;
        }
    }

    private UserSerializer()
    {
        this.savePath = Application.persistentDataPath + "/DelayGram.dat";
    }

    public void RegisterFollowersListener(MonoBehaviour listener)
    {
        this.followerListeners.Add(listener);
    }

    public void UnregisterFollowersListener(MonoBehaviour listener)
    {
        this.followerListeners.Remove(listener);
    }

    public List<DelayGramPost> GetReverseChronologicalPosts()
    {
        List<DelayGramPost> returnList = new List<DelayGramPost>();
        if (this.currentSave.posts != null)
        {
            returnList = this.currentSave.posts;
            returnList.Reverse();
        }
        return returnList;
    }

    public List<DelayGramNotification> GetNotifications()
    {
        List<DelayGramNotification> returnList = new List<DelayGramNotification>();
        if (this.currentSave.notifications != null)
        {
            returnList = this.currentSave.notifications;
            returnList.Reverse();
        }
        return returnList;
    }

    public void AddFollowers(int followers)
    {
        this.currentSave.followers += followers;

        foreach (MonoBehaviour listener in this.followerListeners)
        {
            if (listener)
            {
                listener.BroadcastMessage("OnFollowersUpdated", this.currentSave.followers);
            } else {
                this.followerListeners.Remove(listener);
            }
        }

        this.SaveFile();
    }

    public int Followers
    {
        get { return this.currentSave.followers; }
    }

    public void AddEndorsement(string endorsement)
    {
        this.currentSave.endorsements.Add(endorsement);
        this.SaveFile();
    }

    public List<string> Endorsements
    {
        get { return this.currentSave.endorsements; }
    }

    public bool HasBulldog
    {
        get { return this.currentSave.storyProperties.hasBulldog; }
        set
        {
            this.currentSave.storyProperties.hasBulldog = value;
            this.SaveFile();
        }
    }

    public void SerializePost(DelayGramPost newPost)
    {
        if (this.currentSave.posts == null)
        {
            this.currentSave.posts = new List<DelayGramPost>();
        }
        this.currentSave.posts.Add(newPost);
        SaveFile();
    }

    public void SerializeNotification(DelayGramNotification newNotification)
    {
        if (this.currentSave.notifications == null)
        {
            this.currentSave.notifications = new List<DelayGramNotification>();
        }
        this.currentSave.notifications.Add(newNotification);
        this.SaveFile();
    }

    public DateTime NextPostTime
    {
        get { return this.currentSave.nextPostTime; }
        set
        {
            this.currentSave.nextPostTime = value;
            this.SaveFile();
        }
    }

    public void SerializePostCooldown(DateTime nextPostTime)
    {
        this.currentSave.nextPostTime = nextPostTime;
        this.SaveFile();
    }

    public void SaveFile()
    {
        Thread oThread = new Thread(new ThreadStart(this.SaveGameThread));
        oThread.Start();
    }

    public void SaveGameThread()
    {
        FileStream file = File.Open(this.savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            this.currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, this.currentSave);
            Debug.Log("Saved delay gram file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }

    public bool LoadGame()
    {
        bool fileLoaded = false;
        if (File.Exists(this.savePath))
        {
            FileStream file = File.Open(this.savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                this.currentSave = (UserSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this.savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            this.currentSave = new UserSaveVariables();
            this.currentSave.lastUpdate = DateTime.Now;
            this.currentSave.followers = 0;
            this.currentSave.storyProperties = new StoryProperties();
            this.currentSave.storyProperties.hasBulldog = false;
            this.currentSave.posts = new List<DelayGramPost>();
            this.currentSave.notifications = new List<DelayGramNotification>();
            this.currentSave.endorsements = new List<string>();
            this.currentSave.nextPostTime = DateTime.Now;
            this.SaveFile();
        }

        return fileLoaded;
    }
}

[Serializable]
public class SerializableVector3
{
    public float x;
    public float y;
    public float z;
    public SerializableVector3(Vector3 vector)
    {
        this.x = vector.x;
        this.y = vector.y;
        this.z = vector.z;
    }
}

[Serializable]
public class DelayGramPost
{
    public DateTime dateTime;
    public string imageID;
    public string playerName;
    public string backgroundName;
    public SerializableVector3 avatarPosition;
    public float avatarRotation;
    public float avatarScale;
    public CharacterProperties characterProperties;
    public int likes;
    public int dislikes;
    public List<PictureItem> items;
}

[Serializable]
public class DelayGramNotification
{
    public string text;
    public DateTime dateTime;
}

[Serializable]
public class StoryProperties
{
    public bool hasBulldog;
}

// Can speed up in the future by turning bought items into a bool array.
[Serializable]
class UserSaveVariables
{
    public DateTime lastUpdate;
    public int followers;
    public StoryProperties storyProperties;
    public List<DelayGramPost> posts;
    public List<DelayGramNotification> notifications;
    public List<string> endorsements;
    public DateTime nextPostTime;
}