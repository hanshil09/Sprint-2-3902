using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.Core
{
    public class GameObjectCycler : IGameObjectCycler
    {
        private readonly List<IGameObject> gameObjects;
        private int currentIndex;

        public GameObjectCycler()
        {
            gameObjects = new List<IGameObject>();
            currentIndex = 0;
        }

        public void Add(IGameObject gameObject)
        {
            gameObjects.Add(gameObject);
        }

        public void NextObject()
        {
            if (gameObjects.Count > 0)
            {
                currentIndex = (currentIndex + 1) % gameObjects.Count;
            }
        }

        public void Previous()
        {
            if (gameObjects.Count > 0)
            {
                currentIndex = (currentIndex - 1 + gameObjects.Count) % gameObjects.Count;
            }
        }

        public void Update(GameTime gameTime)
        {
            if (gameObjects.Count > 0)
            {
                gameObjects[currentIndex].Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (gameObjects.Count > 0)
            {
                gameObjects[currentIndex].Draw(spriteBatch);
            }
        }
    }
}
