using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GoalType
{
    Location,
    Item
}

public enum GoalRewardType
{
    ExperiencePoints,
    Item
}

public class GoalsController : MonoBehaviour {
    private GoalSerializer _goalSerializer;

    private List<GoalInformation> _possibleGoals;

    private void Awake()
    {
        this._goalSerializer = GoalSerializer.Instance;
        this._possibleGoals = new List<GoalInformation>();
        this.CreatePossibleGoals();
    }

    // Use this for initialization
    private void Start ()
    {
    }

    // Update is called once per frame
    private void Update () {
	}

    public void FinishGoal(int index)
    {
        var currentGoals = this.GetCurrentGoals();
        if (currentGoals[index] != null)
        {
            currentGoals[index] = null;
        }
        this._goalSerializer.CurrentGoals = currentGoals;
    }

    public void UpdateGoals()
    {
        // Check if the time is up to get new goals
        // Get new goal that is different than current goals
        // Check if any current goals are finished
        // If yes, then finish them up

        // Currently fills up any empty goals
        var currentGoals = this.GetCurrentGoals();
        for (int i=0; i<currentGoals.Length; i++)
        {
            if (currentGoals[i] == null)
            {
                var newGoal = this.GetNewGoal();
                currentGoals[i] = newGoal;
            }
        }
    }

    public GoalInformation[] GetCurrentGoals()
    {
        return this._goalSerializer.CurrentGoals;
    }

    public GoalInformation GetNewGoal()
    {
        var newGoal = this._possibleGoals[UnityEngine.Random.Range(0, this._possibleGoals.Count)];
        return newGoal;
    }

    public void CheckGoalProgress(DelayGramPost post)
    {
        var location = post.backgroundName;
        var items = new List<string>();
        foreach(var item in post.items)
        {
            items.Add(item.name);
        }

        // Go through each goal, see if it matches up with

        var currentGoals = this.GetCurrentGoals();
        for (int i = 0; i < currentGoals.Length; i++)
        {
            bool goalProgressed = false;
            switch (currentGoals[i].goalType) {
                case GoalType.Location:
                    if (currentGoals[i].goalObject == location)
                    {
                        goalProgressed = true;
                    }
                    break;
                case GoalType.Item:
                    foreach(var item in items)
                    {
                        if (currentGoals[i].goalObject == item)
                        {
                            goalProgressed = true;
                        }
                    }
                    break;
            }

            if (goalProgressed)
            {
                var stepsCompleted = currentGoals[i].stepsCompleted;
                stepsCompleted++;
                if (stepsCompleted == currentGoals[i].stepsNeeded)
                {
                    currentGoals[i] = null; // Goal completed
                } else {
                    currentGoals[i].stepsCompleted = stepsCompleted;
                }
            }
        }

        this._goalSerializer.CurrentGoals = currentGoals;
    }

    // Need to call this everytime a user unlocks something new
    public void CreatePossibleGoals()
    {
        // TODO: Populate these lists based on unlocked locations and items
        string[] locations = { "Beach", "City", "Park" };
        string[] items = { "Dog", "Cat", "Drone" };

        foreach (var location in locations)
        {
            for (int i = 1; i <= 3; i++)
            {
                var newGoal = CreateNewGoalInformation(GoalType.Location, location, i);

                this._possibleGoals.Add(newGoal);
            }
        }

        foreach(var item in items)
        {
            for (int i = 1; i <= 3; i++)
            {
                var newGoal = CreateNewGoalInformation(GoalType.Item, item, i);

                this._possibleGoals.Add(newGoal);
            }
        }
    }

    private GoalInformation CreateNewGoalInformation(GoalType goalType, string goalObject, int steps)
    {
        var newGoal = new GoalInformation();
        newGoal.goalType = goalType;
        newGoal.goalObject = goalObject;
        newGoal.rewardType = GoalRewardType.ExperiencePoints;
        newGoal.reward = (10 * steps).ToString();
        newGoal.stepsCompleted = 0;
        newGoal.stepsNeeded = steps;
        return newGoal;
    }
}
