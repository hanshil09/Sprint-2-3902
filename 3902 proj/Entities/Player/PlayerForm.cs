using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Collisions;
using TransformersGame.Interfaces;
using TransformersGame.States;

namespace TransformersGame.Entities;

internal sealed class PlayerForm
{
    private readonly Player player;
    private readonly CollisionManager collisions;
    private IPlayerState state;
    private ISprite sprite;

    public PlayerForm(Player player, CollisionManager collisions)
    {
        this.player = player;
        this.collisions = collisions;
        state = new RobotState(player);
        RefreshSprite();
    }

    public bool IsVehicle => state is VehicleState;
    public int Width => sprite.Width;
    public int Height => sprite.Height;
    public float Speed => state.Speed;

    public bool TryTransform()
    {
        IPlayerState previous = state;
        state.Transform();
        return state != previous;
    }

    public void SetState(IPlayerState newState)
    {
        int oldHeight = Height;
        if (IsVehicle && newState is RobotState)
        {
            ISprite robotSprite = newState.CreateSprite();
            Rectangle bounds = new Rectangle((int)player.Position.X,
                (int)(player.Position.Y + oldHeight - robotSprite.Height), robotSprite.Width, robotSprite.Height);
            if (!collisions.IsSpaceAvailable(player, bounds))
            {
                return;
            }
        }

        state = newState;
        RefreshSprite();
        player.Position = new Vector2(player.Position.X, player.Position.Y + oldHeight - Height);
    }

    public void Shoot(bool angled) => state.Shoot(angled);
    public void UseItem(int itemNumber, bool angled = false) => state.UseItem(itemNumber, angled);
    public void RefreshSprite() => sprite = state.CreateSprite();

    public void Update(GameTime gameTime)
    {
        state.Update(gameTime);
        sprite.Update(gameTime);
    }

    public void Draw(SpriteBatch spriteBatch) => sprite.Draw(spriteBatch, player.Position);
}
