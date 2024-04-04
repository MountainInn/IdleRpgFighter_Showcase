using System.Collections.Generic;
using UniRx;
using System.Linq;

public abstract class Stat : Talent
{
    public abstract int CurrentValue {get;}

    protected string GetFieldDescriptions(int level, params (string fieldName, List<Field> fieldValues)[] fields)
    {
        return
            fields
            .Select(tup => GetDescriptionOfField(tup.fieldValues, level, tup.fieldName))
            .JoinAsString("\n");

        string GetDescriptionOfField(List<Field> field, int level, string fieldName)
        {
            int currentValue = field[level];
            string nextValue;

            if (field.Count > level + 1)
                nextValue = $"{field[level+1]}";
            else
                nextValue = "MAX";

            return $"{fieldName} {currentValue} -> {currentValue}";
        }
    }
}
