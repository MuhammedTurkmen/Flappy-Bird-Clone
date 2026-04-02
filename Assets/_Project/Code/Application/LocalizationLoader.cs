using System;
using System.Collections.Generic;
using System.IO;
using MET.Core.Utilities.Extentions;
using Newtonsoft.Json;
using UnityEngine;

namespace MET.Core.Save
{
    public class LocalizationData
    {
        public string key;
        public string description;
    }

    public static class LocalizationLoader
    {
        private static Dictionary<string, string> _localizedUI;
        private static Dictionary<string, LocalizationData> _localizedStat;

        public static void LoadLocalization(string locale = "tr")
        {
            string fileName = locale + ".json";  // örn: "tr.json"
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            //Debug.Log("Loading localization from: " + path);

            if (!File.Exists(path))
            {
                Debug.LogError("Localization file not found at path: " + path);
                return;
            }

            string json = File.ReadAllText(path);
            try
            {
                _localizedUI = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                //Debug.Log("Localization loaded, entries count: " + _localized.Count);
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse localization JSON: " + e);
            }
        }

        public static string Get(string key)
        {
            if (_localizedUI == null)
            {
                Debug.LogError("Localization not loaded. Call LoadLocalization first.");
                return key;
            }

            if (_localizedUI.TryGetValue(key, out string value))
                return value;

            Debug.LogWarning("Localization key not found: " + key);
            return "$$$$$$";
        }

        public static string GetUppercase(string key)
        {
            if (_localizedUI == null)
            {
                Debug.LogError("Localization not loaded. Call LoadLocalization first.");
                return key;
            }

            if (_localizedUI.TryGetValue(key, out string value))
                return value.ToUpperInvariant();

            Debug.LogWarning("Localization key not found: " + key);
            return "$$$$$$";
        }
        
        public static string GetCapitalized(string key)
        {
            if (_localizedUI == null)
            {
                Debug.LogError("Localization not loaded. Call LoadLocalization first.");
                return key;
            }

            if (_localizedUI.TryGetValue(key, out string value))
                return value.FirstCharToUpperInvariant();

            Debug.LogWarning("Localization key not found: " + key);
            return "$$$$$$";
        }

        public static void LoadStatLocalization(string locale = "tr")
        {
            string fileName = locale + "_stat_details.json";
            string path = Path.Combine(Application.streamingAssetsPath, fileName);

            if (!File.Exists(path))
            {
                Debug.LogError("Stat localization file not found at path: " + path);
                return;
            }

            string json = File.ReadAllText(path);

            try
            {
                _localizedStat = JsonConvert.DeserializeObject<Dictionary<string, LocalizationData>>(json);
                //Debug.Log($"Stat localization loaded: {_localizedStat.Count} entries");
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to parse stat localization JSON: " + e);
            }
        }

        public static LocalizationData GetStat(string key)
        {
            if (_localizedStat == null)
            {
                Debug.LogError("Stat localization not loaded. Call LoadStatLocalization first.");
                return null;
            }

            if (_localizedStat.TryGetValue(key, out var value))
                return value;

            Debug.LogWarning("Stat localization key not found: " + key);
            return null;
        }

    }
}
