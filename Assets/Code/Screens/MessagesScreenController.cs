using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System;
using UnityEngine.UI;

public class MessagesScreenController : MonoBehaviour {
    [SerializeField]
    private GameObject _messageHeader;
    [SerializeField]
    private GameObject _delaygramTitleText;

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

    private Queue<GameObject> _messageQueue;
    private List<GameObject> _messageObjects;
    private bool _messageDotsVisible = false;
    private float _messageWritingTimer = 0.0f;
    private GameObject _currentTypingBubble;

    private enum MessageScreenState
    {
        MessageStubs,
        InMessage
    }
    private MessageScreenState _currentState;

	// Use this for initialization
	void Start () {
        this._messagesSerializer = MessagesSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._messagePost = MessagePost.Instance;
        this._messageCollection = new MessageCollection();
        this._postHelper = new PostHelper();
        this.activeConversations = new List<Conversation>();

        this.createdStubs = new List<GameObject>();
        this.stubStartingY = 0.0f;
        this._messageObjects = new List<GameObject>();

        this._messageHeader.GetComponent<Button>().onClick.AddListener(this.MessageBackClicked);
        this._currentState = MessageScreenState.MessageStubs;
    }
	
	// Update is called once per frame
	void Update () {
	    if (this._messageWritingTimer > 0.0f)
        {
            this._messageWritingTimer -= Time.deltaTime;
            if (this._messageWritingTimer <= 0.0f)
            {
                if (this._messageDotsVisible)
                {
                    this._messageDotsVisible = false;
                    var nextMessage = this._messageQueue.Dequeue();
                    nextMessage.SetActive(true);

                    if (this._messageQueue.Count != 0)
                    {
                        this._messageWritingTimer = 0.3f;
                    } else { // Finished displaying messages
                        if (!this._currentConversation.viewed)
                        {
                            this._currentConversation.viewed = true;
                            this._messagesSerializer.UpdateConversation(this._currentConversation);
                        }
                    }
                } else {
                    this._messageDotsVisible = true;

                    GameObject nextMessage = this._messageQueue.Peek();
                    if (nextMessage)
                    {
                        this._currentTypingBubble = GameObject.Instantiate(Resources.Load("Messages/TypingBubble") as GameObject);
                        this._currentTypingBubble.transform.parent = this.pageScrollArea;
                        Vector3 position = nextMessage.transform.position;
                        this._currentTypingBubble.transform.position = position;
                        var deathTimer = this._currentTypingBubble.AddComponent<DeathByTimer>();
                        deathTimer.deathTimeInSeconds = 1.5f;
                    }

                    this._messageWritingTimer = 2.0f;
                }
            }
        }
    }

    public void CheckClick(string colliderName)
    {
        if (this._messageWritingTimer > 0.0f)
        {
            this._messageWritingTimer = 0.01f;
            if (this._currentTypingBubble)
            {
                GameObject.Destroy(this._currentTypingBubble);
            }
        }

        switch (colliderName)
        {
            default:
                foreach (Conversation conversation in activeConversations)
                {
                    if (colliderName == conversation.npcName)
                    {
                        GenerateConversation(conversation, conversation.messages, 0.0f);
                        ShowMessageUI(conversation.npcName);
                        this._currentState = MessageScreenState.InMessage;
                    }
                }
                break;
            case "Choice1":
                this.MakeChoice(1);
                break;
            case "Choice2":
                this.MakeChoice(2);
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

        this.activeConversations = new List<Conversation>(this._messagesSerializer.ActiveConversations);
        this.activeConversations.Reverse();

        this._messageWritingTimer = 0.0f;

        GenerateMessageStubs();
        this._currentState = MessageScreenState.MessageStubs;
    }

    public bool BackOut()
    {
        if (this._currentState == MessageScreenState.InMessage)
        {
            this.MessageBackClicked();
            return false;
        }
        return true;
    }

    public void DestroyPage()
    {
        if (this.page)
        {
            GameObject.Destroy(page);
        }
        this.HideMessageUI();
    }

    /* Private methods */

    private void MessageBackClicked()
    {
        this.DestroyPage();
        this.EnterScreen();
    }

    private void ShowMessageUI(string npcName)
    {
        var nameText = this._messageHeader.transform.Find("NPCNameText");
        nameText.GetComponent<TextMeshProUGUI>().text = npcName;
        this._messageHeader.SetActive(true);
        this._delaygramTitleText.SetActive(false);
    }
    private void HideMessageUI()
    {
        this._messageHeader.SetActive(false);
        this._delaygramTitleText.SetActive(true);
    }

    private void MakeChoice(int choice)
    {
        var oldConversation = this._messagesSerializer.GetConversationByNPCName(this._currentConversation.npcName);
        if (oldConversation != null)
        {
            var oldMessages = oldConversation.Value.messages;
            this._messagePost.ChoiceMade(this._currentConversation, choice);

            var messageCount = this._messageObjects.Count;
            var yLocation = this._messageObjects[messageCount - 1].transform.localPosition.y;

            // Hard-coded to destroy the last two messages (choices)
            GameObject.Destroy(this._messageObjects[messageCount - 1]);
            GameObject.Destroy(this._messageObjects[messageCount - 2]);
            this._messageObjects.RemoveAt(messageCount - 1);
            this._messageObjects.RemoveAt(messageCount - 2);

            var newConvo = this._messagesSerializer.GetConversationByNPCName(this._currentConversation.npcName);
            if (newConvo != null)
            {
                var newMessages = new List<Message>(newConvo.Value.messages);
                var messagesToDelete = new List<Message>();
                for (int i = 0; i < oldMessages.Count; i++)
                {
                    if (oldMessages[i].text == newMessages[i].text)
                    {
                        messagesToDelete.Add(newMessages[i]);
                    }
                }
                foreach (Message message in messagesToDelete)
                {
                    newMessages.Remove(message);
                }
                this.GenerateConversation(newConvo.Value, newMessages, yLocation);
            }
        }
    }

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
            timeText.GetComponent<TextMeshPro>().text = this._postHelper.GetMessageTimeFromDateTime(message.timeSent);
        }

