using Microsoft.Xna.Framework;

namespace TransformersGame.Interfaces
{
    public interface IPlayerState
    {
        float Speed { get; }

        void Transform();

        void Shoot();

        void UseItem(int itemNumber);

        void Update(GameTime gameTime);

        ISprite CreateSprite();
    }
}
