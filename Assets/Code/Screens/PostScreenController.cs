using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class PostScreenController : MonoBehaviour {
    [SerializeField]
    private Sprite _leftSideNav;
    [SerializeField]
    private Sprite _rightSideNav;

    private float POST_X_OFFSET = -0.05f;
    private float POST_Y_OFFSET = 0.50f;
    private float LENGTH_BETWEEN_POSTS = 3.7f;
    private UserSerializer _userSerializer;
    private NewPostController newPostController;
    private GameObject postPage;
    private GameObject _topNavigation;
    private GameObject _errorText;
    private float checkUpdateRemainingTimer;

    // You objects
    private GameObject _youScrollArea;
    private GameObject _timeRemaining;
    private GameObject _newPostButton;
    private ScrollController _youScrollController;
    private List<GameObject> _youPostObjects;

    // World objects
    private GameObject _worldScrollArea;
    private ScrollController _worldScrollController;
    private List<GameObject> _worldPostObjects;
    private RESTRequester _restRequester;
    private PostHelper _postHelper;
    private GameObject _loadingIcon;

    private enum CurrentSection
    {
        You,
        World
    }
    private CurrentSection _currentSection = CurrentSection.You;

    // Use this for initialization
    void Awake()
    {
        this._userSerializer = UserSerializer.Instance;
        this.newPostController = GetComponent<NewPostController>();
        this._youPostObjects = new List<GameObject>();
        this._worldPostObjects = new List<GameObject>();
        this._restRequester = new RESTRequester();
        this._postHelper = new PostHelper();

        this.checkUpdateRemainingTimer = -1.0f;
	}
	
	// Update is called once per frame
	void Update () {
        if (checkUpdateRemainingTimer >= 0.0f)
        {
            checkUpdateRemainingTimer -= Time.deltaTime;

            if (checkUpdateRemainingTimer <= 0.0f)
            {
                UpdateTimeRemaining();
            }
        }
	}

    public void EnterScreen()
    {
        this._userSerializer = UserSerializer.Instance;
        this._currentSection = CurrentSection.You;

        this.postPage = GameObject.Instantiate(Resources.Load("Posts/DGPostPage") as GameObject);
        this.postPage.transform.position = new Vector3(0.0f, 0.25f, 0.0f);
        this._youScrollArea = this.postPage.transform.Find("YouScrollArea").gameObject;
        this._timeRemaining = this._youScrollArea.transform.Find("TimeRemaining").gameObject;
        this._newPostButton = this._youScrollArea.transform.Find("NewPostButton").gameObject;
        this._worldScrollArea = this.postPage.transform.Find("WorldScrollArea").gameObject;
        this._topNavigation = this.postPage.transform.Find("TopNavigation").gameObject;
        this._errorText = this._worldScrollArea.transform.Find("ErrorText").gameObject;

        if (this._userSerializer.NextPostTime > DateTime.Now) {
            this.ShowTimeRemaining();
        }

        this.GenerateYouPostObjects();
    }

    void OnDestroy()
    {
        this.DestroyPage();
    }

    public void CheckClick(string colliderName)
    {
        if (!newPostController.PopupActive())
        {
            switch (colliderName)
            {
                case "NewPostButton":
                    newPostController.CreatePopup(NewPostCreated);
                    break;
                case "YouButton":
                    if (this._currentSection == CurrentSection.World)
                    {
                        var background = this._topNavigation.transform.Find("TopBackground");
                        background.GetComponent<SpriteRenderer>().sprite = this._leftSideNav;
                        var youButton = this._topNavigation.transform.Find("YouButton");
                        youButton.GetComponent<TextMeshPro>().color = new Color(68 / 255.0f, 68 / 255.0f, 68 / 255.0f);
                        var worldButton = this._topNavigation.transform.Find("WorldButton");
                        worldButton.GetComponent<TextMeshPro>().color = new Color(188 / 255.0f, 188 / 255.0f, 188 / 255.0f);

                        this.EnterYouSection();
                        this._currentSection = CurrentSection.You;
                    }
                    break;
                case "WorldButton":
                    if (this._currentSection == CurrentSection.You)
                    {
                        var background = this._topNavigation.transform.Find("TopBackground");
                        background.GetComponent<SpriteRenderer>().sprite = this._rightSideNav;
                        var youButton = this._topNavigation.transform.Find("YouButton");
                        youButton.GetComponent<TextMeshPro>().color = new Color(188 / 255.0f, 188 / 255.0f, 188 / 255.0f);
                        var worldButton = this._topNavigation.transform.Find("WorldButton");
                        worldButton.GetComponent<TextMeshPro>().color = new Color(68 / 255.0f, 68 / 255.0f, 68 / 255.0f);

                        this.EnterWorldSection();
                        this._currentSection = CurrentSection.World;
                    }
                    break;
            }
        }
        else
        {
            newPostController.CheckClick(colliderName);
        }
    }
    
    public void DestroyPage()
    {
        DestroyPosts(this._youPostObjects);
        this._youPostObjects.Clear();
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();
        if (this.newPostController.PopupActive())
        {
            this.newPostController.DestroyPopup();
        }
        GameObject.Destroy(postPage);
    }
    
    private void EnterYouSection()
    {
        this._youScrollArea.SetActive(true);
        this._worldScrollArea.SetActive(false);
        this._errorText.SetActive(false);
    }

    private void EnterWorldSection()
    {
        this._worldScrollArea.SetActive(true);
        this._youScrollArea.SetActive(false);
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);

        this._restRequester.RequestLastTenPosts(this.UpdateWorldPosts);
        // StartCoroutine(getLastTenPosts);
    }

    private void UpdateWorldPosts(PictureArrayJson pictures, bool success)
    {
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }

        if (!success)
        {
            if (this._errorText)
            {
                this._errorText.SetActive(true);
                this._errorText.GetComponent<TextMeshPro>().text = "Sorry, we were unable to talk to the server.";
            }
        }
        else
        {
            if (pictures.pictureModels.Length == 0)
            {
                this._errorText.SetActive(true);
                this._errorText.GetComponent<TextMeshPro>().text = "No new posts.";
            }
            this.GenerateWorldPostObjects(pictures);
        }
    }

    private void DestroyPosts(List<GameObject> postObjects)
    {
        foreach (GameObject postObject in postObjects)
        {
            if (postObject)
            {
                postObject.SetActive(false);
                GameObject.Destroy(postObject);
            }
        }
    }

    private void GenerateYouPostObjects()
    {
        var posts = _userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        this.GeneratePostObjects(this._youScrollArea, posts, this._youPostObjects, this._youScrollController);
    }

    private void GenerateWorldPostObjects(PictureArrayJson pictureArray)
    {
        var posts = new List<DelayGramPost>();
        foreach (PictureModelJsonReceive picture in pictureArray.pictureModels)
        {
            // Create a picture with information from picture
            var newPost = this._restRequester.ConvertJsonPictureIntoDelayGramPost(picture);

            posts.Add(newPost);
        }
        this.GeneratePostObjects(this._worldScrollArea, posts, this._worldPostObjects, this._worldScrollController);
    }

    private void GeneratePostObjects(
        GameObject scrollArea,
        List<DelayGramPost> posts,
        List<GameObject> postObjects,
        ScrollController scrollController)
    {
        var currentY = scrollArea.transform.localPosition.y + POST_Y_OFFSET;
        foreach (DelayGramPost post in posts)
        {
            var newPost = SetupPostPrefab(post, currentY, scrollArea);

            currentY -= LENGTH_BETWEEN_POSTS;
            postObjects.Add(newPost);
        }

        scrollController = scrollArea.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, -currentY);
    }

    private GameObject SetupPostPrefab(DelayGramPost post, float yPosition, GameObject scrollArea)
    {
        var postPrefab = Resources.Load("Posts/NewPost") as GameObject;
        if (postPrefab)
        {
            var postPrefabInstance = GameObject.Instantiate(postPrefab);
            postPrefabInstance.name = post.imageID;
            postPrefabInstance.transform.parent = scrollArea.transform;
            postPrefabInstance.transform.localPosition = new Vector3(POST_X_OFFSET, yPosition, 0.0f);

            var nameText = postPrefabInstance.transform.Find("NameText").GetComponent<TextMeshPro>();
            nameText.text = post.playerName; // globalVars.PlayerName;

            var timeText = postPrefabInstance.transform.Find("TimeText").GetComponent<TextMeshPro>();
            var timeTextXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.86f, 0.0f, 0.0f)).x;
            timeText.transform.position = new Vector3(
                timeTextXPosition,
                timeText.transform.position.y,
                timeText.transform.position.z);
            var timeSincePost = DateTime.Now - post.dateTime;
            timeText.text = this._restRequester.GetPostTimeFromDateTime(timeSincePost);

            var profilePicBubble = postPrefabInstance.transform.Find("ProfilePicBubble");
            var profileBubbleXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.07f, 0.0f, 0.0f)).x;
            profilePicBubble.transform.position = new Vector3(
                profileBubbleXPosition,
                profilePicBubble.transform.position.y,
                profilePicBubble.transform.position.z);
            _postHelper.SetupProfilePicBubble(profilePicBubble.gameObject, post.characterProperties);

            var likeDislikeArea = postPrefabInstance.transform.Find("LikeDislikeArea");
            var likeDislikeAreaXPosition = Camera.main.ViewportToWorldPoint(new Vector3(0.67f, 0.0f, 0.0f)).x;
            likeDislikeArea.transform.position = new Vector3(
                likeDislikeAreaXPosition,
                likeDislikeArea.transform.position.y,
                likeDislikeArea.transform.position.z);

            this._postHelper.PopulatePostFromData(postPrefabInstance, post);

            return postPrefabInstance;
        }
        else
        {
            return null;
        }
    }

    public void NewPostCreated(DelayGramPost post)
    {
        this.DestroyPosts(this._youPostObjects);
        this._youPostObjects.Clear();

        this.GenerateYouPostObjects();
        this.ShowTimeRemaining();

        var postPicture = this._restRequester.PostPicture(post);
        StartCoroutine(postPicture);
    }

    public bool PopupActive()
    {
        return newPostController.PopupActive();
    }

    public void DestroyPopup()
    {
        newPostController.DestroyPopup();
    }

    private void ShowTimeRemaining()
    {
        if (this._newPostButton)
        {
            this._newPostButton.GetComponent<SpriteRenderer>().enabled = false;
            this._newPostButton.GetComponent<Collider>().enabled = true;
        }

        UpdateTimeRemaining();
    }

    private void UpdateTimeRemaining()
    {
        if (this._timeRemaining)
        {
            var timeTillCanPost = _userSerializer.NextPostTime - DateTime.Now;
            if (timeTillCanPost > TimeSpan.FromDays(1))
            {
                var days = timeTillCanPost.Days;
                var daysText = "";
                if (days == 1) daysText = days + "day";
                else daysText = days + "days";
                this._timeRemaining.GetComponent<TextMesh>().text = daysText + " until next post";
            }
            else if (timeTillCanPost > TimeSpan.FromHours(1))
            {
                var hours = timeTillCanPost.Hours;
                var hoursText = "";
                if (hours == 1) hoursText = hours + "hour";
                else hoursText = hours + "hours";
                this._timeRemaining.GetComponent<TextMesh>().text = hoursText + " until next post";
                checkUpdateRemainingTimer = 600.0f;
            }
            else if (timeTillCanPost > TimeSpan.FromMinutes(1))
            {
                var minutes = timeTillCanPost.Minutes;
                var minutesText = "";
                if (minutes == 1) minutesText = minutes + "min";
                else minutesText = minutes + "mins";
                this._timeRemaining.GetComponent<TextMesh>().text = minutesText + " until next post";
                checkUpdateRemainingTimer = 60.0f;
            }
            else if (timeTillCanPost >= TimeSpan.FromSeconds(1))
            {
                var seconds = timeTillCanPost.Seconds;
                this._timeRemaining.GetComponent<TextMesh>().text = seconds + "secs until next post";
                checkUpdateRemainingTimer = 0.5f;
            }
            else
            {
                this._timeRemaining.GetComponent<TextMesh>().text = "";
                if (this._newPostButton)
                {
                    this._newPostButton.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
        }
    }
}
