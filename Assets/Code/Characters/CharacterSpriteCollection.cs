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
}
