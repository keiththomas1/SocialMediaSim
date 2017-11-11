using UnityEngine;
using System.Collections.Generic;

public class RandomEventController : MonoBehaviour {
    public List<Sprite> eventScreens;
    public List<string> eventResultTypes;
    public List<int> eventResultValues;
    public List<Sprite> eventOneOffScreens;
    public List<string> eventOneOffResultTypes;
    public List<int> eventOneOffResultValues;
    private GlobalVars globalVars;
    private UserSerializer serializer;
    private GameObject eventContainer;
    private GameObject eventScreen;
    private int eventScreensIndex; // Index into event screens
    private int eventIndex; // Index into event results

	// Use this for initialization
	void Start () {
        globalVars = GlobalVars.Instance;
        serializer = UserSerializer.Instance;
        eventContainer = null;
	}
	
	// Update is called once per frame
	void Update () {
	}

    public bool EventInPlay()
    {
        return eventContainer != null;
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            case "YesButton":
                DisableYesAndNoButtons();
                OpenYesScreen();
                break;
            case "NoButton":
                DisableYesAndNoButtons();
                OpenNoScreen();
                break;
            case "OneOffOkayButton":
            case "OkayButton":
                DestroyEvent();
                break;
        }
    }

    public void CreateNewEvent()
    {
        eventContainer = GameObject.Instantiate(Resources.Load("Home/RandomEventContainer") as GameObject);
        eventScreen = eventContainer.transform.Find("EventScreen").gameObject;
        
        if (eventScreen)
        {
            ChooseRandomEvent();
        }
    }

    private void ChooseRandomEvent()
    {
        // Get random from events/3 because each is a 3-phase event
        var eventMultiChoiceRange = eventScreens.Count / 3;
        var eventOneOffRange = eventOneOffScreens.Count;
        var index = Random.Range(0, eventMultiChoiceRange + eventOneOffRange);

        if (index < eventMultiChoiceRange)
        {
            // Ensures you start on a "first" phase of event
            eventIndex = index;
            eventScreensIndex = index * 3;
            eventScreen.GetComponent<SpriteRenderer>().sprite = eventScreens[eventScreensIndex];
        }
        else
        {
            var oneOffIndex = index - eventMultiChoiceRange;
            eventScreen.GetComponent<SpriteRenderer>().sprite = eventOneOffScreens[oneOffIndex];

            var value = eventOneOffResultValues[oneOffIndex];
            switch (eventOneOffResultTypes[oneOffIndex])
            {
                case "Followers":
                    serializer.AddFollowers(value);
                    break;
                case "Money":
                    globalVars.AddCash(value);
                    break;
            }

            DisableYesAndNoButtons();
            var okayButton = eventContainer.transform.Find("OneOffOkayButton");
            if (okayButton)
            {
                okayButton.GetComponent<Collider>().enabled = true;
            }
            var outcomeText = eventContainer.transform.Find("OneOffOutcomeText");
            if (outcomeText)
            {
                GenerateOutcomeText(outcomeText, value, eventOneOffResultTypes[oneOffIndex]);
            }
        }
    }
    private void GenerateOutcomeText(Transform outcomeText, int value, string type)
    {
        outcomeText.GetComponent<Renderer>().enabled = true;

        string text = "";
        if (value >= 0)
        {
            text += "+";
        } else {
            outcomeText.GetComponent<TextMesh>().color = new Color(254, 140, 120);
        }
        text += value.ToString() + " ";
        text += type;

        outcomeText.GetComponent<TextMesh>().text = text;
    }

    private void DisableYesAndNoButtons()
    {
        var yesButton = eventContainer.transform.Find("YesButton");
        if (yesButton)
        {
            yesButton.GetComponent<Collider>().enabled = false;
        }
        var noButton = eventContainer.transform.Find("NoButton");
        if (noButton)
        {
            noButton.GetComponent<Collider>().enabled = false;
        }
    }

    private void OpenYesScreen()
    {
        eventScreensIndex++;
        eventScreen.GetComponent<SpriteRenderer>().sprite = eventScreens[eventScreensIndex];

        var value = eventResultValues[eventIndex];
        var type = eventResultTypes[eventIndex];
        switch (type)
        {
            case "Followers":
                serializer.AddFollowers(value);
                break;
            case "Money":
                globalVars.AddCash(value);
                break;
        }

        var outcomeText = eventContainer.transform.Find("MultipleOutcomeText");
        if (outcomeText)
        {
            GenerateOutcomeText(outcomeText, value, type);
        }

        EnableOkayButton();
    }

    private void OpenNoScreen()
    {
        eventScreensIndex += 2;
        eventScreen.GetComponent<SpriteRenderer>().sprite = eventScreens[eventScreensIndex];

        EnableOkayButton();
    }

    private void EnableOkayButton()
    {
        var okayButton = eventContainer.transform.Find("OkayButton");
        if (okayButton)
        {
            okayButton.GetComponent<Collider>().enabled = true;
        }
    }

    private void DestroyEvent()
    {
        GameObject.Destroy(eventContainer);
        eventContainer = null;
    }
}
