using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LevelingController : MonoBehaviour {
    private UserSerializer _userSerializer;
    private CharacterSerializer _characterSerializer;

    private UIController _uiController;

    private int _previousLevel = 0;
    private int _previousNeededExperience = 0;

    private Tweener _currentTweener = null;
    private bool _currentlyTweening = false;

    private void Awake()
    {
        this._userSerializer = UserSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
    }

    private void Start ()
    {
        this._uiController = GetComponent<UIController>();
        this.UpdatePreviousInformation();
    }

    private void Update () {
	}

    public void SetupLevelDisplay(GameObject levelBarParent)
    {
        var experienceBarFront = levelBarParent.transform.Find("LevelExperienceBar");
        var experienceBarBack = experienceBarFront.transform.Find("ExperienceBar");

        var levelExperience = this._userSerializer.LevelExperience;
        var neededExperience = this._userSerializer.NeededLevelExperience;
        var experiencePercentage = (float)levelExperience / (float)neededExperience;
        experienceBarBack.localScale = new Vector3(experiencePercentage, 1.0f, 1.0f);
    }

    public void AddExperience(int experience, GameObject levelBarParent)
    {
        var currentExperience = this._userSerializer.LevelExperience;
        this._userSerializer.LevelExperience = currentExperience + experience;

        if (this._currentlyTweening)
        {
            this._currentTweener.Kill();
        }

        this.UpdateLevelDisplay(levelBarParent);
    }

    public void UpdateLevelDisplay(GameObject levelBarParent)
    {
        if (this._previousLevel == 0)
        {
            this.UpdatePreviousInformation();
        }

        if (this._previousLevel < this._characterSerializer.AvatarLevel)
        {
            this._uiController.CreateLevelUpPopup();
        }

        var levelBanner = levelBarParent.transform.Find("LevelBanner");

        var levelNumberText = levelBanner.transform.Find("LevelNumberText");
        levelNumberText.GetComponent<TextMeshPro>().text = this._previousLevel.ToString();

        var levelExperienceText = levelBarParent.transform.Find("LevelExperienceText");
        // var previousExperienceText = this._previousLevelExperience.ToString() + "/" + this._previousNeededExperience.ToString();
        // levelExperienceText.GetComponent<TextMeshPro>().text = previousExperienceText;

        var experienceBarFront = levelBarParent.transform.Find("LevelExperienceBar");
        var experienceBarBack = experienceBarFront.transform.Find("ExperienceBar");
        

        var currentLevelExperience = this._userSerializer.LevelExperience;
        var newExperiencePercentage = Mathf.Min((float)currentLevelExperience / (float)this._previousNeededExperience, 1.0f);

        // if (oldExperiencePercentage != newExperiencePercentage)
        // {
        //    this.AnimateExperienceBar(experienceBarBack, newExperiencePercentage, levelBarParent);
        // }

        this.UpdatePreviousInformation();
        this.RolloverLevelExperience();
    }

    private void RolloverLevelExperience()
    {
        var currentLevelExperience = this._userSerializer.LevelExperience;
        var neededExperience = this._userSerializer.NeededLevelExperience;

        if (currentLevelExperience >= neededExperience)
        {
            this._characterSerializer.AvatarLevel += 1;
            this._userSerializer.LevelExperience = currentLevelExperience - neededExperience;
            this._userSerializer.NeededLevelExperience = neededExperience + 20;
        }
    }

    private void AnimateExperienceBar(
        Transform experienceBarBack,
        float newExperiencePercentage,
        GameObject parent)
    {
        if (!this._currentlyTweening)
        {
            this._currentlyTweening = true;
            experienceBarBack.transform
                .DOScaleX(newExperiencePercentage, 0.8f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    this._currentlyTweening = false;
                    this.UpdateLevelDisplay(parent);
                });
        }
    }

    private void UpdatePreviousInformation()
    {
        this._previousLevel = this._characterSerializer.AvatarLevel;
        this._previousNeededExperience = this._userSerializer.NeededLevelExperience;
    }
}
