using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Interfaces;

namespace TransformersGame.States
{
    internal sealed class GameplayState : IGameState
    {
        private readonly Action<GameTime> update;
        private readonly Action draw;

        public GameplayState(Action<GameTime> update, Action draw)
        {
            this.update = update;
            this.draw = draw;
        }

        public void Update(GameTime gameTime) => update(gameTime);
        public void Draw(SpriteBatch batch) => draw();
    }
}
