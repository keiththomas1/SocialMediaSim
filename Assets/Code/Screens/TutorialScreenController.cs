using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreenController : MonoBehaviour {
    private UIController _uiController;
    private ProfileScreenController _profileController;
    private GameObject _introductionObject;
    private TextTypingAnimation _introTextAnimation;

    private List<string> _introDialog;
    private int _currentDialogPosition = 0;

    private enum TutorialState
    {
        Introduction
    }
    private TutorialState _currentState;

	// Use this for initialization
	void Start () {
        this._uiController = GetComponent<UIController>();
        this._profileController = GetComponent<ProfileScreenController>();
        this._introDialog = new List<string>();
        this._introDialog.Add("Hi! I made Delaygram.");
        this._introDialog.Add("...");
        this._introDialog.Add("Okay, well enough of that.");
        this._introDialog.Add("Let's start by creating your avatar!");
        this._currentState = TutorialState.Introduction;
	}
	
	// Update is called once per frame
	void Update () {
		// If a tap happens, try to finish text
        // Get a signal back if text already finished
        // If so, move on to the next text bubble or the next section
	}

    public void EnterScreen()
    {
        this._introductionObject = GameObject.Instantiate(Resources.Load("Tutorial/TutorialIntroduction") as GameObject);
        this._introductionObject.transform.position = new Vector3(0.27f, 0.17f, 0.0f);
        this._introTextAnimation = this._introductionObject.transform.Find("TutorialText").GetComponent<TextTypingAnimation>();

        this.ShowNextDialog();
    }

    public void CheckClick(string colliderName)
    {
        switch(this._currentState)
        {
            case TutorialState.Introduction:
                this.ContinueText();
                break;
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
        this._currentDialogPosition++;
        this._introTextAnimation.ResetText(nextText);
    }

    private void EndTutorial()
    {
        GameObject.Destroy(this._introductionObject);
        this._uiController.GoToProfilePage();
        this._profileController.CreateEditAvatarScreen(false);
    }
}
