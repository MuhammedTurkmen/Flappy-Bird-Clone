using UnityEngine;

namespace MET.Core.Utilities.Extentions
{
    public static class ArrayExtensions
    {
        public static T RandomElement<T>(this T[] array)
        {
            if (array == null || array.Length == 0)
            {
                Debug.LogError("Array null ya da boþ.");
                return default;
            }

            return array[Random.Range(0, array.Length)];
        }
    }
}