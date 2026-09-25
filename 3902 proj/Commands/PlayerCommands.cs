using TransformersGame.Core;
using TransformersGame.Interfaces;
namespace TransformersGame.Commands;
public sealed class MovePlayerCommand(IPlayer player, Direction direction) : ICommand { public void Execute() => player.Move(direction); }
public sealed class TransformPlayerCommand(IPlayer player) : ICommand { public void Execute() => player.Transform(); }
public sealed class DamagePlayerCommand(IPlayer player) : ICommand { public void Execute() => player.TakeDamage(); }
