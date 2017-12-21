using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialScreenController : MonoBehaviour {
    private UIController _uiController;
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
        this._introDialog = new List<string>();
        this._introDialog.Add("Delaygram is the work of many sleepless nights spent in my friend's dorm at college.");
        this._introDialog.Add("Let me show you really quick how to navigate around and then you will be free to explore.");
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
        this._introTextAnimation = this._introductionObject.transform.Find("TutorialText").GetComponent<TextTypingAnimation>();
    }

    public void CheckClick(string colliderName)
    {
        switch(this._currentState)
        {
            case TutorialState.Introduction:
                var continueText = this._introTextAnimation.FinishText();
                if (continueText)
                {
                    if (this._currentDialogPosition >= this._introDialog.Count)
                    {
                        this.EndTutorial();
                        return;
                    }
                    var nextText = this._introDialog[this._currentDialogPosition];
                    this._currentDialogPosition++;
                    this._introTextAnimation.ResetText(nextText);
                }
                break;
        }
    }

    private void EndTutorial()
    {
        GameObject.Destroy(this._introductionObject);
        this._uiController.TutorialFinished();
    }
}
