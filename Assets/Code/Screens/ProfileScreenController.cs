using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ProfileScreenController : MonoBehaviour
{
    [SerializeField]
    private GameObject _chooseNameBox;
    private TextMeshProUGUI _chooseNameText;

    private const float POST_X_OFFSET = -1.06f;
    private const float POST_Y_OFFSET = -5.45f;
    private const float NEW_POST_SCROLL_POSITION = -4.1f;

    private CharacterSerializer characterSerializer;
    private UserSerializer _userSerializer;
    private UIController _uiController;
    private GoalsController _goalsController;
    private LevelingController _levelingController;

    private ScrollController scrollController;
    private RandomNameGenerator randomNameGenerator;
    private PostHelper _postHelper;

    private GameObject page;
    private GameObject scrollArea;
    private GameObject _spriteMask;
    private List<DelayGramPostObject> _youPostObjects;
    private bool _firstPostNew;
    private DelayGramPost _latestPost;

    private GameObject _editScreen;
    private CharacterProperties _previousCharacterProperties;

    private GoalInformation[] _previousGoals;
    private float _tickGoalTimer = 0.0f;

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
        this._uiController = GetComponent<UIController>();
        this._goalsController = this.GetComponent<GoalsController>();
        this._levelingController = this.GetComponent<LevelingController>();

        this.randomNameGenerator = new RandomNameGenerator();
        this._postHelper = new PostHelper();

        this._youPostObjects = new List<DelayGramPostObject>();
        this._firstPostNew = false;
        this._previousGoals = new GoalInformation[] { null, null, null, null, null };

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
            this._levelingController.AddExperience(10);
            this.UpdateLevelDisplay();
        }

        if (this._tickGoalTimer > 0.0f)
        {
            this._tickGoalTimer -= Time.deltaTime;
            if (this._tickGoalTimer <= 0.0f)
            {
                if (this._goalsController.ChangeInWaitingGoals())
                {
                    this.SetupGoalSection();
                }
                else
                {
                    this.UpdateGoalTime();
                }
            }
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
                    this._editScreen.GetComponent<CharacterEditor>().FinalizeCharacter();
                    this.DestroyEditScreen();
                    if (!this._userSerializer.CompletedTutorial)
                    {
                        this._userSerializer.CompletedTutorial = true;
                    }
                    break;
                default:
                    this._editScreen.GetComponent<CharacterEditor>().CheckClick(colliderName);
                    break;
            }
        } else {
            switch (colliderName)
            {
                case "ExpButton":
                    this._levelingController.AddExperience(20);
                    this.UpdateLevelDisplay();
                    break;
                case "EditButton":
                    this.CreateEditAvatarScreen(true);
                    break;
                case "RewardButton1":
                    this.CollectGoalReward(0);
                    break;
                case "RewardButton2":
                    this.CollectGoalReward(1);
                    break;
                case "RewardButton3":
                    this.CollectGoalReward(2);
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

        this._currentState = ProfileScreenState.ProfileDefault;

        this.scrollArea = page.transform.Find("ScrollArea").gameObject;
        this.scrollController = scrollArea.GetComponent<ScrollController>();

        var characterSection = scrollArea.transform.Find("CharacterSection").gameObject;
        this._spriteMask = characterSection.transform.Find("SpriteMask").gameObject;
        this.SetAvatar(this._spriteMask);
        this.SetupItems(this._spriteMask);

        this.SetupGoalSection();

        var statsSection = scrollArea.transform.Find("StatsSection").gameObject;
        this.SetupStatistics(statsSection);

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
        var levelInformation = characterSection.transform.Find("LevelInformation");

        this._levelingController.UpdateLevelDisplay(levelInformation.gameObject);
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

    private void SetupGoalSection()
    {
        if (!scrollArea)
        {
            return;
        }
        var goalSection = scrollArea.transform.Find("GoalSection").gameObject;
        var currentGoals = this._goalsController.GetCurrentGoals();

        for(int i=0; i<currentGoals.Length; i++)
        {
            var goalParent = goalSection.transform.Find("Goal" + i.ToString()).gameObject;
            if (!goalParent)
            {
                Debug.Log("Error finding goal section " + i.ToString());
                continue;
            }

            foreach (Transform child in goalParent.transform)
            {
                child.gameObject.SetActive(false);
            }

            var currentGoal = currentGoals[i];
            if (currentGoal.status == GoalStatus.Active)
            {
                if (currentGoal.stepsCompleted >= currentGoal.stepsNeeded)  // If goal is finished
                {
                    if (this._previousGoals[i] != null && (this._previousGoals[i].stepsCompleted < this._previousGoals[i].stepsNeeded))
                    {   // Then the player has just achieved this goal, so show the 'accept reward' button
                        goalParent.transform.Find("Background").gameObject.SetActive(true);
                        goalParent.transform.Find("GoalText").gameObject.SetActive(true);
                        goalParent.transform.Find("RewardText").gameObject.SetActive(true);
                        goalParent.transform.Find("FinishText").gameObject.SetActive(true);

                        var progressText = goalParent.transform.Find("ProgressText");
                        progressText.gameObject.SetActive(true);
                        var originalScale = progressText.localScale.x;
                        var newScale = originalScale * 1.5f;

                        progressText.DOPunchScale(new Vector3(newScale, newScale, 1.0f), 0.7f);
                        goalParent.transform.Find("FinishText").DOPunchScale(new Vector3(newScale, newScale, 1.0f), 0.7f);
                    }
                    else
                    {
                        // Display accept reward button
                        goalParent.transform.Find("Background").gameObject.SetActive(true);
                        var rewardButtonName = String.Format("RewardButton{0}", i + 1);
                        goalParent.transform.Find(rewardButtonName).gameObject.SetActive(true);
                    }
                } else {    // Goal is in progress
                    goalParent.transform.Find("Background").gameObject.SetActive(true);
                    goalParent.transform.Find("GoalText").gameObject.SetActive(true);
                    goalParent.transform.Find("RewardText").gameObject.SetActive(true);
                    goalParent.transform.Find("FinishText").gameObject.SetActive(true);
                    goalParent.transform.Find("ProgressText").gameObject.SetActive(true);

                    var timesText = (currentGoal.stepsNeeded == 1) ? "time" : "times";
                    string textDescription = "";
                    switch (currentGoal.goalType)
                    {
                        case GoalObjectType.Location:
                            textDescription = String.Format("Post {0} {1} at the {2}",
                                currentGoal.stepsNeeded, timesText, currentGoal.goalObject);
                            break;
                        case GoalObjectType.Item:
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
                }
            } else if (currentGoal.status == GoalStatus.Waiting) { // There's a countdown, so hide the background and show time remaining
                goalParent.transform.Find("NextGoalText").gameObject.SetActive(true);

                this._tickGoalTimer = 0.1f;
            } else {    // If status is 'Inactive', show nothing. Used for beginning of the game
            }
        }

        this._previousGoals = currentGoals;
    }

    private void GenerateProfilePosts()
    {
        var posts = this._userSerializer.GetReverseChronologicalPosts();
        posts.Sort((a, b) => b.dateTime.CompareTo(a.dateTime));
        var feedLength = this._postHelper.GeneratePostFeed(
            this.scrollArea, posts, this._youPostObjects, POST_X_OFFSET, POST_Y_OFFSET);
        var scrollController = this.scrollArea.GetComponent<ScrollController>();
        var sizeBeforeFeed = POST_Y_OFFSET * -1;
        scrollController.UpdateScrollArea(sizeBeforeFeed + feedLength);

        if (this._firstPostNew)
        {
            this.scrollController.ScrollToPosition(NEW_POST_SCROLL_POSITION, this.CheckGoalProgress);

            var postAnimation = GameObject.Instantiate(Resources.Load("Posts/NewPostAnimation") as GameObject);
            var animationPosition = this._youPostObjects[0].postObject.transform.position;
            animationPosition.z += 1;
            postAnimation.transform.position = animationPosition;
            postAnimation.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
            postAnimation.transform.parent = this.scrollArea.transform;

            this._firstPostNew = false;
        }
    }

    private void CollectGoalReward(int index)
    {
        var currentGoals = this._goalsController.GetCurrentGoals();
        if (currentGoals[index].rewardType == GoalRewardType.ExperiencePoints)
        {
            try
            {
                var experienceGained = Convert.ToInt32(currentGoals[index].reward);
                this._levelingController.AddExperience(experienceGained);
                this.UpdateLevelDisplay();
            }
            catch (Exception exception)
            {
                Debug.Log(exception);
            }
        }

        this._goalsController.FinishGoal(index);

        this.SetupGoalSection();

        // Animation of experience points going towards level (or eventually popup for new item)
        // And then trigger animation of experience points being added (in meantime just do it immediately)
    }

    private void CheckGoalProgress()
    {
        this._goalsController.CheckGoalProgress(this._latestPost);

        this.SetupGoalSection();
    }

    private void UpdateGoalTime()
    {
        if (scrollArea)
        {
            var goalSection = scrollArea.transform.Find("GoalSection").gameObject;
            for (int i = 0; i < this._previousGoals.Length; i++)
            {
                if (this._previousGoals[i].status == GoalStatus.Waiting)
                {
                    var timeRemaining = this._previousGoals[i].nextGoalTime - DateTime.Now;
                    var formattedTime = timeRemaining.ToString(@"mm\:ss");

                    var goalParent = goalSection.transform.Find("Goal" + i.ToString()).gameObject;
                    if (!goalParent)
                    {
                        Debug.Log("Error finding goal section " + i.ToString());
                        continue;
                    }

                    goalParent.transform.Find("NextGoalText").GetComponent<TextMeshPro>().text =
                        String.Format("Next goal in {0}", formattedTime);

                    this._tickGoalTimer = 1.0f;
                }
            }
        }
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
