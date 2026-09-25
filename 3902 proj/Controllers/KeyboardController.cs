using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using _3902_proj;
using TransformersGame.Commands;
using TransformersGame.Core;
using TransformersGame.Interfaces;
namespace TransformersGame.Controllers;
public sealed class KeyboardController : IController
{
    private readonly Game1 game;
    private readonly IPlayer player;
    private readonly Dictionary<Keys, ICommand> heldCommands;
    private readonly Dictionary<Keys, ICommand> pressedCommands;
    private KeyboardState previousState;
    public KeyboardController(Game1 game, IPlayer player)
    {
        this.game = game;
        this.player = player;
        heldCommands = new Dictionary<Keys, ICommand>
        {
            [Keys.W] = new MovePlayerCommand(player, Direction.Up), [Keys.Up] = new MovePlayerCommand(player, Direction.Up),
            [Keys.S] = new MovePlayerCommand(player, Direction.Down), [Keys.Down] = new MovePlayerCommand(player, Direction.Down),
            [Keys.A] = new MovePlayerCommand(player, Direction.Left), [Keys.Left] = new MovePlayerCommand(player, Direction.Left),
            [Keys.D] = new MovePlayerCommand(player, Direction.Right), [Keys.Right] = new MovePlayerCommand(player, Direction.Right)
        };
        pressedCommands = new Dictionary<Keys, ICommand>
        {
            [Keys.Enter] = new StartGameCommand(game), [Keys.Space] = new TransformPlayerCommand(player),
            [Keys.E] = new DamagePlayerCommand(player), [Keys.R] = new ResetGameCommand(game),
            [Keys.Q] = new QuitCommand(game), [Keys.Escape] = new QuitCommand(game)
        };
    }
    public void Update()
    {
        KeyboardState currentState = Keyboard.GetState();
        bool moved = false;
        if (game.IsGameplayActive)
        {
            foreach ((Keys key, ICommand command) in heldCommands)
            {
                if (currentState.IsKeyDown(key)) { command.Execute(); moved = true; }
            }
        }
        if (!moved) player.StopMoving();
        foreach ((Keys key, ICommand command) in pressedCommands)
        {
            if (currentState.IsKeyDown(key) && previousState.IsKeyUp(key)) command.Execute();
        }
        previousState = currentState;
    }
}
