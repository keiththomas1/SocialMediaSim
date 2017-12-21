using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject _chooseNameBox;
    private TextMeshProUGUI _chooseNameText;

    private const float POST_X_OFFSET = -0.2f;
    private const float POST_Y_OFFSET = -2.5f;

    private GlobalVars globalVars;
    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private PostHelper _postHelper;

    private GameObject page;
    private GameObject scrollArea;
    private List<GameObject> _youPostObjects;
    private bool _firstPostNew;

    private GameObject _editScreen;
    private CharacterProperties _previousCharacterProperties;

    void Awake () {
        this.globalVars = GlobalVars.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this.randomNameGenerator = new RandomNameGenerator();
        this._postHelper = new PostHelper();

        this._youPostObjects = new List<GameObject>();
        this._firstPostNew = false;

        this._chooseNameText = this._chooseNameBox.transform.Find("TextInput")
            .transform.Find("TextArea")
            .transform.Find("Text")
            .GetComponent<TextMeshProUGUI>();
        var exitButton = this._chooseNameBox.transform.Find("ExitButton").GetComponent<Button>();
        exitButton.onClick.AddListener(this.NameExitClicked);
        var randomizeButton = this._chooseNameBox.transform.Find("RandomizeButton").GetComponent<Button>();
        randomizeButton.onClick.AddListener(this.NameRandomizeClicked);
        var doneButton = this._chooseNameBox.transform.Find("DoneButton").GetComponent<Button>();
        doneButton.onClick.AddListener(this.NameDoneClicked);
    }

    void Start()
    {
    }
    void Update()
    {
    }

    public void MarkFirstPostAsNew()
    {
        this._firstPostNew = true;
    }

    public void CheckClick(string colliderName)
    {
        if (this._chooseNameBox.activeSelf)
        {
            return;
        }
        if (this._editScreen)
        {
            switch (colliderName)
            {
                case "ChooseNameTextBox":
                    this._chooseNameBox.SetActive(true);
                    break;
                case "BackButton":
                    this.ResetCharacterProperties();
                    this.DestroyEditScreen();
                    break;
                case "DoneButton":
                    this.DestroyEditScreen();
                    break;
                default:
                    this._editScreen.GetComponent<CharacterEditor>().CheckClick(colliderName);
                    break;
            }
        } else {
            switch (colliderName)
            {
                case "EditButton":
                    this.CreateEditAvatarScreen();
                    break;
            }
        }
    }

    public void EnterScreen()
    {
        page = GameObject.Instantiate(Resources.Load("Profile/DGProfilePage") as GameObject);
        page.transform.position = new Vector3(0.2f, 1.3f, 0.0f);

        scrollArea = page.transform.Find("ScrollArea").gameObject;
        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, 4.0f);

        var spriteMask = scrollArea.transform.Find("SpriteMask").gameObject;
        this.SetAvatar(spriteMask);
        this.SetupItems(spriteMask);

        this.GenerateProfilePosts();
    }

    public bool BackOut()
    {
        if (this._editScreen)
        {
            this.ResetCharacterProperties();
            this.DestroyEditScreen();
            return false;
        }
        return true;
    }

    public void DestroyPage()
    {
        foreach (GameObject postObject in this._youPostObjects)
        {
            if (postObject)
            {
                postObject.SetActive(false);
                GameObject.Destroy(postObject);
            }
        }
        this._youPostObjects.Clear();

        if (this._editScreen)
        {
            this.ResetCharacterProperties();
            GameObject.Destroy(this._editScreen);
        }

        GameObject.Destroy(page);
    }

    private void NameExitClicked()
    {
        this._chooseNameBox.SetActive(false);
    }
    private void NameRandomizeClicked()
    {
        var randomName = this.randomNameGenerator.GenerateRandomName();
        this._chooseNameText.text = randomName;
    }
    private void NameDoneClicked()
    {
        this.globalVars.PlayerName = this._chooseNameText.text;
        this._chooseNameBox.SetActive(false);
        UpdateText(this._editScreen.transform.Find("ChooseNameTextBox").Find("NameText").gameObject);
    }

    private void DestroyEditScreen()
    {
        GameObject.Destroy(this._editScreen);
        this.page.SetActive(true);
        this.characterSerializer.UpdateAllCharacters();
        this.SetAvatar(scrollArea.transform.Find("SpriteMask").gameObject);
    }

    private void CreateEditAvatarScreen()
    {
        this._editScreen = GameObject.Instantiate(Resources.Load("Profile/CreateCharacterPopup") as GameObject);
        this._editScreen.transform.position = new Vector3(0.3f, 1.55f, 0.0f);
        this.SetAvatar(this._editScreen);
        this.page.SetActive(false);

        UpdateText(this._editScreen.transform.Find("ChooseNameTextBox").Find("NameText").gameObject);

        this._previousCharacterProperties = new CharacterProperties(this.characterSerializer.CurrentCharacterProperties);
    }

    private void ResetCharacterProperties()
    {
        this.characterSerializer.CurrentCharacterProperties = this._previousCharacterProperties;
    }

    private void UpdateText(GameObject textObject)
    {
        if (textObject)
        {
            textObject.GetComponent<TextMeshPro>().text = globalVars.PlayerName;
        }
    }

    private void SetAvatar(GameObject parent)
    {
        var gender = this.characterSerializer.Gender;
        switch (gender)
        {
            case Gender.Female:
                parent.transform.Find("FemaleAvatar").gameObject.SetActive(true);
                parent.transform.Find("MaleAvatar").gameObject.SetActive(false);
                break;
            case Gender.Male:
                parent.transform.Find("MaleAvatar").gameObject.SetActive(true);
                parent.transform.Find("FemaleAvatar").gameObject.SetActive(false);
                break;
        }
    }

    private void SetupItems(GameObject parent)
    {
        if (this._userSerializer.HasBulldog)
        {
            var bulldog = parent.transform.Find("Bulldog");
            if (bulldog)
            {
                bulldog.gameObject.SetActive(true);
            }
        }
        if (this._userSerializer.HasDrone)
        {
            var drone = parent.transform.Find("D-Rone");
            if (drone)
            {
                drone.gameObject.SetActive(true);
            }
        }
    }

    private void GenerateProfilePosts()
    {
        var posts = this._userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        this._postHelper.GeneratePostFeed(
            this.scrollArea, posts, this._youPostObjects, POST_X_OFFSET, POST_Y_OFFSET);

        if (this._firstPostNew)
        {
            this.scrollController.ScrollToPosition(2.3f);

            var postAnimation = GameObject.Instantiate(Resources.Load("Posts/NewPostAnimation") as GameObject);
            var animationPosition = this._youPostObjects[0].transform.position;
            animationPosition.z += 1;
            postAnimation.transform.position = animationPosition;
            postAnimation.transform.parent = this.scrollArea.transform;

            this._firstPostNew = false;
        }
    }
}
