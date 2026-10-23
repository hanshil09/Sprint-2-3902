using Microsoft.Xna.Framework;
using TransformersGame.Core;

namespace TransformersGame.States
{
    internal enum CrumblingPhase { Ready, Cracking, Missing, Reforming }

    // Like the Goomba state-machine example: transitions and timers live outside the object.
    internal sealed class CrumblingPlatformStateMachine
    {
        private const double MissingSeconds = 3;
        private const double ReformSeconds = 0.6;
        private readonly double crumbleSeconds;
        private double timer;

        public CrumblingPlatformStateMachine(CrumbleKind kind)
        {
            switch (kind)
            {
                case CrumbleKind.Fast: crumbleSeconds = 0.6; break;
                case CrumbleKind.Medium: crumbleSeconds = 1.2; break;
                default: crumbleSeconds = 2.0; break;
            }
        }

        public CrumblingPhase Phase { get; private set; } = CrumblingPhase.Ready;

        public bool IsSolid => Phase == CrumblingPhase.Ready || Phase == CrumblingPhase.Cracking;

        public void OnStand()
        {
            if (Phase == CrumblingPhase.Ready)
            {
                ChangePhase(CrumblingPhase.Cracking, crumbleSeconds);
            }
        }

        public void Update(GameTime gameTime, bool canRestore)
        {
            if (Phase == CrumblingPhase.Ready) return;
            timer -= gameTime.ElapsedGameTime.TotalSeconds;
            if (timer > 0) return;
            switch (Phase)
            {
                case CrumblingPhase.Cracking:
                    ChangePhase(CrumblingPhase.Missing, MissingSeconds);
                    break;
                case CrumblingPhase.Missing:
                    if (canRestore) ChangePhase(CrumblingPhase.Reforming, ReformSeconds);
                    break;
                case CrumblingPhase.Reforming:
                    if (canRestore) ChangePhase(CrumblingPhase.Ready, 0);
                    break;
            }
        }

        private void ChangePhase(CrumblingPhase phase, double seconds)
        {
            Phase = phase;
            timer = seconds;
        }
    }
}
