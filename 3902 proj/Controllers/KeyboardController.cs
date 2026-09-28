using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    public class KeyboardController : IController
    {
        private Dictionary<Keys, ICommand> controllerMappings;
        private Dictionary<Keys, ICommand> singlePressMappings;
        private KeyboardState previousState;

        public KeyboardController()
        {
            controllerMappings = new Dictionary<Keys, ICommand>();
            singlePressMappings = new Dictionary<Keys, ICommand>();
        }

        public void RegisterCommand(Keys key, ICommand command)
        {
            controllerMappings.Add(key, command);
        }

        public void RegisterSinglePressCommand(Keys key, ICommand command)
        {
            singlePressMappings.Add(key, command);
        }

        public void Update()
        {
            KeyboardState currentState = Keyboard.GetState();
            Keys[] pressedKeys = currentState.GetPressedKeys();

            foreach (Keys key in pressedKeys)
            {
                if (controllerMappings.TryGetValue(key, out ICommand command))
                {
                    command.Execute();
                }
                else if (previousState.IsKeyUp(key) && singlePressMappings.TryGetValue(key, out command))
                {
                    command.Execute();
                }
            }

            previousState = currentState;
        }
    }
}
