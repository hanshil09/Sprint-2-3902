using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    internal class KeyboardController : IController
    {
        private Dictionary<Keys, ICommand> controllerMappings;
        private Dictionary<Keys, ICommand> singlePressMappings;
        private KeyboardState previousState;

        public KeyboardController()
        {
            controllerMappings = new Dictionary<Keys, ICommand>();
            singlePressMappings = new Dictionary<Keys, ICommand>();
            // Start from the current keys so a key held while the controller is rebuilt does not fire again.
            previousState = Keyboard.GetState();
        }

        public void RegisterCommand(Keys key, ICommand command)
        {
            controllerMappings.Add(key, command);
        }

        // Bind several keys to the same continuous command (e.g. A and Left for "move left").
        public void RegisterCommand(ICommand command, params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                controllerMappings.Add(key, command);
            }
        }

        public void RegisterSinglePressCommand(Keys key, ICommand command)
        {
            singlePressMappings.Add(key, command);
        }

        // Bind several keys to the same edge-triggered command (e.g. 1 and NumPad1).
        public void RegisterSinglePressCommand(ICommand command, params Keys[] keys)
        {
            foreach (Keys key in keys)
            {
                singlePressMappings.Add(key, command);
            }
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
