using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class MessagesScreenController : MonoBehaviour {
    private MessagesSerializer _messagesSerializer;
    private CharacterSerializer _characterSerializer;
    private PostHelper _postHelper;

    private GameObject page;
    private Transform pageScrollArea;
    private ScrollController scrollController;

    private List<Conversation> activeConversations;
    private List<GameObject> createdStubs;
    private Conversation currentConversationWithDialog;
    private const float STUB_STARTING_X = 0.0f;
    private float stubStartingY;

	// Use this for initialization
	void Start () {
        this._messagesSerializer = MessagesSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._postHelper = new PostHelper();
        this.activeConversations = new List<Conversation>();

        createdStubs = new List<GameObject>();
        stubStartingY = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            default:
                foreach (Conversation conversation in activeConversations)
                {
                    if (colliderName == conversation.npcName)
                    {
                        GenerateConversation(conversation);
                    }
                }
                break;
        }
    }

    public void EnterScreen()
    {
        page = GameObject.Instantiate(Resources.Load("Messages/DGMessagesPage") as GameObject);
        pageScrollArea = page.transform.Find("ScrollArea");

        this.activeConversations = this._messagesSerializer.ActiveConversations;

        GenerateMessageStubs();
    }

    public void DestroyPage()
    {
        if (this.page)
        {
            GameObject.Destroy(page);
        }
    }

    /* Private methods */

    private void GenerateMessageStubs()
    {
        float currentYPosition = stubStartingY;
        foreach (Conversation conversation in this.activeConversations)
        {
            var messages = conversation.messages;
            if (messages.Count != 0)
            {
                var firstMessage = messages[0];

                CreateMessageStub(conversation.npcName, firstMessage, currentYPosition);
                currentYPosition -= 1.0f;
            }
        }
    }

    private void CreateMessageStub(string name, Message message, float yPosition)
    {
        var messageStub = GameObject.Instantiate(Resources.Load("Messages/MessageStub") as GameObject);
        messageStub.name = name;
        messageStub.transform.parent = pageScrollArea;
        messageStub.transform.localPosition = new Vector3(STUB_STARTING_X, yPosition, 0.0f);

        var nameText = messageStub.transform.Find("NameText");
        if (nameText)
        {
            nameText.GetComponent<TextMeshPro>().text = name;
        }
        var previewText = messageStub.transform.Find("MessagePreview");
        if (previewText)
        {
            previewText.GetComponent<TextMeshPro>().text = message.text;
        }
        var timeText = messageStub.transform.Find("TimeText");
        if (timeText)
        {
            timeText.GetComponent<TextMeshPro>().text = this.GetMessageTimeFromDateTime(message.timeSent);
        }

        createdStubs.Add(messageStub);
    }

    private string GetMessageTimeFromDateTime(DateTime postTime)
    {
        var timeSincePost = DateTime.Now - postTime;
        if (timeSincePost.Days > 0)
        {
            return timeSincePost.Days.ToString() + "d";
        }
        else if (timeSincePost.Hours > 0)
        {
            return timeSincePost.Hours.ToString() + "h";
        }
        else
        {
            return timeSincePost.Minutes.ToString() + "m";
        }
    }

    private void DestroyMessageStubs()
    {
        foreach(var stub in this.createdStubs)
        {
            GameObject.Destroy(stub);
        }
        this.createdStubs.Clear();
    }

    private void GenerateConversation(Conversation conversation)
    {
        DestroyMessageStubs();

        // scrollController.UpdateScrollArea(popupScrollArea.gameObject, popupScrollArea.transform.localPosition.y, 2);

        float yPosition = 0.0f;
        foreach (Message message in conversation.messages)
        {
            switch (message.type)
            {
                case MessageType.NPC:
                    yPosition -= this.AddNPCMessageObject(conversation, message, true, yPosition);
                    break;
                case MessageType.Choice:
                    // AddChoicesObject(conversation, message, yPosition)
                    break;
                case MessageType.Player:
                    yPosition -= this.AddPlayerMessageObject(conversation, message, yPosition);
                    break;
            }
        }
    }

    private float AddPlayerMessageObject(Conversation conversation, Message message, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/PlayerMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, this._characterSerializer.CurrentCharacterProperties);

        var textHeight = this.SetupText(popupMessage, message);
        return 0.75f + textHeight;
    }

    private float AddNPCMessageObject(Conversation conversation, Message message, bool showPortrait, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/NPCMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var textHeight = this.SetupText(popupMessage, message);

        var messageHeight = 0.75f + textHeight; // One and two liners are 0.75f tall. Every line after is 0.05f.
        return messageHeight;
    }

    private float SetupText(GameObject popupMessage, Message message)
    {
        var messageText = popupMessage.transform.Find("MessageText");
        messageText.GetComponent<TextMeshPro>().text = message.text;
        messageText.GetComponent<TextMeshPro>().ForceMeshUpdate();
        var renderedHeight = messageText.GetComponent<TextMeshPro>().renderedHeight;
        var lineCount = messageText.GetComponent<TextMeshPro>().textInfo.lineCount;

        switch (lineCount)
        {
            case 1:
                popupMessage.transform.Find("MessageBox1").gameObject.SetActive(true);
                break;
            case 2:
                popupMessage.transform.Find("MessageBox2").gameObject.SetActive(true);
                break;
            case 3:
                popupMessage.transform.Find("MessageBox3").gameObject.SetActive(true);
                break;
            case 4:
                popupMessage.transform.Find("MessageBox4").gameObject.SetActive(true);
                break;
            case 5:
            default:
                popupMessage.transform.Find("MessageBox5").gameObject.SetActive(true);
                break;
        }

        return (lineCount > 2) ? (lineCount - 1) * 0.1f : 0.0f;
    }

    private void ConversationOptionSelected(int index)
    {
        string response = "";
        foreach (Message message in currentConversationWithDialog.messages)
        {
            if (message.choices.Count > index)
            {
                response = message.choices[index];

                break;
            }
        }

        // messagesController.AddDialogToConversation(response, currentConversationWithDialog.name);
    }
}
