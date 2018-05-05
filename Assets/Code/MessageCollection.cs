using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageCollection
{
    public const string PROFESSOR_NAME = "Professor Woke";
    public const string PRODUCT_ENGINEER_NAME = "Manipulative Mary";

    private CharacterRandomization _characterRandomization;

    public MessageCollection()
    {
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
            case "Drone":
                return "Messages/Results/DroneUnlocked";
            default:
                return "";
        }
    }

    public Conversation CreateProfessorConversation(List<int> choices)
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.conversationType = DelaygramConversationType.Choice;
        newConversation.npcName = PROFESSOR_NAME;
        newConversation.choiceCount = 3;

        var newMessage1 = new Message();
        newMessage1.text = "Hey there! Welcome to this grand <#A300F9FF>adventure</color> you have set off on! It will surely be the greatest journey of your life!";
        newMessage1.type = DelaygramMessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "Social media is a <#00C111FF>beautiful</color> place where people will be lovely to you and .. and ..";
        newMessage2.type = DelaygramMessageType.NPC;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "You know what, screw it.";
        newMessage3.type = DelaygramMessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "Screw social media. It's a <#FF00B9FF>vain, self-serving, argument-fueling</color> piece of shit.";
        newMessage4.type = DelaygramMessageType.NPC;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = "Feeding on us <#F29A2BFF>psychologically</color>, draining our attention like by like.";
        newMessage5.type = DelaygramMessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage5b = new Message();
        newMessage5b.text = "I need you to help me spread the word. People need to wake the hell up.";
        newMessage5b.type = DelaygramMessageType.NPC;
        newMessage5b.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5b);

        var newMessage6 = new Message();
        newMessage6.text = "But we will get to that later. First, you need to make an important choice to start your journey, like usual.";
        newMessage6.type = DelaygramMessageType.NPC;
        newMessage6.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage6);

        var newMessage8 = new Message();
        newMessage8.text = "Choose a partner! <sprite name=\"1f60e\"> I've got a <#00C0FFFF>cat</color>, a <#FF0005FF>bulldog</color>, and a <#00C111FF>drone</color>. All good pals in their own right.";
        newMessage8.type = DelaygramMessageType.NPC;
        newMessage8.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage8);

        var newMessage9 = new Message();
        if (choices.Count == 0)
        {
            newMessage9.type = DelaygramMessageType.Choice;
            newMessage9.choices = new List<string>();
            newMessage9.choices.Add("Cat, please!");
            newMessage9.choices.Add("Dog, obviously?");
            newMessage9.choices.Add("Drone, clearly..");
            newMessage9.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9);
        }
        else if (choices[0] == 1)
        {
            newMessage9.text = "Cat, please!";
            newMessage9.type = DelaygramMessageType.Player;
            newMessage9.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9);

            var newMessage10a = new Message();
            newMessage10a.text = "Sure thing. Here you go. I'll be sure to check back in with you eventually.";
            newMessage10a.type = DelaygramMessageType.NPC;
            newMessage10a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage10a);

            var newMessage11a = new Message();
            newMessage11a.text = "Also I'll refresh your delay time so you can create a post with your new pal right away. <sprite name=\"1f604\">";
            newMessage11a.type = DelaygramMessageType.NPC;
            newMessage11a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage11a);

            var newMessage12a = new Message();
            newMessage12a.text = "Cat";
            newMessage12a.type = DelaygramMessageType.Result;
            newMessage12a.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage12a);

            newConversation.finished = true;
        }
        else if (choices[0] == 2)
        {
            newMessage9.text = "Dog, obviously?";
            newMessage9.type = DelaygramMessageType.Player;
            newMessage9.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9);

            var newMessage10b = new Message();
            newMessage10b.text = "Sure thing. Here you go. I'll be sure to check back in with you eventually.";
            newMessage10b.type = DelaygramMessageType.NPC;
            newMessage10b.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage10b);

            var newMessage11b = new Message();
            newMessage11b.text = "Also I'll refresh your delay time so you can create a post with your new pal right away. <sprite name=\"1f604\">";
            newMessage11b.type = DelaygramMessageType.NPC;
            newMessage11b.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage11b);

            var newMessage12b = new Message();
            newMessage12b.text = "Dog";
            newMessage12b.type = DelaygramMessageType.Result;
            newMessage12b.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage12b);

            newConversation.finished = true;
        }
        else if (choices[0] == 3)
        {
            newMessage9.text = "Drone, clearly..";
            newMessage9.type = DelaygramMessageType.Player;
            newMessage9.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage9);

            var newMessage10c = new Message();
            newMessage10c.text = "Sure thing. I call him D-Rone because we found him busted up on the south side. <sprite name=\"1f602\">";
            newMessage10c.type = DelaygramMessageType.NPC;
            newMessage10c.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage10c);

            var newMessage11c = new Message();
            newMessage11c.text = "I'll be sure to check back in with you at some point. Also I'll refresh your delay time so you can create a post with your new pal right away.";
            newMessage11c.type = DelaygramMessageType.NPC;
            newMessage11c.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage11c);

            var newMessage12c = new Message();
            newMessage12c.text = "Drone";
            newMessage12c.type = DelaygramMessageType.Result;
            newMessage12c.timeSent = DateTime.Now;
            newConversation.messages.Add(newMessage12c);

            newConversation.finished = true;
        }

        return newConversation;
    }

    public Conversation CreateProductEngineerConversation()
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.conversationType = DelaygramConversationType.Story;
        newConversation.npcName = PRODUCT_ENGINEER_NAME;

        var newMessage0 = new Message();
        newMessage0.text = "Hello. I work at Delaygram and study user behavior.";
        newMessage0.type = DelaygramMessageType.NPC;
        newMessage0.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage0);

        var newMessage1 = new Message();
        newMessage1.text = "Do you think exploiting psychological effects in an app is like quantum physics?";
        newMessage1.type = DelaygramMessageType.NPC;
        newMessage1.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "If we observe it, will it cease to be effective?";
        newMessage2.type = DelaygramMessageType.NPC;
        newMessage2.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2);

        var newMessage2b = new Message();
        newMessage2b.text = "Uhhh ... I don't know";
        newMessage2b.type = DelaygramMessageType.Player;
        newMessage2b.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage2b);

        var newMessage3 = new Message();
        newMessage3.text = "If I tell you I'm going to send you notifications to hopefully spawn an addiction, will you still get addicted?";
        newMessage3.type = DelaygramMessageType.NPC;
        newMessage3.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "If I give you rewards for scrolling your feed, will you actually scroll your feed less?";
        newMessage4.type = DelaygramMessageType.NPC;
        newMessage4.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4);

        var newMessage4b = new Message();
        newMessage4b.text = "Is this all rhetorical?";
        newMessage4b.type = DelaygramMessageType.Player;
        newMessage4b.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage4b);

        var newMessage5 = new Message();
        newMessage5.text = "Uhm. Yes. Of course. Hahahaha.";
        newMessage5.type = DelaygramMessageType.NPC;
        newMessage5.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage5);

        var newMessage6 = new Message();
        newMessage6.text = "I'm enabling notifications for you now.";
        newMessage6.type = DelaygramMessageType.NPC;
        newMessage6.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage6);

        var newMessage7 = new Message();
        newMessage7.text = "See ya!";
        newMessage7.type = DelaygramMessageType.NPC;
        newMessage7.timeSent = DateTime.Now;
        newConversation.messages.Add(newMessage7);

        return newConversation;
    }

    public Conversation CreateFemaleRomance1Conversation(LinkedList<int> choices) {
		var newConversation = this.CreateEmptyConversation ();
		newConversation.conversationType = DelaygramConversationType.Flirty;
		newConversation.npcName = "";
		newConversation.npcProperties = this._characterRandomization.GetFullRandomCharacter(Gender.Female);
		newConversation.npcProperties.happinessLevel = 6;

		return newConversation;
	}

    private Conversation CreateEmptyConversation()
    {
        var newConversation = new Conversation();
        newConversation.messages = new List<Message>();
        newConversation.choicesMade = new List<int>();
        newConversation.viewed = false;
        newConversation.finished = false;

        return newConversation;
    }
}
