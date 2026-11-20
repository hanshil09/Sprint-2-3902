using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ArmadilloGame.Core;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities
{
    internal sealed class BossConfiguration
    {
        public static BossConfiguration Warlord { get; } = new BossConfiguration();

        public EnemyKind Kind { get; init; } = EnemyKind.Warlord;
        public int MaximumHealth { get; init; } = 30;
        public int Size { get; init; } = EnemySpriteFactory.BossSize;
        public int HitboxInset { get; init; } = 14;
        public int HealthHeartCount { get; init; } = 10;
        public Vector2 HealthHeartsPosition { get; init; } = new Vector2(318, 82);
        public Color NormalHealthColor { get; init; } = Color.Gold;
        public Color EnragedHealthColor { get; init; } = Color.OrangeRed;

        public int PhaseOneHealthNumerator { get; init; } = 2;
        public int PhaseTwoHealthNumerator { get; init; } = 1;
        public int PhaseHealthDenominator { get; init; } = 3;

        public double HitCooldownSeconds { get; init; } = 0.08;
        public double FlashSeconds { get; init; } = 0.12;
        public float FireballSpeed { get; init; } = 260f;
        public float HoverY { get; init; } = 96f;
        public float FloorY { get; init; } = 540 - BlockSpriteFactory.BlockSize - EnemySpriteFactory.BossSize;
        public float MinimumX { get; init; } = 16f;
        public float MaximumX { get; init; } = 960 - EnemySpriteFactory.BossSize - 16f;
        public float OpenLeft { get; init; } = 208f;
        public float OpenRight { get; init; } = 592f;
        public float PatrolLeft { get; init; } = 96f;
        public float PatrolRight { get; init; } = 768f;
        public IReadOnlyList<float> PatrolSpeeds { get; init; } = new float[] { 0f, 80f, 110f, 150f };
        public IReadOnlyList<double> FireIntervals { get; init; } = new double[] { 0, 1.8, 1.6, 1.3 };
        public IReadOnlyList<double> SlamIntervals { get; init; } = new double[] { 0, 0, 5.0, 3.2 };
        public IReadOnlyList<float> FireAngleOffsets { get; init; } = new float[] { -14f, 0f, 14f };
        public int RingShotCount { get; init; } = 10;
        public float RingRotationPerVolley { get; init; } = 9f;

        public double WarningSeconds { get; init; } = 0.7;
        public float ShakeAmount { get; init; } = 3f;
        public float SlamLeft { get; init; } = 288f;
        public float SlamRight { get; init; } = 544f;
        public float ReturnSpeed { get; init; } = 260f;
        public float RiseSpeed { get; init; } = 220f;
        public float PlungeSpeed { get; init; } = 480f;
        public float ChargeSpeed { get; init; } = 380f;
        public float EnragedChargeSpeed { get; init; } = 460f;
        public double DestructionSeconds { get; init; } = 1.8;
        public float SinkSpeed { get; init; } = 40f;

        public Func<BossPose, Color, int, ISprite> CreateSprite { get; init; } =
            static (pose, tint, size) => EnemySpriteFactory.Instance.CreateBossSprite(pose, tint, size);
        public Func<ISprite> CreateProjectileSprite { get; init; } =
            static () => ProjectileSpriteFactory.Instance.CreateBossFireballSprite();
    }
}
