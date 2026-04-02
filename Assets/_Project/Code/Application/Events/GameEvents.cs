namespace MET.Applications.Events
{
    public class GameEvents 
    {
        public class OnGameStarted : IGameEvent { };
        public class OnGameEnd : IGameEvent { };
        public class OnGameReset : IGameEvent { };

        public class OnBirdFall : IGameEvent { };
        public class OnPointTaken : IGameEvent { };
        public class OnBestScoreChanged : IGameEvent { };
        public class OnSaveLoaded : IGameEvent { };
    }
}