using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    private CharacterSerializer _characterSerializer;
    private CharacterSpriteCollection _spriteController;
    [SerializeField]
    private bool _loadFromSave;

    private void Awake()
    {
        this._spriteController = GameObject.Find("CONTROLLER").GetComponent<CharacterSpriteCollection>();

        this._characterSerializer = CharacterSerializer.Instance;
        if (this._loadFromSave)
        {
            this._characterSerializer.SetCharacterCustomization(this);
        }
        if (!this._characterSerializer.IsLoaded())
        {
            this._characterSerializer.LoadGame();
        }
        else
        {
            this._characterSerializer.UpdateAllCharacters();
        }
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetCharacterLook(CharacterProperties properties)
    {
        if (properties.gender == Gender.Female)
        {
            // SetBodySprite(properties.bodySprite);
        }
        this.SetFaceSprite(properties.faceSprite);
        this.SetHairSprite(properties.hairSprite);
        this.SetSkinColor(properties.skinColor.GetColor());
        this.SetHairColor(properties.hairColor.GetColor());
        this.SetShirtColor(properties.shirtColor.GetColor());
        this.SetPantsColor(properties.pantsColor.GetColor());
    }

    public void SetBodySprite(string spriteName)
    {
        if (this._spriteController)
        {
            var bodySprites = this._spriteController.BodySprites;
            foreach (Sprite sprite in bodySprites)
            {
                if (sprite.name == spriteName)
                {
                    this.transform.Find("Body").GetComponent<SpriteRenderer>().sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetFaceSprite(string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> faceSprites = new List<Sprite>();
            switch (this._characterSerializer.Gender)
            {
                case Gender.Female:
                    faceSprites = this._spriteController.FemaleFaceSprites;
                    break;
                case Gender.Male:
                    faceSprites = this._spriteController.MaleFaceSprites;
                    break;
            }
            foreach (Sprite sprite in faceSprites)
            {
                if (sprite.name == spriteName)
                {
                    var head = this.transform.Find("Head");
                    head.Find("Face").GetComponent<SpriteRenderer>().sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetHairSprite(string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> hairSprites = new List<Sprite>();
            switch (this._characterSerializer.Gender)
            {
                case Gender.Female:
                    hairSprites = this._spriteController.FemaleHairSprites;
                    break;
                case Gender.Male:
                    hairSprites = this._spriteController.MaleHairSprites;
                    break;
            }
            foreach (Sprite sprite in hairSprites)
            {
                if (sprite.name == spriteName)
                {
                    var head = this.transform.Find("Head");
                    head.Find("Hair").GetComponent<SpriteRenderer>().sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetSkinColor(Color color)
    {
        this.transform.Find("Head").GetComponent<SpriteRenderer>().color = color;
        this.transform.Find("LeftArm").GetComponent<SpriteRenderer>().color = color;
        this.transform.Find("RightArm").GetComponent<SpriteRenderer>().color = color;
    }
    public void SetHairColor(Color color)
    {
        this.transform.Find("Head").transform.Find("Hair").GetComponent<SpriteRenderer>().color = color;
    }
    public void SetShirtColor(Color color)
    {
        this.transform.Find("Body").GetComponent<SpriteRenderer>().color = color;
    }
    public void SetPantsColor(Color color)
    {
        this.transform.Find("LeftLeg").GetComponent<SpriteRenderer>().color = color;
        this.transform.Find("RightLeg").GetComponent<SpriteRenderer>().color = color;
    }
}
