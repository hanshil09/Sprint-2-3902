using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.Collisions;
using TransformersGame.UI;

namespace TransformersGame
{
    public class Game1 : Game
    {
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private static readonly string[] RoomPaths = { "Content/Levels/desert-01.room", "Content/Levels/laboratory-02.room" };
        private const string MenuTitle = "Transformers - Press Enter to Start";
        private const string GameplayTitle = "Transformers - A/D Move, W/Up/J Jump, Left/Right Click Room, S Face, Space Transform, Z/N Shoot, K Angle Shoot, 1/2 Items, E Damage, T/Y Block, U/I Item, O/P Enemy, R Reset, Q Quit";

        private static readonly Color MenuColor = new Color(18, 24, 38);
        private static readonly Color GameplayColor = new Color(42, 55, 70);

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameState gameState;
        private IController menuController;
        private IController gameplayController;
        private CollisionManager collisionManager;
        private RoomManager roomManager;
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

        public void NextRoom()
        {
            roomManager.SwitchNext();
            collisionManager.BuildTree(roomManager.ActiveRoom.GetCollisionObjects(Player));
            RefreshGameplayController();
        }

        public void PreviousRoom()
        {
            roomManager.SwitchPrevious();
            collisionManager.BuildTree(roomManager.ActiveRoom.GetCollisionObjects(Player));
            RefreshGameplayController();
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
            ItemSpriteFactory.Instance.LoadAllTextures(Content);
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

        protected override void UnloadContent()
        {
            controlsOverlay?.Dispose();
            base.UnloadContent();
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
                roomManager.ActiveRoom.Draw(spriteBatch);
                Player.Draw(spriteBatch);
                controlsOverlay.DrawGameplay(spriteBatch);
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void InitializeGameObjects()
        {
            roomManager?.Unload();
            Level initialLevel = LoadRoomLevel(0);
            IProjectileManager initialProjectiles = new ProjectileManager();
            Player = new Player(initialLevel.PlayerStart, initialLevel.Blocks, initialProjectiles);
            Room initialRoom = CreateRoom(initialLevel, initialProjectiles);
            roomManager = new RoomManager(initialRoom, RoomPaths.Length, Player, CreateRoomForIndex);
            collisionManager = new CollisionManager(ScreenWidth, ScreenHeight, DamagePlayer);
            menuController = ControllerFactory.CreateMenuController(this);
            RefreshGameplayController();
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            Room activeRoom = roomManager.ActiveRoom;
            activeRoom.Update(gameTime);
            Player.Update(gameTime);
            KeepPlayerOnScreen();
            activeRoom.CollectItems(Player);
            collisionManager.BuildTree(activeRoom.GetCollisionObjects(Player));
            collisionManager.DetectCollision();
        }

        private Room CreateRoomForIndex(int roomIndex)
        {
            return CreateRoom(LoadRoomLevel(roomIndex), new ProjectileManager());
        }

        private static Level LoadRoomLevel(int roomIndex)
        {
            LevelLoadResult loaded = LevelLoader.Load(RoomPaths[roomIndex]);
            foreach (string message in loaded.Messages)
            {
                Console.WriteLine(message);
            }
            return new Level(loaded.Data);
        }

        private Room CreateRoom(Level roomLevel, IProjectileManager roomProjectiles)
        {
            GameObjectCycler blocks = new GameObjectCycler();
            GameObjectCycler items = new GameObjectCycler();
            GameObjectCycler enemies = new GameObjectCycler();
            roomLevel.FillBlockCycler(blocks);
            roomLevel.FillItemCycler(items);
            roomLevel.FillEnemyCycler(enemies, Player, roomProjectiles);
            return new Room(roomLevel, roomProjectiles, blocks, items, enemies);
        }

        private void RefreshGameplayController()
        {
            Room activeRoom = roomManager.ActiveRoom;
            gameplayController = ControllerFactory.CreateGameplayController(
                this, Player, activeRoom.Blocks, activeRoom.Items, activeRoom.Enemies);
        }

        private void KeepPlayerOnScreen()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Vector2 maximum = new Vector2(viewport.Width - Player.Width, viewport.Height - Player.Height);
            Player.Position = Vector2.Clamp(Player.Position, Vector2.Zero, maximum);
        }
    }
}
