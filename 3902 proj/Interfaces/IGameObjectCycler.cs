using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    interface IGameObjectCycler
    {
        IReadOnlyList<IGameObject> Objects { get; }

        IGameObject CurrentObject { get; }

        void Add(IGameObject gameObject);

        void NextObject();

        void Previous();

        void Update(GameTime gameTime);
    }
}
