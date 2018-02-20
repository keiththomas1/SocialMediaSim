using UnityEngine;

public class CharacterEditor : MonoBehaviour {
    private CharacterSerializer _characterSerializer;
    private CharacterRandomization _characterRandomization;

    private CharacterCustomization _femaleCustomization;
    private CharacterCustomization _maleCustomization;

    private CharacterProperties _currentProperties;

    // Use this for initialization
    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        if (!this._characterSerializer.IsLoaded())
        {
            this._characterSerializer.LoadGame();
        }
        this._characterRandomization = CharacterRandomization.Instance;

        var maleAvatar = this.transform.Find("MaleAvatar");
        var femaleAvatar = this.transform.Find("FemaleAvatar");
        if (maleAvatar && femaleAvatar)
        {
            this._maleCustomization = maleAvatar.GetComponent<CharacterCustomization>();
            this._femaleCustomization = femaleAvatar.GetComponent<CharacterCustomization>();
        }

        this._currentProperties = new CharacterProperties(this._characterSerializer.CurrentCharacterProperties);
    }
	
	// Update is called once per frame
	void Update ()
    {
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            case "MaleButton":
                if (this._currentProperties.gender != Gender.Male)
                {
                    this._currentProperties.gender = Gender.Male;
                    this.UpdateAvatarGender();
                }
                break;
            case "FemaleButton":
                if (this._currentProperties.gender != Gender.Female)
                {
                    this._currentProperties.gender = Gender.Female;
                    this.UpdateAvatarGender();
                }
                break;
            case "RandomEverythingButton":
                this.RandomizeCharacter();
                break;
            case "ColorHairButton":
                var hairColor = this._characterRandomization.GetRandomColor();
                this._currentProperties.hairColor = new SerializableColor(hairColor);
                break;
            case "RandomHairButton":
                this.RandomizeHair();
                break;
            case "RandomFaceButton":
                this.RandomizeEyes();
                break;
            case "ColorSkinButton":
                var skinColor = this._characterRandomization.GetNextSkinColor(
                    this._characterSerializer.SkinColor);
                this._currentProperties.skinColor = new SerializableColor(skinColor);
                break;
            case "ColorShirtButton":
                var shirtColor = this._characterRandomization.GetRandomColor();
                this._currentProperties.shirtColor = new SerializableColor(shirtColor);
                break;
            case "ColorPantsButton":
                var pantsColor = this._characterRandomization.GetRandomColor();
                this._currentProperties.pantsColor = new SerializableColor(pantsColor);
                break;
        }

        this.UpdateAvatars();
    }

    public void FinalizeCharacter()
    {
        this._characterSerializer.CurrentCharacterProperties = this._currentProperties;
    }

    private void RandomizeCharacter()
    {
        this.RandomizeEyes();
        this.RandomizeHair();
        this._currentProperties.hairColor = new SerializableColor(this._characterRandomization.GetRandomColor());
        this._currentProperties.shirtColor = new SerializableColor(this._characterRandomization.GetRandomColor());
        this._currentProperties.skinColor = new SerializableColor(this._characterRandomization.GetRandomSkinColor(Color.cyan));
        this._currentProperties.pantsColor = new SerializableColor(this._characterRandomization.GetRandomColor());
    }

    private void RandomizeHair()
    {
        switch (this._currentProperties.gender)
        {
            case Gender.Male:
                this._currentProperties.hairSprite = this._characterRandomization.GetNextMaleHairSprite(
                    this._currentProperties.hairSprite);
                break;
            case Gender.Female:
                this._currentProperties.hairSprite = this._characterRandomization.GetNextFemaleHairSprite(
                    this._currentProperties.hairSprite);
                break;
        }
    }

    private void RandomizeEyes()
    {
        switch (this._currentProperties.gender)
        {
            case Gender.Male:
                this._currentProperties.eyeSprite = this._characterRandomization.GetRandomMaleEyeSprite(
                    this._currentProperties.eyeSprite);
                break;
            case Gender.Female:
                this._currentProperties.eyeSprite = this._characterRandomization.GetRandomFemaleEyeSprite(
                    this._currentProperties.eyeSprite);
                break;
        }
    }

    private void UpdateAvatars()
    {
        if (this._maleCustomization)
        {
            this._maleCustomization.SetCharacterLook(this._currentProperties);
        }
        if (this._femaleCustomization)
        {
            this._femaleCustomization.SetCharacterLook(this._currentProperties);
        }
    }

    private void UpdateAvatarGender()
    {
        var maleAvatar = this.transform.Find("MaleAvatar");
        var femaleAvatar = this.transform.Find("FemaleAvatar");
        if (!maleAvatar || !femaleAvatar)
        {
            return;
        }

        femaleAvatar.gameObject.SetActive(this._currentProperties.gender == Gender.Female);
        maleAvatar.gameObject.SetActive(this._currentProperties.gender == Gender.Male);
        this.RandomizeHair();
        this.RandomizeEyes();
    }
}
