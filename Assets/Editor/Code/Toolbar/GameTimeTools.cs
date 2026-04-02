using MET.EditorTool;
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

[InitializeOnLoad]
public class MainToolbarTimeScaleDropdown
{
    const string kElementPath = "Runtime/Time Scale";
    private static readonly float[] timeScaleValues = { 0.25f, 0.5f, 0.75f, 1f, 1.25f, 1.5f, 1.75f, 2f, 2.25f, 2.5f, 2.75f, 3f };
    private const string EditorPrefKey = "MyGame_TimeScale";

    static MainToolbarTimeScaleDropdown()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Time.timeScale = EditorPrefs.GetFloat(EditorPrefKey, 1f);
            }
        };
    }

    [MainToolbarElement(kElementPath, defaultDockPosition = MainToolbarDockPosition.Middle)]
    public static MainToolbarElement CreateTimeScaleDropdown()
    {
        float currentValue = EditorPrefs.GetFloat(EditorPrefKey, 1f);
        string labelText = currentValue.ToString("0.##") + "x";

        Texture2D myIcon = AssetDatabase.LoadAssetAtPath<Texture2D>(EditorPaths.IconPath + "timescale.png");

        var content = new MainToolbarContent(labelText, myIcon, "Set Time Scale");

        return new MainToolbarDropdown(content, ShowDropdownMenu);
    }

    private static void ShowDropdownMenu(Rect dropDownRect)
    {
        var menu = new GenericMenu();
        foreach (float value in timeScaleValues)
        {
            string displayName = value.ToString("0.##") + "x";
            bool isSelected = Mathf.Approximately(EditorPrefs.GetFloat(EditorPrefKey, 1f), value);

            menu.AddItem(new GUIContent(displayName), isSelected, () =>
            {
                EditorPrefs.SetFloat(EditorPrefKey, value);

                if (Application.isPlaying)
                    Time.timeScale = value;

                MainToolbar.Refresh(kElementPath);
            });
        }

        menu.DropDown(dropDownRect);
    }
}