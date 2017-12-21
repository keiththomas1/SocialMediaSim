using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GlobalVars
{
    private static GlobalVars instance;
    private PreferenceSaveVariables currentSave;
    private List<Delegate> _gameLoadedCallbacks;
    private string savePath;

    private GlobalVars() {
        this.savePath = Application.persistentDataPath + "/Preferences.dat";
        this._gameLoadedCallbacks = new List<Delegate>();
    }

    public static GlobalVars Instance
    {
        get {
            if (instance == null)
            {
                instance = new GlobalVars();
            }
            return instance;
        }
    }

    public void RegisterLoadedListener(Delegate callback)
    {
        this._gameLoadedCallbacks.Add(callback);
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

    public void SaveFile()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
    }

    private void SaveGameThread()
    {
        FileStream file = File.Open(savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            var timePlayedThisSession = DateTime.Now - currentSave.lastUpdate;
            currentSave.totalTimePlayed += timePlayedThisSession;
            currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, currentSave);
            Debug.Log("Saved global vars file");
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
        if (File.Exists(savePath))
        {
            FileStream file = File.Open(savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                currentSave = (PreferenceSaveVariables)bf.Deserialize(file);
                Debug.Log("Global vars loaded from " + savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            currentSave = new PreferenceSaveVariables();
            currentSave.totalTimePlayed = new TimeSpan(0);
            currentSave.playerName = "Click Me to Change";
            SaveFile();
        }


        currentSave.lastUpdate = DateTime.Now;
        return fileLoaded;
    }
}


[Serializable]
class PreferenceSaveVariables
{
    public DateTime lastUpdate;
    public TimeSpan totalTimePlayed;
    public string playerName;
}