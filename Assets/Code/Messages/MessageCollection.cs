using System;
using System.Collections.Generic;
using UnityEngine;

public class MessageCollection
{
    public const string PROFESSOR_NAME = "Professor Woke";
    public const string PRODUCT_ENGINEER_NAME = "Manipulative Mary";
    public const string PLASTIC_SURGERY_NAME = "Dr. Bits & Pieces";

    private const string PURPLE = "#A300F9FF";
    private const string AQUA = "#00C0FFFF";
    private const string HOT_PINK = "#FF00B9FF";
    private const string RED = "#FF0005FF";
    private const string GREEN = "#00C111FF";
    private const string YELLOW = "#ADB61CFF";
    private const string ORANGE = "#F29A2BFF";

    private CharacterRandomization _characterRandomization;

    public enum MessageAction
    {
        None = 0,
        EnableNotifications,
        BotchBirthmark
    }

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
            case "BotchedBirthmark":
                return "Messages/Results/BotchedSurgery";
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
        newMessage1.text = $"Hey there! Welcome to this grand <{PURPLE}>adventure</color> you have set off on! It will surely be the greatest journey of your life!";
        newMessage1.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = $"Social media is a <{AQUA}>beautiful</color> place where people will be lovely to you and .. and ..";
        newMessage2.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage2);

        var newMessage3 = new Message();
        newMessage3.text = "You know what, screw it.";
        newMessage3.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = $"Screw social media. It's a <{HOT_PINK}>vain, self-serving, argument-fueling</color> piece of shit.";
        newMessage4.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage4);

        var newMessage5 = new Message();
        newMessage5.text = $"Feeding on us <{ORANGE}>psychologically</color>, draining our attention like by like.";
        newMessage5.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage5);

        var newMessage5b = new Message();
        newMessage5b.text = "I need you to help me spread the word. People need to wake the hell up.";
        newMessage5b.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage5b);

        var newMessage6 = new Message();
        newMessage6.text = "But we will get to that later. First, you need to make an important choice to start your journey, like usual.";
        newMessage6.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage6);

        var newMessage8 = new Message();
        newMessage8.text = $"Choose a partner! <sprite name=\"1f60e\"> I've got a <{AQUA}>cat</color>, a <{RED}>bulldog</color>, and a <{GREEN}>drone</color>. All good pals in their own right.";
        newMessage8.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage8);

        var newMessage9 = new Message();
        if (choices.Count == 0)
        {
            newMessage9.type = DelaygramMessageType.Choice;
            newMessage9.choices = new List<string>();
            newMessage9.choices.Add("Cat, please!");
            newMessage9.choices.Add("Dog, obviously?");
            newMessage9.choices.Add("Drone, clearly..");
            newConversation.messages.Add(newMessage9);
        }
        else if (choices[0] == 1)
        {
            newMessage9.text = "Cat, please!";
            newMessage9.type = DelaygramMessageType.Player;
            newConversation.messages.Add(newMessage9);

            var newMessage10a = new Message();
            newMessage10a.text = "Sure thing. Here you go. I'll be sure to check back in with you eventually.";
            newMessage10a.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage10a);

            var newMessage11a = new Message();
            newMessage11a.text = "Also I'll refresh your delay time so you can create a post with your new pal right away. <sprite name=\"1f604\">";
            newMessage11a.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage11a);

            var newMessage12a = new Message();
            newMessage12a.text = "Cat";
            newMessage12a.type = DelaygramMessageType.Result;
            newConversation.messages.Add(newMessage12a);

            newConversation.finished = true;
        }
        else if (choices[0] == 2)
        {
            newMessage9.text = "Dog, obviously?";
            newMessage9.type = DelaygramMessageType.Player;
            newConversation.messages.Add(newMessage9);

            var newMessage10b = new Message();
            newMessage10b.text = "Sure thing. Here you go. I'll be sure to check back in with you eventually.";
            newMessage10b.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage10b);

            var newMessage11b = new Message();
            newMessage11b.text = "Also I'll refresh your delay time so you can create a post with your new pal right away. <sprite name=\"1f604\">";
            newMessage11b.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage11b);

            var newMessage12b = new Message();
            newMessage12b.text = "Dog";
            newMessage12b.type = DelaygramMessageType.Result;
            newConversation.messages.Add(newMessage12b);

            newConversation.finished = true;
        }
        else if (choices[0] == 3)
        {
            newMessage9.text = "Drone, clearly..";
            newMessage9.type = DelaygramMessageType.Player;
            newConversation.messages.Add(newMessage9);

            var newMessage10c = new Message();
            newMessage10c.text = "Sure thing. I call him D-Rone because we found him busted up on the south side. <sprite name=\"1f602\">";
            newMessage10c.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage10c);

            var newMessage11c = new Message();
            newMessage11c.text = "I'll be sure to check back in with you at some point. Also I'll refresh your delay time so you can create a post with your new pal right away.";
            newMessage11c.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(newMessage11c);

            var newMessage12c = new Message();
            newMessage12c.text = "Drone";
            newMessage12c.type = DelaygramMessageType.Result;
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
        newMessage0.text = $"Hello. I work at <{GREEN}>Delaygram</color> and study user behavior.";
        newMessage0.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage0);

        var newMessage1 = new Message();
        newMessage1.text = $"Don't you wonder if exploiting <{HOT_PINK}>psychological</color> effects in an app might be a bit like <{PURPLE}>quantum physics</color>?";
        newMessage1.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage1);

        var newMessage2 = new Message();
        newMessage2.text = "If we observe it, will it cease to be effective?";
        newMessage2.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage2);

        var newMessage2b = new Message();
        newMessage2b.text = "Uhhh ... I don't know";
        newMessage2b.type = DelaygramMessageType.Player;
        newConversation.messages.Add(newMessage2b);

        var newMessage3 = new Message();
        newMessage3.text = $"If I tell you I'm going to send you <{AQUA}>notifications</color> to hopefully spawn an addiction, will you still get addicted?";
        newMessage3.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage3);

        var newMessage4 = new Message();
        newMessage4.text = "If I give you rewards for scrolling your feed, will you actually scroll your feed less?";
        newMessage4.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage4);

        var newMessage4b = new Message();
        newMessage4b.text = "Is this all rhetorical?";
        newMessage4b.type = DelaygramMessageType.Player;
        newConversation.messages.Add(newMessage4b);

        var newMessage5 = new Message();
        newMessage5.text = "Uhm. Yes. Of course. Hahahaha.";
        newMessage5.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage5);

        var newMessage6 = new Message();
        newMessage6.text = $"On an unrelated note, I'm enabling <{AQUA}>notifications</color> for you now.";
        newMessage6.type = DelaygramMessageType.NPC;
        newMessage6.action = MessageAction.EnableNotifications;
        newConversation.messages.Add(newMessage6);

        var newMessage7 = new Message();
        newMessage7.text = "See ya!";
        newMessage7.type = DelaygramMessageType.NPC;
        newConversation.messages.Add(newMessage7);

        return newConversation;
    }


    public Conversation CreatePlasticSurgeryBirthmarkConversation(List<int> choices)
    {
        var newConversation = this.CreateEmptyConversation();
        newConversation.conversationType = DelaygramConversationType.Choice;
        newConversation.npcName = PLASTIC_SURGERY_NAME;
        newConversation.choiceCount = 2;

        newConversation.messages = new List<Message>()
        {
            new Message()
            {
                text = $"Hey hey how ya doin’? We specialize in <{YELLOW}>slicing and dicing</color>. Making you more <{HOT_PINK}>beautiful</color> than your wildest dreams.",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = "Saw one of your posts on here. Think you might be in need of our services.",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = "Right .. how did you even see my post?",
                type = DelaygramMessageType.Player
            },
            new Message()
            {
                text = $"<{ORANGE}>Creeping</color>, of course .. wish you could take a look at my search history, some really messed up shit in there.",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = "Lots of bad plastic surgery jobs out there, that’s for sure",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = $"<{RED}>Dangerous</color> stuff, man",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = "Well, I mean, not if you come to us though. Obviously.",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = "Yes, my confidence in you is now through the roof..",
                type = DelaygramMessageType.Player
            },
            new Message()
            {
                text = "Great! Good to hear. I mean I'm pretty new but my mom always said I was meant for great things!",
                type = DelaygramMessageType.NPC
            },
            new Message()
            {
                text = $"So, yeah how about we start off by getting rid of that <{AQUA}>birthmark</color> of yours?",
                type = DelaygramMessageType.NPC
            }
        };

        if (choices.Count == 0)
        {
            var choiceMessage = new Message();
            choiceMessage.type = DelaygramMessageType.Choice;
            choiceMessage.choices = new List<string>();
            choiceMessage.choices.Add("Heck yeah!");
            choiceMessage.choices.Add("Ehhh, I think I’m good");
            newConversation.messages.Add(choiceMessage);
        }
        else if (choices[0] == 1)
        {
            var yesMessage1 = new Message();
            yesMessage1.text = "Heck yeah!";
            yesMessage1.type = DelaygramMessageType.Player;
            newConversation.messages.Add(yesMessage1);

            // TODO: Add some dialog from NPC about messing up the surgery
            var yesMessage2 = new Message();
            yesMessage2.text = $"<{RED}>*Performing surgery*</color>";
            yesMessage2.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage2);

            var yesMessage3 = new Message();
            yesMessage3.text = ".....";
            yesMessage3.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage3);

            var yesMessage4 = new Message();
            yesMessage4.text = $"Okay, all done! So .. some <{GREEN}>great</color> news and some <{ORANGE}>not-so-great</color> news.";
            yesMessage4.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage4);

            var yesMessage5 = new Message();
            yesMessage5.text = $"<{GREEN}>Great</color> news - birthmark has been removed!! <{ORANGE}>Not-so-great</color> news - you now have a permanent red mark there.";
            yesMessage5.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage5);

            var yesMessage6 = new Message();
            yesMessage6.text = "But what's important here is that I am now more experienced and will likely not botch this job up anymore.";
            yesMessage6.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage6);

            var yesMessage7 = new Message();
            yesMessage7.text = "Thanks for your business! See ya!";
            yesMessage7.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(yesMessage7);

            var yesMessageResult = new Message();
            yesMessageResult.text = "BotchedBirthmark";
            yesMessageResult.type = DelaygramMessageType.Result;
            yesMessageResult.action = MessageAction.BotchBirthmark;
            newConversation.messages.Add(yesMessageResult);

            newConversation.finished = true;
        }
        else if (choices[0] == 2)
        {
            var noMessage1 = new Message();
            noMessage1.text = "Ehhh, I think I’m good";
            noMessage1.type = DelaygramMessageType.Player;
            newConversation.messages.Add(noMessage1);

            var noMessage2 = new Message();
            noMessage2.text = "Okay! Your loss!";
            noMessage2.type = DelaygramMessageType.NPC;
            newConversation.messages.Add(noMessage2);

            newConversation.finished = true;
        }

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
        newConversation.choiceCount = 0;
        newConversation.choicesMade = new List<int>();
        newConversation.viewed = false;
        newConversation.finished = false;
        newConversation.timeSent = DateTime.Now;

        return newConversation;
    }
}
