using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterEditor : MonoBehaviour {
    [SerializeField]
    private GameObject _randomPersonButton;
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
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.name == this._randomPersonButton.name)
                {
                    // var bodySprite = GetRandomBodySprite();
                    // this._characterCustomization.SetBodySprite(bodySprite);
                    // this._characterSerializer.BodySprite = bodySprite;

                    switch (this._characterSerializer.Gender)
                    {
                        case Gender.Male:
                            var maleFace = this._characterRandomization.GetRandomMaleFaceSprite();
                            this._characterSerializer.FaceSprite = maleFace;
                            break;
                        case Gender.Female:
                            var femaleFace = this._characterRandomization.GetRandomFemaleFaceSprite();
                            this._characterSerializer.FaceSprite = femaleFace;
                            break;
                    }

                    var skinColor = this._characterRandomization.GetRandomSkinColor();
                    this._characterSerializer.SkinColor = skinColor;

                    var hairColor = this._characterRandomization.GetRandomColor();
                    this._characterSerializer.HairColor = hairColor;

                    var shirtColor = this._characterRandomization.GetRandomColor();
                    this._characterSerializer.ShirtColor = shirtColor;

                    this._characterSerializer.SaveFile();
                }
            }
        }
    }
}
