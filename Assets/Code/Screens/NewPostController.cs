using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public delegate void CreatePostCallBack(DelayGramPost post);

public class NewPostController : MonoBehaviour
{
    private UserSerializer _userSerializer;
    private CharacterSerializer characterSerializer;
    private MessagePost _messagePost;
    private PostHelper _postHelper;
    private RESTRequester _restRequester;

    private GameObject _postPopupWindow;
    private Transform scrollArea;
    private GameObject _avatar;
    private CreatePostCallBack _postCallBack;

    private string _currentBackground;
    private List<PictureItem> _items;
    private List<GameObject> _itemObjects;

    static private string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private enum NewPostState
    {
        BackgroundSelection,
        Cropping
    }
    private NewPostState _currentPostState;

    void Start()
    {
        this._userSerializer = UserSerializer.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._messagePost = MessagePost.Instance;
        this._postHelper = new PostHelper();
        this._restRequester = new RESTRequester();

        this.Initialize();
    }

    void Update()
    {
    }

    public bool PopupActive()
    {
        return this._postPopupWindow;
    }

    public void CreatePopup(CreatePostCallBack callBack)
    {
        this.Initialize();
        this._postCallBack = callBack;
        this._postPopupWindow = GameObject.Instantiate(Resources.Load("Posts/NewPostPopup") as GameObject);
        this._postPopupWindow.transform.position = new Vector3(2.24f, 0.55f, 0.0f);

        this._postPopupWindow.transform.Find("BackButton").gameObject.SetActive(false);

        var newPost = this._postPopupWindow.transform.Find("NewPost");
        var picture = newPost.transform.Find("Picture");
        switch (this.characterSerializer.Gender)
        {
            case Gender.Male:
                this._avatar = picture.transform.Find("MaleAvatar").gameObject;
                break;
            case Gender.Female:
            default:
                this._avatar = picture.transform.Find("FemaleAvatar").gameObject;
                break;
        }
        this._avatar.SetActive(true);

        this._itemObjects = this.SetupItemsInPost(picture.gameObject);
        var movableObjects = this._itemObjects;
        movableObjects.Add(this._avatar);

        var croppingController = picture.GetComponent<CroppingController>();
        croppingController.SetMovableItems(movableObjects);
    }

    public void CheckClick(string colliderName)
    {
        
        switch (colliderName)
        {
            case "NewPostDoneButton":
                this.CreateNewPost();
                this.DestroyPage();
                break;
            case "Beach":
                this._currentBackground = "Beach";
                this.GotoNewState(NewPostState.Cropping);
                break;
            case "City":
                this._currentBackground = "City";
                this.GotoNewState(NewPostState.Cropping);
                break;
            case "Louvre":
                this._currentBackground = "Louvre";
                this.GotoNewState(NewPostState.Cropping);
                break;
            case "Park":
                this._currentBackground = "Park";
                this.GotoNewState(NewPostState.Cropping);
                break;
            case "CamRoom":
                this._currentBackground = "CamRoom";
                this.GotoNewState(NewPostState.Cropping);
                break;
            case "BackButton":
                this.BackButtonPressed();
                break;
        }
    }

    public bool BackOut()
    {
        if (this._currentPostState != NewPostState.BackgroundSelection)
        {
            this.BackButtonPressed();
            return false;
        }
        return true;
    }

    public void DestroyPage()
    {
        GameObject.Destroy(this._postPopupWindow);
    }

    public string GetRandomImageID()
    {
        string id = "";

        for (int i = 0; i < 20; i++)
        {
            var randomIndex = UnityEngine.Random.Range(0, characters.Length);
            id += characters[randomIndex];
        }

        return id;
    }

    private void Initialize()
    {
        this._currentPostState = NewPostState.BackgroundSelection;
        this._items = new List<PictureItem>();
        this._itemObjects = new List<GameObject>();
    }

