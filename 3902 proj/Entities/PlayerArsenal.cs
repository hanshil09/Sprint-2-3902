using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TransformersGame.Collisions;
using TransformersGame.Core;
using TransformersGame.Factories;
using TransformersGame.Interfaces;

namespace TransformersGame.Entities
{
  
    internal sealed class PlayerArsenal
    {
        private const float ShotSpeed = 520f;
        private const float ShotRecoilDistance = 10f;
        private const float OrbSpeed = 300f;
        private const float PiercingSpeed = 420f;
        private const float SpreadSpeed = 420f;
        private const float SpreadAngleDegrees = 18f;
        private const float MuzzleHeight = 0.3f;

        private readonly Player player;
        private readonly CollisionManager collisionManager;
        private IProjectileManager projectiles;
        private IReadOnlyList<IBlock> blocks;
        private BoomerangProjectile activeBoomerang;

        public PlayerArsenal(Player player, IProjectileManager projectiles, CollisionManager collisionManager,
            IReadOnlyList<IBlock> blocks)
        {
            this.player = player;
            this.projectiles = projectiles;
            this.collisionManager = collisionManager;
            this.blocks = blocks;
        }

        public void EnterRoom(IProjectileManager roomProjectiles, IReadOnlyList<IBlock> roomBlocks)
        {
            projectiles = roomProjectiles;
            blocks = roomBlocks;
            activeBoomerang = null;
        }

        public void FireShot(bool angled)
        {
            ISprite shot = ProjectileSpriteFactory.Instance.CreateShotSprite(player.FacingLeft);
            projectiles.Add(new EnergyProjectile(MuzzlePosition(shot), Forward(ShotSpeed), angled, player.FacingLeft, shot));
            // Recoil always pushes the player in the direction opposite of the shot.
            float recoil = player.FacingLeft ? ShotRecoilDistance : -ShotRecoilDistance;
            player.Position = new Vector2(player.Position.X + recoil, player.Position.Y);
        }

        public void FireOrb()
        {
            ISprite orb = ProjectileSpriteFactory.Instance.CreateOrbSprite(player.FacingLeft);
            projectiles.Add(new EnergyProjectile(MuzzlePosition(orb), Forward(OrbSpeed), false, player.FacingLeft, orb));
        }

        public void FirePiercing()
        {
            ISprite orb = ProjectileSpriteFactory.Instance.CreateOrbSprite(player.FacingLeft);
            projectiles.Add(new PiercingProjectile(MuzzlePosition(orb), Forward(PiercingSpeed), orb));
        }

        public void FireSpread()
        {
            foreach (float degrees in new[] { -SpreadAngleDegrees, 0f, SpreadAngleDegrees })
            {
                ISprite pellet = ProjectileSpriteFactory.Instance.CreateSpreadPelletSprite();
                float radians = MathHelper.ToRadians(degrees);
                Vector2 velocity = new Vector2(MathF.Cos(radians) * Direction, MathF.Sin(radians)) * SpreadSpeed;
                projectiles.Add(new EnergyProjectile(MuzzlePosition(pellet), velocity, false, player.FacingLeft, pellet));
            }
        }

        // Only one boomerang can be out at a time, like the boomerang in Zelda.
        public void ThrowBoomerang()
        {
            if (activeBoomerang != null && activeBoomerang.IsActive)
            {
                return;
            }
            ISprite sprite = ProjectileSpriteFactory.Instance.CreateBoomerangSprite();
            activeBoomerang = new BoomerangProjectile(MuzzlePosition(sprite), (int)Direction, () => player.Center, sprite);
            projectiles.Add(activeBoomerang);
        }

        public void DropBomb(float gravity, int blastLevel)
        {
            ISprite bomb = ProjectileSpriteFactory.Instance.CreateBombSprite();
            float x = player.Position.X + (player.Width - bomb.Width) / 2f;
            float y = player.Position.Y + player.Height - bomb.Height;
            projectiles.Add(new Bomb(new Vector2(x, y), bomb, blocks, gravity,
                () => ProjectileSpriteFactory.Instance.CreateExplosionSprite(blastLevel), collisionManager));
        }

        private float Direction => player.FacingLeft ? -1f : 1f;

        private Vector2 Forward(float speed) => new Vector2(Direction * speed, 0);

        private Vector2 MuzzlePosition(ISprite projectileSprite)
        {
            float x = player.FacingLeft ? player.Position.X - projectileSprite.Width : player.Position.X + player.Width;
            float y = player.Position.Y + player.Height * MuzzleHeight;
            return new Vector2(x, y);
        }
    }
}
