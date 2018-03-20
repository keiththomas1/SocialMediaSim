using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IOController : MonoBehaviour
{
    private CharacterSerializer _characterSerializer;
    private UIController _uiController;
    private WorldScreenController _worldController;
    private ProfileScreenController _profileController;
    private NewPostController _newPostController;
    private RatingScreenController _ratingController;
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
        this._worldController = GetComponent<WorldScreenController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._ratingController = GetComponent<RatingScreenController>();
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
                    case Page.World:
                        this._worldController.HandleClick(colliderName);
                        break;
                    case Page.Profile:
                        this._profileController.HandleClick(colliderName);
                        break;
                    case Page.Post:
                        this._newPostController.HandleClick(colliderName);
                        break;
                    case Page.Rating:
                        this._ratingController.HandleClick(colliderName);
                        break;
                    case Page.Messages:
                        this._messagesController.HandleClick(colliderName);
                        break;
                    case Page.Tutorial:
                        this._tutorialController.HandleClick(colliderName);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
