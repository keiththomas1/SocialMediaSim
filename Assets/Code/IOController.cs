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
    private RandomEventController _eventController;

    // Use this for initialization
    void Start ()
    {
        this._uiController = GetComponent<UIController>();
        this._homeController = GetComponent<HomeScreenController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._newPostController = GetComponent<NewPostController>();
        this._exploreController = GetComponent<ExploreScreenController>();
        this._messagesController = GetComponent<MessagesScreenController>();
        this._eventController = GetComponent<RandomEventController>();
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
                if (this._eventController && this._eventController.EventInPlay())
                {
                    this._eventController.CheckClick(hit.collider.name);
                }
                else
                {
                    CheckPageClick(hit.collider);
                }
            }
        }
    }

    private void CheckPageClick(Collider collider)
    {
        switch (this._uiController.GetCurrentPage())
        {
            case Page.Home:
                this._homeController.CheckClick(collider.name);
                break;
            case Page.Profile:
                this._profileController.CheckClick(collider.name);
                break;
            case Page.Post:
                this._newPostController.CheckClick(collider.name);
                break;
            case Page.Explore:
                this._exploreController.CheckClick(collider);
                break;
            case Page.Messages:
                this._messagesController.CheckClick(collider.name);
                break;
            default:
                break;
        }
    }
}
