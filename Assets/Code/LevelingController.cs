using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class LevelingController : MonoBehaviour {
    private UserSerializer _userSerializer;

    private UIController _uiController;

    private int _previousLevel = 0;
    private int _previousLevelExperience = 0;
    private int _previousNeededExperience = 0;

    private bool _experienceAnimating = false;

    private void Awake()
    {
        this._userSerializer = UserSerializer.Instance;
    }

    private void Start ()
    {
        this._uiController = GetComponent<UIController>();
        this.UpdatePreviousInformation();
    }

    private void Update () {
	}

    public void AddExperience(int experience)
    {
        var currentExperience = this._userSerializer.LevelExperience;
        this._userSerializer.LevelExperience = currentExperience + experience;
    }

    public void UpdateLevelDisplay(GameObject parent)
    {
        if (this._previousLevel == 0)
        {
            this.UpdatePreviousInformation();
        }

        if (this._previousLevel < this._userSerializer.PlayerLevel)
        {
            this._uiController.CreateLevelUpPopup();
        }

        var levelBanner = parent.transform.Find("LevelBanner");

        var levelNumberText = levelBanner.transform.Find("LevelNumberText");
        levelNumberText.GetComponent<TextMeshPro>().text = this._previousLevel.ToString();

        var levelExperienceText = parent.transform.Find("LevelExperienceText");
        var previousExperienceText = this._previousLevelExperience.ToString() + "/" + this._previousNeededExperience.ToString();
        levelExperienceText.GetComponent<TextMeshPro>().text = previousExperienceText;

        var experienceBarFront = parent.transform.Find("LevelExperienceBar");
        var experienceBarBack = experienceBarFront.transform.Find("ExperienceBar");
        
        var oldExperiencePercentage = (float)this._previousLevelExperience / (float)this._previousNeededExperience;
        experienceBarBack.localScale = new Vector3(oldExperiencePercentage, 1.0f, 1.0f);

        var currentLevelExperience = this._userSerializer.LevelExperience;
        var newExperiencePercentage = Mathf.Min((float)currentLevelExperience / (float)this._previousNeededExperience, 1.0f);

        if (oldExperiencePercentage != newExperiencePercentage)
        {
            this.AnimateExperienceBar(experienceBarBack, newExperiencePercentage, parent);
        }

        this.UpdatePreviousInformation();
        this.RolloverLevelExperience();
    }

    private void RolloverLevelExperience()
    {
        var currentLevelExperience = this._userSerializer.LevelExperience;
        var neededExperience = this._userSerializer.NeededLevelExperience;

        if (currentLevelExperience >= neededExperience)
        {
            var level = this._userSerializer.PlayerLevel;

            this._userSerializer.PlayerLevel = level + 1;
            this._userSerializer.LevelExperience = currentLevelExperience - neededExperience;
            this._userSerializer.NeededLevelExperience = neededExperience + 20;
        }
    }

    private void AnimateExperienceBar(
        Transform experienceBarBack,
        float newExperiencePercentage,
        GameObject parent)
    {
        if (!this._experienceAnimating)
        {
            this._experienceAnimating = true;
            experienceBarBack.transform
                .DOScaleX(newExperiencePercentage, 0.8f)
                .SetEase(Ease.OutSine)
                .OnComplete(() =>
                {
                    this._experienceAnimating = false;
                    this.UpdateLevelDisplay(parent);
                });
        }
    }

    private void UpdatePreviousInformation()
    {
        this._previousLevel = this._userSerializer.PlayerLevel;
        this._previousLevelExperience = this._userSerializer.LevelExperience;
        this._previousNeededExperience = this._userSerializer.NeededLevelExperience;
    }
}
