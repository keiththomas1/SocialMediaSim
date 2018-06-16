using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class WorldScreenController : MonoBehaviour {
    [SerializeField]
    private Sprite _leftSideNav;
    [SerializeField]
    private Sprite _rightSideNav;

    private const float POST_X_OFFSET = -0.87f;
    private const float POST_Y_OFFSET = 2.84f;
    private const float PROFILE_POST_X_OFFSET = -1.1f;
    private const float PROFILE_POST_Y_OFFSET = -3.07f;
    private GameObject _postPage;
    private GameObject _errorText;

    // World objects
    private GameObject _worldScrollArea;
    private List<DelayGramPostObject> _worldPostObjects;
    private PostRequester _restRequester;
    private PostHelper _postHelper;
    private GameObject _loadingIcon;
    private GameObject _userStub = null;

    // For handling of selecting an image and resizing/repositioning
    private DelayGramPostObject? _currentSelectedImage;
    private Vector3 _originalImageScale;
    private Vector3 _originalImagePosition;
    private bool _imageCurrentlyShrinking = false;

    // User profile
    private GameObject _userProfileScreen;
    private GameObject _userProfileScrollArea;
    private List<DelayGramPostObject> _userProfilePostObjects;

    private enum HomeScreenState
    {
        WorldFeed,
        SinglePicture,
        UserProfile
    }
    private HomeScreenState _previousState = HomeScreenState.WorldFeed;
    private HomeScreenState _currentState = HomeScreenState.WorldFeed;

    // Use this for initialization
    private void Awake()
    {
        this._worldPostObjects = new List<DelayGramPostObject>();
        this._userProfilePostObjects = new List<DelayGramPostObject>();
        this._restRequester = new PostRequester();
        this._postHelper = new PostHelper();
	}

    private void Start()
    {
        DOTween.Init();
    }

    // Update is called once per frame
    private void Update () {
	}

    public void EnterScreen()
    {
        this._postPage = GameObject.Instantiate(Resources.Load("Home/HomePage") as GameObject);
        this._postPage.transform.position = new Vector3(0.0f, 0.25f, 0.0f);

        this._worldScrollArea = this._postPage.transform.Find("WorldScrollArea").gameObject;
        this._errorText = this._worldScrollArea.transform.Find("ErrorText").gameObject;

        this._currentState = HomeScreenState.WorldFeed;
        this.EnterWorldSection();
    }

    void OnDestroy()
    {
        this.DestroyPage();
    }

    public void HandleClick(string colliderName)
    {
        if (colliderName == "UserPageLink")
        {
            this.DestroyPage();
            this.ShowUserProfile();
        }
        else
        {
            foreach (DelayGramPostObject post in this._worldPostObjects)
            {
                if (post.postObject && colliderName == post.postObject.name)
                {
                    if (this._currentState == HomeScreenState.WorldFeed)
                    {
                        this.EnlargePost(post);
                    }
                    else if (this._currentState == HomeScreenState.SinglePicture)
                    {
                        this.ShrinkPost(this._currentSelectedImage.Value);
                    }
                }
            }
            foreach (DelayGramPostObject post in this._userProfilePostObjects)
            {
                if (post.postObject && colliderName == post.postObject.name)
                {
                    if (this._currentState == HomeScreenState.UserProfile)
                    {
                        this.EnlargePost(post);
                    }
                    else if (this._currentState == HomeScreenState.SinglePicture)
                    {
                        this.ShrinkPost(this._currentSelectedImage.Value);
                    }
                }
            }
        }
    }

    private void ShowUserProfile()
    {
        this._previousState = this._currentState;
        this._currentState = HomeScreenState.UserProfile;

        this._postPage.SetActive(false);
        this._currentSelectedImage.Value.postObject.SetActive(false);

        this._userProfileScreen = GameObject.Instantiate(Resources.Load("Profile/UserProfile") as GameObject);
        this._userProfileScreen.transform.position = new Vector3(0.2f, 1.3f, 0.0f);
        this._userProfileScrollArea = this._userProfileScreen.transform.Find("ScrollArea").gameObject;

        var topBackground = this._userProfileScrollArea.transform.Find("TopBackground");
        var nameText = topBackground.Find("NameText");
        var playerName = this._currentSelectedImage.Value.post.playerName;
        nameText.GetComponent<TextMeshPro>().text = playerName;

        var characterSection = this._userProfileScrollArea.transform.Find("CharacterSection");
        var spriteMask = characterSection.Find("SpriteMask");
        var femaleAvatar = spriteMask.Find("FemaleAvatar");
        var maleAvatar = spriteMask.Find("MaleAvatar");

        var characterProperties = this._currentSelectedImage.Value.post.characterProperties;
        var levelBanner = characterSection.Find("LevelBanner");
        var levelNumberText = levelBanner.Find("LevelNumberText");
        levelNumberText.GetComponent<TextMeshPro>().text = characterProperties.avatarLevel.ToString();

        if (characterProperties.gender == Gender.Female)
        {
            femaleAvatar.GetComponent<AvatarController>().SetCharacterLook(characterProperties);
            maleAvatar.gameObject.SetActive(false);
        }
        else
        {
            maleAvatar.GetComponent<AvatarController>().SetCharacterLook(characterProperties);
            femaleAvatar.gameObject.SetActive(false);
        }

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);
        this._loadingIcon.transform.position = new Vector3(0.0f, -1.35f, 0.0f);
        this._loadingIcon.transform.parent = this._userProfileScrollArea.transform;

        this._restRequester.RequestAllUserPosts(playerName, this.UpdateUserProfilePosts);
        // Populate profile page (character, items, level)
    }

    private void UpdateUserProfilePosts(PictureArrayJson pictures, bool success)
    {
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }

        if (success)
        {
            if (pictures.pictureModels.Length > 0)
            {
                this.GenerateUserProfilePostObjects(pictures);
            }
        }
    }

    private void GenerateUserProfilePostObjects(PictureArrayJson pictureArray)
    {
        var posts = new List<DelayGramPost>();
        foreach (PictureModelJsonReceive picture in pictureArray.pictureModels)
        {
            // Create a picture with information from picture
            var newPost = this._restRequester.ConvertJsonPictureIntoDelayGramPost(picture);

            posts.Add(newPost);
        }
        var feedLength = this._postHelper.GeneratePostFeed(
            this._userProfileScrollArea, posts, this._userProfilePostObjects, PROFILE_POST_X_OFFSET, PROFILE_POST_Y_OFFSET);
        var scrollController = this._userProfileScrollArea.GetComponent<ScrollController>();
        var topSectionHeight = PROFILE_POST_Y_OFFSET * -1;
        scrollController.UpdateScrollArea(topSectionHeight + feedLength);
    }

    private void EnlargePost(DelayGramPostObject post)
    {
        if (this._imageCurrentlyShrinking)
        {
            return;
        }

        if (this._currentState == HomeScreenState.WorldFeed)
        {
            foreach (DelayGramPostObject newPostObject in this._worldPostObjects)
            {
                if (newPostObject.postObject.name != post.postObject.name)
                {
                    newPostObject.postObject.SetActive(false);
                }
            }
        }
        else if (this._currentState == HomeScreenState.UserProfile)
        {
            this._userProfileScrollArea.transform.Find("TopBackground").gameObject.SetActive(false);
            this._userProfileScrollArea.transform.Find("CharacterSection").gameObject.SetActive(false);
            foreach (DelayGramPostObject newPostObject in this._userProfilePostObjects)
            {
                if (newPostObject.postObject.name != post.postObject.name)
                {
                    newPostObject.postObject.SetActive(false);
                }
            }
        }

        this._previousState = this._currentState;
        this._currentState = HomeScreenState.SinglePicture;
        this._currentSelectedImage = post;
        this._originalImageScale = post.postObject.transform.localScale;
        this._originalImagePosition = post.postObject.transform.localPosition;

        this._userStub = this._postHelper.EnlargeAndCenterPost(post);

        post.postObject.transform.parent = null;
    }

    private void ShrinkPost(DelayGramPostObject post)
    {
        this._currentState = this._previousState;
        this._previousState = HomeScreenState.SinglePicture;
        this._imageCurrentlyShrinking = true;

        // Scale post down and position where it used to be
        this._postHelper.SetPostDetails(post.postObject, post.post, false, true);
        this._postHelper.ShrinkAndReturnPost(
            post,
            this._originalImageScale,
            this._originalImagePosition,
            () => this.PostFinishedShrinking(post, false));

        if (this._currentState == HomeScreenState.WorldFeed)
        {
            post.postObject.transform.parent = this._worldScrollArea.transform;

            foreach (DelayGramPostObject newPostObject in this._worldPostObjects)
            {
                if (newPostObject.postObject.name != post.postObject.name)
                {
                    newPostObject.postObject.SetActive(true);
                }
            }
        }
        else if (this._currentState == HomeScreenState.UserProfile)
        {
            this._userProfileScrollArea.transform.Find("TopBackground").gameObject.SetActive(true);
            this._userProfileScrollArea.transform.Find("CharacterSection").gameObject.SetActive(true);
            post.postObject.transform.parent = this._userProfileScrollArea.transform;

            foreach (DelayGramPostObject newPostObject in this._userProfilePostObjects)
            {
                if (newPostObject.postObject.name != post.postObject.name)
                {
                    newPostObject.postObject.SetActive(true);
                }
            }
        }

        if (this._userStub != null)
        {
            GameObject.Destroy(this._userStub);
        }
    }

    private void PostFinishedShrinking(DelayGramPostObject postObject, bool showDetails)
    {
        this._imageCurrentlyShrinking = false;
    }

    public void DestroyPage()
    {
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();
        GameObject.Destroy(this._postPage);

        DestroyPosts(this._userProfilePostObjects);
        this._userProfilePostObjects.Clear();

        if (this._currentSelectedImage.HasValue)
        {
            GameObject.Destroy(this._currentSelectedImage.Value.postObject);
        }
        if (this._userProfileScreen)
        {
            GameObject.Destroy(this._userProfileScreen);
        }
        if (this._userStub != null)
        {
            GameObject.Destroy(this._userStub);
        }
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }
    }

    public bool BackOut()
    {
        return true;
    }

    private void EnterWorldSection()
    {
        this._worldScrollArea.SetActive(true);
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();

        DestroyPosts(this._userProfilePostObjects);
        this._userProfilePostObjects.Clear();

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);

        this._restRequester.RequestRecentPosts(30, this.UpdateWorldPosts);
    }

    private void UpdateWorldPosts(PictureArrayJson pictures, bool success)
    {
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }
        if (!this._postPage)
        {
            return;
        }

        if (!success || pictures == null)
        {
            if (this._errorText)
            {
                this._errorText.SetActive(true);
                this._errorText.GetComponent<TextMeshPro>().text = "No internet connection.";
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

    private void DestroyPosts(List<DelayGramPostObject> postObjects)
    {
        foreach (DelayGramPostObject post in postObjects)
        {
            if (post.postObject)
            {
                post.postObject.SetActive(false);
                GameObject.Destroy(post.postObject);
            }
        }
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
        var feedLength = this._postHelper.GeneratePostFeed(
            this._worldScrollArea, posts, this._worldPostObjects, POST_X_OFFSET, POST_Y_OFFSET);
        var scrollController = this._worldScrollArea.GetComponent<ScrollController>();
        scrollController.UpdateScrollArea(feedLength);
    }
}
