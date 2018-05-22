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
    private AlertsController _alertsController;
    private CharacterRandomization _characterRandomization;
    private MessageCollection _messageCollection;

    private bool _seenProfessorPartnerConvo = false;
    private bool _seenProductEngineerConvo = false;
    private bool _seenBirthmarkConvo = false;

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
        this._alertsController = GameObject.Find("CONTROLLER").GetComponent<AlertsController>();

        foreach (Conversation convo in this._messageSerializer.ActiveConversations)
        {
            switch(convo.npcName)
            {
                case MessageCollection.PROFESSOR_NAME:
                    this._seenProfessorPartnerConvo = true;
                    break;
                case MessageCollection.PRODUCT_ENGINEER_NAME:
                    this._seenProductEngineerConvo = true;
                    break;
                case MessageCollection.PLASTIC_SURGERY_NAME:
                    this._seenBirthmarkConvo = true;
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
                    this._alertsController.CreateNotificationBubble(NotificationType.Message, 1);
                }
                break;
            case MessageTriggerType.SwipeGoal:
                if (CreateNextMessage())
                {
                    this._alertsController.CreateNotificationBubble(NotificationType.Message, 1);
                }
                break;
            default:
                break;
        }
    }

    public bool CreateNextMessage()
    {
        if (!this._seenProfessorPartnerConvo)
        {
            var conversation = this._messageCollection.CreateProfessorConversation(new List<int>());
            this._messageSerializer.AddConversation(conversation);
            this._seenProfessorPartnerConvo = true;
            return true;
        }
        else if (!this._seenProductEngineerConvo)
        {
            var conversation = this._messageCollection.CreateProductEngineerConversation();
            this._messageSerializer.AddConversation(conversation);
            this._seenProductEngineerConvo = true;
            return true;
        }
        else if (!this._seenBirthmarkConvo)
        {
            var conversation = this._messageCollection.CreatePlasticSurgeryBirthmarkConversation(new List<int>());
            this._messageSerializer.AddConversation(conversation);
            this._seenBirthmarkConvo = true;
            return true;
        }

        return false;
    }

    public void ChoiceMade(Conversation conversation, int choice)
    {
        var choices = conversation.choicesMade;
        choices.Add(choice);
        Conversation newConversation = conversation;

        switch (conversation.npcName)
        {
            case MessageCollection.PROFESSOR_NAME:
                if (choice == 1)
                {
                    this._userSerializer.HasCat = true;
                    this._userSerializer.NextPostTime = DateTime.Now;
                }
                else if (choice == 2)
                {
                    this._userSerializer.HasBulldog = true;
                    this._userSerializer.NextPostTime = DateTime.Now;
                }
                else if (choice == 3)
                {
                    this._userSerializer.HasDrone = true;
                    this._userSerializer.NextPostTime = DateTime.Now;
                }
                newConversation = this._messageCollection.CreateProfessorConversation(choices);
                break;
            case MessageCollection.PLASTIC_SURGERY_NAME:
                if (choice == 1)
                {
                    // this._userSerializer.BotchedBirthmark = true or just set birthmark directly
                }
                newConversation = this._messageCollection.CreatePlasticSurgeryBirthmarkConversation(choices);
                break;
        }
        newConversation.choicesMade = choices;
        this._messageSerializer.UpdateConversation(newConversation);
    }
}
