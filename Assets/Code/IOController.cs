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
            Physics.Raycast(ray, out hit);
            this.CheckPageClick(hit.collider);
        }
    }

    private void CheckPageClick(Collider collider)
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
                this._exploreController.CheckClick(collider);
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
