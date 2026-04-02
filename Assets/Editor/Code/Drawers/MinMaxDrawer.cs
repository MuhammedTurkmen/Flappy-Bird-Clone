using MET.Core.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace MET
{
    [CustomPropertyDrawer(typeof(MinMaxAttribute))]
    public class MinMaxDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var attr = (MinMaxAttribute)attribute;

            var root = new VisualElement();
            root.style.flexDirection = FlexDirection.Column;

            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                root.Add(new HelpBox(
                    $"{nameof(MinMaxAttribute)} sadece Vector2 alanlarında kullanılmalı.",
                    HelpBoxMessageType.Error));
                return root;
            }

            var minProp = property.FindPropertyRelative("x");
            var maxProp = property.FindPropertyRelative("y");

            if (minProp == null || maxProp == null)
            {
                root.Add(new HelpBox("x / y bulunamadı.", HelpBoxMessageType.Error));
                return root;
            }

            float rangeMin = attr.Min;
            float rangeMax = attr.Max;

            float ApplyStep(float v)
            {
                if (!attr.HasStep) return v;
                return Mathf.Round(v / attr.Step) * attr.Step;
            }

            var label = new Label(property.displayName);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            label.style.marginBottom = 2;
            root.Add(label);

            var slider = new MinMaxSlider(
                minProp.floatValue,
                maxProp.floatValue,
                rangeMin,
                rangeMax
            );
            slider.style.marginBottom = 4;

            var row = new VisualElement();
            row.style.flexDirection = FlexDirection.Row;
            row.style.justifyContent = Justify.SpaceBetween;

            var minField = new FloatField();
            var maxField = new FloatField();

            minField.style.flexGrow = 1;
            maxField.style.flexGrow = 1;
            minField.style.marginRight = 6;

            minField.labelElement.style.display = DisplayStyle.None;
            maxField.labelElement.style.display = DisplayStyle.None;

            minField.style.minWidth = 0;
            maxField.style.minWidth = 0;

            void RefreshUI()
            {
                property.serializedObject.Update();

                float min = minProp.floatValue;
                float max = maxProp.floatValue;

                slider.SetValueWithoutNotify(new Vector2(min, max));
                minField.SetValueWithoutNotify(min);
                maxField.SetValueWithoutNotify(max);
            }

            void ApplyValue(float min, float max)
            {
                min = ApplyStep(min);
                max = ApplyStep(max);

                min = Mathf.Clamp(min, rangeMin, rangeMax);
                max = Mathf.Clamp(max, rangeMin, rangeMax);

                if (min > max)
                    max = min;

                property.serializedObject.Update();

                minProp.floatValue = min;
                maxProp.floatValue = max;
                property.serializedObject.ApplyModifiedProperties();

                slider.SetValueWithoutNotify(new Vector2(min, max));
                minField.SetValueWithoutNotify(min);
                maxField.SetValueWithoutNotify(max);
            }

            slider.RegisterValueChangedCallback(e =>
            {
                ApplyValue(e.newValue.x, e.newValue.y);
            });

            minField.RegisterValueChangedCallback(e =>
            {
                property.serializedObject.Update();
                ApplyValue(e.newValue, maxProp.floatValue);
            });

            maxField.RegisterValueChangedCallback(e =>
            {
                property.serializedObject.Update();
                ApplyValue(minProp.floatValue, e.newValue);
            });

            root.TrackPropertyValue(property, _ => RefreshUI());


            RefreshUI();

            row.Add(minField);
            row.Add(maxField);

            root.Add(slider);
            root.Add(row);

            return root;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MinMaxAttribute)attribute;

            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.LabelField(position, label.text, "MinMaxAttribute sadece Vector2 ile çalışır.");
                return;
            }

            var minProp = property.FindPropertyRelative("x");
            var maxProp = property.FindPropertyRelative("y");

            if (minProp == null || maxProp == null)
            {
                EditorGUI.LabelField(position, label.text, "x / y bulunamadı.");
                return;
            }

            float rangeMin = attr.Min;
            float rangeMax = attr.Max;

            float ApplyStep(float v)
            {
                if (!attr.HasStep) return v;
                return Mathf.Round(v / attr.Step) * attr.Step;
            }

            EditorGUI.BeginProperty(position, label, property);

            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            Rect labelRect = new Rect(position.x, position.y, position.width, line);
            Rect sliderRect = new Rect(position.x, position.y + line + spacing, position.width, line);
            Rect fieldsRect = new Rect(position.x, position.y + (line + spacing) * 2, position.width, line);

            EditorGUI.LabelField(labelRect, label);

            float min = minProp.floatValue;
            float max = maxProp.floatValue;

            EditorGUI.BeginChangeCheck();
            EditorGUI.MinMaxSlider(sliderRect, ref min, ref max, rangeMin, rangeMax);
            if (EditorGUI.EndChangeCheck())
            {
                min = ApplyStep(min);
                max = ApplyStep(max);

                min = Mathf.Clamp(min, rangeMin, rangeMax);
                max = Mathf.Clamp(max, rangeMin, rangeMax);

                if (min > max)
                    max = min;

                minProp.floatValue = min;
                maxProp.floatValue = max;
            }

            float fieldWidth = (fieldsRect.width - 6f) * 0.5f;
            Rect minRect = new Rect(fieldsRect.x, fieldsRect.y, fieldWidth, line);
            Rect maxRect = new Rect(fieldsRect.x + fieldWidth + 6f, fieldsRect.y, fieldWidth, line);

            EditorGUI.BeginChangeCheck();
            float newMin = EditorGUI.DelayedFloatField(minRect, minProp.floatValue);
            float newMax = EditorGUI.DelayedFloatField(maxRect, maxProp.floatValue);
            if (EditorGUI.EndChangeCheck())
            {
                newMin = ApplyStep(newMin);
                newMax = ApplyStep(newMax);

                newMin = Mathf.Clamp(newMin, rangeMin, rangeMax);
                newMax = Mathf.Clamp(newMax, rangeMin, rangeMax);

                if (newMin > newMax)
                    newMax = newMin;

                minProp.floatValue = newMin;
                maxProp.floatValue = newMax;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            return line * 3 + spacing * 2;
        }
    }
}