using Microsoft.Xna.Framework;
using TransformersGame.Core;
namespace TransformersGame.Interfaces;
public interface IPlayer : IGameObject
{
    int Width { get; }
    int Height { get; }
    Direction Facing { get; }
    void Move(Direction direction);
    void StopMoving();
    void Transform();
    void TakeDamage();
    void Reset(Vector2 position);
}
