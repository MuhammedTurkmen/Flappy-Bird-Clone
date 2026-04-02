using System;
using UnityEngine;

namespace MET.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;
        public readonly float Step;
        public readonly bool HasStep;

        public MinMaxAttribute(float min, float max)
        {
            Min = min;
            Max = max;
            HasStep = false;
        }

        public MinMaxAttribute(float min, float max, float step)
        {
            Min = min;
            Max = max;
            Step = Mathf.Max(0.0001f, step);
            HasStep = true;
        }
    }
}
