using System;
using MET.Applications.Events;
using MET.Core.Applications;
using MET.Core.Manager;
using MET.Core.Patterns;
using MET.Core.Types;
using MET.UI;
using UnityEngine;
using static MET.Applications.Events.GameEvents;

namespace MET.Gameplay
{
    public class GameManager : Singleton<GameManager>
    {
        public bool IsGameStarted { get; private set; }

        [SerializeField] private BirdMovement bird;

        [SerializeField] private float _spawnInterval;

        private void Start()
        {
            EventBus.Me.Subscribe<OnGameEnd>(OnGameEnd);

            Load();
            _sessionStartTime = DateTime.UtcNow;
        }

        private float _spawnTimer;

        private void Update()
        {
            if (IsGameStarted)
            {
                _spawnTimer -= TimerManager.Delta;

                if (_spawnTimer <= 0)
                {
                    _spawnTimer = _spawnInterval;
                    SpawnPipe();
                }
            }
            else
            {
                if (bird.State == BirdState.Selection)
                {
                    if (InputManager.Me.JumpPressed && ShopManager.Me.IsCharacterSelectionValid && !bird.MoveStarted)
                    {
                        IsGameStarted = true;
                        TimerManager.SlowFactor = 1;
                        EventBus.Me.Publish(new OnGameStarted());
                    }
                }
                else
                {
                    if (InputManager.Me.JumpPressed)
                    {
                        EventBus.Me.Publish(new OnGameReset());
                    }
                }
            }
        }

        private void SpawnPipe()
        {
            PipePool.Me.Get();
        }

        public void OnGameEnd(OnGameEnd args)
        {
            IsGameStarted = false;
            TimerManager.SlowFactor = 0;
            SavePoint(_point);
            _point = 0;
        }

        #region GAME TIME

        private DateTime _sessionStartTime;
        private double _totalPlaySeconds;

        public double TotalPlaySeconds => _totalPlaySeconds + GetCurrentSessionSeconds();

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveSessionTime();
            }
            else
            {
                _sessionStartTime = DateTime.UtcNow;
            }
        }

        private void OnApplicationQuit()
        {
            SaveSessionTime();
        }

        private void SaveSessionTime()
        {
            _totalPlaySeconds += GetCurrentSessionSeconds();
            _sessionStartTime = DateTime.UtcNow;

            DataManager.Me.SetTotalGameTime(_totalPlaySeconds.ToString());
        }

        private double GetCurrentSessionSeconds()
        {
            return (DateTime.UtcNow - _sessionStartTime).TotalSeconds;
        }

        private void Load()
        {
            double.TryParse(DataManager.Me.GetTotalGameTime(), out _totalPlaySeconds);
        }

        public string GetFormattedPlayTime()
        {
            TimeSpan time = TimeSpan.FromSeconds(TotalPlaySeconds);
            return $"{(int)time.TotalHours:00}:{time.Minutes:00}:{time.Seconds:00}";
        }

        #endregion

        #region Point 

        private int _point;

        public void AddPoint()
        {
            _point++;
            EventBus.Me.Publish(new OnPointTaken());
        }

        public int GetPoint()
        {
            return _point;
        }

        public void SavePoint(int point)
        {
            DataManager.Me.SetMaxPoint(point);
        }

        #endregion
    }
}