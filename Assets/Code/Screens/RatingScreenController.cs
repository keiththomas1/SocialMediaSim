using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class RatingScreenController : MonoBehaviour
{
    private PostRequester _restRequester;
    private PostHelper _postHelper;
    private UserSerializer _userSerializer;
    private GoalsController _goalsController;

    private GameObject _ratingPage;
    private GameObject _errorText;

    private Queue<PictureModelJsonReceive> _currentPictures;
    private PictureModelJsonReceive _currentTopPicture;
    private GameObject _currentTopPost;
    private PictureModelJsonReceive _currentBottomPicture;
    private GameObject _currentBottomPost;
    private GameObject _vsGraphic;

    private bool _loadingPictures = false;
    private bool _canRate = false;
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
        this._restRequester = new PostRequester();
        this._postHelper = new PostHelper();
        this._userSerializer = UserSerializer.Instance;
        this._goalsController = this.GetComponent<GoalsController>();
    }

    public void HandleClick(string colliderName)
    {
        switch (colliderName)
        {
            case "TopPost":
                if (this._canRate)
                {
                    this._canRate = false;
                    GameObject.Destroy(this._vsGraphic);
                    this.RatePicture(this._currentTopPicture, this._currentTopPost, RatingType.Liked);
                    this.RatePicture(this._currentBottomPicture, this._currentBottomPost, RatingType.Disliked);
                    this.CreatePostPair();
                    this.IterateRatings();
                }
                break;
            case "BottomPost":
                if (this._canRate)
                {
                    this._canRate = false;
                    GameObject.Destroy(this._vsGraphic);
                    this.RatePicture(this._currentBottomPicture, this._currentBottomPost, RatingType.Liked);
                    this.RatePicture(this._currentTopPicture, this._currentTopPost, RatingType.Disliked);
                    this.CreatePostPair();
                    this.IterateRatings();
                }
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

        if (this._loadingIcon)
        {
            GameObject.Destroy(this._loadingIcon);
        }
    }

    private void IterateRatings()
    {
        this._goalsController.AddRatingsGoalProgress(1);
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

    private void RatePicture(
        PictureModelJsonReceive picture,
        GameObject pictureObject,
        RatingType ratingType)
    {
        switch (ratingType)
        {
            case RatingType.Liked:
                var likeTint = pictureObject.transform.Find("LikeTint");
                likeTint.GetComponent<SpriteRenderer>().enabled = true;
                pictureObject.transform.DOMoveX(4.0f, 0.5f)
                    .SetEase(Ease.InBack);

                var addLike = this._restRequester.AddLikeToPicture(picture._id, this._userSerializer.PlayerId);
                StartCoroutine(addLike);
                break;
            case RatingType.Equal:
                break;
            case RatingType.Disliked:
                var dislikeTint = pictureObject.transform.Find("DislikeTint");
                dislikeTint.GetComponent<SpriteRenderer>().enabled = true;
                pictureObject.transform.DOMoveX(-4.0f, 0.5f)
                    .SetEase(Ease.InBack);

                var addDislike = this._restRequester.AddDislikeToPicture(picture._id, this._userSerializer.PlayerId);
                StartCoroutine(addDislike);
                break;
        }

        var deathByTimer = pictureObject.AddComponent<DeathByTimer>();
        deathByTimer.deathTimeInSeconds = 1.0f;
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

        this._vsGraphic = GameObject.Instantiate(Resources.Load("Rating/VsGraphic") as GameObject);
        this._vsGraphic.transform.SetParent(this._ratingPage.transform);
        this._vsGraphic.transform.localPosition = new Vector3(0.045f, .2f, -2.0f);
    }

    private GameObject CreatePost(DelayGramPost postData)
    {
        var postObject = GameObject.Instantiate(Resources.Load("Rating/RatingPost") as GameObject);
        postObject.name = "ExplorePost";
        postObject.transform.parent = this._ratingPage.transform;

        postObject.transform.localScale = new Vector3(0.0f, 0.0f, 0.0f);
        postObject.transform.DOScale(new Vector3(1.2f, 1.2f, 1.0f), 0.7f)
            .SetEase(Ease.InCubic)
            .OnComplete(() => { this._canRate = true; });

        var newPost = postObject.transform.Find("NewPost").gameObject;
        this._postHelper.PopulatePostFromData(newPost, postData);
        this._postHelper.SetPostDetails(newPost, postData, false, true);

        return postObject;
    }
}
