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

    public bool CreatedCharacter
    {
        get { return this._currentSave.storyProperties.createdCharacter; }
        set
        {
            this._currentSave.storyProperties.createdCharacter = value;
            this.SaveFile();

            if (value == true)
            {
                AnalyticsEvent.TutorialStep(1);
            }
        }
    }
    public bool PostedPhoto
    {
        get { return this._currentSave.storyProperties.postedPhoto; }
        set
        {
            this._currentSave.storyProperties.postedPhoto = value;
            this._currentSave.storyProperties.hasBeachBackground = true;
            this._currentSave.storyProperties.hasCityBackground= true;
            this._currentSave.storyProperties.hasParkBackground = true;
            this._currentSave.storyProperties.hasLouvreBackground = true;
            this._currentSave.storyProperties.hasCamRoomBackground = true;
            this._currentSave.storyProperties.hasYachtBackground = true;
            this.SaveFile();

            if (value == true)
            {
                AnalyticsEvent.TutorialStep(2);
            }
        }
    }
    public bool CompletedTutorial
    {
        get { return this._currentSave.storyProperties.completedTutorial; }
        set
        {
            this._currentSave.storyProperties.completedTutorial = value;
            this.SaveFile();

            if (value == true)
            {
                AnalyticsEvent.TutorialComplete();
            }
        }
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
            returnList = this._currentSave.posts;
            returnList.Reverse();
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
            this._currentSave.playerName = "Temp.Name";
            this._currentSave.playerId = "Temp.Name";
            this._currentSave.lastUpdate = DateTime.Now;

            this._currentSave.levelExperience = 0;

            this._currentSave.storyProperties = new StoryProperties();
            this._currentSave.storyProperties.createdCharacter = false;
            this._currentSave.storyProperties.postedPhoto = false;
            this._currentSave.storyProperties.completedTutorial = false;

            this._currentSave.storyProperties.hasBulldog = false;
            this._currentSave.storyProperties.hasCat = false;
            this._currentSave.storyProperties.hasDrone = false;

            this._currentSave.storyProperties.hasBeachBackground = false;
            this._currentSave.storyProperties.hasCityBackground = false;
            this._currentSave.storyProperties.hasParkBackground = false;
            this._currentSave.storyProperties.hasCamRoomBackground = false;
            this._currentSave.storyProperties.hasLouvreBackground = false;
            this._currentSave.storyProperties.hasYachtBackground = false;

            this._currentSave.posts = new List<DelayGramPost>();
            this._currentSave.notifications = new List<DelayGramNotification>();
            this._currentSave.followedIds = new List<string>();
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
    public bool createdCharacter;
    public bool postedPhoto;
    public bool completedTutorial;
    public bool hasBulldog;
    public bool hasCat;
    public bool hasDrone;
    public bool hasBeachBackground;
    public bool hasCityBackground;
    public bool hasParkBackground;
    public bool hasCamRoomBackground;
    public bool hasLouvreBackground;
    public bool hasYachtBackground;
}

// Can speed up in the future by turning bought items into a bool array.
[Serializable]
class UserSaveVariables
{
    public DateTime lastUpdate;
    public String playerName;
    public String playerId;

    public int levelExperience;

    public StoryProperties storyProperties;

    public List<DelayGramPost> posts;
    public List<DelayGramNotification> notifications;
    public List<string> followedIds;
    public DateTime nextPostTime;
}