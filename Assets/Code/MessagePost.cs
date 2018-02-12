using System;
using System.Collections.Generic;
using UnityEngine;

public enum MessageTriggerType
{
    NewPost,
    SwipeGoal
}

public class MessagePost
{
    private static MessagePost _instance;
    private UserSerializer _userSerializer;
    private MessagesSerializer _messageSerializer;
    private NotificationController _notificationController;
    private CharacterRandomization _characterRandomization;
    private MessageCollection _messageCollection;

    private bool _seenLostDogConvo = false;
    private bool _seenNewShirt1Convo = false;

    public static MessagePost Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new MessagePost();
            }
            return _instance;
        }
    }

    private MessagePost()
    {
        this._userSerializer = UserSerializer.Instance;
        this._messageSerializer = MessagesSerializer.Instance;
        this._messageCollection = new MessageCollection();
        this._notificationController = GameObject.Find("CONTROLLER").GetComponent<NotificationController>();

        foreach (Conversation convo in this._messageSerializer.ActiveConversations)
        {
            switch(convo.npcName)
            {
                case MessageCollection.LOST_DOG_NPC_NAME:
                    this._seenLostDogConvo = true;
                    break;
                case MessageCollection.SHIRT1_NPC_NAME:
                    this._seenNewShirt1Convo = true;
                    break;
            }
        }
    }

    public void TriggerActivated(MessageTriggerType trigger)
    {
        switch(trigger)
        {
            case MessageTriggerType.NewPost:
                if (CreateNextMessage())
                {
                    this._notificationController.CreateNotificationBubble(NotificationType.Message, 1);
                }
                break;
            case MessageTriggerType.SwipeGoal:
                if (CreateNextMessage())
                {
                    this._notificationController.CreateNotificationBubble(NotificationType.Message, 1);
                }
                break;
            default:
                break;
        }
    }

    public void ChoiceMade(Conversation conversation, int choice)
    {
        var choices = conversation.choicesMade;
        choices.Add(choice);
        Conversation newConversation = conversation;

        switch (conversation.npcName)
        {
            case MessageCollection.LOST_DOG_NPC_NAME:
                if (choice == 1)
                {
                    this._userSerializer.HasCat = true;
                }
                else if (choice == 2)
                {
                    this._userSerializer.HasBulldog = true;
                }
                newConversation = this._messageCollection.CreateLostDogConversation(choices, conversation.npcProperties);
                break;
        }
        newConversation.choicesMade = choices;
        this._messageSerializer.UpdateConversation(newConversation);
    }

    private bool CreateNextMessage()
    {
        if (!this._seenLostDogConvo)
        {
            var conversation = this._messageCollection.CreateLostDogConversation(new List<int>());
            this._messageSerializer.AddConversation(conversation);
            this._seenLostDogConvo = true;
            return true;
        } else if (!this._seenNewShirt1Convo) {
            var conversation = this._messageCollection.CreateShirtConversation(new List<int>());
            this._messageSerializer.AddConversation(conversation);
            this._seenNewShirt1Convo = true;
            return true;
        }

        return false;
    }
}
