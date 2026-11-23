using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ArmadilloGame.Core;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    // A pickup that grants a weapon. Collecting it unlocks the weapon and tops up its ammo (the
    // loadout grants the weapon's starting ammo each time it is unlocked).
    internal sealed class WeaponPickup : IItem
    {
        private static readonly IReadOnlyDictionary<ItemKind, WeaponKind> WeaponFor = new Dictionary<ItemKind, WeaponKind>
        {
            [ItemKind.PistolPickup] = WeaponKind.Pistol,
            [ItemKind.ShotgunPickup] = WeaponKind.Shotgun,
            [ItemKind.MolotovPickup] = WeaponKind.Molotov
        };

        private readonly ISprite sprite;
        private readonly WeaponKind weapon;
        private readonly ItemBob bob = new ItemBob();
        private bool collected;

        public WeaponPickup(Vector2 position, ISprite sprite, ItemKind kind)
        {
            Position = position;
            this.sprite = sprite;
            weapon = WeaponFor[kind];
        }

        public static bool IsWeaponPickup(ItemKind kind) => WeaponFor.ContainsKey(kind);

        public Vector2 Position { get; set; }

        public Rectangle Bounds => new Rectangle((int)Position.X, (int)Position.Y, sprite.Width, sprite.Height);

        public bool TryCollect(IPlayer player)
        {
            if (collected)
            {
                return false;
            }
            player.UnlockWeapon(weapon);
            collected = true;
            return true;
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
            bob.Update(gameTime);
        }

        public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, Position + bob.Offset);
    }
}
