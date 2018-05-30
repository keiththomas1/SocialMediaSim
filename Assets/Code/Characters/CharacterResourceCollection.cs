using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterResourceCollection : MonoBehaviour
{
    [SerializeField]
    private List<Sprite> _maleBodySprites;
    [SerializeField]
    private List<Sprite> _femaleBodySprites;
    [SerializeField]
    private List<Sprite> _maleYellowFaceSprites;
    [SerializeField]
    private List<Sprite> _maleWhiteFaceSprites;
    [SerializeField]
    private List<Sprite> _femaleYellowFaceSprites;
    [SerializeField]
    private List<Sprite> _femaleWhiteFaceSprites;
    [SerializeField]
    private List<Sprite> _maleHairSprites;
    [SerializeField]
    private List<Sprite> _femaleHairSprites;
    [SerializeField]
    private List<GameObject> _maleEyePrefabs;
    [SerializeField]
    private List<GameObject> _femaleEyePrefabs;

    [SerializeField]
    private List<Sprite> _maleArmSprites;
    [SerializeField]
    private List<Sprite> _femaleArmSprites;
    [SerializeField]
    private List<Sprite> _maleLegSprites;
    [SerializeField]
    private List<Sprite> _femaleLegSprites;


    public AnimatorOverrideController _maleAnimatorControllerLevel2;
    public AnimatorOverrideController _femaleAnimatorControllerLevel2;

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

    public List<Sprite> MaleYellowFaceSprites
    {
        get { return this._maleYellowFaceSprites; }
    }
    public List<Sprite> MaleWhiteFaceSprites
    {
        get { return this._maleWhiteFaceSprites; }
    }
    public List<Sprite> FemaleYellowFaceSprites
    {
        get { return this._femaleYellowFaceSprites; }
    }
    public List<Sprite> FemaleWhiteFaceSprites
    {
        get { return this._femaleWhiteFaceSprites; }
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

    public List<Sprite> MaleArmSprites
    {
        get { return this._maleArmSprites; }
    }
    public List<Sprite> FemaleArmSprites
    {
        get { return this._femaleArmSprites; }
    }

    public List<Sprite> MaleLegSprites
    {
        get { return this._maleLegSprites; }
    }
    public List<Sprite> FemaleLegSprites
    {
        get { return this._femaleLegSprites; }
    }
}
