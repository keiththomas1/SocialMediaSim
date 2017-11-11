using TMPro;
using UnityEngine;

public class ProfileScreenController : MonoBehaviour {
    private GlobalVars globalVars;
    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private GameObject page;
    private GameObject scrollArea;

    private GameObject postsInfo;
    private GameObject followersInfo;
    private GameObject moneyInfo;

    void Awake () {
        this.globalVars = GlobalVars.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this.randomNameGenerator = new RandomNameGenerator();
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
        if (Input.GetMouseButtonUp(0))
        {
            DisableInfoSprites();
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "PostsIcon":
                        DisableInfoSprites();
                        if (postsInfo)
                        {
                            postsInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    case "FollowersIcon":
                        DisableInfoSprites();
                        if (followersInfo)
                        {
                            followersInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    case "MoneyIcon":
                        DisableInfoSprites();
                        if (moneyInfo)
                        {
                            moneyInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    default:
                        DisableInfoSprites();
                        break;
                }
            } else {
                DisableInfoSprites();
            }
        }
    }

    private void DisableInfoSprites()
    {
        if (postsInfo)
        {
            postsInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (followersInfo)
        {
            followersInfo.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (moneyInfo)
        {
            moneyInfo.GetComponent<SpriteRenderer>().enabled = false;
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

        scrollArea = page.transform.Find("ScrollArea").gameObject;
        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, 4.0f);

        var currentGender = this.characterSerializer.Gender;
        this.SetAvatar(currentGender);

        var stats = scrollArea.transform.Find("Stats");
        postsInfo = stats.transform.Find("PostsInfo").gameObject;
        followersInfo = stats.transform.Find("FollowersInfo").gameObject;
        moneyInfo = stats.transform.Find("MoneyInfo").gameObject;

        UpdateText();

        globalVars.RegisterCashListener(this);
        this._userSerializer.RegisterFollowersListener(this);
    }

    private void UpdateText()
    {
        if (!scrollArea)
        {
            return;
        }

        var nameText = scrollArea.transform.Find("NameText");
        if (nameText)
        {
            nameText.gameObject.GetComponent<TextMeshPro>().text = globalVars.PlayerName;
        }

        var stats = scrollArea.transform.Find("Stats");
        var postsText = stats.transform.Find("PostsText");
        if (postsText)
        {
            var postCount = this._userSerializer.GetReverseChronologicalPosts().Count;
            postsText.gameObject.GetComponent<TextMesh>().text = postCount.ToString();
        }
        var followersText = stats.transform.Find("FollowersText");
        if (followersText)
        {
            var followers = this._userSerializer.Followers;
            followersText.gameObject.GetComponent<TextMesh>().text = followers.ToString();
        }
        var moneyText = stats.transform.Find("MoneyText");
        if (moneyText)
        {
            var cash = globalVars.TotalCash;
            if (cash > 0.0f) {
                var formattedCash = cash.ToString("C2");
                moneyText.gameObject.GetComponent<TextMesh>().text = formattedCash;
            } else {
                moneyText.gameObject.GetComponent<TextMesh>().text = "$0.00";
            }
        }
    }

    public void DestroyPage()
    {
        globalVars.UnregisterCashListener(this);
        this._userSerializer.UnregisterFollowersListener(this);
        GameObject.Destroy(page);
    }
}
