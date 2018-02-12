using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IOController : MonoBehaviour
{
    private CharacterSerializer _characterSerializer;
    private UIController _uiController;
    private HomeScreenController _homeController;
    private ProfileScreenController _profileController;
    private NewPostController _newPostController;
    private ExploreScreenController _exploreController;
    private MessagesScreenController _messagesController;
    private TutorialScreenController _tutorialController;

    private GameObject _currentObject;
    private Vector3 _clickPosition;

    private EventSystem _eventSystem;

    // Use this for initialization
    void Start ()
    {
        this._characterSerializer = CharacterSerializer.Instance;
        this._uiController = GetComponent<UIController>();
        this._homeController = GetComponent<HomeScreenController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._exploreController = GetComponent<ExploreScreenController>();
        this._messagesController = GetComponent<MessagesScreenController>();
        this._tutorialController = GetComponent<TutorialScreenController>();

        this._eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                this._currentObject = hit.collider.gameObject;
                this._clickPosition = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonUp(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == this._currentObject)
                {
                    var newClickPosition = Input.mousePosition;
                    if (Vector3.Distance(this._clickPosition, newClickPosition) < 20)
                    {
                        this.CheckPageMouseClick(hit.collider);
                    }
                }
            }
            else
            {
                this.CheckPageMouseClick(null);
            }
        }
    }

    private void CheckPageMouseClick(Collider collider)
    {
        var colliderName = (collider == null) ? "" : collider.name;
        if (this._uiController.LevelPopupVisible())
        {   // Handle clicks on level popup
            var previousProperties = new CharacterProperties(this._characterSerializer.CurrentCharacterProperties);
            switch (colliderName)
            {
                case "HappinessBlock":
                    this._characterSerializer.HappinessLevel++;
                    this._uiController.DestroyLevelPopup(previousProperties);
                    break;
                case "FitnessBlock":
                    this._characterSerializer.FitnessLevel++;
                    this._uiController.DestroyLevelPopup(previousProperties);
                    break;
                case "StyleBlock":
                    this._characterSerializer.StyleLevel++;
                    this._uiController.DestroyLevelPopup(previousProperties);
                    break;
                case "NothingBlock":
                    this._uiController.DestroyLevelPopup(previousProperties);
                    break;
                case "OkayButton":
                    this._uiController.DestroyAvatarTransitionPopup();
                    break;
            }
        } else {
            if (!this._eventSystem.IsPointerOverGameObject())
            {   // Make sure you aren't hovering over a UI element
                switch (this._uiController.GetCurrentPage())
                {
                    case Page.Home:
                        this._homeController.CheckClick(colliderName);
                        break;
                    case Page.Profile:
                        this._profileController.CheckClick(colliderName);
                        break;
                    case Page.Post:
                        this._newPostController.CheckClick(colliderName);
                        break;
                    case Page.Explore:
                        this._exploreController.CheckClick(colliderName);
                        break;
                    case Page.Messages:
                        this._messagesController.CheckClick(colliderName);
                        break;
                    case Page.Tutorial:
                        this._tutorialController.CheckClick(colliderName);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
