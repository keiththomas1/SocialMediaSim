using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;

public class MessagesScreenController : MonoBehaviour {
    private MessagesSerializer _messagesSerializer;
    private CharacterSerializer _characterSerializer;
    private MessagePost _messagePost;
    private MessageCollection _messageCollection;
    private PostHelper _postHelper;

    private GameObject page;
    private Transform pageScrollArea;
    private ScrollController scrollController;

    private List<Conversation> activeConversations;
    private List<GameObject> createdStubs;
    private Conversation _currentConversation;
    private const float STUB_STARTING_X = 0.0f;
    private float stubStartingY;

	// Use this for initialization
	void Start () {
        this._messagesSerializer = MessagesSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._messagePost = MessagePost.Instance;
        this._messageCollection = new MessageCollection();
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
            case "Choice1":
                this._messagePost.ChoiceMade(this._currentConversation, 1);

                this.DestroyPage();
                this.EnterScreen();
                var newConvo = this._messagesSerializer.GetConversationByNPCName(this._currentConversation.npcName);
                if (newConvo != null)
                {
                    this.GenerateConversation(newConvo.Value);
                }
                break;
            case "Choice2":
                this._messagePost.ChoiceMade(this._currentConversation, 2);

                this.DestroyPage();
                this.EnterScreen();
                newConvo = this._messagesSerializer.GetConversationByNPCName(this._currentConversation.npcName);
                if (newConvo != null)
                {
                    this.GenerateConversation(newConvo.Value);
                }
                break;
        }
    }

    public void EnterScreen()
    {
        this.page = GameObject.Instantiate(Resources.Load("Messages/DGMessagesPage") as GameObject);
        this.page.transform.position = new Vector3(0.0f, 0.0f, 0.0f);
        this.pageScrollArea = this.page.transform.Find("ScrollArea");
        scrollController = this.pageScrollArea.gameObject.AddComponent<ScrollController>();
        scrollController.UpdateScrollArea(this.pageScrollArea.gameObject, this.pageScrollArea.localPosition.y, 7.0f);

        this.activeConversations = this._messagesSerializer.ActiveConversations;
        this.activeConversations.Reverse();

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

                CreateMessageStub(conversation, firstMessage, currentYPosition);
                currentYPosition -= 1.0f;
            }
        }
    }

    private void CreateMessageStub(Conversation conversation, Message message, float yPosition)
    {
        var messageStub = GameObject.Instantiate(Resources.Load("Messages/MessageStub") as GameObject);
        messageStub.name = conversation.npcName;
        messageStub.transform.parent = pageScrollArea;
        messageStub.transform.localPosition = new Vector3(STUB_STARTING_X, yPosition, 0.0f);

        var profileBubble = messageStub.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, conversation.npcProperties);

        var nameText = messageStub.transform.Find("NameText");
        if (nameText)
        {
            nameText.GetComponent<TextMeshPro>().text = conversation.npcName;
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

        float yPosition = 0.0f;
        foreach (Message message in conversation.messages)
        {
            switch (message.type)
            {
                case MessageType.NPC:
                    yPosition -= this.AddNPCMessageObject(conversation, message, true, yPosition);
                    break;
                case MessageType.Choice:
                    int choiceCount = 1;
                    foreach(string choice in message.choices)
                    {
                        yPosition -= this.AddPlayerChoiceObject(choice, "Choice" + choiceCount.ToString(), yPosition);
                        choiceCount++;
                    }
                    break;
                case MessageType.Player:
                    yPosition -= this.AddPlayerMessageObject(conversation, message, yPosition);
                    break;
                case MessageType.Result:
                    var prefabName = this._messageCollection.GetResultPrefabName(conversation);
                    yPosition -= this.AddResultMessageObject(prefabName, yPosition);
                    break;
            }
        }

        this._currentConversation = conversation;
    }

    private float AddPlayerChoiceObject(string choiceText, string nameText, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/PlayerChoice") as GameObject);
        popupMessage.name = nameText;
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var textHeight = this.SetupText(popupMessage, choiceText);
        return 0.35f + textHeight;
    }

    private float AddPlayerMessageObject(Conversation conversation, Message message, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/PlayerMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, this._characterSerializer.CurrentCharacterProperties);

        var textHeight = this.SetupText(popupMessage, message.text);
        return 0.75f + textHeight;
    }

    private float AddResultMessageObject(string prefabName, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load(prefabName) as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        return 0.75f;
    }

    private float AddNPCMessageObject(Conversation conversation, Message message, bool showPortrait, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/NPCMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, conversation.npcProperties);

        var textHeight = this.SetupText(popupMessage, message.text);

        var messageHeight = 0.75f + textHeight; // One and two liners are 0.75f tall. Every line after is 0.05f.
        return messageHeight;
    }

    private float SetupText(GameObject popupMessage, string text)
    {
        var messageText = popupMessage.transform.Find("MessageText");
        messageText.GetComponent<TextMeshPro>().text = text;
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
}
