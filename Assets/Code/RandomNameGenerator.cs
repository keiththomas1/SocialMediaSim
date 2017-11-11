using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomNameGenerator
{
    private List<string> _randomFirstNames;
    private List<string> _randomLastNames;

    // Use this for initialization
    public RandomNameGenerator()
    {
        this._randomFirstNames = new List<string>();
        this._randomLastNames = new List<string>();
        this.LoadRandomNames();
    }

    public string GenerateRandomName()
    {
        var firstNameSelection = UnityEngine.Random.Range(0, this._randomFirstNames.Count);
        var name = this._randomFirstNames[firstNameSelection];
        var lastNameSelection = UnityEngine.Random.Range(0, this._randomLastNames.Count);
        name += this._randomLastNames[lastNameSelection];

        return name;
    }

    private void LoadRandomNames()
    {
        this._randomFirstNames.Add("Dixie");
        this._randomFirstNames.Add("Bubbles");
        this._randomFirstNames.Add("Mercedes");
        this._randomFirstNames.Add("Shavonda");
        this._randomFirstNames.Add("Dixie");
        this._randomFirstNames.Add("Pixie");
        this._randomFirstNames.Add("Crystal");
        this._randomFirstNames.Add("Amber");
        this._randomFirstNames.Add("Angel");
        this._randomFirstNames.Add("Dixie");
        this._randomFirstNames.Add("Dixie");

        this._randomLastNames.Add("Mellow");
        this._randomLastNames.Add("Fancy");
        this._randomLastNames.Add("Lovely");
        this._randomLastNames.Add("Rose");
        this._randomLastNames.Add("Splendid");
        this._randomLastNames.Add("Goode");
        this._randomLastNames.Add("Muffintrucks");
        this._randomLastNames.Add("Normous");
        this._randomLastNames.Add("Thotterella");
        this._randomLastNames.Add("Hobag");
        this._randomLastNames.Add("Enigma");
        this._randomLastNames.Add("Beauty");
    }

}
