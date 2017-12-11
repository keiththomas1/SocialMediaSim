using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum Page
{
    Home,
    Profile,
    Post,
    Explore,
    Messages
}

public class UIController : MonoBehaviour {
    [SerializeField]
    private GameObject _bottomNavBackground;
    [SerializeField]
    private GameObject _homeButton;
    [SerializeField]
    private GameObject _profileButton;
    [SerializeField]
    private GameObject _postButton;
    [SerializeField]
    private GameObject _postTime;
    [SerializeField]
    private GameObject _exploreButton;
    [SerializeField]
    private GameObject _messagesButton;

    [SerializeField]
    private Sprite _homeButtonUnselected;
    [SerializeField]
    private Sprite _homeButtonSelected;
    [SerializeField]
    private Sprite _profileButtonUnselected;
    [SerializeField]
    private Sprite _profileButtonSelected;
    [SerializeField]
    private Sprite _postButtonUnselected;
    [SerializeField]
    private Sprite _postButtonSelected;
    [SerializeField]
    private Sprite _exploreButtonUnselected;
    [SerializeField]
    private Sprite _exploreButtonSelected;
    [SerializeField]
    private Sprite _messagesButtonUnselected;
    [SerializeField]
    private Sprite _messagesButtonSelected;

    private HomeScreenController _homeController;
    private ProfileScreenController _profileController;
    private NewPostController _newPostController;
    private ExploreScreenController _exploreController;
    private MessagesScreenController _messagesController;
    private NotificationController _notificationController;
    private SoundController _soundController;
    private UserSerializer _userSerializer;

    private Page _currentPage;
    private List<Page> _lastPages;

    private float _postTimeTimer = 0.0f;
    private float _backOutTimer = 0.0f;

