using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TransformersGame.Collisions;
using TransformersGame.Controllers;
using TransformersGame.Core;
using TransformersGame.Entities;
using TransformersGame.Factories;
using TransformersGame.Interfaces;
using TransformersGame.Sprites;
using TransformersGame.UI;

namespace TransformersGame
{
    internal class Game1 : Game, IPlayerHost
    {
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const string MenuTitle = "Transformers - Press Enter to Start";
        private const string GameplayTitle = "Transformers - A/D Move, W Jump, X Fire, Tab Map / Fireball, 7 Boomerang, 2 Bomb, 3/4 Items, T/Y U/I O/P Showcase, F1 Help, R Reset, Q Quit";

        private static readonly Color MenuColor = new Color(35, 22, 14);
        private static readonly Color GameplayColor = new Color(42, 55, 70);

        private readonly RoomFactory roomFactory = new RoomFactory(
            "Content/Levels/desert-01.room", "Content/Levels/laboratory-02.room",
            "Content/Levels/stone-03.room", "Content/Levels/metal-04.room",
            "Content/Levels/crates-05.room", "Content/Levels/hazards-06.room",
            "Content/Levels/fortress-07.room");

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameState gameState;
        private IController menuController;
        private IController gameplayController;
        private CollisionManager collisionManager;
        private RoomManager roomManager;
        private ControlsOverlay controlsOverlay;
        private PlayerHud playerHud;
        private readonly FireballSelection fireballs = new FireballSelection();
        private RoomMapScreen roomMapScreen;
        private IController mapController;
        private ObjectShowcase showcase;
        private IPlayer player;
        private int pendingDoorDirection;
        private readonly GameAudio audio = new GameAudio();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = MenuTitle;
        }

        // The outermost player (possibly wrapped in decorators). Replacing it rebinds the controls to the new wrapper.
        public IPlayer Player
        {
            get => player;
            set
            {
                player = value;
                RefreshGameplayController();
            }
        }

        public GameState State => gameState;

        public Room ActiveRoom => roomManager.ActiveRoom;

        public void ToggleHelp() => controlsOverlay.ToggleHelp();

        public void ToggleMusic() => audio.ToggleMusic(gameState is GameState.Gameplay or GameState.RoomMap);

        public void OpenMap()
        {
            if (gameState == GameState.Gameplay)
            {
                roomMapScreen.Open(roomManager.ActiveRoomIndex);
                mapController = ControllerFactory.CreateMapController(this);
                gameState = GameState.RoomMap;
            }
        }

        public void CloseMap()
        {
            gameState = GameState.Gameplay;
            RefreshGameplayController();
        }

        public void FireSelectedShot()
        {
            if (gameState == GameState.Gameplay)
            {
                KeyboardState keyboard = Keyboard.GetState();
                bool angled = keyboard.IsKeyDown(Keys.LeftShift) || keyboard.IsKeyDown(Keys.RightShift);
                fireballs.Fire(Player, angled);
            }
        }

        public void StartGame()
        {
            gameState = GameState.Gameplay;
            audio.StartMusic();
            Window.Title = GameplayTitle;
        }

        public void ResetGame()
        {
            audio.StopMusic();
            InitializeGameObjects();
            gameState = GameState.StartMenu;
            Window.Title = MenuTitle;
        }

        public void NextRoom() => SwitchRoom(roomManager.SwitchNext);

        public void PreviousRoom() => SwitchRoom(roomManager.SwitchPrevious);

        public void DamagePlayer()
        {
            if (Player is DamagedPlayer || !Player.TakeDamage())
            {
                return;
            }
            Player = new DamagedPlayer(Player, this);
            audio.Play(GameSound.Hurt);
        }

        public void RemovePlayerDecorator(PlayerDecorator decorator)
        {
            Player = PlayerDecorator.RemoveFromChain(Player, decorator);
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
            audio.Load(Content);
            SpriteAtlas.LoadAllTextures(Content);
            PlayerSpriteFactory.Instance.LoadAllTextures(Content);
            EnemySpriteFactory.Instance.LoadAllTextures(Content);
            ProjectileSpriteFactory.Instance.LoadAllTextures(Content);
            fireballs.LoadPreviews();
            BlockSpriteFactory.Instance.LoadAllTextures(Content);
            ItemSpriteFactory.Instance.LoadAllTextures(Content);
            SpriteFont uiFont = Content.Load<SpriteFont>("Fonts/Controls");

            controlsOverlay = new ControlsOverlay(uiFont, GraphicsDevice);

            Texture2D playerPortrait = Content.Load<Texture2D>(
                "Sprites/dilloSpriteTest2");

            playerHud = new PlayerHud(uiFont, playerPortrait);
            roomMapScreen = new RoomMapScreen(uiFont, fireballs);
            showcase = new ObjectShowcase();
            InitializeGameObjects();
        }

