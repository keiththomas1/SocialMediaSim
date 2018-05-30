using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    private CharacterSerializer _characterSerializer;
    private CharacterResourceCollection _spriteController;

    public SpriteRenderer _middleMole = null;
    public SpriteRenderer _middleBlotch = null;
    public SpriteRenderer _leftMole = null;
    public SpriteRenderer _leftBlotch = null;
    public SpriteRenderer _bottomMole = null;
    public SpriteRenderer _bottomBlotch = null;
    public SpriteRenderer _rightMole = null;
    public SpriteRenderer _rightBlotch = null;
    public SpriteRenderer _topMole = null;
    public SpriteRenderer _topBlotch = null;

    [SerializeField]
    private bool _loadFromSave;
    private bool _customizationInitialized = false;

    private void Awake()
    {
        this._spriteController = GameObject.Find("CONTROLLER").GetComponent<CharacterResourceCollection>();

        this._body = this.transform.Find("Body").GetComponent<SpriteRenderer>();
        if (this.transform.Find("Boobs"))
        {
            this._boobs = this.transform.Find("Boobs").GetComponent<SpriteRenderer>();
        }
        this._dirtMarks.Add(this._body.transform.Find("DirtMark1").gameObject);
        this._dirtMarks.Add(this._body.transform.Find("DirtMark2").gameObject);
        this._dirtMarks.Add(this._body.transform.Find("DirtMark3").gameObject);
        this._dirtMarks.Add(this._body.transform.Find("DirtMark4").gameObject);

        this._head = this.transform.Find("Head");
        this._face = this._head.transform.Find("Face").GetComponent<SpriteRenderer>();
        this._hair = this._head.transform.Find("Hair").GetComponent<SpriteRenderer>();

        this._leftArmTop = this.transform.Find("LeftArmTop").GetComponent<SpriteRenderer>();
        this._leftArmBottom = this._leftArmTop.transform.Find("LeftArmBottom").GetComponent<SpriteRenderer>();
        this._rightArmTop = this.transform.Find("RightArmTop").GetComponent<SpriteRenderer>();
        this._rightArmBottom = this._rightArmTop.transform.Find("RightArmBottom").GetComponent<SpriteRenderer>();

        this._leftLegTop = this.transform.Find("LeftLegTop").GetComponent<SpriteRenderer>();
        this._dirtMarks.Add(this._leftLegTop.transform.Find("DirtMark").gameObject);
        this._leftLegBottom = this._leftLegTop.transform.Find("LeftLegBottom").GetComponent<SpriteRenderer>();
        this._rightLegTop = this.transform.Find("RightLegTop").GetComponent<SpriteRenderer>();
        this._dirtMarks.Add(this._rightLegTop.transform.Find("DirtMark").gameObject);
        this._rightLegBottom = this._rightLegTop.transform.Find("RightLegBottom").GetComponent<SpriteRenderer>();
    }

    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        if (this._loadFromSave && this._characterSerializer.IsLoaded() && !this._customizationInitialized)
        {
            this.SetCharacterLook(this._characterSerializer.CurrentCharacterProperties);
            this._characterSerializer.SetCharacterCustomization(this);
        }
    }
    
    public void SetCharacterLook(CharacterProperties properties)
    {
        this.SetHairSprite(properties.gender, properties.hairSprite);
        this.SetBodySprite(properties.gender, properties.fitnessLevel);
        this.SetArmSprites(properties.gender, properties.fitnessLevel);
        this.SetLegSprites(properties.gender, properties.fitnessLevel);
        this.SetBirthmark(properties.birthmark);
        this.SetSkinColor(properties.skinColor.GetColor());
        this.SetHairColor(properties.hairColor.GetColor());
        this.SetShirtColor(properties.gender, properties.shirtColor.GetColor());
        this.SetPantsColor(properties.pantsColor.GetColor());
        this.SetHappinessLevel(
            properties.gender, properties.happinessLevel, properties.hygieneLevel);
        this.SetHygieneLevel(properties.hygieneLevel);

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

            var startIndex = 0;

            if (fitnessLevel >= 5)
            {
                startIndex = 8;
            }
            else if(fitnessLevel >= 3)
            {
                startIndex = 4;
            }

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
    public void SetLegSprites(Gender gender, int fitnessLevel)
    {
        if (this._spriteController)
        {
            List<Sprite> legSprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    legSprites = this._spriteController.FemaleLegSprites;
                    break;
                case Gender.Male:
                    legSprites = this._spriteController.MaleLegSprites;
                    break;
            }

            var startIndex = 0;

            if (fitnessLevel >= 5)
            {
                startIndex = 8;
            }
            else if (fitnessLevel >= 3)
            {
                startIndex = 4;
            }

            if (startIndex >= legSprites.Count)
            {
                startIndex = legSprites.Count - 4;
            }

            this._leftLegTop.sprite = legSprites[startIndex];
            this._leftLegBottom.sprite = legSprites[startIndex + 1];
            this._rightLegTop.sprite = legSprites[startIndex + 2];
            this._rightLegBottom.sprite = legSprites[startIndex + 3];
        }
    }

    public void SetBirthmark(BirthMarkType birthmark)
    {
        this._middleMole.gameObject.SetActive(birthmark == BirthMarkType.MiddleMole);
        this._middleBlotch.gameObject.SetActive(birthmark == BirthMarkType.MiddleBlotch);
        this._leftMole.gameObject.SetActive(birthmark == BirthMarkType.LeftMole);
        this._leftBlotch.gameObject.SetActive(birthmark == BirthMarkType.LeftBlotch);
        this._bottomMole.gameObject.SetActive(birthmark == BirthMarkType.BottomMole);
        this._bottomBlotch.gameObject.SetActive(birthmark == BirthMarkType.BottomBlotch);
        this._rightMole.gameObject.SetActive(birthmark == BirthMarkType.RightMole);
        this._rightBlotch.gameObject.SetActive(birthmark == BirthMarkType.RightBlotch);
        this._topMole.gameObject.SetActive(birthmark == BirthMarkType.TopMole);
        this._topBlotch.gameObject.SetActive(birthmark == BirthMarkType.TopBlotch);
    }

    public void SetHappinessLevel(Gender gender, int happinessLevel, int hygieneLevel)
    {
        if (this._spriteController)
        {
            List<Sprite> faceSprites = new List<Sprite>();
            switch (gender)
            {
                case Gender.Female:
                    faceSprites = (hygieneLevel > 4) ?
                        this._spriteController.FemaleWhiteFaceSprites
                        : this._spriteController.FemaleYellowFaceSprites;

                    if (happinessLevel > 3)
                    {
                        this.GetComponent<Animator>().runtimeAnimatorController =
                            this._spriteController._femaleAnimatorControllerLevel2;
                    }
                    break;
                case Gender.Male:
                    faceSprites = (hygieneLevel > 4) ?
                        this._spriteController.MaleWhiteFaceSprites
                        : this._spriteController.MaleYellowFaceSprites;

                    if (happinessLevel > 3)
                    {
                        this.GetComponent<Animator>().runtimeAnimatorController =
                            this._spriteController._maleAnimatorControllerLevel2;
                    }
                    break;
            }

            if (faceSprites.Count < happinessLevel)
            {
                this._face.sprite = faceSprites[faceSprites.Count - 1];
            }
            else
            {
                this._face.sprite = faceSprites[happinessLevel - 1];
            }
        }
    }

    public void SetHygieneLevel(int hygieneLevel)
    {
        if (hygieneLevel > 1)
        {
            var flyAnimation = this.transform.Find("FlyAnimation");
            flyAnimation.gameObject.SetActive(false);
        }
        if (hygieneLevel > 2)
        {
            var smellyAnimation = this.transform.Find("SmellyAnimation");
            smellyAnimation.gameObject.SetActive(false);
        }
        if (hygieneLevel > 3)
        {
            var acne = this._head.transform.Find("Acne");
            acne.gameObject.SetActive(false);
        }
        if (hygieneLevel > 4)
        {
            foreach (var dirtMark in this._dirtMarks)
            {
                dirtMark.SetActive(false);
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

    private SpriteRenderer _leftArmTop = null;
    private SpriteRenderer _leftArmBottom = null;
    private SpriteRenderer _rightArmTop = null;
    private SpriteRenderer _rightArmBottom = null;

    private SpriteRenderer _leftLegTop = null;
    private SpriteRenderer _leftLegBottom = null;
    private SpriteRenderer _rightLegTop = null;
    private SpriteRenderer _rightLegBottom = null;

    private List<GameObject> _dirtMarks =
        new List<GameObject>();
}
