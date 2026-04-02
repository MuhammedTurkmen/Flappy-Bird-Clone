using System;
using UnityEngine;

namespace MET.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public class SeparatorAttribute : PropertyAttribute
    {
        public readonly string Title;
        public readonly float Height;
        public readonly int Size;
        public readonly Color TextColor;
        public readonly Color LineColor;

        public SeparatorAttribute(
            float height = 12f,
            float r = 0.5f,
            float g = 0.5f,
            float b = 0.5f
        )
        {
            Title = null;
            Height = height;
            Size = 13;
            TextColor = Color.clear;
            LineColor = new Color(r, g, b);
        }

        public SeparatorAttribute(
            string title,
            float height = 20f,
            int size = 13,
            float r = 0.85f,
            float g = 0.85f,
            float b = 0.85f
        )
        {
            Title = title;
            Height = height;
            Size = size;
            TextColor = new Color(r, g, b);
            LineColor = TextColor;
        }

        public bool HasTitle => !string.IsNullOrEmpty(Title);
    }


}