        protected override void Update(GameTime gameTime)
        {
            switch (gameState)
            {
                case GameState.Gameplay:
                    gameplayController.Update();
                    if (State == GameState.Gameplay)
                    {
                        UpdateGameplay(gameTime);
                    }
                    break;
                case GameState.RoomMap:
                    mapController.Update();
                    if (State == GameState.RoomMap)
                    {
                        roomMapScreen.UpdateNavigation(roomFactory);
                    }
                    break;
                default:
                    menuController.Update();
                    break;
            }
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roomManager?.Unload();
                audio.Dispose();
                controlsOverlay?.Dispose();
                controlsOverlay = null;
                spriteBatch?.Dispose();
                spriteBatch = null;
                graphics?.Dispose();
                graphics = null;
            }
            base.Dispose(disposing);
        }

        protected override void Draw(GameTime gameTime)
        {
            if (gameState == GameState.RoomMap)
            {
                GraphicsDevice.Clear(MenuColor);
                roomMapScreen.Draw(spriteBatch, roomManager, Player, GameplayColor);
            }
            else
            {
                bool playing = gameState == GameState.Gameplay;
                GraphicsDevice.Clear(playing ? GameplayColor : MenuColor);
                spriteBatch.Begin(samplerState: SamplerState.PointClamp);
                if (playing)
                {
                    DrawGameplay();
                }
                else
                {
                    controlsOverlay.DrawMenu(spriteBatch, GraphicsDevice.Viewport, gameState, Player);
                }
                spriteBatch.End();
            }
            base.Draw(gameTime);
        }

        private void DrawGameplay()
        {
            Room room = roomManager.ActiveRoom;
            room.Draw(spriteBatch);
            Player.Draw(spriteBatch);
            string roomStatus = $"{roomManager.ActiveRoomIndex + 1}/{roomManager.RoomCount}";
            playerHud.Draw(spriteBatch, Player, room.Level.Name, roomStatus, fireballs);
            controlsOverlay.DrawSmallText(spriteBatch,
                audio.MusicMuted ? "M: Music OFF" : "M: Music ON", 820, 86, false);
            if (roomManager.ActiveRoomIndex == 0 && room.Level.HasIntactReinforcedCrate)
            {
                controlsOverlay.DrawSmallText(spriteBatch,
                    "SPACE to transform. Roll into the blue crate.", 26, 98, false);
            }
            showcase.Draw(spriteBatch, controlsOverlay);
            controlsOverlay.DrawGameplayHelp(spriteBatch, Player, room.Objective);
        }

        private void InitializeGameObjects()
        {
            roomManager?.Unload();
            roomManager = null;
            roomFactory.StartNewGame();
            fireballs.Select(0);
            Level initialLevel = roomFactory.LoadLevel(0);
            IProjectileManager initialProjectiles = new ProjectileManager();
            collisionManager = new CollisionManager(ScreenWidth, ScreenHeight, DamagePlayer,
                direction => pendingDoorDirection = direction);
            player = new Player(initialLevel.PlayerStart, initialLevel.Blocks, initialProjectiles,
                initialLevel.Gravity, collisionManager);
            ((Player)player).SoundRequested += audio.Play;
            Room initialRoom = RoomFactory.CreateRoom(initialLevel, player, initialProjectiles, collisionManager);
            roomManager = new RoomManager(initialRoom, roomFactory.RoomCount, player,
                index => RoomFactory.CreateRoom(roomFactory.LoadLevel(index), Player,
                    new ProjectileManager(), collisionManager));
            pendingDoorDirection = 0;
            menuController = ControllerFactory.CreateMenuController(this);
            RefreshGameplayController();
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            Room activeRoom = roomManager.ActiveRoom;
            activeRoom.Update(gameTime);
            Player.Update(gameTime);
            KeepPlayerOnScreen();
            ApplyPowerUps(activeRoom.CollectItems(Player));
            showcase.Update(gameTime);
            collisionManager.BuildTree(activeRoom.GetCollisionObjects(Player));
            collisionManager.DetectCollision();
            if (pendingDoorDirection != 0 && Player.Health > 0)
            {
                int direction = pendingDoorDirection;
                pendingDoorDirection = 0;
                if (direction > 0) NextRoom(); else PreviousRoom();
            }
            CheckForGameEnd();
        }

        private void ApplyPowerUps(IReadOnlyList<IItem> collected)
        {
            if (collected.Count > 0) audio.Play(GameSound.Pickup);
            foreach (IItem item in collected)
            {
                if (item is IPowerUp powerUp)
                {
                    Player = powerUp.Apply(Player, this);
                }
            }
        }

        private void CheckForGameEnd()
        {
            if (Player.Health <= 0)
            {
                EndGame(GameState.GameOver, "Game Over - R to reset, Q to quit");
            }
            else if (roomManager.ActiveRoom.IsGoalReached)
            {
                EndGame(GameState.Victory, "Mission Complete - R to play again, Q to quit");
            }
        }

        private void EndGame(GameState endState, string title)
        {
            audio.StopMusic();
            audio.Play(endState == GameState.Victory ? GameSound.Victory : GameSound.Defeat);
            gameState = endState;
            Window.Title = title;
            menuController = ControllerFactory.CreateEndScreenController(this);
        }

        private void SwitchRoom(System.Action switchRoom)
        {
            switchRoom();
            collisionManager.BuildTree(roomManager.ActiveRoom.GetCollisionObjects(Player));
            RefreshGameplayController();
        }

        private void RefreshGameplayController()
        {
            if (roomManager?.ActiveRoom == null)
            {
                return;
            }
            gameplayController = ControllerFactory.CreateGameplayController(
                this, Player, showcase.Blocks, showcase.Items, showcase.Enemies);
        }

        private void KeepPlayerOnScreen()
        {
            Viewport viewport = GraphicsDevice.Viewport;
            Vector2 maximum = new Vector2(viewport.Width - Player.Width, viewport.Height - Player.Height);
            Player.Position = Vector2.Clamp(Player.Position, Vector2.Zero, maximum);
        }
    }
}
