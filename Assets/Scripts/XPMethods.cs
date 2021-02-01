using UnityEngine;

public static class XPMethods
{
    // xp = ((level + 1)^scale)*base
    public static int CalculateByPowScale(int _currentLevel, float _scale, int _baseXP)
    {
        return Mathf.RoundToInt(Mathf.Pow((_currentLevel + 1), _scale) * _baseXP);
    }

    //xp = (level / constant) ^ 2
    public static int CalculateByConstant(int _currentLevel, float _constant)
    {
        return Mathf.RoundToInt(Mathf.Pow((_currentLevel / _constant), 2));
    }

}
