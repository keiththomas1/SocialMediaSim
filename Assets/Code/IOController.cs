using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOController : MonoBehaviour
{
    private UIController _uiController;

    private HomeScreenController _homeController;
    private ProfileScreenController _profileController;
    private NewPostController _newPostController;
    private ExploreScreenController _exploreController;
    private MessagesScreenController _messagesController;
    private TutorialScreenController _tutorialController;

    private GameObject _currentObject;
    private const float CLICK_THRESHHOLD = 0.3f;
    private float _clickTimer = 0.0f;
    private Vector3 _clickPosition;

    // Use this for initialization
    void Start ()
    {
        this._uiController = GetComponent<UIController>();
        this._homeController = GetComponent<HomeScreenController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._exploreController = GetComponent<ExploreScreenController>();
        this._messagesController = GetComponent<MessagesScreenController>();
        this._tutorialController = GetComponent<TutorialScreenController>();
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
                this._clickTimer = CLICK_THRESHHOLD;
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
                    if (this._clickTimer >= 0.0f)
                    {
                        var newClickPosition = Input.mousePosition;
                        if (Vector3.Distance(this._clickPosition, newClickPosition) < 20)
                        {
                            this.CheckPageMouseClick(hit.collider);
                        }
                    }
                }
            }
            else
            {
                this.CheckPageMouseClick(null);
            }
        }

        if (this._clickTimer > 0.0f)
        {
            this._clickTimer -= Time.deltaTime;
        }
    }

    private void CheckPageMouseClick(Collider collider)
    {
        var colliderName = (collider == null) ? "" : collider.name;
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
