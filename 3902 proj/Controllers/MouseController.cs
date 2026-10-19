using System;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    public sealed class MouseController : IController
    {
        private readonly ICommand leftClickCommand;
        private readonly ICommand rightClickCommand;
        private readonly Func<MouseState> getMouseState;
        private MouseState previousState;

        public MouseController(ICommand leftClickCommand, ICommand rightClickCommand)
            : this(leftClickCommand, rightClickCommand, Mouse.GetState)
        {
        }

        public MouseController(
            ICommand leftClickCommand,
            ICommand rightClickCommand,
            Func<MouseState> getMouseState)
        {
            ArgumentNullException.ThrowIfNull(leftClickCommand);
            ArgumentNullException.ThrowIfNull(rightClickCommand);
            ArgumentNullException.ThrowIfNull(getMouseState);

            this.leftClickCommand = leftClickCommand;
            this.rightClickCommand = rightClickCommand;
            this.getMouseState = getMouseState;
            previousState = getMouseState();
        }

        public void Update()
        {
            MouseState currentState = getMouseState();

            if (WasJustPressed(currentState.LeftButton, previousState.LeftButton))
            {
                leftClickCommand.Execute();
            }

            if (WasJustPressed(currentState.RightButton, previousState.RightButton))
            {
                rightClickCommand.Execute();
            }

            previousState = currentState;
        }

        private static bool WasJustPressed(ButtonState current, ButtonState previous)
        {
            return current == ButtonState.Pressed && previous == ButtonState.Released;
        }
    }
}
