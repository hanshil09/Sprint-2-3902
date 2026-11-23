using System;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Controllers
{
    // Reports the left mouse button as a fire phase (press / hold / release) each frame so weapons
    // can charge, autofire, or fire once. The right button still runs a discrete command.
    internal sealed class MouseController : IController
    {
        private readonly Action<FirePhase> onFire;
        private readonly ICommand rightClickCommand;
        private readonly Func<MouseState> getMouseState;
        private MouseState previousState;

        public MouseController(Action<FirePhase> onFire, ICommand rightClickCommand)
            : this(onFire, rightClickCommand, Mouse.GetState)
        {
        }

        public MouseController(Action<FirePhase> onFire, ICommand rightClickCommand, Func<MouseState> getMouseState)
        {
            ArgumentNullException.ThrowIfNull(onFire);
            ArgumentNullException.ThrowIfNull(rightClickCommand);
            ArgumentNullException.ThrowIfNull(getMouseState);

            this.onFire = onFire;
            this.rightClickCommand = rightClickCommand;
            this.getMouseState = getMouseState;
            previousState = getMouseState();
        }

        public void Update()
        {
            MouseState currentState = getMouseState();

            onFire(ResolveFirePhase(currentState.LeftButton, previousState.LeftButton));

            if (currentState.RightButton == ButtonState.Pressed && previousState.RightButton == ButtonState.Released)
            {
                rightClickCommand.Execute();
            }

            previousState = currentState;
        }

        private static FirePhase ResolveFirePhase(ButtonState current, ButtonState previous)
        {
            bool down = current == ButtonState.Pressed;
            bool wasDown = previous == ButtonState.Pressed;
            if (down && !wasDown)
            {
                return FirePhase.Pressed;
            }
            if (down)
            {
                return FirePhase.Held;
            }
            if (wasDown)
            {
                return FirePhase.Released;
            }
            return FirePhase.Up;
        }
    }
}
