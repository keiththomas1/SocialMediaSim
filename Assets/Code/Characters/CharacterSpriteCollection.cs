using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteCollection : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _bodySprites;
    [SerializeField]
    private List<Sprite> _maleFaceSprites;
    [SerializeField]
    private List<Sprite> _femaleFaceSprites;
    [SerializeField]
    private List<Sprite> _maleHairSprites;
    [SerializeField]
    private List<Sprite> _femaleHairSprites;

    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
    }

    public List<Sprite> BodySprites
    {
        get { return this._bodySprites; }
    }

    public List<Sprite> MaleFaceSprites
    {
        get { return this._maleFaceSprites; }
    }
    public List<Sprite> FemaleFaceSprites
    {
        get { return this._femaleFaceSprites; }
    }

    public List<Sprite> MaleHairSprites
    {
        get { return this._maleHairSprites; }
    }
    public List<Sprite> FemaleHairSprites
    {
        get { return this._femaleHairSprites; }
    }
}
