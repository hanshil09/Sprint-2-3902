using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    interface ILevel
    {
        IReadOnlyList<IBlock> Blocks { get; }

        Vector2 PlayerStart { get; }

        void FillEnemyCycler(IGameObjectCycler cycler, IPlayer player, IProjectileManager projectiles);

        IReadOnlyList<IItem> CollectItems(IPlayer player);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);

        IReadOnlyList<IGameObject> GetOnScreenObjects();
    }
}
