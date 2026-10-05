using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TransformersGame.Interfaces
{
    public interface ILevel
    {
        IReadOnlyList<IBlock> Blocks { get; }

        void FillBlockCycler(IGameObjectCycler cycler);

        void FillItemCycler(IGameObjectCycler cycler);

        void FillEnemyCycler(IGameObjectCycler cycler, IPlayer player, IProjectileManager projectiles);

        void CollectItems(IPlayer player);

        void Update(GameTime gameTime);

        void Draw(SpriteBatch spriteBatch);
    }
}
