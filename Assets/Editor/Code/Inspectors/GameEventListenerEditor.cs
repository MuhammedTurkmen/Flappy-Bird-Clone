using System;
using System.Linq;
using MET.Applications.Events;
using UnityEditor;
using UnityEngine;

namespace MET.EditorTool
{
    [CustomEditor(typeof(GameEventListener))]
    public class GameEventListenerEditor : Editor
    {
        private static Type[] _cachedTypes;
        private static string[] _cachedNames;

        [InitializeOnLoadMethod]
        private static void Init()
        {
            _cachedTypes = null;
            _cachedNames = null;
        }

        private static void CacheTypes()
        {
            if (_cachedTypes != null) return;

            _cachedTypes = TypeCache.GetTypesDerivedFrom<IGameEvent>()
                .Where(t => !t.IsInterface && !t.IsAbstract)
                .ToArray();

            _cachedNames = _cachedTypes.Select(t => t.Name).ToArray();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            CacheTypes();

            var listProp = serializedObject.FindProperty("list");

            if (listProp == null)
            {
                EditorGUILayout.HelpBox("list field not found!", MessageType.Error);
                return;
            }

            EditorGUILayout.Space();

            for (int i = 0; i < listProp.arraySize; i++)
            {
                var element = listProp.GetArrayElementAtIndex(i);

                var eventNameProp = element.FindPropertyRelative("_eventTypeName");
                var unityEventProp = element.FindPropertyRelative("Event");

                EditorGUILayout.BeginVertical("box");

                int index = Array.FindIndex(_cachedTypes, t => t.FullName == eventNameProp.stringValue);
                if (index < 0) index = 0;

                int newIndex = EditorGUILayout.Popup("Event Type", index, _cachedNames);
                eventNameProp.stringValue = _cachedTypes[newIndex].FullName;

                EditorGUILayout.PropertyField(unityEventProp);

                if (GUILayout.Button("Remove"))
                {
                    listProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            if (GUILayout.Button("Add Event"))
            {
                listProp.InsertArrayElementAtIndex(listProp.arraySize);
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}