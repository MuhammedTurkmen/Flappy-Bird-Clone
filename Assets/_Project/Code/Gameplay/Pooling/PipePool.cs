using MET.Applications.Events;
using MET.Core.Pooling;
using static MET.Applications.Events.GameEvents;

namespace MET.Gameplay
{
    public class PipePool : Pooling<PipeMovement>
    {
        private void Start()
        {
            EventBus.Me.Subscribe<OnGameReset>(OnGameReset);
        }

        public void OnGameReset(OnGameReset args)
        {
            ReleaseAll();
        }
    }
}
