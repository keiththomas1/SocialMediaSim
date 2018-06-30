using UnityEngine;
using System;
using TMPro;
using System.Collections.Generic;

public delegate void CreatePostCallBack(DelayGramPost post);

public class NewPostController : MonoBehaviour
{
    private UserSerializer _userSerializer;
    private CharacterSerializer characterSerializer;
    private TutorialScreenController _tutorialController;
    private MessagePost _messagePost;
    private PostHelper _postHelper;
    private PostRequester _restRequester;

    private GameObject _postPopupWindow;
    private Transform scrollArea;
    private GameObject _avatar;
    private CreatePostCallBack _postCallBack;
    private CroppingController _croppingController;

    private string _currentBackground;
    private GameObject _currentBackgroundObject = null;
    private List<PictureItem> _items;
    private List<GameObject> _itemObjects;

    static private string characters = "0123456789abcdefghijklmnopqrstuvwxABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private enum NewPostState
    {
        BackgroundSelection,
        Cropping
    }
    private NewPostState _currentPostState;

    private struct LocationParameters
    {
        public Vector3 AvatarLocation;
        public Vector3 AvatarLocalScale;
        public Vector3 AvatarRotation;
    }
    private Dictionary<string, LocationParameters> _defaultLocationParameters
        = new Dictionary<string, LocationParameters>();

    private void Awake()
    {
        this._tutorialController = this.GetComponent<TutorialScreenController>();
        this._postHelper = new PostHelper();
        this._restRequester = new PostRequester();

        this.CreatePopup();
    }

    void Start()
    {
        this._userSerializer = UserSerializer.Instance;
        this.characterSerializer = CharacterSerializer.Instance;
        this._messagePost = MessagePost.Instance;

        this.SetupDefaultLocationParameters();

        this.Initialize();
    }

    public bool PopupActive()
    {
        return this._postPopupWindow.activeSelf;
    }

    public void ShowPopup(CreatePostCallBack callBack)
    {
        this.Initialize();
        this._postCallBack = callBack;
        this._postPopupWindow.SetActive(true);

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

        this.EnterBackgroundSelectionStage();
    }

