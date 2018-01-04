using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCollection
{
    public const string LOST_DOG_NPC_NAME = "Karen L";
    public const string SHIRT1_NPC_NAME = "Gracie Giraffe";

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
        newMessage1.type = DelaygramMessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "Yeah, why?";
        newMessage2.type = DelaygramMessageType.Player;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "I found this bulldog in an alleyway near you. His tag says"
            + " \"Butch\" on it. You know whose it is?";
        newMessage3.type = DelaygramMessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "Nope sorry. never heard of him before.";
        newMessage4.type = DelaygramMessageType.Player;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = "Huh. Maybe he really is a stray..";
        newMessage5.type = DelaygramMessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage6 = new Message();
        newMessage6.text = "He does seem a little .. dangerous. Also extremely dirty. Yeah I don't think I want him anymore. You want him?";
        newMessage6.type = DelaygramMessageType.NPC;
        newMessage6.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage6);

        var newMessage7 = new Message();
        if (choices.Count == 0)
        {
            newMessage7.type = DelaygramMessageType.Choice;
            newMessage7.choices = new List<string>();
            newMessage7.choices.Add("Sure, I'll take him");
            newMessage7.choices.Add("Nah, I'm good..");
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);
        }
        else if (choices[0] == 1)
        {
            newMessage7.text = "Sure, I'll take him";
            newMessage7.type = DelaygramMessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Great! I'll bring him over right now! Send me your address.";
            newMessage8a.type = DelaygramMessageType.NPC;
            newMessage8a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage8a);

            var newMessage9a = new Message();
            newMessage9a.type = DelaygramMessageType.Result;
            newMessage9a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9a);
        }
        else if (choices[0] == 2)
        {
            newMessage7.text = "Nah, I'm good..";
            newMessage7.type = DelaygramMessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Oh okay .. well let me know if you change your mind.";
            newMessage8a.type = DelaygramMessageType.NPC;
            newMessage8a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage8a);
        }

        return newConversation;
    }

    public Conversation CreateShirtConversation(List<int> choices)
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.npcName = SHIRT1_NPC_NAME;
        newConversation.npcProperties = this._characterRandomization.GetFullRandomCharacter();

        var newMessage1 = new Message();
        newMessage1.text = "Hi! Totally loved your latest post, adorable dog! HAHAHA";
        newMessage1.type = DelaygramMessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "So like, yeah, anyways. I design t-shirts and I'm totally full of shit.";
        newMessage2.type = DelaygramMessageType.NPC;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "*good ideas";
        newMessage3.type = DelaygramMessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "full of good ideas";
        newMessage4.type = DelaygramMessageType.NPC;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = "LOL! soooo, anyways, can I make you a shirt? You have to promise to wear it forever. HAHA!!";
        newMessage5.type = DelaygramMessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage7 = new Message();
        if (choices.Count == 0)
        {
            newMessage7.type = DelaygramMessageType.Choice;
            newMessage7.choices = new List<string>();
            newMessage7.choices.Add("Okay, I could use new threads yo yo yo hunny oh");
            newMessage7.choices.Add("No thx, you sound batshit crazy");
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);
        }
        else if (choices[0] == 1)
        {
            newMessage7.text = "Okay, I could use new threads";
            newMessage7.type = DelaygramMessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Awesome! You may or may not (but probably will totally not) regret it!";
            newMessage8a.type = DelaygramMessageType.NPC;
            newMessage8a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage8a);

            var newMessage9a = new Message();
            newMessage9a.type = DelaygramMessageType.Result;
            newMessage9a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9a);
        }
        else if (choices[0] == 2)
        {
            newMessage7.text = "No thx, you sound batshit crazy";
            newMessage7.type = DelaygramMessageType.Player;
            newMessage7.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage7);

            var newMessage8a = new Message();
            newMessage8a.text = "Screw u! u don't deserve my clothes hunny, bye felicia";
            newMessage8a.type = DelaygramMessageType.NPC;
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
