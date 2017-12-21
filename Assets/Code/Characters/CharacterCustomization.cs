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
        this.SetBodySprite(properties.gender, properties.bodySprite);
        this.SetFaceSprite(properties.gender, properties.faceSprite);
        this.SetEyeSprite(properties.gender, properties.eyeSprite);
        this.SetHairSprite(properties.gender, properties.hairSprite);
        this.SetSkinColor(properties.skinColor.GetColor());
        this.SetHairColor(properties.hairColor.GetColor());
        this.SetShirtColor(properties.shirtColor.GetColor());
        this.SetPantsColor(properties.pantsColor.GetColor());
    }

    public void SetBodySprite(Gender gender, string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> bodySprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    bodySprites = this._spriteController.FemaleBodySprites;
                    break;
                case Gender.Male:
                    bodySprites = this._spriteController.MaleBodySprites;
                    break;
            }
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
    public void SetLeftArmSprite(Gender gender, string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> leftArmSprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    leftArmSprites = this._spriteController.FemaleLeftArmSprites;
                    break;
                case Gender.Male:
                    leftArmSprites = this._spriteController.MaleLeftArmSprites;
                    break;
            }
            foreach (Sprite sprite in leftArmSprites)
            {
                if (sprite.name == spriteName)
                {
                    this.transform.Find("LeftArm").GetComponent<SpriteRenderer>().sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetRightArmSprite(Gender gender, string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> rightArmSprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    rightArmSprites = this._spriteController.FemaleRightArmSprites;
                    break;
                case Gender.Male:
                    rightArmSprites = this._spriteController.MaleRightArmSprites;
                    break;
            }
            foreach (Sprite sprite in rightArmSprites)
            {
                if (sprite.name == spriteName)
                {
                    this.transform.Find("RightArm").GetComponent<SpriteRenderer>().sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetFaceSprite(Gender gender, string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> faceSprites = new List<Sprite>();
            switch (gender)
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
    public void SetEyeSprite(Gender gender, string spriteName)
    {
        var head = this.transform.Find("Head");
        switch (gender)
        {
            case Gender.Female:
                foreach (var femaleEye in this._spriteController.FemaleEyeSprites)
                {
                    var femaleEyeObject = head.transform.Find(femaleEye);
                    if (femaleEyeObject)
                    {
                        femaleEyeObject.gameObject.SetActive(femaleEye == spriteName);
                    }
                }
                break;
            case Gender.Male:
                foreach (var maleEye in this._spriteController.MaleEyeSprites)
                {
                    var maleEyeObject = head.transform.Find(maleEye);
                    if (maleEyeObject)
                    {
                        maleEyeObject.gameObject.SetActive(maleEye == spriteName);
                    }
                }
                break;
        }
    }
    public void SetHairSprite(Gender gender, string spriteName)
    {
        if (this._spriteController)
        {
            List<Sprite> hairSprites = new List<Sprite>();
            switch (gender)
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
