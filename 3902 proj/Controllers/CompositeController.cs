using System;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Controllers
{
    internal sealed class CompositeController : IController
    {
        private readonly IController[] controllers;

        public CompositeController(params IController[] controllers)
        {
            ArgumentNullException.ThrowIfNull(controllers);
            this.controllers = controllers;
        }

        public void Update()
        {
            foreach (IController controller in controllers)
            {
                controller.Update();
            }
        }
    }
}