    public void HandleClick(string colliderName)
    {
        switch (colliderName)
        {
            case "NewPostDoneButton":
                this.CreateNewPost();
                this.DestroyPage();
                break;
            case "Apartment":
                if (this._userSerializer.ApartmentEmpty)
                {
                    this._currentBackground = "ApartmentEmpty";
                }
                else
                {
                    this._currentBackground = "Apartment";
                }
                this.GotoNewState(NewPostState.Cropping);
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
            case "Yacht":
                this._currentBackground = "Yacht";
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
        this._postPopupWindow.SetActive(false);
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

    /* Private Methods */

    private void CreatePopup()
    {
        this._postPopupWindow = GameObject.Instantiate(Resources.Load("Posts/NewPostPopup") as GameObject);
        this._postPopupWindow.transform.position = new Vector3(2.0f, 0.55f, 0.0f);
        this._postPopupWindow.transform.localScale = new Vector3(0.92f, 0.92f, 1.0f);

        var newPost = this._postPopupWindow.transform.Find("NewPost");
        var picture = newPost.transform.Find("Picture");
        this._croppingController = picture.GetComponent<CroppingController>();
        this._croppingController.OnAvatarMovedDecentDistance.AddListener(this._tutorialController.FinishedMovingTutorial);
        this._croppingController.OnAvatarResizedAndRotated.AddListener(this._tutorialController.FinishedResizingAndRotatingTutorial);

        this._postPopupWindow.SetActive(false);
    }

    private void SetLocationStates(GameObject postPopup)
    {
        var beach = postPopup.transform.Find("Beach");
        var beachUnlocked = this._userSerializer.HasBeachBackground;
        beach.GetComponent<Collider>().enabled = beachUnlocked;
        beach.transform.Find("BeachMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !beachUnlocked;

        var city = postPopup.transform.Find("City");
        var cityUnlocked = this._userSerializer.HasCityBackground;
        city.GetComponent<Collider>().enabled = cityUnlocked;
        city.transform.Find("CityMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !cityUnlocked;

        var park = postPopup.transform.Find("Park");
        var parkUnlocked = this._userSerializer.HasParkBackground;
        park.GetComponent<Collider>().enabled = parkUnlocked;
        park.transform.Find("ParkMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !parkUnlocked;

        var louvre = postPopup.transform.Find("Louvre");
        var louvreUnlocked = this._userSerializer.HasLouvreBackground;
        louvre.GetComponent<Collider>().enabled = louvreUnlocked;
        louvre.transform.Find("LouvreMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !louvreUnlocked;

        var camRoom = postPopup.transform.Find("CamRoom");
        var camRoomUnlocked = this._userSerializer.HasCamRoomBackground;
        camRoom.GetComponent<Collider>().enabled = camRoomUnlocked;
        camRoom.transform.Find("CamRoomMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !camRoomUnlocked;

        var yacht = postPopup.transform.Find("Yacht");
        var yachtUnlocked = this._userSerializer.HasYachtBackground;
        yacht.GetComponent<Collider>().enabled = yachtUnlocked;
        yacht.transform.Find("YachtMask").Find("DarkTint").GetComponent<SpriteRenderer>().enabled = !yachtUnlocked;
    }

    private void Initialize()
    {
        this._currentPostState = NewPostState.BackgroundSelection;
        this._items = new List<PictureItem>();
        this._itemObjects = new List<GameObject>();
    }

    private List<GameObject> SetupItemsInPost(GameObject picture, GameObject itemsParent)
    {
        if (this._userSerializer.HasBulldog)
        {
            var bulldog = new PictureItem();
            bulldog.name = "Bulldog";
            bulldog.location = new SerializableVector3(new Vector3(0.69f, -0.41f, 0.0f));
            bulldog.rotation = 0;
            bulldog.scale = (itemsParent.name == "YachtBoat") ? 1f : 0.38f;
            this._items.Add(bulldog);
        }
        if (this._userSerializer.HasCat)
        {
            var cat = new PictureItem();
            cat.name = "Cat";
            cat.location = new SerializableVector3(new Vector3(0.66f, -0.7f, 0.0f));
            cat.rotation = 0;
            cat.scale = (itemsParent.name == "YachtBoat") ? 1f : 0.37f;
            this._items.Add(cat);
        }
        if (this._userSerializer.HasDrone)
        {
            var drone = new PictureItem();
            drone.name = "D-Rone";
            drone.location = new SerializableVector3(new Vector3(1.14f, -0.65f, 0.0f));
            drone.rotation = 0;
            drone.scale = (itemsParent.name == "YachtBoat") ? 0.9f : 0.32f;
            this._items.Add(drone);
        }
        var itemObjects = this._postHelper.PopulatePostWithItems(picture, itemsParent, this._items);
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
                this.ShowPopup(this._postCallBack);
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
                this.EnterCroppingStage();
                break;
        }

        this._currentPostState = newState;
    }

    private void EnterBackgroundSelectionStage()
    {
        this.SetLocationStates(this._postPopupWindow);

        this._postPopupWindow.transform.Find("BackButton").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("Apartment").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("Beach").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("City").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("Louvre").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("Park").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("CamRoom").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("Yacht").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("ChooseText").GetComponent<TextMeshPro>().text = "Choose a Location";
        this._postPopupWindow.transform.Find("NewPostDoneButton").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("ChooseFriendSection").gameObject.SetActive(false);

        var post = this._postPopupWindow.transform.Find("NewPost");
        post.gameObject.SetActive(false);
        var picture = post.transform.Find("Picture");
        picture.Find("ApartmentBackground").gameObject.SetActive(false);
        picture.Find("BeachBackground").gameObject.SetActive(false);
        picture.Find("CityBackground").gameObject.SetActive(false);
        picture.Find("LouvreBackground").gameObject.SetActive(false);
        picture.Find("ParkBackground").gameObject.SetActive(false);
        picture.Find("CamRoomBackground").gameObject.SetActive(false);
        picture.Find("YachtBackground").gameObject.SetActive(false);
    }

    private void EnterCroppingStage()
    {
        this._postPopupWindow.transform.Find("BackButton").gameObject.SetActive(true);
        this._postPopupWindow.transform.Find("Apartment").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("Beach").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("City").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("Louvre").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("Park").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("CamRoom").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("Yacht").gameObject.SetActive(false);
        this._postPopupWindow.transform.Find("ChooseText").GetComponent<TextMeshPro>().text
            = "Edit your photo:";
        this._postPopupWindow.transform.Find("NewPostDoneButton").gameObject.SetActive(true);

        var friendsSection = this._postPopupWindow.transform.Find("ChooseFriendSection");
        var followedIdsLength = this._userSerializer.GetFollowedIds().Count;
        friendsSection.gameObject.SetActive(followedIdsLength > 0);

        var post = this._postPopupWindow.transform.Find("NewPost");
        post.gameObject.SetActive(true);
        var picture = post.transform.Find("Picture");
        var itemsParent = picture.gameObject;
        switch (this._currentBackground)
        {
            case "Apartment":
                this._currentBackgroundObject = picture.Find("ApartmentBackground").gameObject;
                break;
            case "ApartmentEmpty":
                this._currentBackgroundObject = picture.Find("ApartmentEmptyBackground").gameObject;
                break;
            case "Beach":
                this._currentBackgroundObject = picture.Find("BeachBackground").gameObject;
                break;
            case "City":
                this._currentBackgroundObject = picture.Find("CityBackground").gameObject;
                break;
            case "Louvre":
                this._currentBackgroundObject = picture.Find("LouvreBackground").gameObject;
                break;
            case "Park":
                this._currentBackgroundObject = picture.Find("ParkBackground").gameObject;
                break;
            case "CamRoom":
                this._currentBackgroundObject = picture.Find("CamRoomBackground").gameObject;
                break;
            case "Yacht":
                this._currentBackgroundObject = picture.Find("YachtBackground").gameObject;
                itemsParent = this._currentBackgroundObject.transform.Find("YachtBoat").gameObject;
                break;
        }
        this._currentBackgroundObject.SetActive(true);

        this._itemObjects = this.SetupItemsInPost(picture.gameObject, itemsParent);
        var movableObjects = new List<GameObject>(this._itemObjects);
        movableObjects.Add(this._avatar);
        this._croppingController.SetMovableItems(movableObjects);
        this.SetAvatarToDefault(this._currentBackground);

        if (!this._userSerializer.PostedPhoto)
        {
            this._tutorialController.ShowMovingTutorialAtPostScreen(this._postPopupWindow);
        }
    }

    private void SetAvatarToDefault(string locationName)
    {
        if (this._defaultLocationParameters.ContainsKey(locationName))
        {
            var currentParameters = this._defaultLocationParameters[locationName];
            this._avatar.transform.localPosition = currentParameters.AvatarLocation;
            this._avatar.transform.localScale = currentParameters.AvatarLocalScale;
            this._avatar.transform.localRotation = Quaternion.Euler(currentParameters.AvatarRotation);
        }
    }

    private void CreateNewPost()
    {
        var delayTime = this.GetDelayTime(this._currentBackground);
        this._userSerializer.NextPostTime = DateTime.Now.AddMinutes(delayTime);

        var newPost = this.CreateNewPostDataStructure();

        var postPicture = this._restRequester.PostPicture(newPost);
        StartCoroutine(postPicture);

        this._postCallBack(newPost);

        this._messagePost.TriggerActivated(MessageTriggerType.NewPost);
    }

    private float GetDelayTime(string backgroundName)
    {
        switch (backgroundName)
        {
            case "Apartment":
            case "ApartmentEmpty":
                return 3f;
            case "Beach":
                return 5f;
            case "City":
                return 5f;
            case "Park":
                return 10f;
            case "CamRoom":
                return 10f;
            case "Louvre":
                return 30f;
            case "Yacht":
                return 60f;
            default:
                return 0f;
        }
    }

    private DelayGramPost CreateNewPostDataStructure()
    {
        var newPost = new DelayGramPost();
        newPost.playerName = this._userSerializer.PlayerName;
        newPost.pictureID = GetRandomImageID();
        newPost.backgroundName = this._currentBackground;
        newPost.avatarPosition = new SerializableVector3(this._avatar.transform.localPosition);
        newPost.avatarRotation = this._avatar.transform.localRotation.eulerAngles.z;
        newPost.avatarScale = this._avatar.transform.localScale.x;
        var animationParameters = this._avatar.GetComponent<Animator>().parameters;
        foreach (var animationParameter in animationParameters)
        {
            var valueSet = this._avatar.GetComponent<Animator>().GetBool(animationParameter.name);
            if (valueSet)
            {
                newPost.avatarPoseName = animationParameter.name;
                break;
            }
        }
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
                    item.transform.localScale.x,
                    null));
        }
        if (this._currentBackground == "Apartment" || this._currentBackground == "ApartmentEmpty")
        {
            // TODO: Reward player with carpet which you can enable here
            var carpet = this._currentBackgroundObject.transform.Find("ApartmentCarpet");
            if (carpet)
            {
                var carpetColor = carpet.GetComponent<SpriteRenderer>().color;
                var carpetItem = new PictureItem(
                    "ApartmentCarpet",
                    null,
                    0f,
                    0f,
                    new SerializableColor(carpetColor));
                newItems.Add(carpetItem);
            }
        }
        newPost.items = newItems;

        return newPost;
    }


    private void SetupDefaultLocationParameters()
    {
        var apartmentParameters = new LocationParameters();
        apartmentParameters.AvatarLocation = new Vector3(-1.43f, -0.35f, 0f);
        apartmentParameters.AvatarLocalScale = new Vector3(0.36f, 0.36f, 1.0f);
        apartmentParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "Apartment",
            apartmentParameters);
        this._defaultLocationParameters.Add(
            "ApartmentEmpty",
            apartmentParameters);

        var cityParameters = new LocationParameters();
        cityParameters.AvatarLocation = new Vector3(-1.43f, -0.39f, 0f);
        cityParameters.AvatarLocalScale = new Vector3(0.26f, 0.26f, 1.0f);
        cityParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "City",
            cityParameters);

        var beachParameters = new LocationParameters();
        beachParameters.AvatarLocation = new Vector3(-1.43f, -0.39f, 0f);
        beachParameters.AvatarLocalScale = new Vector3(0.26f, 0.26f, 1.0f);
        beachParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "Beach",
            beachParameters);

        var parkParameters = new LocationParameters();
        parkParameters.AvatarLocation = new Vector3(-0.56f, -0.11f, 0f);
        parkParameters.AvatarLocalScale = new Vector3(0.33f, 0.33f, 1.0f);
        parkParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "Park",
            parkParameters);

        var louvreParameters = new LocationParameters();
        louvreParameters.AvatarLocation = new Vector3(-1.38f, -0.56f, 0f);
        louvreParameters.AvatarLocalScale = new Vector3(0.42f, 0.42f, 1.0f);
        louvreParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "Louvre",
            louvreParameters);

        var yachtParameters = new LocationParameters();
        yachtParameters.AvatarLocation = new Vector3(-1.54f, 0.18f, 0f);
        yachtParameters.AvatarLocalScale = new Vector3(0.38f, 0.38f, 1.0f);
        yachtParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "Yacht",
            yachtParameters);

        var camRoomParameters = new LocationParameters();
        camRoomParameters.AvatarLocation = new Vector3(-1.44f, -0.43f, 0f);
        camRoomParameters.AvatarLocalScale = new Vector3(0.44f, 0.44f, 1.0f);
        camRoomParameters.AvatarRotation = new Vector3(0f, 0f, 0f);
        this._defaultLocationParameters.Add(
            "CamRoom",
            camRoomParameters);
    }
}
