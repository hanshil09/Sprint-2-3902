using Microsoft.Xna.Framework;

namespace ArmadilloGame.Interfaces
{
    interface IPlayerState
    {
        float Speed { get; }

        void Transform();

        void Shoot(bool angled);

        void UseItem(int itemNumber, bool angled = false);

        void Update(GameTime gameTime);

        ISprite CreateSprite();
    }
}
