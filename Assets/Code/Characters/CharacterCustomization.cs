using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterCustomization : MonoBehaviour
{
    private CharacterSerializer _characterSerializer;
    private CharacterSpriteCollection _spriteController;

    [SerializeField]
    private bool _loadFromSave;
    private bool _customizationInitialized = false;

    private void Awake()
    {
        this._spriteController = GameObject.Find("CONTROLLER").GetComponent<CharacterSpriteCollection>();

        this._body = this.transform.Find("Body").GetComponent<SpriteRenderer>();
        if (this.transform.Find("Boobs"))
        {
            this._boobs = this.transform.Find("Boobs").GetComponent<SpriteRenderer>();
        }

        this._head = this.transform.Find("Head");
        this._face = this._head.transform.Find("Face").GetComponent<SpriteRenderer>();
        this._hair = this._head.transform.Find("Hair").GetComponent<SpriteRenderer>();
        this._noseMole = this._head.Find("NoseMole").GetComponent<SpriteRenderer>();
        this._faceBlotchDarkSpotRight = this._head.Find("FaceBlotchDarkSpotRight").GetComponent<SpriteRenderer>();
        this._faceBlotchRedSquigglyLeft = this._head.Find("FaceBlotchRedSquigglyLeft").GetComponent<SpriteRenderer>();
        this._faceBlotchRedSpotLeft = this._head.Find("FaceBlotchRedSpotLeft").GetComponent<SpriteRenderer>();

        this._leftArmTop = this.transform.Find("LeftArmTop").GetComponent<SpriteRenderer>();
        this._leftArmBottom = this._leftArmTop.transform.Find("LeftArmBottom").GetComponent<SpriteRenderer>();
        this._rightArmTop = this.transform.Find("RightArmTop").GetComponent<SpriteRenderer>();
        this._rightArmBottom = this._rightArmTop.transform.Find("RightArmBottom").GetComponent<SpriteRenderer>();

        this._leftLegTop = this.transform.Find("LeftLegTop").GetComponent<SpriteRenderer>();
        this._leftLegBottom = this._leftLegTop.transform.Find("LeftLegBottom").GetComponent<SpriteRenderer>();
        this._rightLegTop = this.transform.Find("RightLegTop").GetComponent<SpriteRenderer>();
        this._rightLegBottom = this._rightLegTop.transform.Find("RightLegBottom").GetComponent<SpriteRenderer>();
    }

    // Use this for initialization
    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        if (this._loadFromSave && this._characterSerializer.IsLoaded() && !this._customizationInitialized)
        {
            this.SetCharacterLook(this._characterSerializer.CurrentCharacterProperties);
            this._characterSerializer.SetCharacterCustomization(this);
        }
    }

    // Update is called once per frame
    void Update() {
    }

    // TODO: Cache the references to the transforms so you don't have to
    //       Find() them everytime. Will increase performance.
    public void SetCharacterLook(CharacterProperties properties)
    {
        this.SetHairSprite(properties.gender, properties.hairSprite);
        this.SetBodySprite(properties.gender, properties.fitnessLevel);
        this.SetArmSprites(properties.gender, properties.fitnessLevel);
        this.SetBirthmark(properties.birthmark);
        this.SetSkinColor(properties.skinColor.GetColor());
        this.SetHairColor(properties.hairColor.GetColor());
        this.SetShirtColor(properties.gender, properties.shirtColor.GetColor());
        this.SetPantsColor(properties.pantsColor.GetColor());
        this.SetHappinessLevel(properties.gender, properties.happinessLevel);
        this.SetSmelly(properties.smelly);

        this._customizationInitialized = true;
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

            if (startIndex >= armSprites.Count)
            {
                startIndex = armSprites.Count - 4;
            }

            this._leftArmTop.sprite = armSprites[startIndex];
            this._leftArmBottom.sprite = armSprites[startIndex + 1];
            this._rightArmTop.sprite = armSprites[startIndex + 2];
            this._rightArmBottom.sprite = armSprites[startIndex + 3];
        }
    }

    public void SetBirthmark(BirthMarkType birthmark)
    {
        this._noseMole.enabled =
            birthmark == BirthMarkType.NoseMole
            || birthmark == BirthMarkType.EyeMole;
        this._faceBlotchDarkSpotRight.enabled =
            birthmark == BirthMarkType.FaceBlotchDarkRight;
        this._faceBlotchRedSquigglyLeft.enabled =
            birthmark == BirthMarkType.FaceBlotchRedSquiggleLeft;
        this._faceBlotchRedSpotLeft.enabled =
            birthmark == BirthMarkType.FaceBlotchRedSpotLeft;
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

            if (faceSprites.Count < happinessLevel)
            {
                this._face.sprite = faceSprites[faceSprites.Count - 1];
            }
            else if (happinessLevel < 1)
            {
                this._face.sprite = faceSprites[0];
            }
            else
            {
                this._face.sprite = faceSprites[happinessLevel - 1];
            }
        }
    }
    public void SetSmelly(bool smelly)
    {
        var smellyAnimation = this.transform.Find("SmellyAnimation");
        smellyAnimation.gameObject.SetActive(smelly);
        smellyAnimation.GetComponent<ParticleSystem>().time = 10.0f;
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
                this._body.sprite = bodySprites[bodySprites.Count - 1];
            }
            else
            {
                this._body.sprite = bodySprites[fitnessLevel - 1];
            }
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
                    this._hair.sprite = sprite;
                    return;
                }
            }
        }
    }
    public void SetSkinColor(Color color)
    {
        this._head.GetComponent<SpriteRenderer>().color = color;

        this._leftArmTop.color = color;
        this._leftArmBottom.color = color;
        this._rightArmTop.color = color;
        this._rightArmBottom.color = color;
    }
    public void SetHairColor(Color color)
    {
        this._hair.GetComponent<SpriteRenderer>().color = color;
    }
    public void SetShirtColor(Gender gender, Color color)
    {
        this._body.color = color;
        if (this._boobs != null)
        {
            this._boobs.color = color;
        }
    }
    public void SetPantsColor(Color color)
    {
        this._leftLegTop.color = color;
        this._leftLegBottom.color = color;
        this._rightLegTop.color = color;
        this._rightLegBottom.color = color;
    }

    private SpriteRenderer _body = null;
    private SpriteRenderer _boobs = null;

    private Transform _head = null;
    private SpriteRenderer _face = null;
    private SpriteRenderer _hair = null;
    private SpriteRenderer _noseMole = null;
    private SpriteRenderer _faceBlotchDarkSpotRight = null;
    private SpriteRenderer _faceBlotchRedSquigglyLeft = null;
    private SpriteRenderer _faceBlotchRedSpotLeft = null;

    private SpriteRenderer _leftArmTop = null;
    private SpriteRenderer _leftArmBottom = null;
    private SpriteRenderer _rightArmTop = null;
    private SpriteRenderer _rightArmBottom = null;

    private SpriteRenderer _leftLegTop = null;
    private SpriteRenderer _leftLegBottom = null;
    private SpriteRenderer _rightLegTop = null;
    private SpriteRenderer _rightLegBottom = null;
}
