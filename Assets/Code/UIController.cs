using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public enum Page
{
    Profile,
    Messages,
    Post,
    Rating,
    World,
    Tutorial
}

public class UIController : MonoBehaviour {
    [SerializeField]
    private GameObject _bottomNavBackground;

    [SerializeField]
    private GameObject _profileButton;
    private Image _profileIcon;
    [SerializeField]
    private GameObject _messagesButton;
    private Image _messagesIcon;

    public GameObject _postButton;
    private Image _postIcon;
    [SerializeField]
    private GameObject _ratingButton;
    private Image _ratingIcon;
    [SerializeField]
    private GameObject _worldButton;
    private Image _worldIcon;

    [SerializeField]
    private Sprite _profileButtonUnselected;
    [SerializeField]
    private Sprite _profileButtonSelected;
    [SerializeField]
    private Sprite _messagesButtonUnselected;
    [SerializeField]
    private Sprite _messagesButtonSelected;
    [SerializeField]
    private Sprite _postButtonUnselected;
    [SerializeField]
    private Sprite _postButtonSelected;
    [SerializeField]
    private Sprite _ratingButtonUnselected;
    [SerializeField]
    private Sprite _ratingButtonSelected;
    [SerializeField]
    private Sprite _worldButtonUnselected;
    [SerializeField]
    private Sprite _worldButtonSelected;

    private UserSerializer _userSerializer;

    private ProfileScreenController _profileController;
    private MessagesScreenController _messagesController;
    private NewPostController _newPostController;
    private RatingScreenController _ratingController;
    private WorldScreenController _worldController;

    private TutorialScreenController _tutorialController;
    private NotificationController _notificationController;
    private GoalsController _goalsController;

    private Page _currentPage = Page.World;
    private List<Page> _lastPages;

    private float _postTimeTimer = 0.0f;
    private float _backOutTimer = 0.0f;

    private GameObject _nextPostText;
    private GameObject _postTimeText;

    private GameObject _levelUpPopup = null;
    private GameObject _avatarTransitionPopup = null;

