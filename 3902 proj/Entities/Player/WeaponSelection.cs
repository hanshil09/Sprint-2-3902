using System;
using ArmadilloGame.Core;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;

namespace ArmadilloGame.Entities;

// The four selectable weapons. Slot 1-4 selects, click fires.
// Boomerang is unlocked by default; the rest must be unlocked.
internal sealed class WeaponSelection
{
    private static readonly Slot[] Slots =
    {
        new(WeaponKind.Boomerang, "Boomerang", f => f.CreateBoomerangSprite()),
        new(WeaponKind.Pistol, "Pistol", f => f.CreateShotSprite(false)),
        new(WeaponKind.Shotgun, "Shotgun", f => f.CreateSpreadPelletSprite()),
        new(WeaponKind.Molotov, "Molotov", f => f.CreateBombSprite())
    };

    private ISprite[] previews;

    public int SelectedIndex { get; private set; }
    public static int Count => Slots.Length;
    public WeaponKind SelectedKind => Slots[SelectedIndex].Kind;
    public string SelectedName => NameAt(SelectedIndex);
    public ISprite SelectedPreview => PreviewAt(SelectedIndex);

    public static string NameAt(int index) => Slots[index].Name;
    public static WeaponKind KindAt(int index) => Slots[index].Kind;
    public ISprite PreviewAt(int index) => previews[index];

    public void LoadPreviews()
    {
        ProjectileSpriteFactory factory = ProjectileSpriteFactory.Instance;
        previews = Array.ConvertAll(Slots, slot => slot.CreatePreview(factory));
    }

    // Equips a slot only when its weapon is unlocked.
    public bool Select(int index, IPlayer player)
    {
        if (index < 0 || index >= Slots.Length || !player.IsWeaponUnlocked(Slots[index].Kind))
        {
            return false;
        }
        SelectedIndex = index;
        return true;
    }

    public void Fire(IPlayer player) => player.FireWeapon(SelectedKind);

    private sealed record Slot(WeaponKind Kind, string Name, Func<ProjectileSpriteFactory, ISprite> CreatePreview);
}
