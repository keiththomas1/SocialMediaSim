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

    private const float POST_X_OFFSET = -1.06f;
    private const float POST_Y_OFFSET = -2.5f;

    private GlobalVars globalVars;
    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private PostHelper _postHelper;

    private GameObject page;
    private GameObject scrollArea;
    private List<DelayGramPostObject> _youPostObjects;
    private bool _firstPostNew;

    private GameObject _editScreen;
    private CharacterProperties _previousCharacterProperties;

    private enum ProfileScreenState
    {
        ProfileDefault,
        SinglePicture
    }
    private ProfileScreenState _currentState;

    // For handling of selecting an image and resizing/repositioning
    private DelayGramPostObject _currentSelectedImage;
    private Vector3 _originalImageScale;
    private Vector3 _originalImagePosition;
    private bool _imageCurrentlyShrinking = false;

    void Awake () {
        this.globalVars = GlobalVars.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this.randomNameGenerator = new RandomNameGenerator();
        this._postHelper = new PostHelper();

        this._youPostObjects = new List<DelayGramPostObject>();
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
                default:
                    foreach (DelayGramPostObject post in this._youPostObjects)
                    {
                        if (post.postObject && colliderName == post.postObject.name)
                        {
                            if (this._currentState == ProfileScreenState.ProfileDefault)
                            {
                                this.EnlargePost(post);
                            }
                            else
                            {
                                this.ShrinkPost(this._currentSelectedImage);
                            }
                        }
                    }
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

        var characterStats = scrollArea.transform.Find("CharacterStats").gameObject;
        this.SetupStatistics(characterStats);

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
        foreach (DelayGramPostObject post in this._youPostObjects)
        {
            if (post.postObject)
            {
                post.postObject.SetActive(false);
                GameObject.Destroy(post.postObject);
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

    public void CreateEditAvatarScreen()
    {
        this._editScreen = GameObject.Instantiate(Resources.Load("Profile/CreateCharacterPopup") as GameObject);
        this._editScreen.transform.position = new Vector3(0.3f, 1.55f, 0.0f);
        this.SetAvatar(this._editScreen);
        this.page.SetActive(false);

        UpdateText(this._editScreen.transform.Find("ChooseNameTextBox").Find("NameText").gameObject);

        this._previousCharacterProperties = new CharacterProperties(this.characterSerializer.CurrentCharacterProperties);
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

    private void SetupStatistics(GameObject parent)
    {
        var postsCreatedText = parent.transform.Find("PostsCreatedNumber");
        postsCreatedText.GetComponent<TextMeshPro>().text = this._userSerializer.PostCount.ToString();

        var yourLikesText = parent.transform.Find("LikesOnYoursNumber");
        yourLikesText.GetComponent<TextMeshPro>().text = "0";

        var yourDislikesText = parent.transform.Find("DislikesOnYoursNumber");
        yourDislikesText.GetComponent<TextMeshPro>().text = "0";

        var othersLikesText = parent.transform.Find("LikesOnOthersNumber");
        othersLikesText.GetComponent<TextMeshPro>().text = "0";

        var othersDislikesText = parent.transform.Find("DislikesOnOthersNumber");
        othersDislikesText.GetComponent<TextMeshPro>().text = "0";
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
            var animationPosition = this._youPostObjects[0].postObject.transform.position;
            animationPosition.z += 1;
            postAnimation.transform.position = animationPosition;
            postAnimation.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            postAnimation.transform.parent = this.scrollArea.transform;

            this._firstPostNew = false;
        }
    }


    private void EnlargePost(DelayGramPostObject post)
    {
        if (this._imageCurrentlyShrinking)
        {
            return;
        }

        this._currentState = ProfileScreenState.SinglePicture;
        this._currentSelectedImage = post;
        this._originalImageScale = post.postObject.transform.localScale;
        this._originalImagePosition = post.postObject.transform.localPosition;

        this._postHelper.EnlargeAndCenterPost(post);

        post.postObject.transform.parent = null;
        this.page.SetActive(false);
    }

    private void ShrinkPost(DelayGramPostObject post)
    {
        this._currentState = ProfileScreenState.ProfileDefault;
        this._imageCurrentlyShrinking = true;

        // Scale post down and position where it used to be
        this._postHelper.ShrinkAndReturnPost(
            post,
            this._originalImageScale,
            this._originalImagePosition,
            () => this.PostFinishedShrinking(post, false));

        this.page.SetActive(true);
        post.postObject.transform.parent = this.scrollArea.transform;
    }

    private void PostFinishedShrinking(DelayGramPostObject postObject, bool showDetails)
    {
        this._imageCurrentlyShrinking = false;
        this._postHelper.SetPostDetails(postObject.postObject, postObject.post, false);
    }
}
