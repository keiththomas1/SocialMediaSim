using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class LevelingController : MonoBehaviour {
    private UserSerializer _userSerializer;
    private CharacterSerializer _characterSerializer;

    private UIController _uiController;

    private Tweener _currentTweener = null;
    private bool _currentlyTweening = false;

    private Transform _levelBanner;
    private Transform _levelNumberText;
    private Transform _experienceBarFront;
    private Transform _experienceBarBack;
    private Transform _levelExperienceText;

    private void Awake()
    {
        this._userSerializer = UserSerializer.Instance;
        this._characterSerializer = CharacterSerializer.Instance;
        this._uiController = GameObject.Find("CONTROLLER").GetComponent<UIController>();

        this._levelBanner = this.transform.Find("LevelBanner");
        this._levelNumberText = this._levelBanner.transform.Find("LevelNumberText");
        this._experienceBarFront = this.transform.Find("LevelExperienceBar");
        this._experienceBarBack = this._experienceBarFront.transform.Find("ExperienceBar");
        this._levelExperienceText = this.transform.Find("LevelExperienceText");
    }

    private void Start ()
    {
        this.SetupLevelDisplay();
    }

    private void Update () {
	}

    public void LevelUp()
    {
        var levelExperience = this._userSerializer.LevelExperience;
        var neededExperience = 80 + (this._characterSerializer.AvatarLevel * 20);

        this._characterSerializer.AvatarLevel += 1;
        this._levelNumberText.GetComponent<TextMeshPro>().text = this._characterSerializer.AvatarLevel.ToString();
        var newLevelExperience = levelExperience - neededExperience;
        this._userSerializer.LevelExperience = newLevelExperience;

        var newNeededExperience = neededExperience + 20;
        var newExperiencePercentage = (float)newLevelExperience / (float)newNeededExperience;

        this.AnimateExperienceBar(newExperiencePercentage);
    }

    public void AddExperience(int experience)
    {
        var newLevelExperience = this._userSerializer.LevelExperience + experience;
        this._userSerializer.LevelExperience = newLevelExperience;
        var neededExperience = 80 + (this._characterSerializer.AvatarLevel * 20);
        float newExperiencePercentage = 0f;
        if (newLevelExperience >= neededExperience)
        {
            newExperiencePercentage = 1f;
        }
        else
        {
            newExperiencePercentage = (float)newLevelExperience / (float)neededExperience;
        }

        if (this._currentlyTweening)
        {
            this._currentTweener.Kill();
            this._currentlyTweening = false;
        }

        this.AnimateExperienceBar(newExperiencePercentage);
    }

    private void SetupLevelDisplay()
    {
        var levelExperience = this._userSerializer.LevelExperience;
        var neededExperience = 80 + (this._characterSerializer.AvatarLevel * 20);
        var experiencePercentage = (float)levelExperience / (float)neededExperience;
        this._experienceBarBack.localScale = new Vector3(experiencePercentage, 1.0f, 1.0f);

        this._levelNumberText.GetComponent<TextMeshPro>().text = this._characterSerializer.AvatarLevel.ToString();
        this._levelExperienceText.GetComponent<TextMeshPro>().text =
            String.Format("{0}/{1}", levelExperience, neededExperience);

        if (levelExperience >= neededExperience)
        {
            this._experienceBarBack.localScale = new Vector3(0f, 1.0f, 1.0f);
            this._uiController.CreateLevelUpPopup();
        }
    }

    private void AnimateExperienceBar(float newExperiencePercentage)
    {
        if (!this._currentlyTweening)
        {
            this._currentlyTweening = true;
            var neededExperience = 80 + (this._characterSerializer.AvatarLevel * 20);
            this._currentTweener = this._experienceBarBack.transform
                .DOScaleX(newExperiencePercentage, 0.8f)
                .SetEase(Ease.OutSine)
                .OnUpdate(() =>
                {
                    var currentScaleX = this._experienceBarBack.transform.localScale.x;
                    var levelExperience = Mathf.Round(currentScaleX * neededExperience);
                    this._levelExperienceText.GetComponent<TextMeshPro>().text =
                        String.Format("{0}/{1}", levelExperience, neededExperience);
                })
                .OnComplete(() =>
                {
                    this._currentlyTweening = false;
                    if (newExperiencePercentage >= 1f)
                    {
                        this._experienceBarBack.localScale = new Vector3(0f, 1.0f, 1.0f);
                        this._uiController.CreateLevelUpPopup();
                    }
                });
        }
    }
}
