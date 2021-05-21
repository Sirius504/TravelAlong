using System;

public static class Helper
{
    public static float SolveBisection(Func<float, float> function, float a, float b, bool betweenABGuaranteed = false, float precision = .0001f, int maxIterations = 100)
    {
        int tries = 0;
        float d = function(a);
        float c = function(b);
        if (function(a) * function(b) > 0 && !betweenABGuaranteed)
        {
            throw new ArgumentOutOfRangeException("a", "f(x) must have different signs at a and b.");
        }

        float result = 0f;
        while (++tries < maxIterations)
        {
            result = (a + b) / 2;
            if (Math.Abs(function(result)) < precision)
                break;

            if (function(a) * function(result) > 0)
                a = result;
            else
                b = result;
        }
        return result;
    }
}