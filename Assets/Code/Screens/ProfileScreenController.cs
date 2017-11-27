using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProfileScreenController : MonoBehaviour
{
    private const float POST_X_OFFSET = -0.2f;
    private const float POST_Y_OFFSET = -2.0f;

    private GlobalVars globalVars;
    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private PostHelper _postHelper;

    private GameObject page;
    private GameObject scrollArea;
    private List<GameObject> _youPostObjects;

    private GameObject _editScreen;
    private CharacterProperties _previousCharacterProperties;

    void Awake () {
        this.globalVars = GlobalVars.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this.randomNameGenerator = new RandomNameGenerator();
        this._postHelper = new PostHelper();

        this._youPostObjects = new List<GameObject>();
    }

    void Start()
    {
    }

    public void OnTotalCashUpdated(float newCash)
    {
        UpdateText();
    }

    public void OnFollowersUpdated(int newFollowers)
    {
        UpdateText();
    }

    void Update ()
    {
    }

    public void CheckClick(string colliderName)
    {
        if (this._editScreen)
        {
            if (colliderName == "BackButton")
            {
                this.ResetCharacterProperties();
                GameObject.Destroy(this._editScreen);
                this.page.SetActive(true);
            } else if (colliderName == "DoneButton") {
                GameObject.Destroy(this._editScreen);
                this.page.SetActive(true);
            } else {
                this._editScreen.GetComponent<CharacterEditor>().CheckClick(colliderName);
            }
        } else {
            switch (colliderName)
            {
                case "RandomNameButton":
                    var randomName = this.randomNameGenerator.GenerateRandomName();
                    globalVars.PlayerName = randomName;
                    UpdateText();
                    break;
                case "MaleButton":
                    this.characterSerializer.Gender = Gender.Male;
                    this.SetAvatar(scrollArea.transform.Find("SpriteMask").gameObject);
                    this.page.GetComponent<CharacterEditor>().RandomizeCharacter();
                    break;
                case "FemaleButton":
                    this.characterSerializer.Gender = Gender.Female;
                    this.SetAvatar(scrollArea.transform.Find("SpriteMask").gameObject);
                    this.page.GetComponent<CharacterEditor>().RandomizeCharacter();
                    break;
                case "EditButton":
                    this.CreateEditAvatarScreen();
                    break;
            }
        }
    }

    private void CreateEditAvatarScreen()
    {
        this._editScreen = GameObject.Instantiate(Resources.Load("Profile/CreateCharacterPopup") as GameObject);
        this._editScreen.transform.position = new Vector3(0.3f, 1.55f, 0.0f);
        this.SetAvatar(this._editScreen);
        this.page.SetActive(false);

        this._previousCharacterProperties = new CharacterProperties(this.characterSerializer.CurrentCharacterProperties);
    }

    private void ResetCharacterProperties()
    {
        this.characterSerializer.CurrentCharacterProperties = this._previousCharacterProperties;
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

    public void EnterScreen()
    {
        page = GameObject.Instantiate(Resources.Load("Profile/DGProfilePage") as GameObject);
        page.transform.position = new Vector3(0.2f, 1.3f, 0.0f);

        scrollArea = page.transform.Find("ScrollArea").gameObject;
        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, 4.0f);
        this.SetAvatar(scrollArea.transform.Find("SpriteMask").gameObject);

        UpdateText();

        globalVars.RegisterCashListener(this);
        this._userSerializer.RegisterFollowersListener(this);

        this.GenerateProfilePosts();
    }

    private void UpdateText()
    {
        var nameText = scrollArea.transform.Find("NameText");
        if (nameText)
        {
            nameText.gameObject.GetComponent<TextMeshPro>().text = globalVars.PlayerName;
        }
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

        globalVars.UnregisterCashListener(this);
        this._userSerializer.UnregisterFollowersListener(this);
        GameObject.Destroy(page);
    }

    private void GenerateProfilePosts()
    {
        var posts = this._userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        this._postHelper.GeneratePostFeed(
            this.scrollArea, posts, this._youPostObjects, POST_X_OFFSET, POST_Y_OFFSET);
    }
}
