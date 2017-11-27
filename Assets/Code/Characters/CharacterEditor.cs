using System.Collections;
using System.Collections.Generic;
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
        Debug.Log(colliderName);
        switch (colliderName)
        {
            case "RandomEverythingButton":
                this.RandomizeCharacter();
                break;
            case "RandomHairButton":
                this.RandomizeHair();
                break;
            case "RandomFaceButton":
                this.RandomizeFace();
                break;
            case "RandomSkinButton":
                this.RandomizeSkin();
                break;
            case "RandomShirtButton":
                this.RandomizeShirt();
                break;
            case "RandomPantsButton":
                this.RandomizePants();
                break;
        }
    }

    public void RandomizeCharacter()
    {
        // var bodySprite = GetRandomBodySprite();
        // this._characterCustomization.SetBodySprite(bodySprite);
        // this._characterSerializer.BodySprite = bodySprite;

        this.RandomizeFace();
        this.RandomizeHair();
        this.RandomizeShirt();
        this.RandomizeSkin();
        this.RandomizePants();
    }

    private void RandomizeFace()
    {
        switch (this._characterSerializer.Gender)
        {
            case Gender.Male:
                this._characterSerializer.FaceSprite = this._characterRandomization.GetRandomMaleFaceSprite();
                break;
            case Gender.Female:
                this._characterSerializer.FaceSprite = this._characterRandomization.GetRandomFemaleFaceSprite();
                break;
        }
    }

    private void RandomizeHair()
    {
        switch (this._characterSerializer.Gender)
        {
            case Gender.Male:
                this._characterSerializer.HairSprite = this._characterRandomization.GetRandomMaleHairSprite();
                break;
            case Gender.Female:
                this._characterSerializer.HairSprite = this._characterRandomization.GetRandomFemaleHairSprite();
                break;
        }
        var hairColor = this._characterRandomization.GetRandomColor();
        this._characterSerializer.HairColor = hairColor;
    }
    private void RandomizeSkin()
    {
        var skinColor = this._characterRandomization.GetRandomSkinColor();
        this._characterSerializer.SkinColor = skinColor;
    }
    private void RandomizeShirt()
    {
        Debug.Log("here");
        var shirtColor = this._characterRandomization.GetRandomColor();
        this._characterSerializer.ShirtColor = shirtColor;
    }
    private void RandomizePants()
    {

    }
}
