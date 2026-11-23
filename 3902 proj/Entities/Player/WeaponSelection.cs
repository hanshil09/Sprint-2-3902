using System;
using Microsoft.Xna.Framework;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// The four selectable weapons. Slot 1-4 selects; the left mouse button fires through each weapon's
// own fire behavior (fire-once, autofire, or charge-and-release).
internal sealed class WeaponSelection
{
    private static readonly Slot[] Slots =
    {
        new(WeaponKind.Boomerang, "Boomerang", f => f.CreateBoomerangSprite()),
        new(WeaponKind.Pistol, "Pistol", f => f.CreateShotSprite(false)),
        new(WeaponKind.Shotgun, "Shotgun", f => f.CreateSpreadPelletSprite()),
        new(WeaponKind.Molotov, "Molotov", f => f.CreateBombSprite())
    };

    private readonly IWeaponFireBehavior[] behaviors =
    {
        new InstantFireBehavior(WeaponKind.Boomerang),
        new InstantFireBehavior(WeaponKind.Pistol),
        new ShotgunFireBehavior(),
        new MolotovFireBehavior()
    };

    private ISprite[] previews;
    private FirePhase pendingPhase = FirePhase.Up;

    public int SelectedIndex { get; private set; }
    public static int Count => Slots.Length;
    public WeaponKind SelectedKind => Slots[SelectedIndex].Kind;
    public string SelectedName => NameAt(SelectedIndex);
    public ISprite SelectedPreview => PreviewAt(SelectedIndex);

    public static string NameAt(int index) => Slots[index].Name;
    public static WeaponKind KindAt(int index) => Slots[index].Kind;
    public ISprite PreviewAt(int index) => previews[index];

    private IWeaponFireBehavior SelectedBehavior => behaviors[SelectedIndex];

    // The molotov's current charge (0..1) when it is the equipped, charging weapon; else 0.
    // Used by the aim preview to size the arc.
    public float MolotovCharge =>
        SelectedBehavior is MolotovFireBehavior molotov ? molotov.Charge : 0f;

    public void LoadPreviews()
    {
        ProjectileSpriteFactory factory = ProjectileSpriteFactory.Instance;
        previews = Array.ConvertAll(Slots, slot => slot.CreatePreview(factory));
    }

    // Equips a slot only when its weapon is unlocked; switching cancels any in-progress fire.
    public bool Select(int index, IPlayer player)
    {
        if (index < 0 || index >= Slots.Length || !player.IsWeaponUnlocked(Slots[index].Kind))
        {
            return false;
        }
        if (index != SelectedIndex)
        {
            SelectedBehavior.Reset();
            pendingPhase = FirePhase.Up;
        }
        SelectedIndex = index;
        return true;
    }

    // Records the fire button phase for this frame; acted on in Update.
    public void SetFirePhase(FirePhase phase) => pendingPhase = phase;

    // Advances the equipped weapon's fire behavior with the latest phase, then clears edge phases so
    // a press or release only registers once.
    public void Update(IPlayer player, GameTime gameTime)
    {
        SelectedBehavior.Update(player, pendingPhase, gameTime);
        if (pendingPhase == FirePhase.Pressed)
        {
            pendingPhase = FirePhase.Held;
        }
        else if (pendingPhase == FirePhase.Released)
        {
            pendingPhase = FirePhase.Up;
        }
    }

    private sealed record Slot(WeaponKind Kind, string Name, Func<ProjectileSpriteFactory, ISprite> CreatePreview);
}
