using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNameGenerator
{
    private List<string> _beginnings;
    private List<string> _middles;
    private List<string> _endings;
    private List<string> _adjectives;
    private List<string> _nouns;

    // Use this for initialization
    public RandomNameGenerator()
    {
        this._beginnings = new List<string>();
        this._middles = new List<string>();
        this._endings = new List<string>();
        this._adjectives = new List<string>();
        this._nouns = new List<string>();
        this.LoadWords();
    }

    public string GenerateRandomName()
    {
        var beginningSelection = UnityEngine.Random.Range(0, this._beginnings.Count);
        var firstWordSelection = UnityEngine.Random.Range(0, this._adjectives.Count);
        var middleSelection = UnityEngine.Random.Range(0, this._middles.Count);
        var secondWordSelection = UnityEngine.Random.Range(0, this._adjectives.Count);
        var randomNumbers = UnityEngine.Random.Range(0, 2) == 1 ? this.GenerateRandomNumbers() : "";
        var endingSelection = UnityEngine.Random.Range(0, this._endings.Count);
        var name = this._beginnings[beginningSelection] +
            this._adjectives[firstWordSelection] +
            this._middles[middleSelection] +
            this._nouns[secondWordSelection] +
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
        this._adjectives.Add("tasteless");
        this._adjectives.Add("nervous");
        this._adjectives.Add("shallow");
        this._adjectives.Add("disturbed");
        this._adjectives.Add("salty");
        this._adjectives.Add("addicted");
        this._adjectives.Add("paranoid");
        this._adjectives.Add("obnoxious");
        this._adjectives.Add("smooth");
        this._adjectives.Add("magnificent");
        this._adjectives.Add("zany");
        this._adjectives.Add("curvy");
        this._adjectives.Add("brawny");
        this._adjectives.Add("ripped");
        this._adjectives.Add("splendid");
        this._adjectives.Add("lazy");
        this._adjectives.Add("pathetic");
        this._adjectives.Add("plastic");
        this._adjectives.Add("jittery");
        this._adjectives.Add("watery");

        // Female
        // this._adjectives.Add("princess");
        // this._adjectives.Add("baby");
        // this._adjectives.Add("mami");
        // this._adjectives.Add("queen");

        // Male
        // this._adjectives.Add("king");
        // this._adjectives.Add("meathead");
        // this._adjectives.Add("papi");

        this._nouns.Add("diva");
        this._nouns.Add("papi");
        this._nouns.Add("princess");
        this._nouns.Add("baby");
        this._nouns.Add("queen");
        this._nouns.Add("king");
        this._nouns.Add("addict");
        this._nouns.Add("peasant");
        this._nouns.Add("master");
        this._nouns.Add("model");
        this._nouns.Add("idiot");
        this._nouns.Add("jackass");
        this._nouns.Add("vlogger");
        this._nouns.Add("meathead");
        this._nouns.Add("yogafan");
        this._nouns.Add("footballer");
        this._nouns.Add("athlete");
        this._nouns.Add("slob");
        this._nouns.Add("footballer");
        this._nouns.Add("nerd");
        this._nouns.Add("geek");
        this._nouns.Add("sleazeball");
    }

}
