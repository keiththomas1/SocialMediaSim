using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum MessageType
{
    NPC,
    Player,
    Choice
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
        savePath = Application.persistentDataPath + "/MessageInfo.dat";
    }

    public List<Conversation> ActiveConversations
    {
        get { return currentSave.activeConversations; }
    }

    public void AddConversation(Conversation conversation)
    {
        currentSave.activeConversations.Insert(0, conversation);
        SaveFile();
    }

    public void SaveFile()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
    }

    public void SaveGameThread()
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, currentSave);
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
        if (hasBeenLoaded)
        {
            return true;
        }

        bool fileLoaded = false;
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                currentSave = (MessageSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            currentSave = new MessageSaveVariables();
            currentSave.lastUpdate = DateTime.Now;
            currentSave.activeConversations = new List<Conversation>();
            SaveFile();
        }

        hasBeenLoaded = true;
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
    public bool viewed;
    public string npcName;
    public CharacterProperties npcProperties;
    public int number;
    public List<Message> messages;
    public int currentPosition;
}

[Serializable]
public struct Message
{
    public MessageType type;
    public string text;
    public List<string> choices;
    public DateTime timeSent;
}