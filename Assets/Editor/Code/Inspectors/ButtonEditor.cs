using UnityEditor;
using UnityEngine;
using System.Reflection;
using MET.Core.Attributes;

namespace MET
{
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var targetType = target.GetType();
            var methods = targetType.GetMethods(
                BindingFlags.Instance |
                BindingFlags.Static |
                BindingFlags.Public |
                BindingFlags.NonPublic
            );

            foreach (var method in methods)
            {
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                if (attr == null) continue;

                if (method.GetParameters().Length > 0) continue;

                string label = string.IsNullOrEmpty(attr.Label)
                    ? method.Name
                    : attr.Label;

                if (GUILayout.Button(label))
                {
                    method.Invoke(target, null);
                }
            }
        }
    }


}
