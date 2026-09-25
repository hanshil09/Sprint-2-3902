using _3902_proj;
using TransformersGame.Interfaces;
namespace TransformersGame.Commands;
public sealed class QuitCommand(Game1 game) : ICommand { public void Execute() => game.Exit(); }
public sealed class StartGameCommand(Game1 game) : ICommand { public void Execute() => game.StartGame(); }
public sealed class ResetGameCommand(Game1 game) : ICommand { public void Execute() => game.ResetGame(); }
