using UnityEngine;
using System;
using System.Collections.Generic;
using TMPro;

public class HomeScreenController : MonoBehaviour {
    [SerializeField]
    private Sprite _leftSideNav;
    [SerializeField]
    private Sprite _rightSideNav;

    private const float POST_X_OFFSET = -0.05f;
    private const float POST_Y_OFFSET = 1.34f;
    private UserSerializer _userSerializer;
    private GameObject _postPage;
    private GameObject _errorText;

    // World objects
    private GameObject _worldScrollArea;
    private ScrollController _worldScrollController;
    private List<GameObject> _worldPostObjects;
    private RESTRequester _restRequester;
    private PostHelper _postHelper;
    private GameObject _loadingIcon;

    // Use this for initialization
    void Awake()
    {
        this._userSerializer = UserSerializer.Instance;
        this._worldPostObjects = new List<GameObject>();
        this._restRequester = new RESTRequester();
        this._postHelper = new PostHelper();
	}
	
	// Update is called once per frame
	void Update () {
	}

    public void EnterScreen()
    {
        this._userSerializer = UserSerializer.Instance;

        this._postPage = GameObject.Instantiate(Resources.Load("Home/HomePage") as GameObject);
        this._postPage.transform.position = new Vector3(0.0f, 0.25f, 0.0f);

        this._worldScrollArea = this._postPage.transform.Find("WorldScrollArea").gameObject;
        this._errorText = this._worldScrollArea.transform.Find("ErrorText").gameObject;

        this.EnterWorldSection();
    }

    void OnDestroy()
    {
        this.DestroyPage();
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            default:
                break;
        }
    }
    
    public void DestroyPage()
    {
        DestroyPosts(this._worldPostObjects);
        this._worldPostObjects.Clear();
        GameObject.Destroy(this._postPage);
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
            this._worldScrollArea, posts, this._worldPostObjects, this._worldScrollController, POST_X_OFFSET, POST_Y_OFFSET);
    }
}