    // Use this for initialization
    void Start () {
        this._bottomNavBackground.GetComponent<Image>().enabled = true;
        this._homeButton.GetComponent<Button>().onClick.AddListener(this.OnHomeClick);
        this._profileButton.GetComponent<Button>().onClick.AddListener(this.OnProfileClick);
        this._postButton.GetComponent<Button>().onClick.AddListener(this.OnPostClick);
        this._postTime.GetComponent<TextMeshProUGUI>().text = "";
        this._exploreButton.GetComponent<Button>().onClick.AddListener(this.OnExploreClick);
        this._messagesButton.GetComponent<Button>().onClick.AddListener(this.OnMessagesClick);

        this._homeController = GetComponent<HomeScreenController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._exploreController = GetComponent<ExploreScreenController>();
        this._messagesController = GetComponent<MessagesScreenController>();
        this._notificationController = GetComponent<NotificationController>();
        this._soundController = GameObject.Find("SoundController").GetComponent<SoundController>();
        this._userSerializer = UserSerializer.Instance;

        this._lastPages = new List<Page>();
        this.GoToProfilePage();

        if (this._userSerializer.NextPostTime > DateTime.Now)
        {
            this._postTimeTimer = 0.3f;
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (this._postTimeTimer > 0.0f)
        {
            this._postTimeTimer -= Time.deltaTime;
            if (this._postTimeTimer <= 0.0f)
            {
                this.UpdateTimeRemaining();
                this._postTimeTimer = 1;
            }
        }
        if (this._backOutTimer > 0.0f)
        {
            this._backOutTimer -= Time.deltaTime;
        }

        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                if (this._backOutTimer <= 0.0f)
                {
                    this.BackOut();
                    this._backOutTimer = 0.5f;
                }
            }
        }
    }

    public Page GetCurrentPage()
    {
        return this._currentPage;
    }

    /* Private methods */

    private void BackOut()
    {
        switch(this._currentPage)
        {
            case Page.Home:
                if (!this._homeController.BackOut())
                {
                    return;
                }
                break;
            case Page.Profile:
                if (!this._profileController.BackOut())
                {
                    return;
                }
                break;
            case Page.Post:
                if (!this._newPostController.BackOut())
                {
                    return;
                }
                break;
            case Page.Explore:
                if (!this._exploreController.BackOut())
                {
                    return;
                }
                break;
            case Page.Messages:
                if (!this._messagesController.BackOut())
                {
                    return;
                }
                break;
        }

        if (this._lastPages.Count > 0)
        {
            var lastPage = this._lastPages[this._lastPages.Count - 1];
            this._lastPages.RemoveAt(this._lastPages.Count - 1);

            switch (lastPage)
            {
                case Page.Home:
                    this.GoToHomePage();
                    break;
                case Page.Profile:
                    this.GoToProfilePage();
                    break;
                case Page.Post:
                    this.GoToPostPage();
                    break;
                case Page.Explore:
                    this.GoToExplorePage();
                    break;
                case Page.Messages:
                    this.GoToMessagesPage();
                    break;
            }
        }
    }

    private void OnHomeClick()
    {
        this.UpdateLastVisited();
        this.GoToHomePage();
    }
    private void GoToHomePage()
    {
        GenerateHomePage();
        this._soundController.PlayClickSound(1);
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateHomePage()
    {
        if (this._currentPage != Page.Home)
        {
            DestroyPage(this._currentPage);
            this._homeController.EnterScreen();
            this._currentPage = Page.Home;
        }
    }

    private void OnProfileClick()
    {
        this.UpdateLastVisited();
        this.GoToProfilePage();
    }
    private void GoToProfilePage()
    {
        GenerateProfilePage();
        this._soundController.PlayClickSound(1);
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateProfilePage()
    {
        if (this._currentPage != Page.Profile)
        {
            DestroyPage(this._currentPage);
            this._profileController.EnterScreen();
            this._currentPage = Page.Profile;
        }
    }

    private void OnPostClick()
    {
        this.UpdateLastVisited();
        this.GoToPostPage();
    }
    private void GoToPostPage()
    {
        GeneratePostPage();
        this._soundController.PlayClickSound(1);
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GeneratePostPage()
    {
        if (this._currentPage != Page.Post)
        {
            DestroyPage(this._currentPage);
            this._newPostController.CreatePopup(this.FinishedCreatingPicture);
            this._currentPage = Page.Post;
        }
    }

    private void OnExploreClick()
    {
        this.UpdateLastVisited();
        this.GoToExplorePage();
    }
    private void GoToExplorePage()
    {
        GenerateExplorePage();
        this._soundController.PlayClickSound(1);
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateExplorePage()
    {
        if (this._currentPage != Page.Explore)
        {
            DestroyPage(this._currentPage);
            this._exploreController.EnterScreen();
            this._currentPage = Page.Explore;
        }
    }

    private void OnMessagesClick()
    {
        this.UpdateLastVisited();
        this.GoToMessagesPage();
    }
    private void GoToMessagesPage()
    {
        GenerateMessagesPage();
        this._soundController.PlayClickSound(1);
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateMessagesPage()
    {
        if (this._currentPage != Page.Messages)
        {
            DestroyPage(this._currentPage);
            this._messagesController.EnterScreen();
            this._currentPage = Page.Messages;
        }
    }

    private void UpdateLastVisited()
    {
        if (this._lastPages.Contains(this._currentPage))
        {
            this._lastPages.Remove(this._currentPage);
        }
        this._lastPages.Add(this._currentPage);
    }

    private void DestroyPage(Page page)
    {
        switch (page)
        {
            case Page.Home:
                this._homeController.DestroyPage();
                break;
            case Page.Profile:
                this._profileController.DestroyPage();
                break;
            case Page.Post:
                this._newPostController.DestroyPage();
                break;
            case Page.Explore:
                this._exploreController.DestroyPage();
                break;
            case Page.Messages:
                this._messagesController.DestroyPage();
                break; 
        }
    }

    private void FinishedCreatingPicture(DelayGramPost post)
    {
        this._postTimeTimer = 0.1f;
        this._profileController.MarkFirstPostAsNew();
        this.GoToProfilePage();
    }

    private void ShowPostTimeRemaining(DelayGramPost post)
    {
    }
    private void UpdateTimeRemaining()
    {
        var timeTillCanPost = _userSerializer.NextPostTime - DateTime.Now;
        var formattedTime = timeTillCanPost.ToString(@"mm\:ss");
        this._postTime.GetComponent<TextMeshProUGUI>().text = formattedTime;
    }

    private void UpdateButtonState()
    {
        this._homeButton.GetComponent<Image>().sprite = (this._currentPage == Page.Home) ? this._homeButtonSelected : this._homeButtonUnselected;
        this._profileButton.GetComponent<Image>().sprite = (this._currentPage == Page.Profile) ? this._profileButtonSelected : this._profileButtonUnselected;
        this._postButton.GetComponent<Image>().sprite = (this._currentPage == Page.Post) ? this._postButtonSelected : this._postButtonUnselected;
        this._exploreButton.GetComponent<Image>().sprite = (this._currentPage == Page.Explore) ? this._exploreButtonSelected : this._exploreButtonUnselected;
        this._messagesButton.GetComponent<Image>().sprite = (this._currentPage == Page.Messages) ? this._messagesButtonSelected : this._messagesButtonUnselected;
    }
}
