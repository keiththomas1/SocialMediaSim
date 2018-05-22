using UnityEngine;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.UI;

public class MessagesScreenController : MonoBehaviour {
    [SerializeField]
    private GameObject _messageHeader;
    [SerializeField]
    private GameObject _delaygramTitleText;

    private MessagesSerializer _messagesSerializer;
    private CharacterSerializer _characterSerializer;
    private UserSerializer _userSerializer;
    private MessagePost _messagePost;

    private NotificationScreenController _notificationScreenController;
    private MessageCollection _messageCollection;
    private PostHelper _postHelper;

    private GameObject _messagePage;
    private Transform _pageScrollArea;
    private ScrollController _scrollController;

    private List<Conversation> activeConversations;
    private List<GameObject> createdStubs;
    private Conversation _currentConversation;
    private float _currentConversationLength = 0.0f;
    private const float DEFAULT_MESSAGE_SIZE = 0.65f;
    private const float STUB_STARTING_X = 0.0f;
    private const float STUB_STARTING_Y = -.2f;

    private Queue<MessageReference> _messageQueue;
    private List<GameObject> _messageObjects;
    private const float TYPING_TIME = 1.5f;
    private bool _messageDotsVisible = false;
    private float _messageWritingTimer = 0.0f;
    private GameObject _currentTypingBubble;
    private Tweener _currentTypingTween;

    private Color32 STORY_COLOR = new Color32(75, 195, 255, 255);
    private Color32 CHOICE_COLOR = new Color32(223, 28, 194, 255);
    private Color32 FINISHED_COLOR = new Color32(167, 167, 167, 255);

    private enum MessageScreenState
    {
        MessageStubs,
        InMessage
    }
    private MessageScreenState _currentState;

    private class MessageReference
    {
        public Message message;
        public GameObject gameObject;
    }

    // Use this for initialization
    void Start() {
        this._messagesSerializer = MessagesSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this._messagePost = MessagePost.Instance;

        this._notificationScreenController = this.GetComponent<NotificationScreenController>();
        this._messageCollection = new MessageCollection();
        this._postHelper = new PostHelper();
        this.activeConversations = new List<Conversation>();

        this.createdStubs = new List<GameObject>();
        this._messageObjects = new List<GameObject>();

        this._messageHeader.GetComponent<Button>().onClick.AddListener(this.MessageBackClicked);
        this._currentState = MessageScreenState.MessageStubs;
    }

    // Update is called once per frame
    void Update() {
        // TODO: This section is too large to understand, refactor it into methods
        if (this._messageWritingTimer > 0.0f)
        {
            this._messageWritingTimer -= Time.deltaTime;
            if (this._messageWritingTimer <= 0.0f)
            {
                if (this._messageDotsVisible)
                {
                    this._messageDotsVisible = false;
                    var currentMessage = this._messageQueue.Dequeue();
                    if (currentMessage.message.action != MessageCollection.MessageAction.None)
                    {
                        this.HandleMessageActions(currentMessage.message.action);
                    }

                    if (this._messageQueue.Count != 0)
                    {
                        this._messageWritingTimer = 0.3f;
                    } else { // Finished displaying messages
                        if (!this._currentConversation.viewed)
                        {
                            this._currentConversation.viewed = true;
                        }
                        if (!this._currentConversation.finished
                            && this._currentConversation.choiceCount == 0)
                        {
                            this._currentConversation.finished = true;
                        }
                        this._messagesSerializer.UpdateConversation(this._currentConversation);
                    }
                } else {
                    this.StartNextMessageAnimation();
                }
            }
        }

#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M))
        {
            this._messagePost.CreateNextMessage();
        }
#endif
    }

    public void HandleClick(string colliderName)
    {
                if (this._messageWritingTimer > 0.0f)
        {
            this._messageWritingTimer = 0.01f;
            if (this._currentTypingBubble)
            {
                GameObject.Destroy(this._currentTypingBubble);
                this._currentTypingTween.Goto(0.0f, true);
            }
        }

        switch (colliderName)
        {
            default:
                foreach (Conversation conversation in activeConversations)
                {
                    if (colliderName == conversation.npcName)
                    {
                        this._currentConversationLength = GenerateConversation(conversation, conversation.messages, 0.0f);

                        this._scrollController.UpdateScrollArea(this._currentConversationLength);
                        if (conversation.viewed)
                        {
                            // Since we want to scroll down, that is going in the negative direction so assuming
                            // that we start at zero Y, it should be negative conversationLength downwards
                            this._scrollController.ScrollToBottom();
                        }

                        ShowMessageUI(conversation.npcName);
                        this._currentState = MessageScreenState.InMessage;
                        break;
                    }
                }
                break;
            case "Choice1":
                this.MakeChoice(1);
                break;
            case "Choice2":
                this.MakeChoice(2);
                break;
            case "Choice3":
                this.MakeChoice(3);
                break;
        }
    }

