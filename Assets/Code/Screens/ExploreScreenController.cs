using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;

public class ExploreScreenController : MonoBehaviour
{
    private RESTRequester _restRequester;
    private PostHelper _postHelper;
    private MessagePost _messagePost;

    private GameObject _explorePage;

    private SpriteRenderer _dislikeBorder;
    private Color _originalDislikeColor;
    private Color _transparentDislikeColor;
    private SpriteRenderer _likeBorder;
    private Color _originalLikeColor;
    private Color _transparentLikeColor;

    private GameObject _currentDragObject;
    private Vector3 _dragStartMouseDifference;
    private float _dragObjectDepth;

    private Queue<PictureModelJsonReceive> _currentPictures;
    private PictureModelJsonReceive _currentPicture;
    private bool _loadingPictures = false;
    private GameObject _loadingIcon;

    private GameObject _swipeCountText;
    private int _swipesLeft = 10;

    // Use this for initialization
    void Start()
    {
        this._restRequester = new RESTRequester();
        this._postHelper = new PostHelper();
        this._messagePost = MessagePost.Instance;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.name == "ExplorePost")
                {
                    this._currentDragObject = hit.collider.gameObject;
                    this._dragStartMouseDifference =
                        Camera.main.ScreenToWorldPoint(Input.mousePosition) - this._currentDragObject.transform.position;
                    this._dragObjectDepth = this._currentDragObject.transform.position.z;
                }
            }
        }
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
                this.ShowDislikeBar();
                this._dislikeBorder.color = this._originalDislikeColor;
            }
            else if (newObjectPosition.x >= 1.0f)
            { // Like
                this.ShowLikeBar();
                this._likeBorder.color = this._originalLikeColor;
            }
            else
            {
                if (this._dislikeBorder.color != this._transparentDislikeColor)
                {
                    this.HideDislikeBar();
                    this._dislikeBorder.color = this._transparentDislikeColor;
                }
                if (this._likeBorder.color != this._transparentLikeColor)
                {
                    this.HideLikeBar();
                    this._likeBorder.color = this._transparentLikeColor;
                }
            }
        }
    }

    public void CheckClick(string colliderName)
    {
    }

    public void EnterScreen()
    {
        this._explorePage = GameObject.Instantiate(Resources.Load("Explore/ExplorePage") as GameObject);
        this._explorePage.transform.position = new Vector3(0.0f, 0.5f, 0.0f);

        this._dislikeBorder = this._explorePage.transform.Find("DislikeBorder").GetComponent<SpriteRenderer>();
        this._originalDislikeColor = this._dislikeBorder.color;
        this._transparentDislikeColor = this._dislikeBorder.color;
        this._transparentDislikeColor.a = 0.45f;
        this._dislikeBorder.color = this._transparentDislikeColor;
        this.HideDislikeBar();

        this._likeBorder = this._explorePage.transform.Find("LikeBorder").GetComponent<SpriteRenderer>();
        this._originalLikeColor = this._likeBorder.color;
        this._transparentLikeColor = this._likeBorder.color;
        this._transparentLikeColor.a = 0.45f;
        this._likeBorder.color = this._transparentLikeColor;
        this.HideLikeBar();

        this._swipeCountText = this._explorePage.transform.Find("SwipeText2").gameObject;
        this._swipeCountText.GetComponent<TextMeshPro>().text = this._swipesLeft.ToString();

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);
        this._loadingIcon.transform.position = new Vector3(0.0f, 0.65f, 0.0f);

        this._loadingPictures = true;
        this._restRequester.RequestLastTenPosts(this.SetPhotos);
    }

    public void SetPhotos(PictureArrayJson pictures, bool success)
    {
        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }

        if (success)
        {
            this._currentPictures = new Queue<PictureModelJsonReceive>();
            foreach (PictureModelJsonReceive picture in pictures.pictureModels)
            {
                this._currentPictures.Enqueue(picture);
            }
            this._loadingPictures = false;
            this.CreateNewExplorePost();
        } else {
            if (this._explorePage)
            {
                var errorText = this._explorePage.transform.Find("ErrorText");
                if (errorText)
                {
                    errorText.gameObject.SetActive(true);
                    errorText.GetComponent<TextMeshPro>().text = "No internet connection.";
                }
            }
        }
    }

    public bool BackOut()
    {
        return true;
    }

    public void DestroyPage()
    {
        GameObject.Destroy(this._explorePage);
    }

    private void IterateSwipes()
    {
        this._swipesLeft--;
        if (this._swipesLeft <= 0)
        {
            this._messagePost.TriggerActivated(MessageTriggerType.SwipeGoal);
            this._swipesLeft = 10;
        }
        this._swipeCountText.GetComponent<TextMeshPro>().text = this._swipesLeft.ToString();
    }

    private void LikePicture()
    {
        var addLike = this._restRequester.AddLikeToPicture(this._currentPicture._id);
        StartCoroutine(addLike);

        CreateNewExplorePost();
        DisableCurrentDragObject(PictureRotateAway.RotateDirection.Right);
        this._likeBorder.color = this._transparentLikeColor;

        this.IterateSwipes();
        this.HideLikeBar();

        // GameObject.Instantiate(Resources.Load("Explore/Upvote") as GameObject);
    }

    private void DislikePicture()
    {
        var addDislike = this._restRequester.AddDislikeToPicture(this._currentPicture._id);
        StartCoroutine(addDislike);

        CreateNewExplorePost();
        DisableCurrentDragObject(PictureRotateAway.RotateDirection.Left);
        this._dislikeBorder.color = this._transparentDislikeColor;

        this.IterateSwipes();
        this.HideDislikeBar();

        // GameObject.Instantiate(Resources.Load("Explore/Downvote") as GameObject);
    }

    private void HideDislikeBar()
    {
        this._dislikeBorder.transform.DOLocalMoveX(-2.7f, 1.0f).SetEase(Ease.OutSine);
    }
    private void HideLikeBar()
    {
        this._likeBorder.transform.DOLocalMoveX(2.7f, 1.0f).SetEase(Ease.OutSine);
    }
    private void ShowDislikeBar()
    {
        this._dislikeBorder.transform.DOLocalMoveX(-1.7f, 0.5f).SetEase(Ease.OutSine);
    }
    private void ShowLikeBar()
    {
        this._likeBorder.transform.DOLocalMoveX(1.74f, 0.5f).SetEase(Ease.OutSine);
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

        explorePost.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        explorePost.transform.DOScale(new Vector3(0.7f, 0.7f, 1.0f), 0.5f).SetEase(Ease.OutSine);

        this._postHelper.PopulatePostFromData(newPost, data);
    }
}
