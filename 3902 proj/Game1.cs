using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.UI;

namespace TransformersGame
{
    public class Game1 : Game
    {
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const string MenuTitle = "Transformers - Press Enter to Start";
        private const string GameplayTitle = "Transformers - A/D Move, W/Up/J Jump, S Face, Space Transform, Z/N Shoot, 1/2 Items, E Damage, T/Y Block, U/I Item, O/P Enemy, R Reset, Q Quit";

        private static readonly Vector2 PlayerStartPosition = new Vector2(80, 400);
        private static readonly Color MenuColor = new Color(18, 24, 38);
        private static readonly Color GameplayColor = new Color(42, 55, 70);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameState gameState;
        private IController menuController;
        private IController gameplayController;
        private Level level;
        private ProjectileManager projectiles;
        private GameObjectCycler blockCycler;
        private GameObjectCycler itemCycler;
        private GameObjectCycler enemyCycler;
        private ControlsOverlay controlsOverlay;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = MenuTitle;
        }

        public IPlayer Player { get; set; }

        public void StartGame()
        {
            gameState = GameState.Gameplay;
            Window.Title = GameplayTitle;
        }

        public void ResetGame()
        {
            InitializeGameObjects();
            gameState = GameState.StartMenu;
            Window.Title = MenuTitle;
        }

        public void DamagePlayer()
        {
            if (Player is DamagedPlayer)
            {
                return;
            }
            Player.TakeDamage();
            Player = new DamagedPlayer(Player, this);
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = ScreenWidth;
            graphics.PreferredBackBufferHeight = ScreenHeight;
            graphics.ApplyChanges();
            gameState = GameState.StartMenu;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            PlayerSpriteFactory.Instance.LoadAllTextures(Content);
            EnemySpriteFactory.Instance.LoadAllTextures(Content);
            ProjectileSpriteFactory.Instance.LoadAllTextures(Content);
            BlockSpriteFactory.Instance.LoadAllTextures(Content);
            controlsOverlay = new ControlsOverlay(Content.Load<SpriteFont>("Fonts/Controls"), GraphicsDevice);
            InitializeGameObjects();
        }

        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameState.StartMenu)
            {
                menuController.Update();
            }
            else
            {
                gameplayController.Update();
                UpdateGameplay(gameTime);
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (gameState == GameState.StartMenu)
            {
                GraphicsDevice.Clear(MenuColor);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                controlsOverlay.DrawMenu(spriteBatch, GraphicsDevice.Viewport);
                spriteBatch.End();
            }
            else
            {
                GraphicsDevice.Clear(GameplayColor);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                level.Draw(spriteBatch);
                blockCycler.Draw(spriteBatch);
                itemCycler.Draw(spriteBatch);
                enemyCycler.Draw(spriteBatch);
                projectiles.Draw(spriteBatch);
                Player.Draw(spriteBatch);
                controlsOverlay.DrawGameplay(spriteBatch);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void InitializeGameObjects()
        {
            level = new Level();
            projectiles = new ProjectileManager();
            Player = new Player(PlayerStartPosition, level.Blocks, projectiles);
            blockCycler = new GameObjectCycler();
            itemCycler = new GameObjectCycler();
            enemyCycler = new GameObjectCycler();
            Level.FillBlockCycler(blockCycler);
            level.FillEnemyCycler(enemyCycler);
            menuController = ControllerFactory.CreateMenuController(this);
            gameplayController = ControllerFactory.CreateGameplayController(this, Player, blockCycler, itemCycler, enemyCycler);
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            level.Update(gameTime);
            blockCycler.Update(gameTime);
            itemCycler.Update(gameTime);
            enemyCycler.Update(gameTime);
            Player.Update(gameTime);
            projectiles.Update(gameTime);
            KeepPlayerOnScreen();
        }

        private void KeepPlayerOnScreen()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Vector2 maximum = new Vector2(viewport.Width - Player.Width, viewport.Height - Player.Height);
            Player.Position = Vector2.Clamp(Player.Position, Vector2.Zero, maximum);
        }
    }
}
