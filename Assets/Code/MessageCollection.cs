using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageCollection
{
    public const string LOST_DOG_NPC_NAME = "Karen L";
    public const string SHIRT1_NPC_NAME = "Gracie Giraffe";

    private UserSerializer _userSerializer;
    private CharacterRandomization _characterRandomization;

    public MessageCollection()
    {
        this._userSerializer = UserSerializer.Instance;
        this._characterRandomization = CharacterRandomization.Instance;
    }

    public string GetResultPrefabName(string resultText)
    {
        switch (resultText)
        {
            case "Dog":
                return "Messages/Results/BulldogUnlocked";
            case "Cat":
                return "Messages/Results/CatUnlocked";
            default:
                return "";
        }
    }

    public Conversation CreateLostDogConversation(List<int> choices, CharacterProperties npcProperties = null)
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.npcName = LOST_DOG_NPC_NAME;
        newConversation.npcProperties =
            (npcProperties == null) ? this._characterRandomization.GetFullRandomCharacter() : npcProperties;
        newConversation.choiceCount = 3;

        var newMessage1 = new Message();
        newMessage1.text = "Hi, " + this._userSerializer.PlayerName + "! I saw your post and noticed a severe lack of fluffy friends in it.";
        newMessage1.type = DelaygramMessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "I work with the local non-profit shelter, and we are absolutely STOCKED with pets, sadly.";
        newMessage2.type = DelaygramMessageType.NPC;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "But lucky for YOU, I have an interesting offer.";
        newMessage3.type = DelaygramMessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "You can adopt for free one of our animals!";
        newMessage4.type = DelaygramMessageType.NPC;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = "Here's the catch: You HAVE to include them in your next three pictures! You know, free publicity for the shelter and all.";
        newMessage5.type = DelaygramMessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage6 = new Message();
        newMessage6.text = "If you don't .. we may have to take them back. So! Which furry friend would you like?";
        newMessage6.type = DelaygramMessageType.NPC;
        newMessage6.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage6);

        bool messageLoop = true;
        int choiceIndex = 0;
        while (messageLoop)
        {
            var newMessage7 = new Message();
            if (choices.Count == choiceIndex)
            {   // At the point where user hasn't made a choice at this index
                newMessage7.type = DelaygramMessageType.Choice;
                newMessage7.choices = new List<string>();
                newMessage7.choices.Add("Cat!");
                newMessage7.choices.Add("Dog!");
                if (choices.Count == 0)
                {   // Only show this the first time
                    newMessage7.choices.Add("A mixture of both!");
                }
                newMessage7.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage7);

                messageLoop = false;
            }
            else if (choices[choiceIndex] == 1)
            {
                newMessage7.text = "Cat!";
                newMessage7.type = DelaygramMessageType.Player;
                newMessage7.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage7);

                var newMessage8a = new Message();
                newMessage8a.text = "Great! I'll bring her over right now! Send me your address.";
                newMessage8a.type = DelaygramMessageType.NPC;
                newMessage8a.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage8a);

                var newMessage9a = new Message();
                newMessage9a.text = "Cat";
                newMessage9a.type = DelaygramMessageType.Result;
                newMessage9a.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage9a);

                messageLoop = false;
            }
            else if (choices[choiceIndex] == 2)
            {
                newMessage7.text = "Dog!";
                newMessage7.type = DelaygramMessageType.Player;
                newMessage7.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage7);

                var newMessage8a = new Message();
                newMessage8a.text = "Great! I'll bring him over right now! Send me your address.";
                newMessage8a.type = DelaygramMessageType.NPC;
                newMessage8a.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage8a);

                var newMessage9a = new Message();
                newMessage9a.text = "Dog";
                newMessage9a.type = DelaygramMessageType.Result;
                newMessage9a.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage9a);

                messageLoop = false;
            }
            else if (choices[choiceIndex] == 3)
            {
                newMessage7.text = "A mixture of both!";
                newMessage7.type = DelaygramMessageType.Player;
                newMessage7.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage7);

                var newMessage8a = new Message();
                newMessage8a.text = "Okay dude, this isn't a 90's TV show .. can you just pick an actual animal please?";
                newMessage8a.type = DelaygramMessageType.NPC;
                newMessage8a.timeSent = DateTime.Now;
                newConversation.messages.Add(newMessage8a);
            }

            choiceIndex++;
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
            newMessage7.choices.Add("Okay, I could use new threads I guess, sign me up");
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
