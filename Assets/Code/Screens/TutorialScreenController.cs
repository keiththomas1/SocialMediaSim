using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialScreenController : MonoBehaviour {
    [SerializeField]
    private GameObject _goToPostScreenPopup;
    [SerializeField]
    private GameObject _goToMessageScreenPopup;

    private UserSerializer _userSerializer;
    private CharacterSerializer _characterSerializer;
    private UIController _uiController;
    private ProfileScreenController _profileController;
    private MessagePost _messagePost;

    private GameObject _introductionObject;
    private TextTypingAnimation _introTextAnimation;
    private GameObject _postPopupWindow;

    [SerializeField]
    private List<Sprite> _introAvatarFaces;
    private List<string> _introDialog = new List<string>();
    private int _currentDialogPosition = 0;

    [SerializeField]
    private GameObject _comicPanel;
    private bool _comicPanelShown = false;
    private int _currentComicPanel = 0;
    private GameObject _avatarTransitionPopup;

    private bool _displayTutorialAtStart = false;

    private enum TutorialState
    {
        Introduction,
        ProfileScreenAboutToPostPhoto,
        MovingAvatar,
        RotatingResizingAvatar,
        PostedFirstPhoto,
        ReadingMessage,
        ShowingComic,
        Finished
    }
    private TutorialState _currentState;

	// Use this for initialization
	void Awake () {
        this._userSerializer = UserSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._uiController = GetComponent<UIController>();
        this._uiController._postButton.GetComponent<Button>().onClick.AddListener(
            () => { this.HideGoToPostScreenPopup(); });
        this._uiController.MessageButtonClicked.AddListener(
            () => { this.HideGoToMessageScreenPopup(); });

        this._messagePost = MessagePost.Instance;
        this._messagePost.OnTriggerActivated.AddListener(this.OnMessagePosted);

        this._comicPanel.GetComponent<Button>().onClick.AddListener(
            () =>
            {
                if (this._comicPanelShown)
                {
                    this.IterateComic();
                }
            });
        this._comicPanel.SetActive(false);

        this._profileController = GetComponent<ProfileScreenController>();
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
            this._currentState = TutorialState.PostedFirstPhoto;
        }
        else if (this._userSerializer.CreatedCharacter)
        {
            this._currentState = TutorialState.Introduction;
        }
        else
        {
            this._currentState = TutorialState.Introduction;
        }

        if (this._displayTutorialAtStart)
        {
            this.ShowNextDialog();
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
        if (this._currentState == TutorialState.Introduction)
        {
            this._introductionObject = GameObject.Instantiate(Resources.Load("Tutorial/TutorialIntroduction") as GameObject);
            this._introductionObject.transform.position = new Vector3(0.27f, 0.17f, 0.0f);
            var tutorialBubble = this._introductionObject.transform.Find("TutorialBubble");
            this._introTextAnimation = tutorialBubble.Find("TutorialText").GetComponent<TextTypingAnimation>();

            if (this._introDialog.Count > 0)
            {
                this.ShowNextDialog();
            }
            else
            {
                this._displayTutorialAtStart = true;
            }
        }
        else if (this._currentState == TutorialState.ShowingComic)
        {
            if (!this._comicPanelShown)
            {
                this._comicPanelShown = true;

                this._comicPanel.SetActive(true);
                this._comicPanel.transform.Find("FirstPanel").gameObject.SetActive(true);
            }
        }
    }

    public void HandleClick(string colliderName)
    {
        switch (this._currentState)
        {
            case TutorialState.Introduction:
                this.ContinueText();
                break;
            case TutorialState.ShowingComic:
                if (colliderName == "ConfirmButton")
                {
                    this.IterateComic();
                }
                break;
        }
    }

    public void OnDisable()
    {
        Debug.Log("Here");
        Debug.Log("OnDisable()");
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
            this._currentState = TutorialState.PostedFirstPhoto;
            var tutorialPopup = this._postPopupWindow.transform.Find("TutorialPopup");
            tutorialPopup.gameObject.SetActive(false);
        }
    }

    public void ShowGoToMessageScreenPopup()
    {
        if (this._currentState == TutorialState.PostedFirstPhoto)
        {
            this._currentState = TutorialState.ReadingMessage;
            this._goToMessageScreenPopup.SetActive(true);
        }
    }
    public void HideGoToMessageScreenPopup()
    {
        if (this._currentState == TutorialState.ReadingMessage)
        {
            this._currentState = TutorialState.ShowingComic;
            this._goToMessageScreenPopup.SetActive(false);
        }
    }

    public void IterateComic()
    {
        this._currentComicPanel++;
        switch (this._currentComicPanel)
        {
            case 1:
                this._comicPanel.transform.Find("FirstPanel").gameObject.SetActive(false);
                this._comicPanel.transform.Find("SecondPanel").gameObject.SetActive(true);
                break;
            case 2:
                this._comicPanel.transform.Find("SecondPanel").gameObject.SetActive(false);
                this._comicPanel.transform.Find("ThirdPanel").gameObject.SetActive(true);
                break;
            case 3:
                this._comicPanel.transform.Find("ThirdPanel").gameObject.SetActive(false);
                this._comicPanel.transform.Find("FourthPanel").gameObject.SetActive(true);
                break;
            case 4:
                this._comicPanel.transform.Find("FourthPanel").gameObject.SetActive(false);
                this._comicPanel.transform.Find("FifthPanel").gameObject.SetActive(true);
                break;
            case 5:
                this._comicPanel.SetActive(false);
                this.CreateAvatarTransitionPopup();
                break;
            case 6:
                GameObject.Destroy(this._avatarTransitionPopup);
                this._characterSerializer.AvatarLevel = 1;
                this._characterSerializer.HappinessLevel = 1;
                this._characterSerializer.FitnessLevel = 1;
                this._characterSerializer.HygieneLevel = 1;
                this._currentState = TutorialState.Finished;
                break;
        }
    }

    private void CreateAvatarTransitionPopup()
    {
        this._avatarTransitionPopup = GameObject.Instantiate(Resources.Load("UI/AvatarTransitionPopup") as GameObject);
        this._avatarTransitionPopup.transform.position = new Vector3(0.0f, -0.06f, -5.0f);

        var avatarSection = this._avatarTransitionPopup.transform.Find("AvatarTransition");
        var spriteMask = avatarSection.transform.Find("SpriteMask");
        var oldMaleAvatar = spriteMask.transform.Find("OldMaleAvatar");
        var oldFemaleAvatar = spriteMask.transform.Find("OldFemaleAvatar");
        var newMaleAvatar = spriteMask.transform.Find("NewMaleAvatar");
        var newFemaleAvatar = spriteMask.transform.Find("NewFemaleAvatar");

        var previousCharacterProperties = this._characterSerializer.CurrentCharacterProperties;
        var newCharacterProperties = new CharacterProperties(previousCharacterProperties);
        newCharacterProperties.happinessLevel = 1;
        newCharacterProperties.fitnessLevel = 1;
        newCharacterProperties.hygieneLevel = 1;
        newCharacterProperties.avatarLevel = 1;

        var gender = previousCharacterProperties.gender;
        oldMaleAvatar.gameObject.SetActive(gender == Gender.Male);
        oldFemaleAvatar.gameObject.SetActive(gender == Gender.Female);
        var newGender = this._characterSerializer.Gender;
        newMaleAvatar.gameObject.SetActive(newGender == Gender.Male);
        newFemaleAvatar.gameObject.SetActive(newGender == Gender.Female);
        switch (gender)
        {
            case Gender.Female:
                oldFemaleAvatar.GetComponent<AvatarController>().SetCharacterLook(previousCharacterProperties);
                newFemaleAvatar.GetComponent<AvatarController>().SetCharacterLook(newCharacterProperties);
                break;
            case Gender.Male:
                oldMaleAvatar.GetComponent<AvatarController>().SetCharacterLook(previousCharacterProperties);
                newMaleAvatar.GetComponent<AvatarController>().SetCharacterLook(newCharacterProperties);
                break;
        }

        var leftTopText = this._avatarTransitionPopup.transform.Find("TransitionTextLeftTop");
        leftTopText.GetComponent<TextMeshPro>().text = "";
        var leftBottomText = this._avatarTransitionPopup.transform.Find("TransitionTextLeftBottom").GetComponent<TextMeshPro>();
        var leftBottomPrevious = leftBottomText.text;
        leftBottomText.text = String.Format(leftBottomPrevious, previousCharacterProperties.avatarLevel);
        var rightTopText = this._avatarTransitionPopup.transform.Find("TransitionTextRightTop");
        rightTopText.GetComponent<TextMeshPro>().text = "";
        var rightBottomText = this._avatarTransitionPopup.transform.Find("TransitionTextRightBottom").GetComponent<TextMeshPro>();
        var rightBottomPrevious = rightBottomText.text;
        rightBottomText.text = String.Format(rightBottomPrevious, newCharacterProperties.avatarLevel);

        var upgradesPanel = this._avatarTransitionPopup.transform.Find("UpgradesPanel");
        var background1 = upgradesPanel.Find("Background1");
        background1.gameObject.SetActive(false);
        var background2 = upgradesPanel.Find("Background2");
        background2.gameObject.SetActive(false);
        var upgradeText1 = upgradesPanel.Find("UpgradeText1");
        upgradeText1.gameObject.SetActive(false);
        var upgradeText2 = upgradesPanel.Find("UpgradeText2");
        upgradeText2.gameObject.SetActive(false);
    }

    private void OnMessagePosted(MessageTriggerType triggerType)
    {
        if (this._currentState == TutorialState.PostedFirstPhoto)
        {
            this._userSerializer.PostedPhoto = true;
            this.ShowGoToMessageScreenPopup();
        }
        if (triggerType == MessageTriggerType.NewPost && this._currentState == TutorialState.ShowingComic)
        {
            this._uiController.GoToTutorialPage();
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
        var avatar = introAvatar.Find("Avatar");
        var introAvatarFace = avatar.Find("Head").Find("Face");
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
