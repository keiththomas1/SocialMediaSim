using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class CharacterEditor : MonoBehaviour {
    private CharacterSerializer _characterSerializer;
    private CharacterRandomization _characterRandomization;

    private AvatarController _femaleCustomization;
    private AvatarController _maleCustomization;

    private CharacterProperties _currentProperties;

    private GameObject _colorPickerParent = null;
    private GameObject _colorPickerObject = null;
    private ColorPicker _colorPicker = null;
    private Transform _avatarHair = null;
    private Transform _avatarBody = null;
    private Transform _avatarBoobs = null;
    private Transform _avatarPantsTopLeft = null;
    private Transform _avatarPantsBottomLeft = null;
    private Transform _avatarPantsTopRight = null;
    private Transform _avatarPantsBottomRight = null;

    // Use this for initialization
    void Awake ()
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
            this._maleCustomization = maleAvatar.GetComponent<AvatarController>();
            this._femaleCustomization = femaleAvatar.GetComponent<AvatarController>();
        }

        if (this._colorPickerParent == null)
        {
            this._colorPickerParent = GameObject.Instantiate(Resources.Load("UI/ColorPicker") as GameObject);
            var uiCanvas = GameObject.FindObjectOfType<Canvas>();
            this._colorPickerParent.transform.SetParent(uiCanvas.transform);
            this._colorPickerParent.GetComponent<RectTransform>().localPosition = new Vector3(-3f, 332f, 0f);

            this._colorPickerObject = this._colorPickerParent.transform.Find("PickerSource").gameObject;
            this._colorPicker = this._colorPickerObject.GetComponent<ColorPicker>();

            this._colorPickerParent.SetActive(false);
        }

        this._currentProperties = new CharacterProperties(this._characterSerializer.CurrentCharacterProperties);
    }

    private void Start()
    {
        this.UpdateAvatarGender();
    }

    private void OnDestroy()
    {
        if (this._colorPickerParent)
        {
            GameObject.Destroy(this._colorPickerParent);
        }
    }

    public void HandleClick(string colliderName)
    {
        if (this._colorPickerParent && this._colorPickerParent.activeSelf)
        {
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                this._colorPickerParent.SetActive(false);
            }
        }

        switch (colliderName)
        {
            case "MaleButton":
                if (this._currentProperties.gender != Gender.Male)
                {
                    this._currentProperties.gender = Gender.Male;
                    this.UpdateAvatarGender();
                    this.RandomizeHair();
                }
                break;
            case "FemaleButton":
                if (this._currentProperties.gender != Gender.Female)
                {
                    this._currentProperties.gender = Gender.Female;
                    this.UpdateAvatarGender();
                    this.RandomizeHair();
                }
                break;
            case "RandomEverythingButton":
                this.RandomizeCharacter();
                break;
            case "ColorHairButton":
                this.EnableColorPicker();
                this._colorPicker.Event.RemoveAllListeners();
                this._colorPicker.Event.AddListener(() => {
                    this._avatarHair.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    this._currentProperties.hairColor = new SerializableColor(this._colorPicker.PUBLIC_GetColor());
                });
                break;
            case "RandomHairButton":
                this.RandomizeHair();
                break;
            case "ColorSkinButton":
                var skinColor = this._characterRandomization.GetNextSkinColor(
                    this._currentProperties.skinColor.GetColor());
                this._currentProperties.skinColor = new SerializableColor(skinColor);
                break;
            case "ColorShirtButton":
                this.EnableColorPicker();
                this._colorPicker.Event.RemoveAllListeners();
                this._colorPicker.Event.AddListener(() => {
                    this._avatarBody.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    if (this._avatarBoobs)
                    {
                        this._avatarBoobs.GetComponent<SpriteRenderer>().color =
                            this._colorPicker.PUBLIC_GetColor();
                    }
                    this._currentProperties.shirtColor = new SerializableColor(this._colorPicker.PUBLIC_GetColor());
                });
                break;
            case "ColorPantsButton":
                this.EnableColorPicker();
                this._colorPicker.Event.RemoveAllListeners();
                this._colorPicker.Event.AddListener(() => {
                    this._avatarPantsTopLeft.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    this._avatarPantsBottomLeft.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    this._avatarPantsTopRight.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    this._avatarPantsBottomRight.GetComponent<SpriteRenderer>().color =
                        this._colorPicker.PUBLIC_GetColor();
                    this._currentProperties.pantsColor = new SerializableColor(this._colorPicker.PUBLIC_GetColor());
                });
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
        this.RandomizeHair();
        this._currentProperties.birthmark = this._characterRandomization.GetRandomBirthMark();
        this._currentProperties.hairColor = new SerializableColor(CharacterRandomization.GetRandomColor());
        this._currentProperties.shirtColor = new SerializableColor(CharacterRandomization.GetRandomColor());
        this._currentProperties.skinColor = new SerializableColor(this._characterRandomization.GetRandomSkinColor(Color.cyan));
        this._currentProperties.pantsColor = new SerializableColor(CharacterRandomization.GetRandomColor());
    }

    private void EnableColorPicker()
    {
        this._colorPickerParent.SetActive(true);
        this._colorPickerParent.transform.localScale = new Vector3(0.0f, 0.0f, 1f);
        this._colorPickerParent.transform.DOScale(new Vector3(0.85f, 0.85f, 1f), 0.5f)
            .SetEase(Ease.OutBack);
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

        if (this._currentProperties.gender == Gender.Female)
        {
            femaleAvatar.gameObject.SetActive(true);
            maleAvatar.gameObject.SetActive(false);

            this._avatarHair = femaleAvatar.transform.Find("Head").Find("Hair");
            this._avatarBody = femaleAvatar.transform.Find("Body");
            this._avatarBoobs = femaleAvatar.transform.Find("Boobs");
            this._avatarPantsTopLeft = femaleAvatar.transform.Find("LeftLegTop");
            this._avatarPantsBottomLeft = _avatarPantsTopLeft.Find("LeftLegBottom");
            this._avatarPantsTopRight = femaleAvatar.transform.Find("RightLegTop");
            this._avatarPantsBottomRight = _avatarPantsTopRight.Find("RightLegBottom");
        }
        else
        {
            maleAvatar.gameObject.SetActive(true);
            femaleAvatar.gameObject.SetActive(false);

            this._avatarHair = maleAvatar.transform.Find("Head").Find("Hair");
            this._avatarBody = maleAvatar.transform.Find("Body");
            this._avatarBoobs = null;
            this._avatarPantsTopLeft = maleAvatar.transform.Find("LeftLegTop");
            this._avatarPantsBottomLeft = _avatarPantsTopLeft.Find("LeftLegBottom");
            this._avatarPantsTopRight = maleAvatar.transform.Find("RightLegTop");
            this._avatarPantsBottomRight = _avatarPantsTopRight.Find("RightLegBottom");
        }
    }
}
