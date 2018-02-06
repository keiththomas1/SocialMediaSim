using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRandomization {
    private static CharacterRandomization instance;
    private CharacterSerializer _characterSerializer;
    private CharacterSpriteCollection _spriteCollection;
    private List<Color> _skinColors;

    public static CharacterRandomization Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CharacterRandomization();
            }
            return instance;
        }
    }

    private CharacterRandomization()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        this._spriteCollection = GameObject.Find("CONTROLLER").GetComponent<CharacterSpriteCollection>();
        this._skinColors = new List<Color>();
        this.LoadSkinColors();

        if (!this._characterSerializer.Initialized)
        {
            this._characterSerializer.CurrentCharacterProperties = this.GetFullRandomCharacter();
        }
    }

    public CharacterProperties GetFullRandomCharacter(CharacterProperties oldProperties = null)
    {
        oldProperties = (oldProperties == null) ? new CharacterProperties() : oldProperties;
        var newProperties = oldProperties;
        if (Random.Range(0, 2) == 0)
        {
            newProperties.faceSprite = this.GetRandomMaleFaceSprite();
            newProperties.eyeSprite = this.GetRandomMaleEyeSprite();
            newProperties.hairSprite = this.GetRandomMaleHairSprite();
            newProperties.gender = Gender.Male;
        }
        else
        {
            newProperties.faceSprite = this.GetRandomFemaleFaceSprite();
            newProperties.eyeSprite = this.GetRandomFemaleEyeSprite();
            newProperties.hairSprite = this.GetRandomFemaleHairSprite();
            newProperties.gender = Gender.Female;
        }
        newProperties.hairColor = new SerializableColor(this.GetRandomColor());
        newProperties.shirtColor = new SerializableColor(this.GetRandomColor());
        newProperties.pantsColor = new SerializableColor(this.GetRandomColor());
        newProperties.skinColor = new SerializableColor(this.GetRandomSkinColor(Color.cyan));
        return newProperties;
    }

    public string GetRandomMaleFaceSprite(string oldSprite = "")
    {
        var faceSprites = this._spriteCollection.MaleFaceSprites;
        var finalSprite = oldSprite;
        if (faceSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = faceSprites[Random.Range(0, faceSprites.Count)].name;
            }
        }
        return faceSprites[0].name;
    }
    public string GetRandomFemaleFaceSprite(string oldSprite = "")
    {
        var faceSprites = this._spriteCollection.FemaleFaceSprites;
        var finalSprite = oldSprite;
        if (faceSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = faceSprites[Random.Range(0, faceSprites.Count)].name;
            }
        }
        return faceSprites[0].name;
    }

    public string GetRandomMaleEyeSprite(string oldSprite = "")
    {
        var eyeSprites = this._spriteCollection.MaleEyeSprites;
        var finalSprite = oldSprite;
        if (eyeSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = eyeSprites[Random.Range(0, eyeSprites.Count)];
            }
        }
        return eyeSprites[0];
    }
    public string GetRandomFemaleEyeSprite(string oldSprite = "")
    {
        var eyeSprites = this._spriteCollection.FemaleEyeSprites;
        var finalSprite = oldSprite;
        if (eyeSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = eyeSprites[Random.Range(0, eyeSprites.Count)];
            }
        }
        return eyeSprites[0];
    }

    public string GetRandomMaleHairSprite(string oldSprite = "")
    {
        var hairSprites = this._spriteCollection.MaleHairSprites;
        var finalSprite = oldSprite;
        if (hairSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = hairSprites[Random.Range(0, hairSprites.Count)].name;
            }
        }
        return hairSprites[0].name;
    }
    public string GetRandomFemaleHairSprite(string oldSprite = "")
    {
        var hairSprites = this._spriteCollection.FemaleHairSprites;
        var finalSprite = oldSprite;
        if (hairSprites.Count > 1)
        {
            while (finalSprite == oldSprite)
            {
                finalSprite = hairSprites[Random.Range(0, hairSprites.Count)].name;
            }
        }
        return hairSprites[0].name;
    }

    public string GetNextMaleHairSprite(string oldSprite = "")
    {
        var index = this._spriteCollection.MaleHairSprites.FindIndex(s => s.name == oldSprite);
        if (index != -1)
        {
            var nextIndex = this._spriteCollection.MaleHairSprites.Count == (index + 1) ? 0 : (index + 1);
            return this._spriteCollection.MaleHairSprites[nextIndex].name;
        }

        return this._spriteCollection.MaleHairSprites[0].name;
    }
    public string GetNextFemaleHairSprite(string oldSprite = "")
    {
        var index = this._spriteCollection.FemaleHairSprites.FindIndex(s => s.name == oldSprite);
        if (index != -1)
        {
            var nextIndex = this._spriteCollection.FemaleHairSprites.Count == (index + 1) ? 0 : (index + 1);
            return this._spriteCollection.FemaleHairSprites[nextIndex].name;
        }

        return this._spriteCollection.FemaleHairSprites[0].name;
    }

    public Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public Color GetRandomSkinColor(Color oldSkinColor)
    {
        var finalColor = oldSkinColor;
        if (this._skinColors.Count > 1)
        {
            while (finalColor == oldSkinColor)
            {
                finalColor = this._skinColors[Random.Range(0, this._skinColors.Count)];
            }
        }
        return finalColor;
    }

    public Color GetNextSkinColor(Color previousColor)
    {
        var index = this._skinColors.FindIndex(c => c == previousColor);
        if (index != -1) {
            var nextIndex = this._skinColors.Count == (index + 1) ? 0 : (index + 1);
            return this._skinColors[nextIndex];
        }

        return this._skinColors[0];
    }

    private void LoadSkinColors()
    {
        // In increasing order of light to dark
        Color currentColor;
        ColorUtility.TryParseHtmlString("#6D570FFF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#967E2FFF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#BA9E40FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#D2B656FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#EACE70FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#FFE89EFF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#FFF4D0FF", out currentColor);
        this._skinColors.Add(currentColor);
    }
}
