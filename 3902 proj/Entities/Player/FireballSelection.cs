using System;
using TransformersGame.Interfaces;
using TransformersGame.Factories;

namespace TransformersGame.Entities;

internal sealed class FireballSelection
{
    private const int OrbItem = 1;
    private const int PiercingItem = 5;
    private const int SpreadItem = 6;
    public const int IceItem = 8;

    private static readonly Shot[] Shots =
    {
        new("Standard", (player, angled) => player.Shoot(angled), factory => factory.CreateShotSprite(false)),
        new("Orb", (player, angled) => player.UseItem(OrbItem, angled), factory => factory.CreateOrbSprite(false)),
        new("Piercing", (player, angled) => player.UseItem(PiercingItem, angled), factory => factory.CreateOrbSprite(false)),
        new("Spread", (player, _) => player.UseItem(SpreadItem), factory => factory.CreateSpreadPelletSprite()),
        new("Ice", (player, angled) => player.UseItem(IceItem, angled), factory => factory.CreateIceSprite(false))
    };

    private ISprite[] previews;

    public ISprite SelectedPreview => PreviewAt(SelectedIndex);

    public ISprite PreviewAt(int index) => previews[index];

    public void LoadPreviews()
    {
        ProjectileSpriteFactory factory = ProjectileSpriteFactory.Instance;
        previews = Array.ConvertAll(Shots, shot => shot.CreatePreview(factory));
    }

    public int SelectedIndex { get; private set; }
    public static int Count => Shots.Length;
    public string SelectedName => NameAt(SelectedIndex);

    public static string NameAt(int index) => Shots[index].Name;
    public void Select(int index) => SelectedIndex = index;
    public void Fire(IPlayer player, bool angled) => Shots[SelectedIndex].Fire(player, angled);

    private sealed record Shot(string Name, Action<IPlayer, bool> Fire, Func<ProjectileSpriteFactory, ISprite> CreatePreview);
}
