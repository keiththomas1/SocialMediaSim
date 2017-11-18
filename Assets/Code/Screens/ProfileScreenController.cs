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
    private ScrollController _youScrollController;
    private List<GameObject> _youPostObjects;

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
        CheckHover();
    }

    private void CheckHover()
    {
        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    default:
                        break;
                }
            }
        }
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            case "RandomNameButton":
                var randomName = this.randomNameGenerator.GenerateRandomName();
                globalVars.PlayerName = randomName;
                UpdateText();
                break;
            case "MaleButton":
                this.SetAvatar(Gender.Male);
                this.characterSerializer.Gender = Gender.Male;
                break;
            case "FemaleButton":
                this.SetAvatar(Gender.Female);
                this.characterSerializer.Gender = Gender.Female;
                break;
        }
    }

    private void SetAvatar(Gender gender)
    {
        switch (gender)
        {
            case Gender.Female:
                scrollArea.transform.Find("FemaleAvatar").gameObject.SetActive(true);
                scrollArea.transform.Find("MaleAvatar").gameObject.SetActive(false);
                break;
            case Gender.Male:
                scrollArea.transform.Find("MaleAvatar").gameObject.SetActive(true);
                scrollArea.transform.Find("FemaleAvatar").gameObject.SetActive(false);
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

        var currentGender = this.characterSerializer.Gender;
        this.SetAvatar(currentGender);

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

        globalVars.UnregisterCashListener(this);
        this._userSerializer.UnregisterFollowersListener(this);
        GameObject.Destroy(page);
    }

    private void GenerateProfilePosts()
    {
        var posts = this._userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        this._postHelper.GeneratePostFeed(
            this.scrollArea, posts, this._youPostObjects, this._youScrollController, POST_X_OFFSET, POST_Y_OFFSET);
    }
}
