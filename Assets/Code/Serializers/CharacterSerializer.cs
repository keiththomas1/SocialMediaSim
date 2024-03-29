using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public enum LevelUpAttribute
{
    Happiness,
    Fitness,
    Hygiene,
    Nothing
}

public class CharacterSerializer
{
    private static CharacterSerializer instance;
    private List<AvatarController> _customizationListeners;
    private CharacterSaveVariables _currentSave;
    private string _savePath;
    private bool _fileLoaded;

    public static CharacterSerializer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterSerializer();
            }
            return instance;
        }
    }

    // Use this for initialization
    private CharacterSerializer() {
        this._customizationListeners = new List<AvatarController>();
        this._savePath = Application.persistentDataPath + "/Character.dat";
        this._fileLoaded = false;

        this.LoadGame();
    }

    public void SetCharacterCustomization(AvatarController characterCustomization)
    {
        if (!this._customizationListeners.Contains(characterCustomization))
        {
            this._customizationListeners.Add(characterCustomization);
        }
    }

    // Getters / Setters
    public Gender Gender
    {
        get
        {
            return this._currentSave.properties.gender;
        }
        set
        {
            this._currentSave.properties.gender = value;
            this.SaveFile();
        }
    }
    public string EyeSprite
    {
        get
        {
            return this._currentSave.properties.eyeSprite;
        }
        set
        {
            this._currentSave.properties.eyeSprite = value;
            this.SaveFile();
        }
    }
    public string HairSprite
    {
        get
        {
            return this._currentSave.properties.hairSprite;
        }
        set
        {
            this._currentSave.properties.hairSprite = value;
            this.SaveFile();
        }
    }
    public Color SkinColor
    {
        get
        {
            return this._currentSave.properties.skinColor.GetColor();
        }
        set
        {
            this._currentSave.properties.skinColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color HairColor
    {
        get
        {
            return this._currentSave.properties.hairColor.GetColor();
        }
        set
        {
            this._currentSave.properties.hairColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color ShirtColor
    {
        get
        {
            return this._currentSave.properties.hairColor.GetColor();
        }
        set
        {
            this._currentSave.properties.shirtColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color PantsColor
    {
        get
        {
            return this._currentSave.properties.pantsColor.GetColor();
        }
        set
        {
            this._currentSave.properties.pantsColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public int AvatarLevel
    {
        get { return this._currentSave.properties.avatarLevel; }
        set
        {
            this._currentSave.properties.avatarLevel = value;
            this.SaveFile();
        }
    }
    public int HappinessLevel
    {
        get { return this._currentSave.properties.happinessLevel; }
        set
        {
            this._currentSave.properties.happinessLevel = value;
            this.SaveFile();
        }
    }
    public int FitnessLevel
    {
        get { return this._currentSave.properties.fitnessLevel; }
        set
        {
            this._currentSave.properties.fitnessLevel = value;
            this.SaveFile();
        }
    }
    public int HygieneLevel
    {
        get { return this._currentSave.properties.hygieneLevel; }
        set
        {
            this._currentSave.properties.hygieneLevel = value;
            this.SaveFile();
        }
    }
    public BirthMarkType Birthmark
    {
        get { return this._currentSave.properties.birthmark; }
        set
        {
            this._currentSave.properties.birthmark = value;
            this.SaveFile();
        }
    }

    public CharacterProperties CurrentCharacterProperties
    {
        get
        {
            return this._currentSave.properties;
        }
        set
        {
            this._currentSave.properties = value;
            this._currentSave.initialized = true;
            this.SaveFile();
        }
    }

    public bool Initialized
    {
        get
        {
            return this._currentSave.initialized;
        }
    }

    public void UpdateAllCharacters()
    {
        foreach (AvatarController character in this._customizationListeners)
        {
            if (character)
            {
                character.SetCharacterLook(this._currentSave.properties);
            }
        }
    }

    public void SaveFile()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
        this.UpdateAllCharacters();
    }

    public bool IsLoaded()
    {
        return this._fileLoaded;
    }

    public bool LoadGame()
    {
        if (this._fileLoaded)
        {
            return true;
        }

        this._fileLoaded = true;
        bool loadSuccess = false;
        if (File.Exists(this._savePath))
        {
            FileStream file = File.Open(this._savePath, FileMode.Open);

            if (file.CanRead)
            {
                BinaryFormatter bf = new BinaryFormatter();
                this._currentSave = (CharacterSaveVariables)bf.Deserialize(file);
                Debug.Log("Save game loaded from " + this._savePath);
                loadSuccess = true;
            }

            file.Close();
        }

        if (!loadSuccess)
        {
            this._currentSave = new CharacterSaveVariables();
            this._currentSave.lastUpdate = DateTime.Now;
            this._currentSave.properties = new CharacterProperties();
            this._currentSave.cleanUpTime = DateTime.Now;
            this._currentSave.initialized = false;
            this.SaveFile();
        }

        this.UpdateAllCharacters();
        return loadSuccess;
    }

    private void SaveGameThread()
    {
        FileStream file = File.Open(this._savePath, FileMode.OpenOrCreate);

        if (file.CanWrite)
        {
            BinaryFormatter bf = new BinaryFormatter();
            this._currentSave.lastUpdate = DateTime.Now;
            bf.Serialize(file, this._currentSave);
            Debug.Log("Saved character file");
        }
        else
        {
            Debug.Log("Problem opening " + file.Name + " for writing");
        }

        file.Close();
    }
}

[Serializable]
public class SerializableColor
{
    public float red;
    public float green;
    public float blue;

    public SerializableColor(float r, float g, float b)
    {
        red = r; green = g; blue = b;
    }
    public SerializableColor(Color color)
    {
        red = color.r;
        green = color.g;
        blue = color.b;
    }
    public Color GetColor()
    {
        return new Color(red, green, blue);
    }
}

[Serializable]
public enum Gender
{
    Female = 1,
    Male = 2
}

[Serializable]
public enum BirthMarkType
{
    MiddleMole,
    MiddleBlotch,
    LeftMole,
    LeftBlotch,
    BottomMole,
    BottomBlotch,
    RightMole,
    RightBlotch,
    TopMole,
    TopBlotch,
    None
}

[Serializable]
public class CharacterProperties
{
    public Gender gender;
    public string hairSprite;
    public string eyeSprite;
    public BirthMarkType birthmark;
    public SerializableColor skinColor;
    public SerializableColor hairColor;
    public SerializableColor shirtColor;
    public SerializableColor pantsColor;
    public int avatarLevel;
    public int happinessLevel;
    public int fitnessLevel;
    public int hygieneLevel;

    public CharacterProperties()
    {
        gender = Gender.Female;
        birthmark = BirthMarkType.None;
        hairSprite = "";
        eyeSprite = "";
        skinColor = new SerializableColor(255, 255, 255);
        hairColor = new SerializableColor(255, 255, 255);
        shirtColor = new SerializableColor(255, 255, 255);
        pantsColor = new SerializableColor(255, 255, 255);
        avatarLevel = 16;
        happinessLevel = 5;
        fitnessLevel = 5;
        hygieneLevel = 5;
    }
    public CharacterProperties(CharacterProperties other)
    {
        gender = other.gender;
        birthmark = other.birthmark;
        eyeSprite = other.eyeSprite;
        hairSprite = other.hairSprite;
        skinColor = other.skinColor;
        hairColor = other.hairColor;
        shirtColor = other.shirtColor;
        pantsColor = other.pantsColor;
        avatarLevel = other.avatarLevel;
        happinessLevel = other.happinessLevel;
        fitnessLevel = other.fitnessLevel;
        hygieneLevel = other.hygieneLevel;
    }
}

[Serializable]
class CharacterSaveVariables
{
    public DateTime lastUpdate;
    public CharacterProperties properties;
    public DateTime cleanUpTime;
    public bool initialized;
}