    // Use this for initialization
    void Start () {
        this._bottomNavBackground.transform.Find("BottomNavImage").GetComponent<Image>().enabled = true;
        this._profileButton.GetComponent<Button>().onClick.AddListener(this.OnProfileClick);
        this._profileIcon = this._profileButton.transform.Find("ProfileButtonIcon").GetComponent<Image>();
        this._messagesButton.GetComponent<Button>().onClick.AddListener(this.OnMessagesClick);
        this._messagesIcon = this._messagesButton.transform.Find("MessageButtonIcon").GetComponent<Image>();
        this._postButton.GetComponent<Button>().onClick.AddListener(this.OnPostClick);
        this._postIcon = this._postButton.transform.Find("CameraButtonIcon").GetComponent<Image>();
        this._nextPostText = this._postButton.transform.Find("NextPostText").gameObject;
        this._postTimeText = this._postButton.transform.Find("PostTimeText").gameObject;
        this._postTimeText.GetComponent<TextMeshProUGUI>().text = "";
        this._ratingButton.GetComponent<Button>().onClick.AddListener(this.OnRatingClick);
        this._ratingIcon = this._ratingButton.transform.Find("RatingButtonIcon").GetComponent<Image>();
        this._worldButton.GetComponent<Button>().onClick.AddListener(this.OnworldClick);
        this._worldIcon = this._worldButton.transform.Find("WorldButtonIcon").GetComponent<Image>();

        this._userSerializer = UserSerializer.Instance;

        this._profileController = GetComponent<ProfileScreenController>();
        this._messagesController = GetComponent<MessagesScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._ratingController = GetComponent<RatingScreenController>();
        this._worldController = GetComponent<WorldScreenController>();

        this._tutorialController = GetComponent<TutorialScreenController>();
        this._notificationController = GetComponent<NotificationController>();
        this._goalsController = GetComponent<GoalsController>();

        this._lastPages = new List<Page>();

        if (!this._userSerializer.CreatedCharacter)
        {
            this._goalsController.SetFirstGoal();
            this.GoToTutorialPage();
        } else {
            this.GoToProfilePage();
        }

        if (this._userSerializer.NextPostTime > DateTime.Now)
        {
            this._postTimeTimer = 0.3f;

            this._postIcon.enabled = false;
#if !UNITY_EDITOR
            this._postButton.GetComponent<Button>().enabled = false;
#endif
            this._nextPostText.SetActive(true);
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

    public void CreateLevelUpPopup()
    {
        this._levelUpPopup = GameObject.Instantiate(Resources.Load("UI/LevelUpPopup") as GameObject);
        this._levelUpPopup.transform.position = new Vector3(0.0f, -0.06f, -5.0f);
    }

    public void DestroyLevelPopup(CharacterProperties previousCharacterProperties = null)
    {
        GameObject.Destroy(this._levelUpPopup);
        this._levelUpPopup = null;
        if (previousCharacterProperties != null)
        {
            this.CreateAvatarTransitionPopup(previousCharacterProperties);
        }
    }
    public void DestroyAvatarTransitionPopup()
    {
        GameObject.Destroy(this._avatarTransitionPopup);
        this._avatarTransitionPopup = null;
    }

    public bool LevelPopupVisible()
    {
        return (this._levelUpPopup != null || this._avatarTransitionPopup != null);
    }

    /* Private methods */

    private void BackOut()
    {
        switch(this._currentPage)
        {
            case Page.World:
                if (!this._worldController.BackOut())
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
            case Page.Rating:
                if (!this._ratingController.BackOut())
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
                case Page.World:
                    this.GoToworldPage();
                    break;
                case Page.Profile:
                    this.GoToProfilePage();
                    break;
                case Page.Post:
                    this.GoToPostPage();
                    break;
                case Page.Rating:
                    this.GoToRatingPage();
                    break;
                case Page.Messages:
                    this.GoToMessagesPage();
                    break;
            }
        }
    }

    private void CreateAvatarTransitionPopup(CharacterProperties previousCharacterProperties)
    {
        this._avatarTransitionPopup = GameObject.Instantiate(Resources.Load("UI/AvatarTransitionPopup") as GameObject);
        this._avatarTransitionPopup.transform.position = new Vector3(0.0f, -0.06f, -5.0f);

        var avatarSection = this._avatarTransitionPopup.transform.Find("AvatarTransition");
        var spriteMask = avatarSection.transform.Find("SpriteMask");
        var oldFemaleAvatar = spriteMask.transform.Find("OldFemaleAvatar");
        var oldMaleAvatar = spriteMask.transform.Find("OldMaleAvatar");
        var newMaleAvatar = spriteMask.transform.Find("NewMaleAvatar");
        var newFemaleAvatar = spriteMask.transform.Find("NewFemaleAvatar");

        var gender = previousCharacterProperties.gender;
        switch(gender)
        {
            case Gender.Female:
                oldFemaleAvatar.GetComponent<CharacterCustomization>().SetCharacterLook(previousCharacterProperties);

                oldMaleAvatar.gameObject.SetActive(false);
                newMaleAvatar.gameObject.SetActive(false);
                break;
            case Gender.Male:
                oldMaleAvatar.GetComponent<CharacterCustomization>().SetCharacterLook(previousCharacterProperties);

                oldFemaleAvatar.gameObject.SetActive(false);
                newFemaleAvatar.gameObject.SetActive(false);
                break;
        }
    }

    private void OnworldClick()
    {
        if (this._userSerializer.PostedPhoto)
        {
            this.UpdateLastVisited();
            this.GoToworldPage();
        }
    }
    public void GoToworldPage()
    {
        GenerateworldPage();
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateworldPage()
    {
        if (this._currentPage != Page.World)
        {
            DestroyPage(this._currentPage);
            this._worldController.EnterScreen();
            this._currentPage = Page.World;
        }
    }

    private void OnProfileClick()
    {
        if (this._userSerializer.PostedPhoto)
        {
            this.UpdateLastVisited();
            this.GoToProfilePage();
        }
    }
    public void GoToProfilePage()
    {
        GenerateProfilePage();
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
        if (this._userSerializer.CreatedCharacter)
        {
            this.UpdateLastVisited();
            this.GoToPostPage();
        }
    }
    public void GoToPostPage()
    {
        GeneratePostPage();
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

    private void OnRatingClick()
    {
        if (this._userSerializer.PostedPhoto)
        {
            this.UpdateLastVisited();
            this.GoToRatingPage();
        }
    }
    public void GoToRatingPage()
    {
        GenerateRatingPage();
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateRatingPage()
    {
        if (this._currentPage != Page.Rating)
        {
            DestroyPage(this._currentPage);
            this._ratingController.EnterScreen();
            this._currentPage = Page.Rating;
        }
    }

    private void OnMessagesClick()
    {
        if (this._userSerializer.PostedPhoto)
        {
            this.UpdateLastVisited();
            this.GoToMessagesPage();
        }
    }
    public void GoToMessagesPage()
    {
        GenerateMessagesPage();
        UpdateButtonState();
        this._notificationController.ClearNotifications(this._currentPage);
    }
    private void GenerateMessagesPage()
    {
        // Even if we are currently in messages, destroy and refresh inbox
        DestroyPage(this._currentPage);
        this._messagesController.EnterScreen();
        this._currentPage = Page.Messages;
    }

    private void GoToTutorialPage()
    {
        this._tutorialController.EnterScreen();
        this._currentPage = Page.Tutorial;
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
            case Page.World:
                this._worldController.DestroyPage();
                break;
            case Page.Profile:
                this._profileController.DestroyPage();
                break;
            case Page.Post:
                this._newPostController.DestroyPage();
                break;
            case Page.Rating:
                this._ratingController.DestroyPage();
                break;
            case Page.Messages:
                this._messagesController.DestroyPage();
                break; 
        }
    }

    private void FinishedCreatingPicture(DelayGramPost post)
    {
        this._postTimeTimer = 0.1f;
        this._postIcon.enabled = false;
#if !UNITY_EDITOR
        this._postButton.GetComponent<Button>().enabled = false;
#endif
        this._nextPostText.SetActive(true);

        this._profileController.FinishedCreatingPicture(post);
        this.GoToProfilePage();
    }

    private void UpdateTimeRemaining()
    {
        if (this._userSerializer.NextPostTime > DateTime.Now)
        {
            var timeTillCanPost = this._userSerializer.NextPostTime - DateTime.Now;
            var formattedTime = timeTillCanPost.ToString(@"mm\:ss");
            this._postTimeText.GetComponent<TextMeshProUGUI>().text = formattedTime;

            this._postTimeTimer = 1.0f;
        } else {
            this._postIcon.enabled = true;
            this._postButton.GetComponent<Button>().enabled = true;
            this._postTimeText.GetComponent<TextMeshProUGUI>().text = "";
            this._nextPostText.SetActive(false);
        }
    }

    private void UpdateButtonState()
    {
        this._worldIcon.sprite = (this._currentPage == Page.World) ? this._worldButtonSelected : this._worldButtonUnselected;
        this._profileIcon.sprite = (this._currentPage == Page.Profile) ? this._profileButtonSelected : this._profileButtonUnselected;
        this._postIcon.sprite = (this._currentPage == Page.Post) ? this._postButtonSelected : this._postButtonUnselected;
        this._ratingIcon.sprite = (this._currentPage == Page.Rating) ? this._ratingButtonSelected : this._ratingButtonUnselected;
        this._messagesIcon.sprite = (this._currentPage == Page.Messages) ? this._messagesButtonSelected : this._messagesButtonUnselected;
    }
}
