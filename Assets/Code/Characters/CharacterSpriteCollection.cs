using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteCollection : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _maleBodySprites;
    [SerializeField]
    private List<Sprite> _femaleBodySprites;
    [SerializeField]
    private List<Sprite> _maleFaceSprites;
    [SerializeField]
    private List<Sprite> _femaleFaceSprites;
    [SerializeField]
    private List<Sprite> _maleHairSprites;
    [SerializeField]
    private List<Sprite> _femaleHairSprites;
    [SerializeField]
    private List<GameObject> _maleEyePrefabs;
    [SerializeField]
    private List<GameObject> _femaleEyePrefabs;
    [SerializeField]
    private List<Sprite> _maleLeftArmSprites;
    [SerializeField]
    private List<Sprite> _femaleLeftArmSprites;
    [SerializeField]
    private List<Sprite> _maleRightArmSprites;
    [SerializeField]
    private List<Sprite> _femaleRightArmSprites;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
    }

    public List<Sprite> MaleHairSprites
    {
        get { return this._maleHairSprites; }
    }
    public List<Sprite> FemaleHairSprites
    {
        get { return this._femaleHairSprites; }
    }

    public List<Sprite> MaleFaceSprites
    {
        get { return this._maleFaceSprites; }
    }
    public List<Sprite> FemaleFaceSprites
    {
        get { return this._femaleFaceSprites; }
    }

    public List<string> MaleEyeSprites
    {
        get
        {
            var eyeNames = new List<string>();
            foreach (var eyePrefab in this._maleEyePrefabs)
            {
                eyeNames.Add(eyePrefab.name);
            }
            return eyeNames;
        }
    }
    public List<string> FemaleEyeSprites
    {
        get
        {
            var eyeNames = new List<string>();
            foreach (var eyePrefab in this._femaleEyePrefabs)
            {
                eyeNames.Add(eyePrefab.name);
            }
            return eyeNames;
        }
    }

    public List<Sprite> MaleBodySprites
    {
        get { return this._maleBodySprites; }
    }
    public List<Sprite> FemaleBodySprites
    {
        get { return this._femaleBodySprites; }
    }

    public List<Sprite> MaleLeftArmSprites
    {
        get { return this._maleLeftArmSprites; }
    }
    public List<Sprite> FemaleLeftArmSprites
    {
        get { return this._femaleLeftArmSprites; }
    }

    public List<Sprite> MaleRightArmSprites
    {
        get { return this._maleRightArmSprites; }
    }
    public List<Sprite> FemaleRightArmSprites
    {
        get { return this._femaleRightArmSprites; }
    }
}
