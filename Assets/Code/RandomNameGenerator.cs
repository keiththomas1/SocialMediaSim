using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNameGenerator
{
    private List<string> _beginnings;
    private List<string> _middles;
    private List<string> _endings;
    private List<string> _words;

    // Use this for initialization
    public RandomNameGenerator()
    {
        this._beginnings = new List<string>();
        this._middles = new List<string>();
        this._endings = new List<string>();
        this._words = new List<string>();
        this.LoadWords();
    }

    public string GenerateRandomName()
    {
        var beginningSelection = UnityEngine.Random.Range(0, this._beginnings.Count);
        var firstWordSelection = UnityEngine.Random.Range(0, this._words.Count);
        var middleSelection = UnityEngine.Random.Range(0, this._middles.Count);
        var secondWordSelection = UnityEngine.Random.Range(0, this._words.Count);
        var randomNumbers = UnityEngine.Random.Range(0, 2) == 1 ? this.GenerateRandomNumbers() : "";
        var endingSelection = UnityEngine.Random.Range(0, this._endings.Count);
        var name = this._beginnings[beginningSelection] +
            this._words[firstWordSelection] +
            this._middles[middleSelection] +
            this._words[secondWordSelection] +
            randomNumbers +
            this._endings[endingSelection];

        return name;
    }

    private string GenerateRandomNumbers()
    {
        return UnityEngine.Random.Range(0, 999).ToString();
    }

    private void LoadWords()
    {
        this._beginnings.Add("");
        this._beginnings.Add("xX");
        this._beginnings.Add("~");

        this._middles.Add("");
        this._middles.Add(".");
        this._middles.Add("X");

        this._endings.Add("");
        this._endings.Add("Xx");
        this._beginnings.Add("~");

        // Neutral
        this._words.Add("love");
        this._words.Add("diva");
        this._words.Add("badass");
        this._words.Add("rich");
        this._words.Add("nasty");
        this._words.Add("AF");
        this._words.Add("dope");
        this._words.Add("shady");
        this._words.Add("hard");
        this._words.Add("huge");
        this._words.Add("massive");
        this._words.Add("tiny");
        this._words.Add("champagne");
        this._words.Add("weeb");
        this._words.Add("amor");

        // Female
        this._words.Add("princess");
        this._words.Add("baby");
        this._words.Add("mami");
        this._words.Add("queen");

        // Male
        this._words.Add("king");
        this._words.Add("meathead");
        this._words.Add("papi");
    }

}
