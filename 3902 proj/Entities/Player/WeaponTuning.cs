using Microsoft.Xna.Framework;
using ArmadilloGame.Core;

namespace ArmadilloGame.Entities;

// Shared aim/throw values so the actual shot and the on-screen aim preview stay in sync.
// Keeping them in one place avoids the preview and the arsenal drifting apart.
internal static class WeaponTuning
{
    // How far the straight-aim preview line reaches for each hitscan-style weapon.
    public const float PistolPreviewLength = 320f;
    public const float ShotgunRange = 260f;
    public const float BoomerangPreviewLength = 300f;

    // Molotov throw speed scales with charge; the preview arc uses the same mapping.
    public const float MolotovMinSpeed = 320f;
    public const float MolotovMaxSpeed = 820f;

    public static float MolotovSpeed(float charge) =>
        MathHelper.Lerp(MolotovMinSpeed, MolotovMaxSpeed, MathHelper.Clamp(charge, 0f, 1f));
}
