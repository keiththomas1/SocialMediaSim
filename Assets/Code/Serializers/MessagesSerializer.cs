using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum DelaygramConversationType
{
	Story,
	Choice,
	Flirty
}

public enum DelaygramMessageType
{
    NPC,
    Player,
    Choice,
    Result
}

public class MessagesSerializer
{
    private static MessagesSerializer instance;
    private MessageSaveVariables currentSave;
    private string savePath;
    private bool hasBeenLoaded = false;

    public static MessagesSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MessagesSerializer();
            }
            return instance;
        }
    }

    private MessagesSerializer()
    {
        this.savePath = Application.persistentDataPath + "/MessageInfo.dat";
    }

    public List<Conversation> ActiveConversations
    {
        get { return this.currentSave.activeConversations; }
    }
    public Conversation? GetConversationByNPCName(string npcName)
    {
        foreach(Conversation convo in this.currentSave.activeConversations)
        {
            if (convo.npcName == npcName)
            {
                return convo;
            }
        }

        return null;
    }

    public void AddConversation(Conversation conversation)
    {
        this.currentSave.activeConversations.Add(conversation);
        this.SaveFile();
    }
    public void UpdateConversation(Conversation conversation)
    {
        var index = this.currentSave.activeConversations.FindIndex(c => c.npcName == conversation.npcName);
        if (index != -1)
        {
            this.currentSave.activeConversations[index] = conversation;
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
        FileStream file = File.Open(this.savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, this.currentSave);
            Debug.Log("Saved messages file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }

    public bool LoadGame()
    {
        if (this.hasBeenLoaded)
        {
            return true;
        }

        bool fileLoaded = false;
        if (File.Exists(this.savePath))
        {
            FileStream file = File.Open(this.savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                currentSave = (MessageSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this.savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            this.currentSave = new MessageSaveVariables();
            this.currentSave.lastUpdate = DateTime.Now;
            this.currentSave.activeConversations = new List<Conversation>();
            this.SaveFile();
        }

        this.hasBeenLoaded = true;
        return fileLoaded;
    }
}

[Serializable]
public struct MessageSaveVariables
{
    public DateTime lastUpdate;
    public List<Conversation> activeConversations;
}

[Serializable]
public struct Conversation
{
    public string npcName;
	public DelaygramConversationType conversationType;
    public CharacterProperties npcProperties;
    public List<Message> messages;
    public int choiceCount; // How many choices available (only supports one choice per convo right now)
    public List<int> choicesMade;
    public bool viewed;
}

[Serializable]
public struct Message
{
    public DelaygramMessageType type;
    public string text;
    public List<string> choices;
    public DateTime timeSent;
}