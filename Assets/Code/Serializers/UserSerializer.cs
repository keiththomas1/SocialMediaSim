using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.Analytics;

public class UserSerializer
{
    private static UserSerializer instance;
    private UserSaveVariables _currentSave;

    // For saving/loading
    private string _savePath;

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
        this._savePath = Application.persistentDataPath + "/DelayGram.dat";
    }

    public string PlayerName
    {
        get { return _currentSave.playerName; }
        set
        {
            _currentSave.playerName = value;
            _currentSave.playerId = value; // TODO - make this an actual ID
            SaveFile();
        }
    }
    public string PlayerId
    {
        get { return _currentSave.playerId; }
        set
        {
            _currentSave.playerId = value;
            SaveFile();
        }
    }

    public int LevelExperience
    {
        get { return this._currentSave.levelExperience; }
        set
        {
            this._currentSave.levelExperience = value;
            this.SaveFile();
        }
    }

    public DateTime LastUpdate
    {
        get { return this._currentSave.lastUpdate; }
    }
    public DateTime NextPostTime
    {
        get { return this._currentSave.nextPostTime; }
        set
        {
            this._currentSave.nextPostTime = value;
            this.SaveFile();
        }
    }

    public TutorialState TutorialState
    {
        get { return this._currentSave.storyProperties.tutorialState; }
        set
        {
            this._currentSave.storyProperties.tutorialState = value;
            this.SaveFile();

            AnalyticsEvent.TutorialStep((int)value);
        }
    }
    public bool CreatedCharacter
    {
        get
        {
            return this._currentSave.storyProperties.tutorialState >= TutorialState.ProfileScreenAboutToPostPhoto;
        }
    }
    public bool PostedPhoto
    {
        get { return this._currentSave.storyProperties.tutorialState >= TutorialState.PostedFirstPhoto; }
    }
    public bool CompletedTutorial
    {
        get { return this._currentSave.storyProperties.tutorialState >= TutorialState.Finished; }
    }

    public bool HasBulldog
    {
        get { return this._currentSave.storyProperties.hasBulldog; }
        set
        {
            this._currentSave.storyProperties.hasBulldog = value;
            this.SaveFile();
        }
    }
    public bool HasCat
    {
        get { return this._currentSave.storyProperties.hasCat; }
        set
        {
            this._currentSave.storyProperties.hasCat = value;
            this.SaveFile();
        }
    }
    public bool HasDrone
    {
        get { return this._currentSave.storyProperties.hasDrone; }
        set
        {
            this._currentSave.storyProperties.hasDrone = value;
            this.SaveFile();
        }
    }
    public bool ApartmentEmpty
    {
        get { return this._currentSave.storyProperties.apartmentEmpty; }
        set
        {
            this._currentSave.storyProperties.apartmentEmpty = value;
            this.SaveFile();
        }
    }
    public bool HasBeachBackground
    {
        get { return this._currentSave.storyProperties.hasBeachBackground; }
        set
        {
            this._currentSave.storyProperties.hasBeachBackground = value;
            this.SaveFile();
        }
    }
    public bool HasCityBackground
    {
        get { return this._currentSave.storyProperties.hasCityBackground; }
        set
        {
            this._currentSave.storyProperties.hasCityBackground = value;
            this.SaveFile();
        }
    }
    public bool HasParkBackground
    {
        get { return this._currentSave.storyProperties.hasParkBackground; }
        set
        {
            this._currentSave.storyProperties.hasParkBackground = value;
            this.SaveFile();
        }
    }
    public bool HasCamRoomBackground
    {
        get { return this._currentSave.storyProperties.hasCamRoomBackground; }
        set
        {
            this._currentSave.storyProperties.hasCamRoomBackground = value;
            this.SaveFile();
        }
    }
    public bool HasLouvreBackground
    {
        get { return this._currentSave.storyProperties.hasLouvreBackground; }
        set
        {
            this._currentSave.storyProperties.hasLouvreBackground = value;
            this.SaveFile();
        }
    }
    public bool HasYachtBackground
    {
        get { return this._currentSave.storyProperties.hasYachtBackground; }
        set
        {
            this._currentSave.storyProperties.hasYachtBackground = value;
            this.SaveFile();
        }
    }
    public bool NotificationsEnabled
    {
        get { return this._currentSave.storyProperties.notificationsEnabled; }
        set
        {
            this._currentSave.storyProperties.notificationsEnabled = value;
            this.SaveFile();
        }
    }

    public int PostCount
    {
        get
        {
            return this._currentSave.posts.Count;
        }
    }

    public List<DelayGramPost> GetReverseChronologicalPosts()
    {
        List<DelayGramPost> returnList = new List<DelayGramPost>();
        if (this._currentSave.posts != null)
        {
            returnList = new List<DelayGramPost>(this._currentSave.posts);
            returnList.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        }
        return returnList;
    }

    public DelayGramPost FindPost(string pictureID)
    {
        Debug.Log("Looking for .. " + pictureID);
        foreach (var post in this._currentSave.posts)
        {
            Debug.Log(".. " + post.pictureID);
            if (post.pictureID == pictureID)
            {
                return post;
            }
        }

        return null;
    }

    public List<DelayGramNotification> GetNotifications()
    {
        List<DelayGramNotification> returnList = new List<DelayGramNotification>();
        if (this._currentSave.notifications != null)
        {
            returnList = this._currentSave.notifications;
            returnList.Reverse();
        }
        return returnList;
    }
    public List<string> GetFollowedIds()
    {
        List<string> returnList = new List<string>();
        if (this._currentSave.followedIds != null)
        {
            returnList = this._currentSave.followedIds;
        }
        return returnList;
    }

    public void SerializePost(DelayGramPost newPost)
    {
        if (this._currentSave.posts == null)
        {
            this._currentSave.posts = new List<DelayGramPost>();
        }
        this._currentSave.posts.Add(newPost);
        SaveFile();
    }

    public void SerializeNotification(DelayGramNotification newNotification)
    {
        if (this._currentSave.notifications == null)
        {
            this._currentSave.notifications = new List<DelayGramNotification>();
        }
        this._currentSave.notifications.Add(newNotification);
        this.SaveFile();
    }

    public void SerializePostCooldown(DateTime nextPostTime)
    {
        this._currentSave.nextPostTime = nextPostTime;
        this.SaveFile();
    }

    public void UpdatePost(DelayGramPost newPost, bool saveAfter)
    {
        for (int i = 0; i < this._currentSave.posts.Count; i++)
        {
            if (this._currentSave.posts[i].pictureID == newPost.pictureID)
            {
                this._currentSave.posts[i] = newPost;
            }
        }

        if (saveAfter)
        {
            this.SaveFile();
        }
    }

    public void SaveFile()
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
        if (File.Exists(this._savePath))
        {
            FileStream file = File.Open(this._savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                this._currentSave = (UserSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this._savePath);
                fileLoaded = true;

                AnalyticsEvent.Custom("Loaded game");
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            this._currentSave = new UserSaveVariables();
            this._currentSave.lastUpdate = DateTime.Now;
            this._currentSave.nextPostTime = DateTime.Now;
            this.SaveFile();

            AnalyticsEvent.TutorialStart();
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
    public string pictureID;
    public string playerID;
    public string playerName;
    public string backgroundName;
    public SerializableVector3 avatarPosition;
    public float avatarRotation;
    public float avatarScale;
    public string avatarPoseName;
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
    public TutorialState tutorialState = TutorialState.Introduction;

    public bool hasBulldog = false;
    public bool hasCat = false;
    public bool hasDrone = false;

    public bool apartmentEmpty = false;
    public bool hasBeachBackground = false;
    public bool hasCityBackground = false;
    public bool hasParkBackground = false;
    public bool hasCamRoomBackground = false;
    public bool hasLouvreBackground = false;
    public bool hasYachtBackground = false;

    public bool notificationsEnabled = false;
}

// Can speed up in the future by turning bought items into a bool array.
[Serializable]
public class UserSaveVariables
{
    public DateTime lastUpdate;
    public String playerName = "Temp.Name";
    public String playerId = "Temp.Name";

    public int levelExperience = 0;

    public StoryProperties storyProperties = new StoryProperties();

    public List<DelayGramPost> posts = new List<DelayGramPost>();
    public List<DelayGramNotification> notifications = new List<DelayGramNotification>();
    public List<string> followedIds = new List<string>();
    public DateTime nextPostTime;
}