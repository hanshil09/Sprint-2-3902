using System;
using TransformersGame.Interfaces;

namespace TransformersGame.Controllers
{
    public sealed class CompositeController : IController
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
