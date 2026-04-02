using UnityEditor;
using UnityEngine;

namespace MET.Core.Attributes
{

    [CustomPropertyDrawer(typeof(SeparatorAttribute))]
    public class SeparatorDrawer : DecoratorDrawer
    {
        private const float _spacing = 4f;

        public override float GetHeight()
        {
            var sep = (SeparatorAttribute)attribute;
            return sep.Height + _spacing;
        }

        public override void OnGUI(Rect position)
        {
            var sep = (SeparatorAttribute)attribute;

            position.height -= _spacing;

            DrawSeparator(position, sep);
        }

        private void DrawSeparator(Rect position, SeparatorAttribute sep)
        {
            float centerY = position.y + position.height / 2f;

            if (!sep.HasTitle)
            {
                EditorGUI.DrawRect(
                    new Rect(position.x, centerY, position.width, 1f),
                    Color.gray
                );
                return;
            }

            float padding = 6f;

            GUIStyle style = new(EditorStyles.boldLabel)
            {
                fontSize = sep.Size,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = sep.TextColor }
            };

            Vector2 textSize = style.CalcSize(new GUIContent(sep.Title));
            float textX = position.x + (position.width - textSize.x) / 2f;

            EditorGUI.DrawRect(
                new Rect(position.x, centerY, textX - position.x - padding, 1f),
                Color.gray
            );

            EditorGUI.DrawRect(
                new Rect(
                    textX + textSize.x + padding,
                    centerY,
                    position.xMax - (textX + textSize.x + padding),
                    1f
                ),
                Color.gray
            );

            EditorGUI.LabelField(
                new Rect(textX, position.y, textSize.x, position.height),
                sep.Title,
                style
            );
        }
    }
}