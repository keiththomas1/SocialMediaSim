﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalStatus
{
    Active,
    Waiting,
    Inactive
}

public enum GoalObjectType
{
    Location,
    Pet,
    Ratings,
    Scrolling
}

public enum GoalRewardType
{
    ExperiencePoints,
    Item
}

public class GoalsController : MonoBehaviour {
    private GoalSerializer _goalSerializer;
    private UserSerializer _userSerializer;

    private List<GoalInformation> _possibleGoals = new List<GoalInformation>();

    private void Awake()
    {
        this._goalSerializer = GoalSerializer.Instance;
        this._userSerializer = UserSerializer.Instance;
    }

    public GoalInformation[] GetCurrentGoals()
    {
        return this._goalSerializer.CurrentGoals;
    }

    public void SetFirstGoal()  // First goal is not random to help flow of game
    {
        var currentGoals = this.GetCurrentGoals();

        var firstGoal = new GoalInformation();
        firstGoal.status = GoalStatus.Active;
        firstGoal.goalType = GoalObjectType.Location;
        firstGoal.goalObject = "Apartment";
        firstGoal.rewardType = GoalRewardType.ExperiencePoints;
        firstGoal.reward = "40";
        firstGoal.stepsCompleted = 0;
        firstGoal.stepsNeeded = 1;

        currentGoals[0] = firstGoal;
        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public void SetSecondGoal()
    {
        var currentGoals = this.GetCurrentGoals();

        var secondGoal = new GoalInformation();
        secondGoal.status = GoalStatus.Active;
        secondGoal.goalType = GoalObjectType.Ratings;
        secondGoal.goalObject = "";
        secondGoal.rewardType = GoalRewardType.ExperiencePoints;
        secondGoal.reward = "50";
        secondGoal.stepsCompleted = 0;
        secondGoal.stepsNeeded = 10;

        currentGoals[1] = secondGoal;
        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public void FinishGoal(int index)
    {
        var currentGoals = this.GetCurrentGoals();
        var newWaitingGoal = new GoalInformation(GoalStatus.Waiting);
        newWaitingGoal.nextGoalTime = DateTime.Now.AddMinutes(5.0f);
        currentGoals[index] = newWaitingGoal;

        if (currentGoals[1].status == GoalStatus.Inactive && currentGoals[2].status == GoalStatus.Inactive)
        {
            if (index == 0)
            {   // Then user just completed their first goal
                this.SetSecondGoal();
                return;
            }
            else if (index == 1)
            {
                currentGoals[0] = this.GetNewGoal();
                currentGoals[2] = this.GetNewGoal();
            }
        }

        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public void AddRatingsGoalProgress(int ratings)
    {
        var currentGoals = this.GetCurrentGoals();
        for (int i = 0; i < currentGoals.Length; i++)
        {
            if (currentGoals[i].goalType == GoalObjectType.Ratings)
            {
                var stepsCompleted = currentGoals[i].stepsCompleted;
                stepsCompleted = Math.Min(
                    currentGoals[i].stepsNeeded,
                    stepsCompleted + ratings);
                currentGoals[i].stepsCompleted = stepsCompleted;
            }
        }

        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public void CheckGoalProgress(DelayGramPost post)
    {
        var items = new List<string>();
        foreach (var item in post.items)
        {
            items.Add(item.name);
        }

        var location = post.backgroundName;
        if (location == "ApartmentEmpty")
        {
            location = "Apartment";
        }
        var currentGoals = this.GetCurrentGoals();
        for (int i = 0; i < currentGoals.Length; i++)
        {
            bool goalProgressed = false;
            switch (currentGoals[i].goalType)
            {
                case GoalObjectType.Location:
                    if (currentGoals[i].goalObject == location)
                    {
                        goalProgressed = true;
                    }
                    break;
                case GoalObjectType.Pet:
                    foreach (var item in items)
                    {
                        if (currentGoals[i].goalObject == item)
                        {
                            goalProgressed = true;
                        }
                    }
                    break;
                case GoalObjectType.Ratings:
                    break;
                case GoalObjectType.Scrolling:
                    break;
            }

            if (goalProgressed)
            {
                var stepsCompleted = currentGoals[i].stepsCompleted;
                stepsCompleted++;
                currentGoals[i].stepsCompleted = stepsCompleted;
            }
        }

        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public bool ChangeInWaitingGoals()
    {
        bool changed = false;
        var currentGoals = this.GetCurrentGoals();
        for(int i=0; i<currentGoals.Length; i++)
        {
            var goal = currentGoals[i];
            if (goal.status == GoalStatus.Waiting && goal.nextGoalTime <= DateTime.Now)
            {
                var newGoal = this.GetNewGoal();
                currentGoals[i] = newGoal;
                changed = true;
            }
        }

        if (changed)
        {
            this._goalSerializer.CurrentGoals = currentGoals;
        }

        return changed;
    }

    // public string[] Get

    private GoalInformation GetNewGoal()
    {
        this.CreatePossibleGoals();
        var newGoal = this._possibleGoals[UnityEngine.Random.Range(0, this._possibleGoals.Count)];
        return newGoal;
    }

    // Need to call this everytime a user unlocks something new
    private void CreatePossibleGoals()
    {
        this._possibleGoals.Clear();

        List<string> locations = new List<string>() { "Apartment"};
        if (this._userSerializer.HasBeachBackground)
        {
            locations.Add("Beach");
        }
        if (this._userSerializer.HasCityBackground)
        {
            locations.Add("City");
        }
        if (this._userSerializer.HasParkBackground)
        {
            locations.Add("Park");
        }
        if (this._userSerializer.HasCamRoomBackground)
        {
            locations.Add("CamRoom");
        }
        if (this._userSerializer.HasLouvreBackground)
        {
            locations.Add("Louvre");
        }
        if (this._userSerializer.HasYachtBackground)
        {
            locations.Add("Yacht");
        }
        List<string> items = new List<string>();
        if (this._userSerializer.HasBulldog)
        {
            items.Add("Bulldog");
        }
        if (this._userSerializer.HasCat)
        {
            items.Add("Cat");
        }
        if (this._userSerializer.HasDrone)
        {
            items.Add("D-Rone");
        }

        foreach (var location in locations)
        {
            const int MaxPostsPerGoal = 1; // Only allowing goals with one post for now, to speed up gameplay
            for (int i = 1; i <= MaxPostsPerGoal; i++)
            {
                var newGoal = CreateNewGoalInformation(
                    GoalObjectType.Location, location, i, (ExperiencePerStep * i));

                this._possibleGoals.Add(newGoal);
            }
        }

        foreach(var item in items)
        {
            const int MaxPostsPerGoal = 1; // Only allowing goals with one post for now, to speed up gameplay
            for (int i = 1; i <= MaxPostsPerGoal; i++)
            {
                var newGoal = CreateNewGoalInformation(
                    GoalObjectType.Pet, item, i, (ExperiencePerStep * i));

                this._possibleGoals.Add(newGoal);
            }
        }

        var ratingsGoal1 = CreateNewGoalInformation(GoalObjectType.Ratings, null, 10, 10);
        this._possibleGoals.Add(ratingsGoal1);
        var ratingsGoal2 = CreateNewGoalInformation(GoalObjectType.Ratings, null, 15, 15);
        this._possibleGoals.Add(ratingsGoal2);
        var ratingsGoal3 = CreateNewGoalInformation(GoalObjectType.Ratings, null, 20, 20);
        this._possibleGoals.Add(ratingsGoal3);
    }

    private GoalInformation CreateNewGoalInformation(
        GoalObjectType goalType, string goalObject, int steps, int experienceReward)
    {
        var newGoal = new GoalInformation();
        newGoal.goalType = goalType;
        newGoal.goalObject = goalObject;
        newGoal.rewardType = GoalRewardType.ExperiencePoints;
        newGoal.reward = experienceReward.ToString();
        newGoal.stepsCompleted = 0;
        newGoal.stepsNeeded = steps;
        return newGoal;
    }

    private const int ExperiencePerStep = 20;
}
