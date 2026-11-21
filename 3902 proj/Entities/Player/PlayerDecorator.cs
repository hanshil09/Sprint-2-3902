using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
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
        public virtual bool IsInBallForm => Inner.IsInBallForm;
        public virtual Direction Facing => Inner.Facing;
        public virtual Vector2 AimDirection => Inner.AimDirection;
        public virtual void SetAimTarget(Vector2 worldTarget) => Inner.SetAimTarget(worldTarget);
        public virtual int Health => Inner.Health;
        public virtual int MaxHealth => Inner.MaxHealth;
        public virtual PlayerInventoryView Inventory => Inner.Inventory;
        public virtual int KeyCount => Inner.KeyCount;
        public virtual bool IsShieldActive => Inner.IsShieldActive;
        public virtual bool IsInvincible => Inner.IsInvincible;
        public virtual float SpeedScale { get => Inner.SpeedScale; set => Inner.SpeedScale = value; }

        public virtual void Move(Direction direction) => Inner.Move(direction);
        public virtual void Jump() => Inner.Jump();
        public virtual void Bounce(float speed) => Inner.Bounce(speed);
        public virtual void Shoot(bool angled) => Inner.Shoot(angled);
        public virtual void UseItem(int itemNumber, bool angled = false) => Inner.UseItem(itemNumber, angled);
        public virtual void Transform() => Inner.Transform();
        public virtual bool TakeDamage() => Inner.TakeDamage();
        public virtual bool TryAddItem(ItemKind kind) => Inner.TryAddItem(kind);
        public virtual bool UseMedkit() => Inner.UseMedkit();
        public virtual bool UseShield() => Inner.UseShield();
        public virtual bool UseShotgunCall() => Inner.UseShotgunCall();
        public virtual bool FireShotgun(bool aimUp = false) => Inner.FireShotgun(aimUp);
        public virtual bool IsWeaponUnlocked(WeaponKind kind) => Inner.IsWeaponUnlocked(kind);
        public virtual int WeaponAmmo(WeaponKind kind) => Inner.WeaponAmmo(kind);
        public virtual bool WeaponHasUnlimitedAmmo(WeaponKind kind) => Inner.WeaponHasUnlimitedAmmo(kind);
        public virtual void UnlockWeapon(WeaponKind kind) => Inner.UnlockWeapon(kind);
        public virtual void AddWeaponAmmo(WeaponKind kind, int amount) => Inner.AddWeaponAmmo(kind, amount);
        public virtual void UnlockAllWeapons() => Inner.UnlockAllWeapons();
        public virtual bool FireWeapon(WeaponKind kind) => Inner.FireWeapon(kind);
        public virtual bool TryUseKey() => Inner.TryUseKey();
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
