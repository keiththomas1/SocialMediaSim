using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;

public class HomeScreenController : MonoBehaviour {
    [SerializeField]
    private Sprite _leftSideNav;
    [SerializeField]
    private Sprite _rightSideNav;

    private const float POST_X_OFFSET = -0.87f;
    private const float POST_Y_OFFSET = 2.84f;
    private GameObject _postPage;
    private GameObject _errorText;

    // World objects
    private GameObject _worldScrollArea;
    private List<DelayGramPostObject> _worldPostObjects;
    private RESTRequester _restRequester;
    private PostHelper _postHelper;
    private GameObject _loadingIcon;

    // For handling of selecting an image and resizing/repositioning
    private DelayGramPostObject _currentSelectedImage;
    private Vector3 _originalImageScale;
    private Vector3 _originalImagePosition;
    private bool _imageCurrentlyShrinking = false;

    private enum HomeScreenState
    {
        WorldFeed,
        SinglePicture
    }
    private HomeScreenState _currentState = HomeScreenState.WorldFeed;

    // Use this for initialization
    private void Awake()
    {
        this._worldPostObjects = new List<DelayGramPostObject>();
        this._restRequester = new RESTRequester();
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

    public void CheckClick(string colliderName)
    {
        foreach(DelayGramPostObject post in this._worldPostObjects)
        {
            if (post.postObject && colliderName == post.postObject.name)
            {
                if (this._currentState == HomeScreenState.WorldFeed)
                {
                    this.EnlargePost(post);
                } else {
                    this.ShrinkPost(this._currentSelectedImage);
                }
            }
        }
    }

    private void EnlargePost(DelayGramPostObject post)
    {
        if (this._imageCurrentlyShrinking)
        {
            return;
        }

        this._currentState = HomeScreenState.SinglePicture;
        this._currentSelectedImage = post;
        this._originalImageScale = post.postObject.transform.localScale;
        this._originalImagePosition = post.postObject.transform.localPosition;

        this._postHelper.EnlargeAndCenterPost(post);

        foreach(DelayGramPostObject newPostObject in this._worldPostObjects)
        {
            if (newPostObject.postObject.name != post.postObject.name)
            {
                newPostObject.postObject.SetActive(false);
            }
        }
    }

    private void ShrinkPost(DelayGramPostObject post)
    {
        this._currentState = HomeScreenState.WorldFeed;
        this._imageCurrentlyShrinking = true;

        // Scale post down and position where it used to be
        this._postHelper.ShrinkAndReturnPost(
            post,
            this._originalImageScale,
            this._originalImagePosition,
            () => this.PostFinishedShrinking(post, false));

        foreach (DelayGramPostObject newPostObject in this._worldPostObjects)
        {
            if (newPostObject.postObject.name != post.postObject.name)
            {
                newPostObject.postObject.SetActive(true);
            }
        }
    }

    private void PostFinishedShrinking(DelayGramPostObject postObject, bool showDetails)
    {
        this._imageCurrentlyShrinking = false;
        this._postHelper.SetPostDetails(postObject.postObject, postObject.post, false, true);
    }

    public void DestroyPage()
    {
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();
        GameObject.Destroy(this._postPage);
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

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);

        this._restRequester.RequestLastTenPosts(this.UpdateWorldPosts);
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
        this._postHelper.GeneratePostFeed(
            this._worldScrollArea, posts, this._worldPostObjects, POST_X_OFFSET, POST_Y_OFFSET);
    }
}
