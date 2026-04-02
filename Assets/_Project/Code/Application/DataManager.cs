using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MET.Applications.Events;
using MET.Core.Attributes;
using MET.Core.Patterns;
using MET.Core.Save;
using Newtonsoft.Json;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.Core.Manager
{
    #region Definitions

    [Serializable]
    public class GameDataWrapper
    {
        public Gameplay GameplayData;
        public AudioSettings AudioSettingsData;
        public Costume CostumeData;
    }

    [Serializable]
    public class AudioSettings
    {
        public float MasterVolume;
        public float MusicVolume;
        public float SFXVolume;
    }

    [Serializable]
    public class Gameplay
    {
        public string TotalPlayTimeSeconds;
        public int MaxPoint;
    }

    [Serializable]
    public class Costume
    {
        public int SelectedCostumeIndex;
        public List<int> BuyedCostumeIndexes;
    }

    #endregion

    public class DataManager : Singleton<DataManager>
    {
        public string jsonFileName = "characterStats.json";
        public GameDataWrapper allData;

        string SaveDirectory => Application.persistentDataPath;
        string SavePath => Path.Combine(SaveDirectory, jsonFileName);

        protected override void Awake()
        {
            base.Awake();

            LocalizationLoader.LoadLocalization("en");
            LocalizationLoader.LoadStatLocalization("en");

            _ = LoadDataFromFile();

            //AdvancedSoundManager.Me.SetupSounds();
        }

        private void Start()
        {
            EventBus.Me.Subscribe<OnGameEnd>(OnGameEnd);
        }

        private void OnGameEnd(OnGameEnd args)
        {
            SaveAll();
        }

        public void SetMaxPoint(int maxPoint)
        {
            if (allData.GameplayData.MaxPoint >= maxPoint) return;

            allData.GameplayData.MaxPoint = maxPoint;

            SaveAll();
        }

        public int GetMaxPoint()
        {
            return allData.GameplayData.MaxPoint;
        }

        public void SetTotalGameTime(string totalTime)
        {
            allData.GameplayData.TotalPlayTimeSeconds = totalTime;

            SaveAll();
        }

        public string GetTotalGameTime()
        {
            return allData.GameplayData.TotalPlayTimeSeconds;
        }


        [Button("LOAD DATA")]
        [ContextMenu("LOAD DATA")]
        public async Task LoadAll() => await LoadDataFromFile();

        [Button("SAVE DATA")]
        [ContextMenu("SAVE DATA")]
        public void SaveAll() => _ = SaveDataToFile();

        [Button("SAVE BACKUP DATA")]
        [ContextMenu("SAVE BACKUP DATA")]
        public void SaveBackupAll() => _ = SaveBackupDataToFile();

        [Button("RESET DATA")]
        [ContextMenu("RESET DATA")]
        public void ResetAll() => ResetAllData();

        [ContextMenu("OPEN SAVE FOLDER")]
        [Button("OPEN SAVE FOLDER")]
        public void OpenFolder()
        {
            string path = Application.persistentDataPath;

            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Klasör bulunamadı: " + path);
                return;
            }

            Application.OpenURL("file:///" + path);
        }

        void ResetAllData()
        {
            //Debug.Log("🔄 Resetting all data to defaults...");

            allData = new GameDataWrapper
            {
                GameplayData = new Gameplay
                {
                    MaxPoint = 0,
                    TotalPlayTimeSeconds = "0"
                },
                AudioSettingsData = new AudioSettings
                {
                    MasterVolume = 0.3f,
                    MusicVolume = 0.3f,
                    SFXVolume = 0.3f,
                },
                CostumeData = new Costume
                {
                    SelectedCostumeIndex = 0,
                    BuyedCostumeIndexes = new List<int>
                    {
                        0
                    }
                }
            };

            _ = SaveDataToFile();
            //Debug.Log("✅ Default data created and saved.");
        }

        async Task LoadDataFromFile()
        {
            if (!File.Exists(SavePath))
            {
                //Debug.LogWarning("⚠️ JSON not found, creating default data...");
                ResetAllData();
                return;
            }

            string json = await File.ReadAllTextAsync(SavePath);

            try
            {
                allData = JsonConvert.DeserializeObject<GameDataWrapper>(json);

                //Debug.Log("✅ All data loaded successfully!");

                //Debug.Log($"🎚 Master Volume: {allData.settings.masterVolume}");
                //Debug.Log($"🩸 Blood Drops: {allData.game.bloodDropAmount}");
                //Debug.Log($"🛡 Defence Level: {allData.stats[0].level}");
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Failed to parse JSON: " + e.Message);
            }

            if (Application.isPlaying)
            {
                EventBus.Me.Publish(new OnSaveLoaded());
            }
        }

        private static readonly SemaphoreSlim _saveLock = new SemaphoreSlim(1, 1);

        async Task SaveDataToFile()
        {
            await _saveLock.WaitAsync();
            try
            {
                if (!Directory.Exists(SaveDirectory))
                    Directory.CreateDirectory(SaveDirectory);

                string json = JsonConvert.SerializeObject(allData, Formatting.Indented);

                await File.WriteAllTextAsync(SavePath, json);

                if (Application.isPlaying)
                {
                    EventBus.Me.Publish(new OnBestScoreChanged());
                }
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Failed to save JSON: " + e.Message);
            }
            finally
            {
                _saveLock.Release();
            }
        }

        async Task SaveBackupDataToFile()
        {
            try
            {
                if (!Directory.Exists(SaveDirectory))
                    Directory.CreateDirectory(SaveDirectory);

                string json = JsonConvert.SerializeObject(allData, Formatting.Indented);

                string backupPath = Path.Combine(SaveDirectory, Path.GetFileNameWithoutExtension(jsonFileName) + "_backup.json");
                await File.WriteAllTextAsync(backupPath, json);
                //Debug.Log("📦 Backup created at: " + backupPath);
            }
            catch (Exception e)
            {
                Debug.LogError("❌ Failed to save JSON: " + e.Message);
            }
        }
    }
}