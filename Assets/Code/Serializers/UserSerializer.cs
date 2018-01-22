using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class UserSerializer
{
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

    public string PlayerName
    {
        get { return currentSave.playerName; }
        set
        {
            currentSave.playerName = value;
            SaveFile();
        }
    }

    public TimeSpan GetTotalTimePlayed()
    {
        var timePlayedThisSession = DateTime.Now - currentSave.lastUpdate;
        return currentSave.totalTimePlayed + timePlayedThisSession;
    }

    public int PlayerLevel
    {
        get { return this.currentSave.playerLevel; }
        set
        {
            this.currentSave.playerLevel = value;
            this.SaveFile();
        }
    }

    public int LevelExperience
    {
        get { return this.currentSave.levelExperience; }
        set
        {
            this.currentSave.levelExperience = value;
            this.SaveFile();
        }
    }

    public int NeededLevelExperience
    {
        get { return this.currentSave.neededLevelExperience; }
        set
        {
            this.currentSave.neededLevelExperience = value;
            this.SaveFile();
        }
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

    public bool CompletedTutorial
    {
        get { return this.currentSave.completedTutorial; }
        set
        {
            this.currentSave.completedTutorial = value;
            this.SaveFile();
        }
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
    public bool HasCat
    {
        get { return this.currentSave.storyProperties.hasCat; }
        set
        {
            this.currentSave.storyProperties.hasCat = value;
            this.SaveFile();
        }
    }
    public bool HasDrone
    {
        get { return this.currentSave.storyProperties.hasDrone; }
        set
        {
            this.currentSave.storyProperties.hasDrone = value;
            this.SaveFile();
        }
    }

    public int PostCount
    {
        get
        {
            return this.currentSave.posts.Count;
        }
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
            this.currentSave.playerName = "Temp.Name";
            this.currentSave.totalTimePlayed = new TimeSpan(0);
            this.currentSave.lastUpdate = DateTime.Now;

            this.currentSave.playerLevel = 1;
            this.currentSave.levelExperience = 0;
            this.currentSave.neededLevelExperience = 100;

            this.currentSave.storyProperties = new StoryProperties();
            this.currentSave.storyProperties.hasBulldog = false;
            this.currentSave.storyProperties.hasCat = false;
            this.currentSave.storyProperties.hasDrone = false;
            this.currentSave.completedTutorial = false;

            this.currentSave.posts = new List<DelayGramPost>();
            this.currentSave.notifications = new List<DelayGramNotification>();
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
    public bool hasCat;
    public bool hasDrone;
}

// Can speed up in the future by turning bought items into a bool array.
[Serializable]
class UserSaveVariables
{
    public DateTime lastUpdate;
    public string playerName;
    public TimeSpan totalTimePlayed;

    public int playerLevel;
    public int levelExperience;
    public int neededLevelExperience;

    public StoryProperties storyProperties;
    public bool completedTutorial;

    public List<DelayGramPost> posts;
    public List<DelayGramNotification> notifications;
    public DateTime nextPostTime;
}