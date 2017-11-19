using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCollection
{
    public const string LOST_DOG_NPC_NAME = "Karen L";

    private GlobalVars _globalVars;
    private CharacterRandomization _characterRandomization;

    public MessageCollection()
    {
        this._globalVars = GlobalVars.Instance;
        this._characterRandomization = CharacterRandomization.Instance;
    }

    public string GetResultPrefabName(Conversation conversation)
    {
        switch (conversation.npcName)
        {
            case LOST_DOG_NPC_NAME:
                return "Messages/BulldogUnlocked";
            default:
                return "";
        }
    }

    public Conversation CreateLostDogConversation(List<int> choices)
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.npcName = LOST_DOG_NPC_NAME;
        newConversation.npcProperties = this._characterRandomization.GetFullRandomCharacter();

        var newMessage1 = new Message();
        newMessage1.text = "Hey " + this._globalVars.PlayerName + ", I’m a friend of Mike's. You live in Little Italy, right?";
        newMessage1.type = MessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "yeah, why?";
        newMessage2.type = MessageType.Player;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "I found this bulldog in an alleyway near you. His tag says"
            + " \"Butch\" on it. You know whose it is?";
        newMessage3.type = MessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "nope sorry. never heard of him before.";
        newMessage4.type = MessageType.Player;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = "Huh. Maybe he really is a stray..";
        newMessage5.type = MessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage6 = new Message();
        newMessage6.text = "He does seem a little .. dangerous. Also extremely dirty. Yeah I don't think I want him anymore. You want him?";
        newMessage6.type = MessageType.NPC;
        newMessage6.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage6);

        var newMessage7 = new Message();
        if (choices.Count == 0)
        {
            newMessage7.type = MessageType.Choice;
            newMessage7.choices = new List<string>();
            newMessage7.choices.Add("Sure, I'll take him");
            newMessage7.choices.Add("Nah, I'm good..");
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);
        }
        else if (choices[0] == 1)
        {
            newMessage7.text = "Sure, I'll take him";
            newMessage7.type = MessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Great! I'll bring him over right now! Send me your address.";
            newMessage8a.type = MessageType.NPC;
            newMessage8a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage8a);

            var newMessage9a = new Message();
            newMessage9a.type = MessageType.Result;
            newMessage9a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9a);
        }
        else if (choices[0] == 2)
        {
            newMessage7.text = "Nah, I'm good..";
            newMessage7.type = MessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Oh okay .. well let me know if you change your mind.";
            newMessage8a.type = MessageType.NPC;
            newMessage8a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage8a);
        }

        return newConversation;
    }

    private Conversation CreateEmptyConversation()
    {
        var newConversation = new Conversation();
        newConversation.messages = new List<Message>();
        newConversation.choicesMade = new List<int>();
        newConversation.viewed = false;

        return newConversation;
    }
}
