using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenController : MonoBehaviour {
    [SerializeField]
    private GameObject _goToPostScreenPopup;
    [SerializeField]
    private GameObject _goToMessageScreenPopup;

    private UserSerializer _userSerializer;
    private UIController _uiController;
    private ProfileScreenController _profileController;
    private GameObject _introductionObject;
    private TextTypingAnimation _introTextAnimation;
    private GameObject _postPopupWindow;

    [SerializeField]
    private List<Sprite> _introAvatarFaces;
    private List<string> _introDialog;
    private int _currentDialogPosition = 0;

    private enum TutorialState
    {
        Introduction,
        ProfileScreenAboutToPostPhoto,
        MovingAvatar,
        RotatingResizingAvatar,
        PostingPhoto,
        ReadingMessage,
        Finished
    }
    private TutorialState _currentState;

	// Use this for initialization
	void Awake () {
        this._userSerializer = UserSerializer.Instance;
        this._uiController = GetComponent<UIController>();
        this._uiController._postButton.GetComponent<Button>().onClick.AddListener(
            () => { this.HideGoToPostScreenPopup(); });

        this._profileController = GetComponent<ProfileScreenController>();
        this._introDialog = new List<string>();
        this._introDialog.Add("Hello! Welcome to <size=\"3\"><#790CB2>Delaygram</color></size>, a new type of social network.");
        this._introDialog.Add("<size=\"3\"><#0076FFFF>Post</color></size> pictures of yourself and get <#982409FF>ratings</color> from other real people.");
        // this._introDialog.Add("Participate in <size=\"3\"><#09982DFF>challenges</color></size> to win special items.");
        this._introDialog.Add("To participate, simply <size=\"3\"><#982409FF>rate</color></size> other's photos.");
        this._introDialog.Add("Alright, time to make your <size=\"3\">avatar!</size>");
	}

    private void Start()
    {
        if (this._userSerializer.PostedPhoto)
        {
            this._currentState = TutorialState.PostingPhoto;
        }
        else if (this._userSerializer.CreatedCharacter)
        {
            this._currentState = TutorialState.Introduction;
        }
        else
        {
            this._currentState = TutorialState.Introduction;
        }
    }

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.S))
        {
            this.FinishedResizingAndRotatingTutorial();
        }
	}

    public void EnterScreen()
    {
        this._introductionObject = GameObject.Instantiate(Resources.Load("Tutorial/TutorialIntroduction") as GameObject);
        this._introductionObject.transform.position = new Vector3(0.27f, 0.17f, 0.0f);
        var tutorialBubble = this._introductionObject.transform.Find("TutorialBubble");
        this._introTextAnimation = tutorialBubble.Find("TutorialText").GetComponent<TextTypingAnimation>();

        this.ShowNextDialog();
    }

    public void HandleClick(string colliderName)
    {
        switch(this._currentState)
        {
            case TutorialState.Introduction:
                this.ContinueText();
                break;
        }
    }

    public void ShowGoToPostScreenPopup()
    {
        if (this._currentState == TutorialState.Introduction)
        {
            this._currentState = TutorialState.ProfileScreenAboutToPostPhoto;
            this._goToPostScreenPopup.SetActive(true);
        }
    }
    public void HideGoToPostScreenPopup()
    {
        if (this._currentState == TutorialState.ProfileScreenAboutToPostPhoto)
        {
            this._currentState = TutorialState.MovingAvatar;
            this._goToPostScreenPopup.SetActive(false);
        }
    }

    public void ShowMovingTutorialAtPostScreen(GameObject postPopupWindow)
    {
        if (this._currentState == TutorialState.MovingAvatar)
        {
            postPopupWindow.transform.Find("BackButton").gameObject.SetActive(false);

            var tutorialPopup = postPopupWindow.transform.Find("TutorialPopup");
            tutorialPopup.gameObject.SetActive(true);
            tutorialPopup.transform.Find("TopMessage").gameObject.SetActive(true);
            tutorialPopup.transform.Find("BottomMessage").gameObject.SetActive(false);
            this._postPopupWindow = postPopupWindow;
        }
    }
    public void FinishedMovingTutorial()
    {
        if (this._currentState == TutorialState.MovingAvatar)
        {
            this._currentState = TutorialState.RotatingResizingAvatar;
            this.ShowResizingTutorialAtPostScreen();
        }
    }

    public void ShowResizingTutorialAtPostScreen()
    {
        if (this._currentState == TutorialState.RotatingResizingAvatar)
        {
            var tutorialPopup = this._postPopupWindow.transform.Find("TutorialPopup");
            tutorialPopup.gameObject.SetActive(true);
            tutorialPopup.transform.Find("BottomMessage").gameObject.SetActive(true);
            tutorialPopup.transform.Find("TopMessage").gameObject.SetActive(false);
        }
    }
    public void FinishedResizingAndRotatingTutorial()
    {
        if (this._currentState == TutorialState.RotatingResizingAvatar)
        {
            this._currentState = TutorialState.PostingPhoto;
            var tutorialPopup = this._postPopupWindow.transform.Find("TutorialPopup");
            tutorialPopup.gameObject.SetActive(false);

            this._userSerializer.PostedPhoto = true;
        }
    }

    public void ShowGoToMessageScreenPopup()
    {
        if (this._currentState == TutorialState.PostingPhoto)
        {
            this._currentState = TutorialState.ReadingMessage;
            this._goToMessageScreenPopup.SetActive(true);
        }
    }
    public void HideGoToMessageScreenPopup()
    {
        if (this._currentState == TutorialState.ReadingMessage)
        {
            this._currentState = TutorialState.Finished;
            this._goToMessageScreenPopup.SetActive(false);
        }
    }

    private void ContinueText()
    {
        bool continueText = this._introTextAnimation.FinishText();
        if (continueText)
        {
            if (this._currentDialogPosition >= this._introDialog.Count)
            {
                this.EndTutorial();
            } else {
                this.ShowNextDialog();
            }
        }
    }

    private void ShowNextDialog()
    {
        var nextText = this._introDialog[this._currentDialogPosition];
        this._introTextAnimation.ResetText(nextText);

        var introAvatar = this._introductionObject.transform.Find("IntroAvatar");
        var introAvatarFace = introAvatar.Find("Head").Find("Face");
        introAvatarFace.GetComponent<SpriteRenderer>().sprite = this._introAvatarFaces[this._currentDialogPosition];

        this._currentDialogPosition++;
    }

    private void EndTutorial()
    {
        GameObject.Destroy(this._introductionObject);
        this._uiController.GoToProfilePage();
        this._profileController.CreateEditAvatarScreen(false);
    }
}
