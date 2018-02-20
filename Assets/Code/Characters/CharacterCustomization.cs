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
    }

    // Use this for initialization
    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        if (this._loadFromSave && this._characterSerializer.IsLoaded())
        {
            this.SetCharacterLook(this._characterSerializer.CurrentCharacterProperties);
            this._characterSerializer.SetCharacterCustomization(this);
        }
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetCharacterLook(CharacterProperties properties)
    {
        this.SetEyeSprite(properties.gender, properties.eyeSprite);
        this.SetHairSprite(properties.gender, properties.hairSprite);
        this.SetBodySprite(properties.gender, properties.fitnessLevel);
        this.SetArmSprites(properties.gender, properties.fitnessLevel);
        this.SetSkinColor(properties.skinColor.GetColor());
        this.SetHairColor(properties.hairColor.GetColor());
        this.SetShirtColor(properties.gender, properties.shirtColor.GetColor());
        this.SetPantsColor(properties.pantsColor.GetColor());
        this.SetHappinessLevel(properties.gender, properties.happinessLevel);
    }
    
    public void SetArmSprites(Gender gender, int fitnessLevel)
    {
        if (this._spriteController)
        {
            List<Sprite> armSprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    armSprites = this._spriteController.FemaleArmSprites;
                    break;
                case Gender.Male:
                    armSprites = this._spriteController.MaleArmSprites;
                    break;
            }
            if (fitnessLevel > 2) fitnessLevel = 2;
            var startIndex = ((fitnessLevel - 1) * 4); // Level 1 is 0, Level 2 is 4, Level 3 is 8, etc

            var leftArmTop = this.transform.Find("LeftArmTop");
            leftArmTop.GetComponent<SpriteRenderer>().sprite = armSprites[startIndex];
            var leftArmBottom = leftArmTop.transform.Find("LeftArmBottom");
            leftArmBottom.GetComponent<SpriteRenderer>().sprite = armSprites[startIndex + 1];
            var rightArmTop = this.transform.Find("RightArmTop");
            rightArmTop.GetComponent<SpriteRenderer>().sprite = armSprites[startIndex + 2];
            var rightArmBottom = rightArmTop.transform.Find("RightArmBottom");
            rightArmBottom.GetComponent<SpriteRenderer>().sprite = armSprites[startIndex + 3];
        }
    }
    public void SetHappinessLevel(Gender gender, int happinessLevel)
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

            var head = this.transform.Find("Head");
            if (faceSprites.Count < happinessLevel)
            {
                head.Find("Face").GetComponent<SpriteRenderer>().sprite = faceSprites[faceSprites.Count - 1];
            }
            else if (happinessLevel < 1)
            {
                head.Find("Face").GetComponent<SpriteRenderer>().sprite = faceSprites[0];
            }
            else
            {
                head.Find("Face").GetComponent<SpriteRenderer>().sprite = faceSprites[happinessLevel - 1];
            }
        }
    }
    public void SetBodySprite(Gender gender, int fitnessLevel)
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

            // If there are less body sprites than the current fitness level
            if (bodySprites.Count < fitnessLevel)
            {
                this.transform.Find("Body").GetComponent<SpriteRenderer>().sprite = bodySprites[bodySprites.Count - 1];
            }
            else
            {
                this.transform.Find("Body").GetComponent<SpriteRenderer>().sprite = bodySprites[fitnessLevel - 1];
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
        var leftArm = this.transform.Find("LeftArmTop");
        leftArm.GetComponent<SpriteRenderer>().color = color;
        leftArm.transform.Find("LeftArmBottom").GetComponent<SpriteRenderer>().color = color;
        var rightArm = this.transform.Find("RightArmTop");
        rightArm.GetComponent<SpriteRenderer>().color = color;
        rightArm.transform.Find("RightArmBottom").GetComponent<SpriteRenderer>().color = color;
    }
    public void SetHairColor(Color color)
    {
        this.transform.Find("Head").transform.Find("Hair").GetComponent<SpriteRenderer>().color = color;
    }
    public void SetShirtColor(Gender gender, Color color)
    {
        this.transform.Find("Body").GetComponent<SpriteRenderer>().color = color;
        if (this.transform.Find("Boobs"))
        {
            this.transform.Find("Boobs").GetComponent<SpriteRenderer>().color = color;
        }
    }
    public void SetPantsColor(Color color)
    {
        var leftLeg = this.transform.Find("LeftLegTop");
        leftLeg.GetComponent<SpriteRenderer>().color = color;
        leftLeg.transform.Find("LeftLegBottom").GetComponent<SpriteRenderer>().color = color;
        var rightLeg = this.transform.Find("RightLegTop");
        rightLeg.GetComponent<SpriteRenderer>().color = color;
        rightLeg.transform.Find("RightLegBottom").GetComponent<SpriteRenderer>().color = color;
    }
}
