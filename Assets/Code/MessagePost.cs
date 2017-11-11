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
    }

    public void TriggerActivated(MessageTriggerType trigger)
    {
        switch(trigger)
        {
            case MessageTriggerType.NewPost:
                CreateNextMessage();
                break;
            default:
                break;
        }
    }

    private void CreateNextMessage()
    {
        CreateLostDogConversation();
    }

    private void CreateLostDogConversation()
    {
        this._userSerializer.HasBulldog = true;

        var newConversation = new Conversation();
        newConversation.messages = new List<Message>();

        var newMessage = new Message();
        newMessage.text = "Hey (player name), I’m a friend of Mike. You live near 8th and Madison, right?" +
            "Blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah";
        newMessage.type = MessageType.NPC;
        newMessage.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage);

        var newMessage2 = new Message();
        newMessage2.text = "Blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah blah";
        newMessage2.type = MessageType.NPC;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "Test2";
        newMessage3.type = MessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "Okay, yeah that sounds great.";
        newMessage4.type = MessageType.Player;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.type = MessageType.Choice;
       newMessage5.choices = new List<string>();
       newMessage5.choices.Add("Choice1");
       newMessage5.choices.Add("Choice2");
       newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        newConversation.npcName = "Karen";
        newConversation.viewed = false;
        this._messageSerializer.AddConversation(newConversation);
    }
}
