using System;
using UniRx;
using System.Collections.Generic;

public class Level
{
    public ReactiveProperty<int> level = new ReactiveProperty<int>();

    protected List<Action<int>> statCalculations;

    public Level(Action<int> statsCalculation)
    {
        statCalculations = new List<Action<int>>(){
            statsCalculation
        };

        SetLevel(0);
    }

    public void AddCalculation(params Action<int>[] statCalculations)
    {
        this.statCalculations.AddRange(statCalculations);
    }

    public void Up()
    {
        SetLevel(level.Value + 1);
    }

    void SetLevel(int level)
    {
        this.level.Value = level;

        foreach (var item in statCalculations)
        {
            item.Invoke(level);
        }
    }
}