    public void EnterScreen()
    {
        this._messagePage = GameObject.Instantiate(Resources.Load("Messages/DGMessagesPage") as GameObject);
        this._messagePage.transform.position = new Vector3(0.0f, 0.0f, 2.0f);
        this._pageScrollArea = this._messagePage.transform.Find("ScrollArea");
        this._scrollController = this._pageScrollArea.GetComponent<ScrollController>();

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
        if (this._messagePage)
        {
            GameObject.Destroy(_messagePage);
        }
        this.HideMessageUI();
    }

    /* Private methods */

    private void StartNextMessageAnimation()
    {
        MessageReference nextMessage = null;
        try
        {
            nextMessage = this._messageQueue.Peek();
        }
        catch (Exception exception)
        {
            Debug.Log(exception);
            return;
        }

        this._messageDotsVisible = true;
        if (nextMessage != null && nextMessage.gameObject != null)
        {
            nextMessage.gameObject.SetActive(true);

            this.CreateTypingBackground(nextMessage.gameObject);

            this._messageWritingTimer = 2.0f;
        }
    }

    private void HandleMessageActions(MessageCollection.MessageAction action)
    {
        switch (action)
        {
            case MessageCollection.MessageAction.EnableNotifications:
                this._userSerializer.NotificationsEnabled = true;
                this._notificationScreenController.CreateNotificationButton();
                break;
            case MessageCollection.MessageAction.BotchBirthmark:
                switch (this._characterSerializer.Birthmark)
                {
                    case BirthMarkType.MiddleMole:
                        this._characterSerializer.Birthmark = BirthMarkType.MiddleBlotch;
                        break;
                    case BirthMarkType.LeftMole:
                        this._characterSerializer.Birthmark = BirthMarkType.LeftBlotch;
                        break;
                    case BirthMarkType.BottomMole:
                        this._characterSerializer.Birthmark = BirthMarkType.BottomBlotch;
                        break;
                    case BirthMarkType.RightMole:
                        this._characterSerializer.Birthmark = BirthMarkType.RightBlotch;
                        break;
                    case BirthMarkType.TopMole:
                        this._characterSerializer.Birthmark = BirthMarkType.TopBlotch;
                        break;
                }
                break;
            case MessageCollection.MessageAction.None:
            default:
                break;
        }
    }

    private void CreateTypingBackground(GameObject nextMessage)
    {
        Transform typingBackground = null;
        foreach (Transform childTransform in nextMessage.transform)
        {
            if (childTransform.name.Contains("MessageBox") && childTransform.gameObject.activeSelf)
            {
                childTransform.GetComponent<SpriteRenderer>().enabled = true;
                typingBackground = childTransform;
            }
        }

        if (typingBackground != null)
        {
            var scaleFactor = this.GetScaleFactorForMessageBackgrounds(typingBackground.name);
            var previousScale = typingBackground.localScale;
            typingBackground.localScale = new Vector3(scaleFactor.x, scaleFactor.y, 1.0f);

            this._currentTypingTween = typingBackground.DOScale(previousScale, 0.3f)
                .SetEase(Ease.OutSine)
                .SetDelay(TYPING_TIME)
                .OnComplete(() => {
                    nextMessage.transform.Find("MessageText").gameObject.SetActive(true);
                    nextMessage.transform.Find("ProfilePicBubble").gameObject.SetActive(true);
                });

            this.CreateTypingBubble(nextMessage);
        }
    }

    private void CreateTypingBubble(GameObject nextMessage)
    {
        this._currentTypingBubble = GameObject.Instantiate(Resources.Load("Messages/TypingBubble") as GameObject);
        this._currentTypingBubble.transform.parent = this._pageScrollArea;
        var messagePosition = nextMessage.transform.localPosition;
        messagePosition.z -= 1.0f;
        this._currentTypingBubble.transform.localPosition = messagePosition;
        var deathTimer = this._currentTypingBubble.AddComponent<DeathByTimer>();
        deathTimer.deathTimeInSeconds = TYPING_TIME;
    }

