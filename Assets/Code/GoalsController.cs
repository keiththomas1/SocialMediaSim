using System;
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
        firstGoal.goalObject = "Beach";
        firstGoal.rewardType = GoalRewardType.ExperiencePoints;
        firstGoal.reward = "50";
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
        secondGoal.goalType = GoalObjectType.Location;
        secondGoal.goalObject = "City";
        secondGoal.rewardType = GoalRewardType.ExperiencePoints;
        secondGoal.reward = "50";
        secondGoal.stepsCompleted = 0;
        secondGoal.stepsNeeded = 1;

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

    public void CheckGoalProgress(DelayGramPost post)
    {
        var location = post.backgroundName;
        var items = new List<string>();
        foreach (var item in post.items)
        {
            items.Add(item.name);
        }

        // Go through each goal, see if it matches up with

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
                case GoalObjectType.Item:
                    foreach (var item in items)
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
        var newGoal = this._possibleGoals[UnityEngine.Random.Range(0, this._possibleGoals.Count)];
        return newGoal;
    }

    // Need to call this everytime a user unlocks something new
    private void CreatePossibleGoals()
    {
        // TODO: Populate these lists based on unlocked locations and items
        string[] locations = { "Beach", "City", "Park" };
        string[] items = { "Bulldog", "Sylvester", "D-Rone" };

        foreach (var location in locations)
        {
            for (int i = 1; i <= 3; i++)
            {
                var newGoal = CreateNewGoalInformation(GoalObjectType.Location, location, i);

                this._possibleGoals.Add(newGoal);
            }
        }

        foreach(var item in items)
        {
            for (int i = 1; i <= 3; i++)
            {
                var newGoal = CreateNewGoalInformation(GoalObjectType.Item, item, i);

                this._possibleGoals.Add(newGoal);
            }
        }
    }

    private GoalInformation CreateNewGoalInformation(GoalObjectType goalType, string goalObject, int steps)
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
