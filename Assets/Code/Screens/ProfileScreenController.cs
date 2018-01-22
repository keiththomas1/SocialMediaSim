using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ProfileScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject _chooseNameBox;
    private TextMeshProUGUI _chooseNameText;

    private const float POST_X_OFFSET = -1.06f;
    private const float POST_Y_OFFSET = -3.3f;

    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private PostHelper _postHelper;
    private GoalsController _goalsController;

    private GameObject page;
    private GameObject scrollArea;
    private GameObject _spriteMask;
    private List<DelayGramPostObject> _youPostObjects;
    private bool _firstPostNew;
    private DelayGramPost _latestPost;

    private GameObject _editScreen;
    private CharacterProperties _previousCharacterProperties;

    private int _previousLevel;
    private int _previousLevelExperience;
    private int _previousNeededExperience;

    private enum ProfileScreenState
    {
        ProfileDefault,
        SinglePicture
    }
    private ProfileScreenState _currentState;

    // For handling of selecting an image and resizing/repositioning
    private DelayGramPostObject _currentSelectedImage;
    private Vector3 _originalImageScale;
    private Vector3 _originalImagePosition;
    private bool _imageCurrentlyShrinking = false;

    void Awake () {
        this.characterSerializer = CharacterSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
        this.randomNameGenerator = new RandomNameGenerator();
        this._postHelper = new PostHelper();
        this._goalsController = this.GetComponent<GoalsController>();

        this._youPostObjects = new List<DelayGramPostObject>();
        this._firstPostNew = false;

        this._chooseNameText = this._chooseNameBox.transform.Find("TextInput")
            .transform.Find("TextArea")
            .transform.Find("Text")
            .GetComponent<TextMeshProUGUI>();
        var exitButton = this._chooseNameBox.transform.Find("ExitButtonBack").GetComponent<Button>();
        exitButton.onClick.AddListener(this.NameExitClicked);
        var randomizeButton = this._chooseNameBox.transform.Find("RandomizeButtonBack").GetComponent<Button>();
        randomizeButton.onClick.AddListener(this.NameRandomizeClicked);
        var doneButton = this._chooseNameBox.transform.Find("DoneButtonBack").GetComponent<Button>();
        doneButton.onClick.AddListener(this.NameDoneClicked);
    }

    void Start()
    {
    }
    void Update()
    {
        // HACK for testing
        if (Input.GetKeyDown(KeyCode.L))
        {
            this._previousLevelExperience += 10;
            this.UpdateLevelDisplay();
        }
    }

    public void FinishedCreatingPicture(DelayGramPost post)
    {
        this._firstPostNew = true;
        this._latestPost = post;
    }

    public void CheckClick(string colliderName)
    {
        if (this._chooseNameBox.activeSelf)
        {
            return;
        }
        if (this._editScreen)
        {
            switch (colliderName)
            {
                case "ChooseNameTextBox":
                    this._chooseNameBox.SetActive(true);
                    break;
                case "BackButton":
                    this.ResetCharacterProperties();
                    this.DestroyEditScreen();
                    break;
                case "DoneButton":
                    this.DestroyEditScreen();
                    break;
                default:
                    this._editScreen.GetComponent<CharacterEditor>().CheckClick(colliderName);
                    break;
            }
        } else {
            switch (colliderName)
            {
                case "EditButton":
                    this.CreateEditAvatarScreen(true);
                    break;
                default:
                    foreach (DelayGramPostObject post in this._youPostObjects)
                    {
                        if (post.postObject && colliderName == post.postObject.name)
                        {
                            if (this._currentState == ProfileScreenState.ProfileDefault)
                            {
                                this.EnlargePost(post);
                            }
                            else
                            {
                                this.ShrinkPost(this._currentSelectedImage);
                            }
                        }
                    }
                    break;
            }
        }
    }

    public void EnterScreen()
    {
        this.page = GameObject.Instantiate(Resources.Load("Profile/DGProfilePage") as GameObject);
        this.page.transform.position = new Vector3(0.2f, 1.3f, 0.0f);

        this.scrollArea = page.transform.Find("ScrollArea").gameObject;
        // this.scrollController = scrollArea.AddComponent<ScrollController>();
        this.scrollController = scrollArea.GetComponent<ScrollController>();
        this.scrollController.UpdateScrollArea(scrollArea, scrollArea.transform.localPosition.y, 4.0f);

        var characterSection = scrollArea.transform.Find("CharacterSection").gameObject;
        this._spriteMask = characterSection.transform.Find("SpriteMask").gameObject;
        this.SetAvatar(this._spriteMask);
        this.SetupItems(this._spriteMask);

        this._goalsController.UpdateGoals();
        var goalSection = scrollArea.transform.Find("GoalSection").gameObject;
        this.SetupGoalSection(goalSection);

        var statsSection = scrollArea.transform.Find("StatsSection").gameObject;
        this.SetupStatistics(statsSection);

        this._previousLevel = this._userSerializer.PlayerLevel;
        this._previousLevelExperience = this._userSerializer.LevelExperience;
        this._previousNeededExperience = this._userSerializer.NeededLevelExperience;
        this.UpdateLevelDisplay();

        this.GenerateProfilePosts();
    }

    public bool BackOut()
    {
        if (this._editScreen)
        {
            this.ResetCharacterProperties();
            this.DestroyEditScreen();
            return false;
        }
        return true;
    }

    public void DestroyPage()
    {
        foreach (DelayGramPostObject post in this._youPostObjects)
        {
            if (post.postObject)
            {
                post.postObject.SetActive(false);
                GameObject.Destroy(post.postObject);
            }
        }
        this._youPostObjects.Clear();

        if (this._editScreen)
        {
            this.ResetCharacterProperties();
            GameObject.Destroy(this._editScreen);
        }

        GameObject.Destroy(page);
    }

    public void CreateEditAvatarScreen(bool backButtonEnabled)
    {
        this._editScreen = GameObject.Instantiate(Resources.Load("Profile/CreateCharacterPopup") as GameObject);
        this._editScreen.transform.position = new Vector3(0.3f, 1.55f, 0.0f);
        this.SetAvatar(this._editScreen);
        this.page.SetActive(false);

        var backButton = this._editScreen.transform.Find("BackButton");
        backButton.gameObject.SetActive(backButtonEnabled);

        UpdateText(this._editScreen.transform.Find("ChooseNameTextBox").Find("NameText").gameObject);

        this._previousCharacterProperties = new CharacterProperties(this.characterSerializer.CurrentCharacterProperties);
    }

    private void NameExitClicked()
    {
        this._chooseNameBox.SetActive(false);
    }
    private void NameRandomizeClicked()
    {
        var randomName = this.randomNameGenerator.GenerateRandomName();
        this._chooseNameText.text = randomName;
    }
    private void NameDoneClicked()
    {
        this._userSerializer.PlayerName = this._chooseNameText.text;
        this._chooseNameBox.SetActive(false);
        UpdateText(this._editScreen.transform.Find("ChooseNameTextBox").Find("NameText").gameObject);
    }

    private void DestroyEditScreen()
    {
        GameObject.Destroy(this._editScreen);
        this.page.SetActive(true);
        this.characterSerializer.UpdateAllCharacters();
        this.SetAvatar(this._spriteMask);
    }

    private void ResetCharacterProperties()
    {
        this.characterSerializer.CurrentCharacterProperties = this._previousCharacterProperties;
    }

    private void UpdateText(GameObject textObject)
    {
        if (textObject)
        {
            textObject.GetComponent<TextMeshPro>().text = _userSerializer.PlayerName;
        }
    }

    private void UpdateLevelDisplay()
    {
        var characterSection = this.scrollArea.transform.Find("CharacterSection");
        var levelBanner = characterSection.transform.Find("LevelBanner");

        var levelNumberText = levelBanner.transform.Find("LevelNumberText");
        levelNumberText.GetComponent<TextMeshPro>().text = this._previousLevel.ToString();

        var levelExperienceText = characterSection.transform.Find("LevelExperienceText");
        var experienceText = this._previousLevelExperience.ToString() + "/" + this._previousNeededExperience.ToString();
        levelExperienceText.GetComponent<TextMeshPro>().text = experienceText;

        var experienceBarFront = characterSection.transform.Find("LevelExperienceBar");
        var experienceBarBack = experienceBarFront.transform.Find("ExperienceBar");
        var experiencePercentage = (float)this._previousLevelExperience / (float)this._previousNeededExperience;
        experienceBarBack.transform.localScale = new Vector3(experiencePercentage, 1.0f, 1.0f);
    }

    private void SetupStatistics(GameObject parent)
    {
        var postsCreatedText = parent.transform.Find("PostsCreatedNumber");
        postsCreatedText.GetComponent<TextMeshPro>().text = this._userSerializer.PostCount.ToString();

        var yourLikesText = parent.transform.Find("LikesOnYoursNumber");
        yourLikesText.GetComponent<TextMeshPro>().text = "0";

        var yourDislikesText = parent.transform.Find("DislikesOnYoursNumber");
        yourDislikesText.GetComponent<TextMeshPro>().text = "0";

        var othersLikesText = parent.transform.Find("LikesOnOthersNumber");
        othersLikesText.GetComponent<TextMeshPro>().text = "0";

        var othersDislikesText = parent.transform.Find("DislikesOnOthersNumber");
        othersDislikesText.GetComponent<TextMeshPro>().text = "0";
    }

    private void SetAvatar(GameObject parent)
    {
        var gender = this.characterSerializer.Gender;
        switch (gender)
        {
            case Gender.Female:
                parent.transform.Find("FemaleAvatar").gameObject.SetActive(true);
                parent.transform.Find("MaleAvatar").gameObject.SetActive(false);
                break;
            case Gender.Male:
                parent.transform.Find("MaleAvatar").gameObject.SetActive(true);
                parent.transform.Find("FemaleAvatar").gameObject.SetActive(false);
                break;
        }
    }

    private void SetupItems(GameObject parent)
    {
        if (this._userSerializer.HasBulldog)
        {
            var bulldog = parent.transform.Find("Bulldog");
            if (bulldog)
            {
                bulldog.gameObject.SetActive(true);
            }
        }
        if (this._userSerializer.HasCat)
        {
            var cat = parent.transform.Find("Sylvester");
            if (cat)
            {
                cat.gameObject.SetActive(true);
            }
        }
        if (this._userSerializer.HasDrone)
        {
            var drone = parent.transform.Find("D-Rone");
            if (drone)
            {
                drone.gameObject.SetActive(true);
            }
        }
    }

    private void SetupGoalSection(GameObject goalSection)
    {
        var currentGoals = this._goalsController.GetCurrentGoals();

        for(int i=0; i<currentGoals.Length; i++)
        {
            var goalParent = goalSection.transform.Find("Goal" + i.ToString()).gameObject;
            if (!goalParent)
            {
                Debug.Log("Error finding goal section " + i.ToString());
                continue;
            }

            var currentGoal = currentGoals[i];
            if (currentGoal != null)
            {
                var timesText = (currentGoal.stepsNeeded == 1) ? "time" : "times";
                string textDescription = "";
                switch (currentGoal.goalType)
                {
                    case GoalType.Location:
                        textDescription = String.Format("Post {0} {1} at the {2}",
                            currentGoal.stepsNeeded, timesText, currentGoal.goalObject);
                        break;
                    case GoalType.Item:
                        textDescription = String.Format("Post {0} {1} with your {2}",
                            currentGoal.stepsNeeded, timesText, currentGoal.goalObject);
                        break;
                }
                goalParent.transform.Find("GoalText").GetComponent<TextMeshPro>().text =
                    String.Format("{0}) {1}", i + 1, textDescription);

                if (currentGoal.rewardType == GoalRewardType.ExperiencePoints)
                {
                    goalParent.transform.Find("RewardText").GetComponent<TextMeshPro>().text =
                        String.Format("Reward: {0} exp", currentGoal.reward);
                }

                goalParent.transform.Find("ProgressText").GetComponent<TextMeshPro>().text = currentGoal.stepsCompleted.ToString();
                goalParent.transform.Find("FinishText").GetComponent<TextMeshPro>().text = "/" + currentGoal.stepsNeeded.ToString();
            } else {
                goalParent.transform.Find("Background").gameObject.SetActive(false);
                goalParent.transform.Find("NextGoalText").gameObject.SetActive(true);

                // Get next goal wait time
                // Show seconds at 59 mins and put it on a timer so it shows the per-second countdown
                goalParent.transform.Find("NextGoalText").GetComponent<TextMeshPro>().text = "Next goal in 24h";
            }
        }
    }

    private void GenerateProfilePosts()
    {
        var posts = this._userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        this._postHelper.GeneratePostFeed(
            this.scrollArea, posts, this._youPostObjects, POST_X_OFFSET, POST_Y_OFFSET);

        if (this._firstPostNew)
        {
            this.scrollController.ScrollToPosition(1.8f, this.CheckGoalProgress);

            var postAnimation = GameObject.Instantiate(Resources.Load("Posts/NewPostAnimation") as GameObject);
            var animationPosition = this._youPostObjects[0].postObject.transform.position;
            animationPosition.z += 1;
            postAnimation.transform.position = animationPosition;
            postAnimation.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            postAnimation.transform.parent = this.scrollArea.transform;

            this._firstPostNew = false;
        }
    }

    private void CheckGoalProgress()
    {
        this._goalsController.CheckGoalProgress(this._latestPost);
        this._goalsController.UpdateGoals();

        var goalSection = scrollArea.transform.Find("GoalSection").gameObject;
        this.SetupGoalSection(goalSection);
    }

    private void EnlargePost(DelayGramPostObject post)
    {
        if (this._imageCurrentlyShrinking)
        {
            return;
        }

        this._currentState = ProfileScreenState.SinglePicture;
        this._currentSelectedImage = post;
        this._originalImageScale = post.postObject.transform.localScale;
        this._originalImagePosition = post.postObject.transform.localPosition;

        this._postHelper.EnlargeAndCenterPost(post);

        post.postObject.transform.parent = null;
        this.page.SetActive(false);
    }

    private void ShrinkPost(DelayGramPostObject post)
    {
        if (!post.postObject)
        {
            return;
        }

        this._currentState = ProfileScreenState.ProfileDefault;
        this._imageCurrentlyShrinking = true;

        // Scale post down and position where it used to be
        this._postHelper.ShrinkAndReturnPost(
            post,
            this._originalImageScale,
            this._originalImagePosition,
            () => this.PostFinishedShrinking(post, false));

        this.page.SetActive(true);
        post.postObject.transform.parent = this.scrollArea.transform;
    }

    private void PostFinishedShrinking(DelayGramPostObject postObject, bool showDetails)
    {
        this._imageCurrentlyShrinking = false;
        this._postHelper.SetPostDetails(postObject.postObject, postObject.post, false, true);
    }
}
