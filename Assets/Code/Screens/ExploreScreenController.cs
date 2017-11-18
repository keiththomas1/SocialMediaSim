using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class ExploreScreenController : MonoBehaviour
{
    private GameObject _explorePage;
    private GameObject _dislikeBorder;
    private GameObject _likeBorder;
    private RESTRequester _restRequester;
    private PostHelper _postHelper;

    private GameObject _currentDragObject;
    private Vector3 _dragStartMouseDifference;
    private float _dragObjectDepth;

    private Queue<PictureModelJsonReceive> _currentPictures;
    private PictureModelJsonReceive _currentPicture;
    private bool _loadingPictures = false;
    private GameObject _loadingIcon;

    // Use this for initialization
    void Awake()
    {
        this._restRequester = new RESTRequester();
        this._postHelper = new PostHelper();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (this._currentDragObject)
            {
                if (this._currentDragObject.transform.position.x <= -1.0f)
                { // Dislike
                    this.DislikePicture();
                }
                else if (this._currentDragObject.transform.position.x >= 1.0f)
                { // Like
                    this.LikePicture();
                }

                this._currentDragObject = null;
            }
        }

        if (this._currentDragObject)
        {
            var newObjectPosition =
                Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._dragStartMouseDifference;
            newObjectPosition.z = this._dragObjectDepth;
            this._currentDragObject.transform.position = newObjectPosition;

            if (newObjectPosition.x <= -1.0f)
            { // Dislike
                this._dislikeBorder.SetActive(true);
            }
            else if (newObjectPosition.x >= 1.0f)
            { // Like
                this._likeBorder.SetActive(true);
            }
            else
            {
                if (this._likeBorder.activeSelf == true)
                {
                    this._likeBorder.SetActive(false);
                }
                if (this._dislikeBorder.activeSelf == true)
                {
                    this._dislikeBorder.SetActive(false);
                }
            }
        }
    }

    // Update is called once per frame
    public void CheckClick(Collider collider)
    {
        switch (collider.name)
        {
            case "ExplorePost":
                this._currentDragObject = collider.gameObject;
                this._dragStartMouseDifference =
                    Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._currentDragObject.transform.position;
                this._dragObjectDepth = this._currentDragObject.transform.position.z;
                break;
        }
    }

    public void EnterScreen()
    {
        this._explorePage = GameObject.Instantiate(Resources.Load("Explore/ExplorePage") as GameObject);
        this._explorePage.transform.position = new Vector3(0.0f, 0.5f, 0.0f);
        this._dislikeBorder = this._explorePage.transform.Find("DislikeBorder").gameObject;
        this._dislikeBorder.SetActive(false);
        this._likeBorder = this._explorePage.transform.Find("LikeBorder").gameObject;
        this._likeBorder.SetActive(false);

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);

        this._loadingPictures = true;
        this._restRequester.RequestLastTenPosts(this.SetPhotos);
        // StartCoroutine(requestLastTen);
    }

    public void SetPhotos(PictureArrayJson pictures, bool success)
    {
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }

        this._currentPictures = new Queue<PictureModelJsonReceive>();
        foreach(PictureModelJsonReceive picture in pictures.pictureModels)
        {
            this._currentPictures.Enqueue(picture);
        }
        this._loadingPictures = false;
        this.CreateNewExplorePost();
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
        GameObject.Destroy(this._explorePage);
    }

    private void LikePicture()
    {
        var addLike = this._restRequester.AddLikeToPicture(this._currentPicture._id);
        StartCoroutine(addLike);

        CreateNewExplorePost();
        DisableCurrentDragObject(PictureRotateAway.RotateDirection.Right);
        this._likeBorder.SetActive(false);

        // GameObject.Instantiate(Resources.Load("Explore/Upvote") as GameObject);
    }

    private void DislikePicture()
    {
        var addDislike = this._restRequester.AddDislikeToPicture(this._currentPicture._id);
        StartCoroutine(addDislike);

        CreateNewExplorePost();
        DisableCurrentDragObject(PictureRotateAway.RotateDirection.Left);
        this._dislikeBorder.SetActive(false);

        // GameObject.Instantiate(Resources.Load("Explore/Downvote") as GameObject);
    }

    private void DisableCurrentDragObject(PictureRotateAway.RotateDirection rotateDirection)
    {
        this._currentDragObject.name = "DeadPost";
        var deathByTimer = this._currentDragObject.AddComponent<DeathByTimer>();
        deathByTimer.deathTimeInSeconds = 2.0f;
        this._currentDragObject.GetComponent<PictureRotateAway>().StartAnimation(rotateDirection);
    }

    private void CreateNewExplorePost()
    {
        if (this._loadingPictures)
        {
            return;
        }
        if (this._currentPictures.Count == 0)
        {
            this._loadingPictures = true;
            this._restRequester.RequestLastTenPosts(this.SetPhotos);
            // StartCoroutine(requestLastTen);
            return;
        }
        this._currentPicture = this._currentPictures.Dequeue();
        var data = this._restRequester.ConvertJsonPictureIntoDelayGramPost(this._currentPicture);

        var explorePost = GameObject.Instantiate(Resources.Load("Explore/ExplorePost") as GameObject);
        explorePost.name = "ExplorePost";
        explorePost.transform.parent = this._explorePage.transform;
        explorePost.transform.localPosition = new Vector3(0.0f, 0.66f, 0.0f);
        var newPost = explorePost.transform.Find("NewPost").gameObject;

        this._postHelper.PopulatePostFromData(newPost, data);
    }
}
