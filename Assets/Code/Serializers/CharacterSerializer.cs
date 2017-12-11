using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using UnityEngine;

public class CharacterSerializer
{
    private static CharacterSerializer instance;
    private List<CharacterCustomization> _customizationListeners;
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
        this._customizationListeners = new List<CharacterCustomization>();
        this._savePath = Application.persistentDataPath + "/Character.dat";
        this._fileLoaded = false;
    }

    public void SetCharacterCustomization(CharacterCustomization characterCustomization)
    {
        this._customizationListeners.Add(characterCustomization);
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
    public string BodySprite
    {
        set
        {
            this._currentSave.properties.bodySprite = value;
            this.SaveFile();
        }
    }
    public string FaceSprite
    {
        set
        {
            this._currentSave.properties.faceSprite = value;
            this.SaveFile();
        }
    }
    public string HairSprite
    {
        set
        {
            this._currentSave.properties.hairSprite = value;
            this.SaveFile();
        }
    }
    public Color SkinColor
    {
        set
        {
            this._currentSave.properties.skinColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color HairColor
    {
        set
        {
            this._currentSave.properties.hairColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color ShirtColor
    {
        set
        {
            this._currentSave.properties.shirtColor = new SerializableColor(value);
            this.SaveFile();
        }
    }
    public Color PantsColor
    {
        set
        {
            this._currentSave.properties.pantsColor = new SerializableColor(value);
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
            this.SaveFile();
        }
    }

    public void SaveFile()
    {
        Thread oThread = new Thread(new ThreadStart(SaveGameThread));
        oThread.Start();
        this.UpdateAllCharacters();
    }

    public void SaveGameThread()
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
            this.SaveFile();
        }

        this.UpdateAllCharacters();
        return loadSuccess;
    }

    public void UpdateAllCharacters()
    {
        foreach (CharacterCustomization character in this._customizationListeners)
        {
            if (character)
            {
                character.SetCharacterLook(this._currentSave.properties);
            }
        }
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
public class CharacterProperties
{
    public Gender gender;
    public string bodySprite;
    public string faceSprite;
    public string hairSprite;
    public SerializableColor skinColor;
    public SerializableColor hairColor;
    public SerializableColor shirtColor;
    public SerializableColor pantsColor;

    public CharacterProperties()
    {
        gender = Gender.Female;
        bodySprite = "DefaultBody";
        skinColor = new SerializableColor(255, 255, 255);
        hairColor = new SerializableColor(255, 255, 255);
        shirtColor = new SerializableColor(255, 255, 255);
        pantsColor = new SerializableColor(255, 255, 255);
    }
    public CharacterProperties(CharacterProperties other)
    {
        gender = other.gender;
        bodySprite = other.bodySprite;
        faceSprite = other.faceSprite;
        hairSprite = other.hairSprite;
        skinColor = other.skinColor;
        hairColor = other.hairColor;
        shirtColor = other.shirtColor;
        pantsColor = other.pantsColor;
    }
}

[Serializable]
class CharacterSaveVariables
{
    public DateTime lastUpdate;
    public CharacterProperties properties;
}