        createdStubs.Add(messageStub);
    }

    private void DestroyMessageStubs()
    {
        foreach(var stub in this.createdStubs)
        {
            GameObject.Destroy(stub);
        }
        this.createdStubs.Clear();
    }

    private void GenerateConversation(Conversation conversation, List<Message> messages, float yPosition)
    {
        DestroyMessageStubs();

        this._messageQueue = new Queue<GameObject>();
        foreach (Message message in messages)
        {
            switch (message.type)
            {
                case DelaygramMessageType.NPC:
                    yPosition -= this.AddNPCMessageObject(conversation, message, true, yPosition);
                    break;
                case DelaygramMessageType.Choice:
                    int choiceCount = 1;
                    foreach(string choice in message.choices)
                    {
                        yPosition -= this.AddPlayerChoiceObject(conversation, choice, "Choice" + choiceCount.ToString(), yPosition);
                        choiceCount++;
                    }
                    break;
                case DelaygramMessageType.Player:
                    yPosition -= this.AddPlayerMessageObject(conversation, message, yPosition);
                    break;
                case DelaygramMessageType.Result:
                    var prefabName = this._messageCollection.GetResultPrefabName(conversation);
                    yPosition -= this.AddResultMessageObject(conversation, prefabName, yPosition);
                    break;
            }
        }

        if (!conversation.viewed)
        {
            this._messageWritingTimer = 0.1f;
            this._messageDotsVisible = false;
        }

        this._currentConversation = conversation;
        conversation.viewed = true;
    }

    private float AddPlayerMessageObject(Conversation conversation, Message message, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/PlayerMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, this._characterSerializer.CurrentCharacterProperties);


        var textHeight = this.SetupText(popupMessage, message.text);
        this.StoreMessageObject(conversation, popupMessage);
        return 0.75f + textHeight;
    }

    private float AddPlayerChoiceObject(Conversation conversation, string choiceText, string nameText, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/PlayerChoice") as GameObject);
        popupMessage.name = nameText;
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var textHeight = this.SetupText(popupMessage, choiceText);
        this.StoreMessageObject(conversation, popupMessage);
        return 0.35f + textHeight;
    }

    private float AddNPCMessageObject(Conversation conversation, Message message, bool showPortrait, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/NPCMessage") as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupProfilePicBubble(profileBubble.gameObject, conversation.npcProperties);

        var textHeight = this.SetupText(popupMessage, message.text);
        this.StoreMessageObject(conversation, popupMessage);

        var messageHeight = 0.75f + textHeight; // One and two liners are 0.75f tall. Every line after is 0.05f.
        return messageHeight;
    }

    private float AddResultMessageObject(Conversation conversation, string prefabName, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load(prefabName) as GameObject);
        popupMessage.transform.parent = this.pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        this.StoreMessageObject(conversation, popupMessage);
        return 0.75f;
    }

    private void StoreMessageObject(Conversation conversation, GameObject messageObject)
    {
        if (!conversation.viewed)
        {
            messageObject.SetActive(false);
            this._messageQueue.Enqueue(messageObject);
        }
        this._messageObjects.Add(messageObject);
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