    private List<GameObject> SetupItemsInPost(GameObject pictureObject)
    {
        if (this._userSerializer.HasBulldog)
        {
            var bulldog = new PictureItem();
            bulldog.name = "Bulldog";
            bulldog.location = new SerializableVector3(new Vector3(1.2f, -0.5f, 0.0f));
            bulldog.rotation = 0;
            bulldog.scale = 0.45f;
            this._items.Add(bulldog);
        }
        if (this._userSerializer.HasCat)
        {
            var cat = new PictureItem();
            cat.name = "Sylvester";
            cat.location = new SerializableVector3(new Vector3(1.2f, -0.5f, 0.0f));
            cat.rotation = 0;
            cat.scale = 0.45f;
            this._items.Add(cat);
        }
        if (this._userSerializer.HasDrone)
        {
            var drone = new PictureItem();
            drone.name = "D-Rone";
            drone.location = new SerializableVector3(new Vector3(0.0f, 0.245f, 0.0f));
            drone.rotation = 0;
            drone.scale = 0.34f;
            this._items.Add(drone);
        }
        var itemObjects = this._postHelper.PopulatePostWithItems(pictureObject, this._items);
        foreach (var item in itemObjects)
        {
            var components = item.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var component in components)
            {
                if (component.name == "LeftArrow" || component.name == "RightArrow"
                    || component.name == "TopArrow" || component.name == "BottomArrow")
                {
                    component.gameObject.SetActive(true);
                }
            }
        }
        return itemObjects;
    }

    private void BackButtonPressed()
    {
        switch (this._currentPostState)
        {
            case NewPostState.BackgroundSelection:
                this.DestroyPage();
                break;
            case NewPostState.Cropping:
                this.DestroyPage();
                this.CreatePopup(this._postCallBack);
                this.GotoNewState(NewPostState.BackgroundSelection);
                break;
        }
    }

    private void GotoNewState(NewPostState newState)
    {
        switch (newState)
        {
            case NewPostState.BackgroundSelection:
                break;
            case NewPostState.Cropping:
                this._postPopupWindow.transform.Find("BackButton").gameObject.SetActive(true);
                this._postPopupWindow.transform.Find("Beach").gameObject.SetActive(false);
                this._postPopupWindow.transform.Find("City").gameObject.SetActive(false);
                this._postPopupWindow.transform.Find("Louvre").gameObject.SetActive(false);
                this._postPopupWindow.transform.Find("Park").gameObject.SetActive(false);
                this._postPopupWindow.transform.Find("CamRoom").gameObject.SetActive(false);
                this._postPopupWindow.transform.Find("ChooseText").GetComponent<TextMeshPro>().text
                    = "Edit your photo:";
                this._postPopupWindow.transform.Find("NewPostDoneButton").gameObject.SetActive(true);

                var post = this._postPopupWindow.transform.Find("NewPost");
                post.gameObject.SetActive(true);
                var picture = post.transform.Find("Picture");
                switch (this._currentBackground)
                {
                    case "Beach":
                        picture.transform.Find("BeachBackground").gameObject.SetActive(true);
                        break;
                    case "City":
                        picture.transform.Find("CityBackground").gameObject.SetActive(true);
                        break;
                    case "Louvre":
                        picture.transform.Find("LouvreBackground").gameObject.SetActive(true);
                        break;
                    case "Park":
                        picture.transform.Find("ParkBackground").gameObject.SetActive(true);
                        break;
                    case "CamRoom":
                        picture.transform.Find("CamRoomBackground").gameObject.SetActive(true);
                        break;
                }

                break;
        }

        this._currentPostState = newState;
    }

    private void CreateNewPost()
    {
        this._userSerializer.NextPostTime = DateTime.Now.AddMinutes(5.0f); // 10.0f);

        var newPost = this.CreateNewPostDataStructure();

        this._messagePost.TriggerActivated(MessageTriggerType.NewPost);

        var postPicture = this._restRequester.PostPicture(newPost);
        StartCoroutine(postPicture);

        this._postCallBack(newPost);
    }

    private DelayGramPost CreateNewPostDataStructure()
    {
        var newPost = new DelayGramPost();
        newPost.playerName = this._userSerializer.PlayerName;
        newPost.imageID = GetRandomImageID();
        newPost.backgroundName = this._currentBackground;
        newPost.avatarPosition = new SerializableVector3(this._avatar.transform.localPosition);
        newPost.avatarRotation = this._avatar.transform.localRotation.eulerAngles.z;
        newPost.avatarScale = this._avatar.transform.localScale.x;
        newPost.characterProperties = this.characterSerializer.CurrentCharacterProperties;
        newPost.likes = 0;
        newPost.dislikes = 0;
        newPost.dateTime = DateTime.Now;

        var newItems = new List<PictureItem>();
        foreach(GameObject item in this._itemObjects)
        {
            newItems.Add(
                new PictureItem(
                    item.name,
                    new SerializableVector3(item.transform.localPosition),
                    item.transform.localRotation.eulerAngles.z,
                    item.transform.localScale.x));
        }
        newPost.items = newItems;

        this._userSerializer.SerializePost(newPost);
        return newPost;
    }
}
