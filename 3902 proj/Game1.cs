using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Commands;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;

namespace _3902_proj;

public class Game1 : Game
{
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch = null!;
    private KeyboardController keyboardController = null!;
    private Player player = null!;
    private ChasingEnemy chasingEnemy = null!;
    private GameState gameState;
    private GameObjectCycler blockCycler = new GameObjectCycler();
    private GameObjectCycler itemCycler = new GameObjectCycler();
    private GameObjectCycler enemyCycler = new GameObjectCycler();

    public Game1()
    {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.Title = "Transformers - Press Enter to Start";
    }

    protected override void Initialize()
    {
        graphics.PreferredBackBufferWidth = 960;
        graphics.PreferredBackBufferHeight = 540;
        graphics.ApplyChanges();
        gameState = GameState.StartMenu;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        Texture2D rightSpriteSheet = Content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet");
        Texture2D leftSpriteSheet = Content.Load<Texture2D>("Sprites/transformer-platformer-spritesheet-left");
        Texture2D directionalSpriteSheet = Content.Load<Texture2D>("Sprites/transformer-directional-combat-spritesheet");
        PlaceholderSpriteFactory spriteFactory = new(rightSpriteSheet, leftSpriteSheet, directionalSpriteSheet);
        player = new Player(new Vector2(440, 250), spriteFactory);
        chasingEnemy = new ChasingEnemy(new Vector2(80, 80), spriteFactory);
        keyboardController = new KeyboardController(this, player);
        RegisterCycleCommands();
    }

    protected override void Update(GameTime gameTime)
    {
        keyboardController.Update();
        if (gameState == GameState.Gameplay)
        {
            blockCycler.Update(gameTime);
            itemCycler.Update(gameTime);
            enemyCycler.Update(gameTime);
            player.Update(gameTime);
            chasingEnemy.Chase(player.Position);
            chasingEnemy.Update(gameTime);
            CheckProjectileHits();
            KeepPlayerOnScreen();
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(gameState == GameState.StartMenu ? new Color(18, 24, 38) : new Color(42, 55, 70));
        if (gameState == GameState.Gameplay)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            blockCycler.Draw(spriteBatch);
            itemCycler.Draw(spriteBatch);
            enemyCycler.Draw(spriteBatch);
            chasingEnemy.Draw(spriteBatch);
            player.Draw(spriteBatch);
            spriteBatch.End();
        }
        base.Draw(gameTime);
    }

    public void StartGame()
    {
        gameState = GameState.Gameplay;
        Window.Title = "Transformers - WASD/Arrows Move, Space Transform, E Damage, T/Y Block, U/I Item, O/P Enemy, R Reset, Q Quit";
    }

    public void ResetGame()
    {
        player.Reset(new Vector2(440, 250));
        chasingEnemy.Reset(new Vector2(80, 80));
        blockCycler.Reset();
        itemCycler.Reset();
        enemyCycler.Reset();
        gameState = GameState.StartMenu;
        Window.Title = "Transformers - Press Enter to Start";
    }

    private void CheckProjectileHits()
    {
        if (chasingEnemy.IsDefeated) return;
        foreach (EnergyProjectile projectile in player.Projectiles)
        {
            if (!projectile.IsActive || !projectile.Bounds.Intersects(chasingEnemy.Bounds)) continue;
            projectile.Deactivate();
            chasingEnemy.TakeDamage(1);
        }
    }

    public bool IsGameplayActive => gameState == GameState.Gameplay;

    private void KeepPlayerOnScreen()
    {
        Viewport viewport = GraphicsDevice.Viewport;
        player.Position = Vector2.Clamp(player.Position, Vector2.Zero,
            new Vector2(viewport.Width - player.Width, viewport.Height - player.Height));
    }

    private void RegisterCycleCommands()
    {
        keyboardController.RegisterCommand(Keys.T, new PreviousObjectCommand(blockCycler));
        keyboardController.RegisterCommand(Keys.Y, new NextObjectCommand(blockCycler));
        keyboardController.RegisterCommand(Keys.U, new PreviousObjectCommand(itemCycler));
        keyboardController.RegisterCommand(Keys.I, new NextObjectCommand(itemCycler));
        keyboardController.RegisterCommand(Keys.O, new PreviousObjectCommand(enemyCycler));
        keyboardController.RegisterCommand(Keys.P, new NextObjectCommand(enemyCycler));
    }
}
