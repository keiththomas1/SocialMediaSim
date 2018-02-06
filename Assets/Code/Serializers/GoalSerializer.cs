using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GoalSerializer
{
    private static GoalSerializer instance;
    private GoalSaveVariables _currentSave;
    private string _savePath;
    private bool _hasBeenLoaded = false;

    public static GoalSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GoalSerializer();
            }
            return instance;
        }
    }

    private GoalSerializer()
    {
        this._savePath = Application.persistentDataPath + "/GoalsInfo.dat";
    }

    public GoalInformation[] CurrentGoals
    {
        get { return this._currentSave.currentGoals; }
        set
        {
            this._currentSave.currentGoals = value;
            this.SaveGame();
        }
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
                this._currentSave = (GoalSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this._savePath);
                fileLoaded = true;
            }

            file.Close();
        }

        if (!fileLoaded)
        {
            this._currentSave = new GoalSaveVariables();
            this._currentSave.lastUpdate = DateTime.Now;
            this._currentSave.currentGoals = new GoalInformation[3];
            this._currentSave.currentGoals[0] = new GoalInformation(GoalStatus.Inactive);
            this._currentSave.currentGoals[1] = new GoalInformation(GoalStatus.Inactive);
            this._currentSave.currentGoals[2] = new GoalInformation(GoalStatus.Inactive);
            this.SaveGame();
        }

        this._hasBeenLoaded = true;
        return fileLoaded;
    }
}

[Serializable]
public class GoalInformation
{
    public DateTime nextGoalTime;
    public GoalStatus status;
    public GoalObjectType goalType;
    public string goalObject;
    public GoalRewardType rewardType;
    public string reward; // "30" for exp, "dog" for item
    public int stepsCompleted;
    public int stepsNeeded;

    public GoalInformation() { }

    public GoalInformation(GoalStatus _status)
    {
        status = _status;
    }
}

[Serializable]
public struct GoalSaveVariables
{
    public DateTime lastUpdate;
    public GoalInformation[] currentGoals;
    public DateTime nextGoalTime;
}