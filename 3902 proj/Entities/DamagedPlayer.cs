using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    internal class DamagedPlayer : IPlayer
    {
        private const double DamageDuration = 1000;
        private const double BlinkInterval = 100;

        private IPlayer decoratedPlayer;
        private Game1 game;
        private double timer;

        public DamagedPlayer(IPlayer decoratedPlayer, Game1 game)
        {
            this.decoratedPlayer = decoratedPlayer;
            this.game = game;
            timer = DamageDuration;
        }

        public Vector2 Position
        {
            get
            {
                return decoratedPlayer.Position;
            }
            set
            {
                decoratedPlayer.Position = value;
            }
        }

        public int Width
        {
            get
            {
                return decoratedPlayer.Width;
            }
        }

        public int Height
        {
            get
            {
                return decoratedPlayer.Height;
            }
        }

        public Direction Facing
        {
            get
            {
                return decoratedPlayer.Facing;
            }
        }

        public int Health => decoratedPlayer.Health;

        public int MaxHealth => decoratedPlayer.MaxHealth;

        public int MedkitCount => decoratedPlayer.MedkitCount;

        public int ShieldCount => decoratedPlayer.ShieldCount;

        public bool IsShieldActive => decoratedPlayer.IsShieldActive;

        public void Move(Direction direction)
        {
            decoratedPlayer.Move(direction);
        }

        public void Jump()
        {
            decoratedPlayer.Jump();
        }

        public void Shoot(bool angled)
        {
            decoratedPlayer.Shoot(angled);
        }

        public void UseItem(int itemNumber)
        {
            decoratedPlayer.UseItem(itemNumber);
        }

        public void Transform()
        {
            decoratedPlayer.Transform();
        }

        public bool TakeDamage()
        {
            return false;
        }

        public void AddItem(ItemKind kind)
        {
            decoratedPlayer.AddItem(kind);
        }

        public bool UseMedkit()
        {
            return decoratedPlayer.UseMedkit();
        }

        public bool UseShield()
        {
            return decoratedPlayer.UseShield();
        }

        public void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position)
        {
            decoratedPlayer.EnterRoom(blocks, projectiles, position);
        }

        public void Update(GameTime gameTime)
        {
            timer -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timer <= 0)
            {
                RemoveDecorator();
            }
            decoratedPlayer.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if ((int)(timer / BlinkInterval) % 2 == 0)
            {
                decoratedPlayer.Draw(spriteBatch);
            }
        }

        private void RemoveDecorator()
        {
            game.Player = decoratedPlayer;
        }
    }
}
