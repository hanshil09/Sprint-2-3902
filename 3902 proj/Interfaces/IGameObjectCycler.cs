using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    public interface IGameObjectCycler
    {
        void Add(IGameObject gameObject);

        void NextObject();

        void Previous();

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