    private Vector2 GetScaleFactorForMessageBackgrounds(string backgroundName)
    {
        switch (backgroundName)
        {
            case "MessageBox1":
                return new Vector2(0.26f, 0.61f);
            case "MessageBox2":
                return new Vector2(0.26f, 0.33f);
            case "MessageBox3":
                return new Vector2(0.26f, 0.21f);
            case "MessageBox4":
                return new Vector2(0.26f, 0.16f);
            case "MessageBox5":
                return new Vector2(0.26f, 0.12f);
            case "MessageBox6":
                return new Vector2(0.26f, 0.09f);
            case "MessageBox7":
                return new Vector2(0.26f, 0.07f);
            case "ResultMessageBox":
                return new Vector2(0.25f, 0.12f);
            default:
                Debug.Log("Passed unexpected name for background");
                return new Vector2(0.0f, 0.0f);
        }
    }

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
        if (oldConversation.HasValue)
        {
            var oldMessages = oldConversation.Value.messages;
            this._messagePost.ChoiceMade(this._currentConversation, choice);

            var messageCount = this._messageObjects.Count;
            var choicesCount = this._currentConversation.choiceCount;
            var lastNonChoiceMessage = this._messageObjects[messageCount - 1 - choicesCount];
            var yLocation = this._messageObjects[messageCount - choicesCount].transform.localPosition.y;

            // Hard-coded to destroy the last two messages (choices)
            for (int i = 1; i <= this._currentConversation.choiceCount; i++)
            {
                messageCount = this._messageObjects.Count;
                GameObject.Destroy(this._messageObjects[messageCount - 1]);
                this._messageObjects.RemoveAt(messageCount - 1);
            }

            var newConvo = this._messagesSerializer.GetConversationByNPCName(this._currentConversation.npcName);
            if (newConvo.HasValue)
            {
                var newMessages = new List<Message>(newConvo.Value.messages);
                var messagesToDelete = new List<Message>();
                for (int i = 0; i < oldMessages.Count; i++)
                {
                    if (newMessages.Count > i)
                    {
                        if (oldMessages[i].text == newMessages[i].text)
                        {
                            messagesToDelete.Add(newMessages[i]);
                        }
                    }
                }
                foreach (Message message in messagesToDelete)
                {
                    newMessages.Remove(message);
                }
                var newConversationLength = this.GenerateConversation(newConvo.Value, newMessages, yLocation);
                this._currentConversationLength += newConversationLength;

                this._scrollController.UpdateScrollArea(this._currentConversationLength);
            }
        }
    }

    private void GenerateMessageStubs()
    {
        float currentYPosition = STUB_STARTING_Y;
        foreach (Conversation conversation in this.activeConversations)
        {
            var messages = conversation.messages;
            if (messages.Count != 0)
            {
                var firstMessage = messages[0];

                CreateMessageStub(conversation, firstMessage, currentYPosition);
                currentYPosition -= 0.9f;
            }
        }

        if (this.activeConversations.Count == 0)
        {
            var noMessagesText1 = this._pageScrollArea.Find("NoMessagesText1");
            noMessagesText1.gameObject.SetActive(true);
            var noMessagesText2 = this._pageScrollArea.Find("NoMessagesText2");
            noMessagesText2.gameObject.SetActive(true);
        }

        // TODO: Set this based on how many messages there are
        // TODO: Do this again when you are within a chat on the fly to make in-message the right length
        this._scrollController.UpdateScrollArea(STUB_STARTING_Y - currentYPosition);
    }

    private void CreateMessageStub(Conversation conversation, Message message, float yPosition)
    {
        var messageStub = GameObject.Instantiate(Resources.Load("Messages/MessageStub") as GameObject);
        messageStub.name = conversation.npcName;
        messageStub.transform.parent = _pageScrollArea;
        messageStub.transform.localPosition = new Vector3(STUB_STARTING_X, yPosition, 0.0f);
        messageStub.transform.localScale = new Vector3(.95f, .95f, 1.0f);

        var profileBubble = messageStub.transform.Find("ProfilePicBubble");
        this.SetNPCMessageAvatarMask(profileBubble, conversation);

        var stubBackground = messageStub.transform.Find("StubBackground");
        if (stubBackground)
        {
            if (conversation.finished)
            {
                stubBackground.GetComponent<SpriteRenderer>().color = FINISHED_COLOR;
            }
            else
            {
                switch (conversation.conversationType)
                {
                    case DelaygramConversationType.Choice:
                        stubBackground.GetComponent<SpriteRenderer>().color = CHOICE_COLOR;
                        break;
                    case DelaygramConversationType.Story:
                    default:
                        stubBackground.GetComponent<SpriteRenderer>().color = STORY_COLOR;
                        break;
                }
            }
        }
        var typeText = messageStub.transform.Find("TypeText");
        if (typeText)
        {
            typeText.GetComponent<TextMeshPro>().text = conversation.conversationType.ToString();
        }
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
            timeText.GetComponent<TextMeshPro>().text = this._postHelper.GetMessageTimeFromDateTime(
                conversation.timeSent);
        }

        if (!conversation.viewed)
        {
            var newBubble = messageStub.transform.Find("NewBubble");
            newBubble.gameObject.SetActive(true);
        }
        // User has already gone through the whole conversation
        if (conversation.finished)
        {
            messageStub.transform.Find("StubBackground").GetComponent<SpriteRenderer>().color =
                new Color(167f / 255f, 167f / 255f, 167f / 255f, 1f);
        }

        createdStubs.Add(messageStub);
    }

    private void DestroyMessageStubs()
    {
        foreach (var stub in this.createdStubs)
        {
            GameObject.Destroy(stub);
        }
        this.createdStubs.Clear();
    }

    private float GenerateConversation(Conversation conversation, List<Message> messages, float yPosition)
    {
        this.DestroyMessageStubs();
        this.SetTypeText("Choice", true);
        var currentYPosition = yPosition;

        this._messageQueue = new Queue<MessageReference>();
        foreach (Message message in messages)
        {
            switch (message.type)
            {
                case DelaygramMessageType.NPC:
                    currentYPosition -= this.AddNPCMessageObject(conversation, message, true, currentYPosition);
                    break;
                case DelaygramMessageType.Choice:
                    int choiceCount = 1;
                    foreach (string choice in message.choices)
                    {
                        currentYPosition -= this.AddPlayerChoiceObject(
                            conversation, message, choice, "Choice" + choiceCount.ToString(), currentYPosition);
                        choiceCount++;
                    }
                    break;
                case DelaygramMessageType.Player:
                    currentYPosition -= this.AddPlayerMessageObject(conversation, message, currentYPosition);
                    break;
                case DelaygramMessageType.Result:
                    var prefabName = this._messageCollection.GetResultPrefabName(message.text);
                    currentYPosition -= this.AddResultMessageObject(conversation, message, prefabName, currentYPosition);
                    break;
            }
        }

        if (!conversation.viewed)
        {
            this._messageWritingTimer = 0.1f;
            this._messageDotsVisible = false;
        }

        this._currentConversation = conversation;
        // conversation.viewed = true;

        return (yPosition - currentYPosition);
    }

    private void SetTypeText(string text, bool visible)
    {
        var typeText = this._pageScrollArea.Find("TypeText");
        typeText.GetComponent<TextMeshPro>().text = text;
        typeText.gameObject.SetActive(visible);
    }

    private float AddPlayerMessageObject(Conversation conversation, Message message, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/Player/NewPlayerMessage") as GameObject);
        popupMessage.transform.parent = this._pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(-0.3f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this._postHelper.SetupAvatarMask(profileBubble.gameObject, this._characterSerializer.CurrentCharacterProperties);

        var textHeight = this.SetupText(popupMessage, message.text);
        this.StoreMessageObject(conversation, message, popupMessage);
        return DEFAULT_MESSAGE_SIZE + textHeight;
    }

    private float AddPlayerChoiceObject(
        Conversation conversation, Message message, string choiceText, string nameText, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/Player/PlayerChoice") as GameObject);
        popupMessage.name = nameText;
        popupMessage.transform.parent = this._pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var textHeight = this.SetupText(popupMessage, choiceText);
        this.StoreMessageObject(conversation, message, popupMessage);
        return 0.35f + textHeight;
    }

    private float AddNPCMessageObject(Conversation conversation, Message message, bool showPortrait, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load("Messages/NPC/NewNPCMessage") as GameObject);
        popupMessage.transform.parent = this._pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.3f, yPosition, 0.0f);

        var profileBubble = popupMessage.transform.Find("ProfilePicBubble");
        this.SetNPCMessageAvatarMask(profileBubble, conversation);

        var textHeight = this.SetupText(popupMessage, message.text);
        this.StoreMessageObject(conversation, message, popupMessage);

        var messageHeight = DEFAULT_MESSAGE_SIZE + textHeight;
        return messageHeight;
    }

    private float AddResultMessageObject(
        Conversation conversation, Message message, string prefabName, float yPosition)
    {
        var popupMessage = GameObject.Instantiate(Resources.Load(prefabName) as GameObject);
        popupMessage.transform.parent = this._pageScrollArea;
        popupMessage.transform.localPosition = new Vector3(0.0f, yPosition, 0.0f);

        var profilePicBubble = popupMessage.transform.Find("ProfilePicBubble");
        var maleAvatar = profilePicBubble.Find("MaleAvatar");
        if (maleAvatar)
        {
            var femaleAvatar = profilePicBubble.Find("FemaleAvatar");
            var gender = this._characterSerializer.Gender;
            switch (gender)
            {
                case Gender.Male:
                    maleAvatar.gameObject.SetActive(true);
                    femaleAvatar.gameObject.SetActive(false);
                    break;
                case Gender.Female:
                    maleAvatar.gameObject.SetActive(false);
                    femaleAvatar.gameObject.SetActive(true);
                    break;
            }
        }

        this.StoreMessageObject(conversation, message, popupMessage);

        // TODO: Figure out the size dynamically somehow, or just make all the results the same height
        return 1.6f;
    }

    private void SetNPCMessageAvatarMask(Transform profileBubble, Conversation conversation)
    {
        profileBubble.Find("MaleAvatar").gameObject.SetActive(false);
        profileBubble.Find("FemaleAvatar").gameObject.SetActive(false);
        profileBubble.Find("Professor").gameObject.SetActive(conversation.npcName == MessageCollection.PROFESSOR_NAME);
        profileBubble.Find("ProductEngineer").gameObject.SetActive(
            conversation.npcName == MessageCollection.PRODUCT_ENGINEER_NAME);
        profileBubble.Find("PlasticSurgeon").gameObject.SetActive(
            conversation.npcName == MessageCollection.PLASTIC_SURGERY_NAME);

        if (conversation.npcName != MessageCollection.PROFESSOR_NAME
            && conversation.npcName != MessageCollection.PRODUCT_ENGINEER_NAME
            && conversation.npcName != MessageCollection.PLASTIC_SURGERY_NAME)
        {
            this._postHelper.SetupAvatarMask(profileBubble.gameObject, conversation.npcProperties);
        }
    }

    private void StoreMessageObject(Conversation conversation, Message message, GameObject messageObject)
    {
        var messageReference = new MessageReference()
        {
            message = message,
            gameObject = messageObject
        };
        if (!conversation.viewed)
        {
            var messageText = messageObject.transform.Find("MessageText");
            if (messageText)
            {
                messageText.gameObject.SetActive(false);
            }
            var profilePicBubble = messageObject.transform.Find("ProfilePicBubble");
            if (profilePicBubble)
            {
                profilePicBubble.gameObject.SetActive(false);
            }
            messageObject.SetActive(false);

            this._messageQueue.Enqueue(messageReference);
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

        this.SetupMessageCollider(popupMessage, lineCount);

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
                popupMessage.transform.Find("MessageBox5").gameObject.SetActive(true);
                break;
            case 6:
                popupMessage.transform.Find("MessageBox6").gameObject.SetActive(true);
                break;
            case 7:
            default:
                popupMessage.transform.Find("MessageBox7").gameObject.SetActive(true);
                break;
        }

        return ((lineCount - 1) * 0.15f);
    }

    private void SetupMessageCollider(GameObject message, int lineCount)
    {
        // For now this is just for the choices but it's possible in the future the messages
        // may have colliders so i'm leaving it generic
        if (message.GetComponent<BoxCollider>())
        {
            var newColliderSize = message.GetComponent<BoxCollider>().size;
            newColliderSize.y = lineCount * 0.18f;
            message.GetComponent<BoxCollider>().size = newColliderSize;

            var newColliderCenter = message.GetComponent<BoxCollider>().center;
            switch (lineCount)
            {
                case 1:
                    newColliderCenter.y = .04f;
                    break;
                case 2:
                    newColliderCenter.y = -.06f;
                    break;
                case 3:
                    newColliderCenter.y = -.13f;
                    break;
                case 4:
                    newColliderCenter.y = -.22f;
                    break;
                case 5:
                    newColliderCenter.y = -.30f;
                    break;
                default:
                    break;
            }
            message.GetComponent<BoxCollider>().center = newColliderCenter;
        }
    }
}
