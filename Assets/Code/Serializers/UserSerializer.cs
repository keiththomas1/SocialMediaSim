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

    public int LevelExperience
    {
        get { return this.currentSave.experienceProperties.levelExperience; }
        set
        {
            this.currentSave.experienceProperties.levelExperience = value;
            this.SaveFile();
        }
    }
    public int NeededLevelExperience
    {
        get { return this.currentSave.experienceProperties.neededLevelExperience; }
        set
        {
            this.currentSave.experienceProperties.neededLevelExperience = value;
            this.SaveFile();
        }
    }

    public DateTime LastUpdate
    {
        get { return this.currentSave.lastUpdate; }
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

    public bool CreatedCharacter
    {
        get { return this.currentSave.storyProperties.createdCharacter; }
        set
        {
            this.currentSave.storyProperties.createdCharacter = value;
            this.SaveFile();
        }
    }
    public bool PostedPhoto
    {
        get { return this.currentSave.storyProperties.postedPhoto; }
        set
        {
            this.currentSave.storyProperties.postedPhoto = value;
            this.SaveFile();
        }
    }
    public bool CompletedTutorial
    {
        get { return this.currentSave.storyProperties.completedTutorial; }
        set
        {
            this.currentSave.storyProperties.completedTutorial = value;
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
            this.currentSave.lastUpdate = DateTime.Now;

            this.currentSave.experienceProperties = new ExperienceProperties();
            this.currentSave.experienceProperties.levelExperience = 0;
            this.currentSave.experienceProperties.neededLevelExperience = 100;

            this.currentSave.storyProperties = new StoryProperties();
            this.currentSave.storyProperties.createdCharacter = false;
            this.currentSave.storyProperties.postedPhoto = false;
            this.currentSave.storyProperties.completedTutorial = false;
            this.currentSave.storyProperties.hasBulldog = false;
            this.currentSave.storyProperties.hasCat = false;
            this.currentSave.storyProperties.hasDrone = false;

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
    public bool createdCharacter;
    public bool postedPhoto;
    public bool completedTutorial;
    public bool hasBulldog;
    public bool hasCat;
    public bool hasDrone;
}

[Serializable]
public class ExperienceProperties
{
    public int levelExperience;
    public int neededLevelExperience;
}

// Can speed up in the future by turning bought items into a bool array.
[Serializable]
class UserSaveVariables
{
    public DateTime lastUpdate;
    public String playerName;

    public ExperienceProperties experienceProperties;

    public StoryProperties storyProperties;

    public List<DelayGramPost> posts;
    public List<DelayGramNotification> notifications;
    public DateTime nextPostTime;
}