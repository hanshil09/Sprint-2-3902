using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Core;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
    
    internal abstract class PlayerDecorator : IPlayer
    {
        private readonly IPlayerHost host;
        private readonly double duration;

        protected PlayerDecorator(IPlayer inner, IPlayerHost host, double durationMilliseconds)
        {
            Inner = inner;
            this.host = host;
            duration = durationMilliseconds;
            RemainingMilliseconds = durationMilliseconds;
        }

        public IPlayer Inner { get; private set; }

        public double RemainingMilliseconds { get; private set; }

        public virtual Vector2 Position { get => Inner.Position; set => Inner.Position = value; }
        public virtual int Width => Inner.Width;
        public virtual int Height => Inner.Height;
        public virtual Direction Facing => Inner.Facing;
        public virtual int Health => Inner.Health;
        public virtual int MaxHealth => Inner.MaxHealth;
        public virtual int MedkitCount => Inner.MedkitCount;
        public virtual int ShieldCount => Inner.ShieldCount;
        public virtual int BombLevel => Inner.BombLevel;
        public virtual int EnergonCount => Inner.EnergonCount;
        public virtual bool IsShieldActive => Inner.IsShieldActive;
        public virtual bool IsInvincible => Inner.IsInvincible;
        public virtual float SpeedScale { get => Inner.SpeedScale; set => Inner.SpeedScale = value; }

        public virtual void Move(Direction direction) => Inner.Move(direction);
        public virtual void Jump() => Inner.Jump();
        public virtual void Bounce(float speed) => Inner.Bounce(speed);
        public virtual void Shoot(bool angled) => Inner.Shoot(angled);
        public virtual void UseItem(int itemNumber) => Inner.UseItem(itemNumber);
        public virtual void Transform() => Inner.Transform();
        public virtual bool TakeDamage() => Inner.TakeDamage();
        public virtual bool TryAddItem(ItemKind kind) => Inner.TryAddItem(kind);
        public virtual bool UseMedkit() => Inner.UseMedkit();
        public virtual bool UseShield() => Inner.UseShield();
        public virtual void Draw(SpriteBatch spriteBatch) => Inner.Draw(spriteBatch);

        public virtual void EnterRoom(IReadOnlyList<IBlock> blocks, IProjectileManager projectiles, Vector2 position, float gravity = Physics.DefaultGravity)
        {
            Inner.EnterRoom(blocks, projectiles, position, gravity);
        }

        public virtual void Update(GameTime gameTime)
        {
            RemainingMilliseconds -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (RemainingMilliseconds <= 0)
            {
                OnExpired();
                host.RemovePlayerDecorator(this);
            }
            Inner.Update(gameTime);
        }

        // Picking up the same power-up again restarts its timer instead of stacking a second wrapper.
        public void Refresh() => RemainingMilliseconds = duration;

        // Removes 'target' wherever it sits in the decorator chain and returns the (possibly new) outermost player.
        public static IPlayer RemoveFromChain(IPlayer root, PlayerDecorator target)
        {
            if (ReferenceEquals(root, target))
            {
                return target.Inner;
            }
            for (IPlayer current = root; current is PlayerDecorator decorator; current = decorator.Inner)
            {
                if (ReferenceEquals(decorator.Inner, target))
                {
                    decorator.Inner = target.Inner;
                    break;
                }
            }
            return root;
        }

        public static T Find<T>(IPlayer root) where T : PlayerDecorator
        {
            for (IPlayer current = root; current is PlayerDecorator decorator; current = decorator.Inner)
            {
                if (decorator is T match)
                {
                    return match;
                }
            }
            return null;
        }

        protected virtual void OnExpired()
        {
        }
    }
}
