namespace ArmadilloGame.Controllers
{
    // The per-frame state of the fire button, so weapons can react to a press, a hold, or a release.
    // Pressed and Released are single-frame edges; Held repeats while the button stays down; Up means
    // the button is not pressed.
    internal enum FirePhase
    {
        Up,
        Pressed,
        Held,
        Released
    }
}
