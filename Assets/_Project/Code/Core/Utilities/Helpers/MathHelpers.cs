using UnityEngine;

namespace MET.Core.Utilities.Helpers
{
    public static class MathHelpers 
    {
        public static float Remap(float value, float inMin, float inMax, float outMin, float outMax)
        {
            if (Mathf.Approximately(inMin, inMax))
                return outMin;

            float t = Mathf.InverseLerp(inMin, inMax, value);
            return Mathf.Lerp(outMin, outMax, t);
        }
    }
}