using UnityEngine;

public class CharacterEditor : MonoBehaviour {
    private CharacterSerializer _characterSerializer;
    private CharacterRandomization _characterRandomization;

    // Use this for initialization
    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        if (!this._characterSerializer.IsLoaded())
        {
            this._characterSerializer.LoadGame();
        }
        this._characterRandomization = CharacterRandomization.Instance;
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
                this._characterSerializer.Gender = Gender.Male;
                this.UpdateAvatarGender();
                this.RandomizeCharacter();
                break;
            case "FemaleButton":
                this._characterSerializer.Gender = Gender.Female;
                this.UpdateAvatarGender();
                this.RandomizeCharacter();
                break;
            case "RandomEverythingButton":
                this.RandomizeCharacter();
                break;
            case "RandomHairButton":
                switch (this._characterSerializer.Gender)
                {
                    case Gender.Male:
                        this._characterSerializer.HairSprite = this._characterRandomization.GetRandomMaleHairSprite(
                            this._characterSerializer.HairSprite);
                        break;
                    case Gender.Female:
                        this._characterSerializer.HairSprite = this._characterRandomization.GetRandomFemaleHairSprite(
                            this._characterSerializer.HairSprite);
                        break;
                }
                var hairColor = this._characterRandomization.GetRandomColor();
                this._characterSerializer.HairColor = hairColor;
                break;
            case "RandomFaceButton":
                switch (this._characterSerializer.Gender)
                {
                    case Gender.Male:
                        this._characterSerializer.FaceSprite = this._characterRandomization.GetRandomMaleFaceSprite(
                            this._characterSerializer.FaceSprite);
                        this._characterSerializer.EyeSprite = this._characterRandomization.GetRandomMaleEyeSprite(
                            this._characterSerializer.EyeSprite);
                        break;
                    case Gender.Female:
                        this._characterSerializer.FaceSprite = this._characterRandomization.GetRandomFemaleFaceSprite(
                            this._characterSerializer.FaceSprite);
                        this._characterSerializer.EyeSprite = this._characterRandomization.GetRandomFemaleEyeSprite(
                            this._characterSerializer.EyeSprite);
                        break;
                }
                break;
            case "RandomSkinButton":
                var skinColor = this._characterRandomization.GetRandomSkinColor(
                    this._characterSerializer.SkinColor);
                this._characterSerializer.SkinColor = skinColor;
                break;
            case "RandomShirtButton":
                var shirtColor = this._characterRandomization.GetRandomColor();
                this._characterSerializer.ShirtColor = shirtColor;
                break;
            case "RandomPantsButton":
                var pantsColor = this._characterRandomization.GetRandomColor();
                this._characterSerializer.PantsColor = pantsColor;
                break;
        }
    }

    public void RandomizeCharacter()
    {
        // var bodySprite = GetRandomBodySprite();
        // this._characterCustomization.SetBodySprite(bodySprite);
        // this._characterSerializer.BodySprite = bodySprite;

        var characterProperties = this._characterSerializer.CurrentCharacterProperties;
        switch (this._characterSerializer.Gender)
        {
            case Gender.Male:
                characterProperties.faceSprite = this._characterRandomization.GetRandomMaleFaceSprite();
                characterProperties.eyeSprite = this._characterRandomization.GetRandomMaleEyeSprite();
                break;
            case Gender.Female:
                characterProperties.faceSprite = this._characterRandomization.GetRandomFemaleFaceSprite();
                characterProperties.eyeSprite = this._characterRandomization.GetRandomFemaleEyeSprite();
                break;
        }
        switch (this._characterSerializer.Gender)
        {
            case Gender.Male:
                characterProperties.hairSprite = this._characterRandomization.GetRandomMaleHairSprite();
                break;
            case Gender.Female:
                characterProperties.hairSprite = this._characterRandomization.GetRandomFemaleHairSprite();
                break;
        }
        characterProperties.hairColor = new SerializableColor(this._characterRandomization.GetRandomColor());
        characterProperties.shirtColor = new SerializableColor(this._characterRandomization.GetRandomColor());
        characterProperties.skinColor = new SerializableColor(this._characterRandomization.GetRandomSkinColor(Color.cyan));
        characterProperties.pantsColor = new SerializableColor(this._characterRandomization.GetRandomColor());
        this._characterSerializer.CurrentCharacterProperties = characterProperties;
    }

    private void UpdateAvatarGender()
    {
        var maleAvatar = this.transform.Find("MaleAvatar");
        var femaleAvatar = this.transform.Find("FemaleAvatar");
        if (!maleAvatar || !femaleAvatar)
        {
            return;
        }
        var gender = this._characterSerializer.Gender;
        switch (gender)
        {
            case Gender.Female:
                femaleAvatar.gameObject.SetActive(true);
                maleAvatar.gameObject.SetActive(false);
                break;
            case Gender.Male:
                maleAvatar.gameObject.SetActive(true);
                femaleAvatar.gameObject.SetActive(false);
                break;
        }
    }
}
