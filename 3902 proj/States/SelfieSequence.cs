using System;

namespace TransformersGame.States
{
    internal enum SelfiePhase { Entering, Handoff, Posing, Capture, Leaving, Finished }

    internal sealed class SelfieSequence
    {
        private const double EnterSpeed = 155;
        private const double LeaveSpeed = 185;
        private const double HandoffSeconds = 0.55;
        private const double PoseSeconds = 0.65;
        private const double FlashSeconds = 0.12;
        private double enterSeconds;
        private double leaveSeconds;

        public SelfiePhase Phase { get; private set; } = SelfiePhase.Finished;
        public double Elapsed { get; private set; }
        public bool IsPosing => Phase is SelfiePhase.Posing or SelfiePhase.Capture;
        public float RunProgress => Phase switch
        {
            SelfiePhase.Entering => (float)(Elapsed / enterSeconds),
            SelfiePhase.Leaving => (float)(Elapsed / leaveSeconds),
            _ => 0f
        };
        public float FlashOpacity => Phase == SelfiePhase.Leaving
            ? 0.75f * (float)Math.Max(0, 1 - Elapsed / FlashSeconds) : 0f;

        public void Start(int distance)
        {
            enterSeconds = distance / EnterSpeed;
            leaveSeconds = distance / LeaveSpeed;
            ChangePhase(SelfiePhase.Entering);
        }

        public void Update(double elapsedSeconds)
        {
            Elapsed += elapsedSeconds;
            switch (Phase)
            {
                case SelfiePhase.Entering when Elapsed >= enterSeconds: ChangePhase(SelfiePhase.Handoff); break;
                case SelfiePhase.Handoff when Elapsed >= HandoffSeconds: ChangePhase(SelfiePhase.Posing); break;
                case SelfiePhase.Posing when Elapsed >= PoseSeconds: ChangePhase(SelfiePhase.Capture); break;
                case SelfiePhase.Leaving when Elapsed >= leaveSeconds: ChangePhase(SelfiePhase.Finished); break;
            }
        }

        // Wait for the shutter frame, then flash and preview during the exit run.
        public void PhotoTaken()
        {
            if (Phase == SelfiePhase.Capture) ChangePhase(SelfiePhase.Leaving);
        }

        private void ChangePhase(SelfiePhase phase)
        {
            Phase = phase;
            Elapsed = 0;
        }
    }
}
