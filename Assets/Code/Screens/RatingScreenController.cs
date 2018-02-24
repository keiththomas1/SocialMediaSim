using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class RatingScreenController : MonoBehaviour
{
    private RESTRequester _restRequester;
    private PostHelper _postHelper;

    private GameObject _ratingPage;
    private GameObject _errorText;

    private Queue<PictureModelJsonReceive> _currentPictures;
    private PictureModelJsonReceive _currentTopPicture;
    private GameObject _currentTopPost;
    private PictureModelJsonReceive _currentBottomPicture;
    private GameObject _currentBottomPost;

    private bool _loadingPictures = false;
    private GameObject _loadingIcon;

    private GameObject _swipeCountText;
    private int _swipesLeft = 15;

    private enum RatingType
    {
        Liked,
        Disliked,
        Equal
    }

    // Use this for initialization
    void Start()
    {
        this._restRequester = new RESTRequester();
        this._postHelper = new PostHelper();
    }

    void Update()
    {
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            case "TopPost":
                GameObject.Destroy(this._currentTopPost);
                GameObject.Destroy(this._currentBottomPost);
                this.RatePicture(this._currentTopPicture, RatingType.Liked);
                this.RatePicture(this._currentBottomPicture, RatingType.Disliked);
                this.CreatePostPair();
                this.IterateRatings();
                break;
            case "BottomPost":
                GameObject.Destroy(this._currentTopPost);
                GameObject.Destroy(this._currentBottomPost);
                this.RatePicture(this._currentBottomPicture, RatingType.Liked);
                this.RatePicture(this._currentTopPicture, RatingType.Disliked);
                this.CreatePostPair();
                this.IterateRatings();
                break;
        }
    }

    public void EnterScreen()
    {
        this._ratingPage = GameObject.Instantiate(Resources.Load("Rating/RatingPage") as GameObject);
        this._ratingPage.transform.position = new Vector3(0.0f, 0.5f, 0.0f);

        var headerText = this._ratingPage.transform.Find("HeaderText");
        this._swipeCountText = headerText.Find("SwipeText2").gameObject;
        this._swipeCountText.GetComponent<TextMeshPro>().text = this._swipesLeft.ToString();

        this._loadingIcon = GameObject.Instantiate(Resources.Load("LoadingIcon") as GameObject);
        this._loadingIcon.transform.position = new Vector3(0.0f, 0.65f, 0.0f);

        this._errorText = this._ratingPage.transform.Find("ErrorText").gameObject;
        this._loadingPictures = true;
        this._restRequester.RequestNeededFeedbackPosts(10, this.SetPhotos);
    }

    public void SetPhotos(PictureArrayJson pictures, bool success)
    {
        this._loadingPictures = false;

        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }

        if (success)
        {
            if (pictures.pictureModels.Length > 0)
            {
                this._currentPictures = new Queue<PictureModelJsonReceive>();
                foreach (PictureModelJsonReceive picture in pictures.pictureModels)
                {
                    this._currentPictures.Enqueue(picture);
                }
                this.CreatePostPair();
            }
            else
            {
                if (this._errorText)
                {
                    this._errorText.gameObject.SetActive(true);
                    this._errorText.GetComponent<TextMeshPro>().color = new Color(.98f, 0.86f, 0.07f);
                    this._errorText.GetComponent<TextMeshPro>().text = "No more pictures to swipe. Please check back later!";
                }
            }
        }
        else
        {
            if (this._ratingPage)
            {
                if (this._errorText)
                {
                    this._errorText.gameObject.SetActive(true);
                    this._errorText.GetComponent<TextMeshPro>().color = new Color(1.0f, 0.43f, 0.43f);
                    this._errorText.GetComponent<TextMeshPro>().text = "No internet connection.";
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
        GameObject.Destroy(this._ratingPage);
    }

    private void IterateRatings()
    {
        this._swipesLeft--;
        if (this._swipesLeft <= 0)
        {
            if (this._errorText)
            {
                this._errorText.gameObject.SetActive(true);
                this._errorText.GetComponent<TextMeshPro>().color = new Color(1.0f, 0.43f, 0.43f);
                this._errorText.GetComponent<TextMeshPro>().text = "No more pictures to swipe. Please check back later!";
            }
        }
        this._swipeCountText.GetComponent<TextMeshPro>().text = this._swipesLeft.ToString();
    }

    private void RatePicture(PictureModelJsonReceive picture, RatingType ratingType)
    {
        switch (ratingType)
        {
            case RatingType.Liked:
            case RatingType.Equal:
                var addLike = this._restRequester.AddLikeToPicture(picture._id);
                StartCoroutine(addLike);
                break;
            case RatingType.Disliked:
                var addDislike = this._restRequester.AddDislikeToPicture(picture._id);
                StartCoroutine(addDislike);
                break;
        }
    }

    private void CreatePostPair()
    {
        if (this._loadingPictures)
        {
            return;
        }
        if (this._currentPictures.Count < 2)
        {
            this._loadingPictures = true;
            this._restRequester.RequestNeededFeedbackPosts(15, this.SetPhotos);
            return;
        }
        this._currentTopPicture = this._currentPictures.Dequeue();
        var topData = this._restRequester.ConvertJsonPictureIntoDelayGramPost(this._currentTopPicture);
        this._currentTopPost = this.CreatePost(topData);
        this._currentTopPost.transform.localPosition = new Vector3(0.0f, 1.15f, 0.0f);
        this._currentTopPost.name = "TopPost";

        this._currentBottomPicture = this._currentPictures.Dequeue();
        var bottomData = this._restRequester.ConvertJsonPictureIntoDelayGramPost(this._currentBottomPicture);
        this._currentBottomPost = this.CreatePost(bottomData);
        this._currentBottomPost.transform.localPosition = new Vector3(0.0f, -0.91f, 0.0f);
        this._currentBottomPost.name = "BottomPost";
    }

    private GameObject CreatePost(DelayGramPost postData)
    {
        var postObject = GameObject.Instantiate(Resources.Load("Rating/RatingPost") as GameObject);
        postObject.name = "ExplorePost";
        postObject.transform.parent = this._ratingPage.transform;

        postObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        postObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.0f), 0.5f).SetEase(Ease.OutSine);

        var newPost = postObject.transform.Find("NewPost").gameObject;
        this._postHelper.PopulatePostFromData(newPost, postData);

        return postObject;
    }
}
