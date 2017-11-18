using System;
using System.Collections.Generic;
using UnityEngine;

public enum MessageTriggerType
{
    NewPost
}

public class MessagePost
{
    private static MessagePost _instance;
    private UserSerializer _userSerializer;
    private MessagesSerializer _messageSerializer;
    private CharacterRandomization _characterRandomization;
    private MessageCollection _messageCollection;

    private bool _seenLostDogConvo = false;

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

        foreach (Conversation convo in this._messageSerializer.ActiveConversations)
        {
            if (convo.npcName == MessageCollection.LOST_DOG_NPC_NAME)
            {
                this._seenLostDogConvo = true;
            }
        }
    }

    public bool TriggerActivated(MessageTriggerType trigger)
    {
        switch(trigger)
        {
            case MessageTriggerType.NewPost:
                return CreateNextMessage();
            default:
                return false;
        }
    }

    public void ChoiceMade(Conversation conversation, int choice)
    {
        conversation.choicesMade.Add(choice);
        Conversation newConversation = conversation;

        switch (conversation.npcName)
        {
            case MessageCollection.LOST_DOG_NPC_NAME:
                if (conversation.choicesMade.Count == 1)
                {
                    if (choice == 1)
                    {
                        Debug.Log("set bulldog true");
                        this._userSerializer.HasBulldog = true;
                    }
                }
                newConversation = this._messageCollection.CreateLostDogConversation(conversation.choicesMade);
                break;
        }

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
        }

        return false;
    }
}
