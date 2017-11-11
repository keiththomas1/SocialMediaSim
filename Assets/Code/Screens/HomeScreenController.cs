using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class HomeScreenController : MonoBehaviour {
    public Sprite[] photos;
    public Sprite[] faces;
    public Sprite anonymousMalePortrait;
    public Sprite anonymousFemalePortrait;
    private UserSerializer serializer;
    private RandomEventController eventController;
    private GameObject post;
    private GameObject tapLabel;
    private ArrayList hashTags;
    private float currentClickCount;
    private float currentRandomEventCount;

    private GameObject annoyMeter;
    private GameObject annoyFace;
    private GameObject annoyInfo;
    private Vector3 originalAnnoyFacePosition;
    private Vector3 finalAnnoyFacePosition;
    private int currentAnnoyance;
    private float defaultAnnoyTimer;
    private float currentAnnoyTimer;
    private int clickMultiplier;

    void Awake()
    {
        serializer = UserSerializer.Instance;
        eventController = GetComponent<RandomEventController>();
        FillHashTags();
        currentClickCount = 0.0f;
        currentRandomEventCount = 1.0f;
        currentAnnoyance = 0;
        defaultAnnoyTimer = 0.3f;
        currentAnnoyTimer = defaultAnnoyTimer;
	}
	
	void Update () {
        currentAnnoyTimer -= Time.deltaTime;
        if (currentAnnoyTimer <= 0.0f)
        {
            UpdateAnnoyance(-1);
            UpdateAnnoyanceMeter();
            currentAnnoyTimer = defaultAnnoyTimer;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (annoyInfo)
            {
                annoyInfo.GetComponent<SpriteRenderer>().enabled = false;
            }
        } else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                switch (hit.collider.name)
                {
                    case "Scale":
                        if (annoyInfo)
                        {
                            annoyInfo.GetComponent<SpriteRenderer>().enabled = true;
                        }
                        break;
                    default:
                        if (annoyInfo)
                        {
                            annoyInfo.GetComponent<SpriteRenderer>().enabled = false;
                        }
                        break;
                }
            } else {
                if (annoyInfo)
                {
                    annoyInfo.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
        }
    }

    public void CheckClick(string colliderName)
    {
        switch (colliderName)
        {
            case "RandomPicture":
                PostClicked();
                break;
        }
    }

    public void EnterScreen()
    {
        CreateNewRandomPost();
        CreateAnnoyanceMeter();

        tapLabel = GameObject.Instantiate(Resources.Load("Home/TapLabel") as GameObject);

        currentAnnoyTimer = 0.05f;
        clickMultiplier = 1; // serializer.IsDoubleClickEnabled() ? 2 : 1;
    }

    private void PostClicked()
    {
        if (this.post)
        {
            RemovePost(this.post);
        }
        if (this.tapLabel)
        {
            GameObject.Destroy(this.tapLabel);
        }
        this.CreateNewRandomPost();
        this.UpdateClickCount(this.clickMultiplier);
        this.CheckClickOutcome();
        this.UpdateAnnoyance(3);

        GameObject.Instantiate(Resources.Load("Home/FollowedText") as GameObject);
    }

    private void CreateAnnoyanceMeter()
    {
        annoyMeter = GameObject.Instantiate(Resources.Load("Home/AnnoyanceMeter") as GameObject);
        var annoyMeterPosition = annoyMeter.transform.position;
        annoyMeterPosition.x = 0.1f;
        annoyMeterPosition.y = -1.2f;
        annoyMeter.transform.position = annoyMeterPosition;

        annoyFace = annoyMeter.transform.Find("Face").gameObject;
        originalAnnoyFacePosition = annoyFace.transform.localPosition;

        var annoyFaceEnd = annoyMeter.transform.Find("FaceEnd").gameObject;
        finalAnnoyFacePosition = annoyFaceEnd.transform.localPosition;

        annoyInfo = annoyMeter.transform.Find("Info").gameObject;
    }

    public void CreateNewRandomPost()
    {
        post = GameObject.Instantiate(Resources.Load("Home/DGHomePage") as GameObject);
        // var postPosition= post.transform.position;
        // postPosition.y = 1.3f;
        // post.transform.position = postPosition;

        var likesText = post.transform.Find("LikesText");
        if (likesText)
        {
            likesText.GetComponent<TextMeshPro>().text = GetRandomLikes();
        }
        var randomPicture = post.transform.Find("RandomPicture");
        if (randomPicture)
        {
            randomPicture.GetComponent<SpriteRenderer>().sprite = GetRandomPicture();
        }
        var hashTagText = post.transform.Find("HashTagText");
        if (hashTagText)
        {
            hashTagText.GetComponent<TextMesh>().text = GetRandomHashtags();
        }
        var personPortrait = post.transform.Find("AnonymousPerson");
        if (personPortrait)
        {
            personPortrait.GetComponent<SpriteRenderer>().sprite = GetRandomPortrait();
        }
    }

    private void UpdateClickCount(int clickCount)
    {
        if (currentAnnoyance < 32)
        {
            currentClickCount += .2f * clickCount;
        }
        else if (currentAnnoyance < 64)
        {
            currentClickCount += .15f * clickCount;
        }
        else if (currentAnnoyance < 90)
        {
            currentClickCount += .1f * clickCount;
        }
        else
        {
            currentClickCount += .05f * clickCount;
        }
    }

    private void CheckClickOutcome()
    {
        if (currentClickCount >= 1.0f)
        {
            currentClickCount -= 1.0f;
            int newFollowers = 1; // Can eventually do random, or increased based on items
            serializer.AddFollowers(newFollowers);
        }

        currentRandomEventCount = currentRandomEventCount * 1.2f;
        var randomEventRoll = Random.Range(0, 500);
        if (randomEventRoll < currentRandomEventCount)
        {
            eventController.CreateNewEvent();
            currentRandomEventCount = 1;
        }
    }

    private void RemovePost(GameObject post)
    {
        RemoveComponentNames(post);
        var deathByTimer = post.AddComponent<DeathByTimer>();
        deathByTimer.deathTimeInSeconds = 3.0f;
        post.GetComponent<PictureRotateAway>().StartAnimation(PictureRotateAway.RotateDirection.Random);
    }

    private void RemoveComponentNames(GameObject post)
    {
        var likesText = GameObject.Find("LikesText");
        if (likesText)
        {
            likesText.name = "LikesTextTemp";
        }
        var randomPicture = GameObject.Find("RandomPicture");
        if (randomPicture)
        {
            randomPicture.name = "RandomPictureTemp";
        }
        var hashTagText = GameObject.Find("HashTagText");
        if (hashTagText)
        {
            hashTagText.name = "HashTagTextTemp";
        }
        var personPortrait = GameObject.Find("AnonymousPerson");
        if (personPortrait)
        {
            personPortrait.name = "PersonPortraitTemp";
        }
    }

    public void DestroyPage()
    {
        DestroyPost();

        GameObject.Destroy(tapLabel);
        GameObject.Destroy(annoyMeter);
    }

    public void DestroyPost()
    {
        if (post)
        {
#if UNITY_EDITOR
        GameObject.DestroyImmediate(post);
#else
        GameObject.Destroy(post);
#endif
        }
    }

    private void UpdateAnnoyance(int change)
    {
        if (currentAnnoyance + change < 0)
        {
            currentAnnoyance = 0;
        }
        else if (currentAnnoyance + change > 100)
        {
            currentAnnoyance = 100;
        }
        else
        {
            currentAnnoyance += change;
        }
        UpdateAnnoyanceMeter();
    }

    private void UpdateAnnoyanceMeter()
    {
        if (annoyFace)
        {
            if (faces.Length < 3)
            {
                return;
            }
            if (currentAnnoyance < 32)
            {
                annoyFace.GetComponent<SpriteRenderer>().sprite = faces[0];
            }
            else if (currentAnnoyance < 64)
            {
                annoyFace.GetComponent<SpriteRenderer>().sprite = faces[1];
            }
            else if (currentAnnoyance < 90)
            {
                annoyFace.GetComponent<SpriteRenderer>().sprite = faces[2];
            }
            else
            {
                annoyFace.GetComponent<SpriteRenderer>().sprite = faces[3];
            }
            // switch (annoyance) if below 30, face = 1 else if below 60, face = 2, etc

            var totalXDistance = finalAnnoyFacePosition.x - originalAnnoyFacePosition.x;
            var totalYDistance = finalAnnoyFacePosition.y - originalAnnoyFacePosition.y;
            var newAnnoyFacePosition = originalAnnoyFacePosition;

            var annoyancePercentage = currentAnnoyance / 100.0f;
            newAnnoyFacePosition.x = originalAnnoyFacePosition.x + (totalXDistance * annoyancePercentage);
            newAnnoyFacePosition.y = originalAnnoyFacePosition.y + (totalYDistance * annoyancePercentage);

            annoyFace.transform.localPosition = newAnnoyFacePosition;
        }
    }

    private string GetRandomHashtags()
    {
        var numberOfHashTags = Random.Range(1, 7);
        List<int> usedIndexes = new List<int>();
        string hashTagString = "";
        int currentLineLength = 0;
        for (int i = 0; i < numberOfHashTags; i++)
        {
            int hashTagIndex;
            do
            {
                hashTagIndex = Random.Range(0, hashTags.Count);
            } while (usedIndexes.Contains(hashTagIndex));

            var newHashTag = (string)hashTags[hashTagIndex];
            var newLineLength = currentLineLength + newHashTag.Length;
            if (newLineLength < 30)
            {
                hashTagString += hashTags[hashTagIndex];
                hashTagString += " ";
                currentLineLength = newLineLength;
            } else {
                if (usedIndexes.Count > 0)
                {
                    hashTagString += System.Environment.NewLine;
                }
                hashTagString += hashTags[hashTagIndex];
                hashTagString += " ";
                currentLineLength = newHashTag.Length;
            }

            usedIndexes.Add(hashTagIndex);
        }

        return hashTagString;
    }

    // Eventually make this favor the lower end of likes
    private string GetRandomLikes()
    {
        string likeCountText;
        var likeCount = Random.Range(0, 5000);
        if (likeCount > 1000)
        {
            likeCount = likeCount / 100;
            var likeCountFloat = likeCount / 10.0f;
            likeCountText = likeCountFloat.ToString() + "k people love this";
        }
        else
        {
            likeCountText = likeCount.ToString() + " people love this";
        }
        return likeCountText;
    }

    private Sprite GetRandomPicture()
    {
        var randomPhotoIndex = Random.Range(0, photos.Length);
        return photos[randomPhotoIndex];
    }

    private Sprite GetRandomPortrait()
    {
        var randomPortrait = Random.Range(0.0f, 1.0f);
        if (randomPortrait > 0.5f)
        {
            return anonymousFemalePortrait;
        }
        else
        {
            return anonymousMalePortrait;
        }
    }

    // Might be nice to have this draw from external text file
    private void FillHashTags() 
    {
        hashTags = new ArrayList();
        hashTags.Add("#Sick");
        hashTags.Add("#FitFam");
        hashTags.Add("#FitLife");
        hashTags.Add("#SuperSick");
        hashTags.Add("#KillingIt");
        hashTags.Add("#DopeAF");
        hashTags.Add("#OnFleek");
        hashTags.Add("#CoolShit");
        hashTags.Add("#MountainLife");
        hashTags.Add("#BeachLife");
        hashTags.Add("#BeachBum");
        hashTags.Add("#GirlsWithTattoos");
        hashTags.Add("#InkedModel");
        hashTags.Add("#InkNation");
        hashTags.Add("#instahot");
        hashTags.Add("#Like4Like");
        hashTags.Add("#FollowBack");
        hashTags.Add("#DoubleTap");
        hashTags.Add("#Model");
        hashTags.Add("#GoodTimes");
        hashTags.Add("#Yolo");
        hashTags.Add("#MyLifeIsAmazing");
        hashTags.Add("#WhatsItLikeBeingPoor");
        hashTags.Add("#PoorPeopleAreGross");
        hashTags.Add("#ItsHardBeingSoRich");
        hashTags.Add("#ThankGodMyDaddysRich");
        hashTags.Add("#MyLifeIsBetterThanYours");
        hashTags.Add("#USayVainISayGorgeous");
        hashTags.Add("#ICouldPartyLikeAllDay");
    }
}
