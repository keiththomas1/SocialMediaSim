using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterRandomization {
    private static CharacterRandomization instance;
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
        this._spriteCollection = GameObject.Find("CONTROLLER").GetComponent<CharacterSpriteCollection>();
        this._skinColors = new List<Color>();
        this.LoadSkinColors();
    }

    // private string GetRandomBodySprite()
    // {
    //     var bodySprites = this._characterCustomization.BodySprites;
    //     return bodySprites[Random.Range(0, bodySprites.Count - 1)].name;
    // }

    public string GetRandomMaleFaceSprite()
    {
        var faceSprites = this._spriteCollection.MaleFaceSprites;
        return faceSprites[Random.Range(0, faceSprites.Count - 1)].name;
    }
    public string GetRandomFemaleFaceSprite()
    {
        var faceSprites = this._spriteCollection.FemaleFaceSprites;
        return faceSprites[Random.Range(0, faceSprites.Count - 1)].name;
    }

    public Color GetRandomColor()
    {
        return new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
    }

    public Color GetRandomSkinColor()
    {
        var randomColor = Random.Range(0, this._skinColors.Count);
        return this._skinColors[randomColor];
    }

    private void LoadSkinColors()
    {
        // In increasing order of light to dark
        Color currentColor;
        ColorUtility.TryParseHtmlString("#FFF4D0FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#FFE89EFF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#EACE70FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#D2B656FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#BA9E40FF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#967E2FFF", out currentColor);
        this._skinColors.Add(currentColor);
        ColorUtility.TryParseHtmlString("#6D570FFF", out currentColor);
        this._skinColors.Add(currentColor);
    }
}
