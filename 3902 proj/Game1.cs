
using System;
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
using TransformersGame.States;
using TransformersGame.UI;

namespace TransformersGame
{
    internal class Game1 : Game, IPlayerHost
    {
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const string MenuTitle = "Transformers - Press Enter to Start";
        private const string GameplayTitle = "Transformers - A/D Move, W Jump, X Fire, Tab Map / Fireball, B Barrel Storm, 7 Boomerang, 2 Bomb, 3/4 Items, R Reset, Q Quit";

        private static readonly Color MenuColor = new Color(35, 22, 14);
        private static readonly Color GameplayColor = new Color(42, 55, 70);

        private readonly RoomFactory roomFactory = new RoomFactory(
            "Content/Levels/hub-00.room",
            "Content/Levels/desert-01.room", "Content/Levels/laboratory-02.room",
            "Content/Levels/stone-03.room", "Content/Levels/metal-04.room",
            "Content/Levels/crates-05.room", "Content/Levels/hazards-06.room",
            "Content/Levels/fortress-07.room");

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private GameState gameState;
        private IGameState currentState;
        private GameplayState gameplayState;
        private MenuState menuState;
        private IController gameplayController;
        private CollisionManager collisionManager;
        private RoomManager roomManager;
        private ControlsOverlay controlsOverlay;
        private PlayerHud playerHud;
        private readonly FireballSelection fireballs = new FireballSelection();
        private MapState mapState;
        private IController selfieShortcut;
        private SelfieState selfieState;
        private GalleryState galleryState;
        private bool selfieNeedsSpace;
        private ObjectShowcase showcase;
        private Camera2D camera;
        private IPlayer player;
        private Door pendingDoor;

        // Targeted locked doors the player has already opened.
        private readonly System.Collections.Generic.HashSet<(int From, int To)> unlockedDoors = new();

        private readonly GameAudio audio = new GameAudio();
        private BarrelStorm barrelStorm;
        private BazookaAbility bazooka;

        // Repeating gameplay background texture.
        private Texture2D backgroundTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.Title = MenuTitle;
        }

        // The outermost player (possibly wrapped in decorators).
        // Replacing it rebinds the controls to the new wrapper.
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

        public void ToggleMute() => audio.ToggleMute();

        public void OpenSelfie()
        {
            if (gameState != GameState.Gameplay) return;

            selfieNeedsSpace = !selfieState.Open(
                ActiveRoom,
                Player,
                camera.GetViewMatrix());

            if (selfieNeedsSpace) return;

            SetState(GameState.Selfie, selfieState);
        }

        public void CloseSelfie()
        {
            SetState(GameState.Gameplay, gameplayState);
            RefreshGameplayController();
        }

        public void OpenGallery()
        {
            if (gameState != GameState.Gameplay) return;

            galleryState.Open();
            SetState(GameState.Gallery, galleryState);
        }

        public void CloseGallery()
        {
            SetState(GameState.Gameplay, gameplayState);
            RefreshGameplayController();
        }

        public void OpenMap()
        {
            if (gameState == GameState.Gameplay)
            {
                mapState.Open(roomManager.ActiveRoomIndex);
                SetState(GameState.RoomMap, mapState);
            }
        }

        public void CloseMap()
        {
            SetState(GameState.Gameplay, gameplayState);
            RefreshGameplayController();
        }

        public void FireSelectedShot()
        {
            if (gameState == GameState.Gameplay)
            {
                KeyboardState keyboard = Keyboard.GetState();

                bool angled =
                    keyboard.IsKeyDown(Keys.LeftShift) ||
                    keyboard.IsKeyDown(Keys.RightShift);

                fireballs.Fire(Player, angled);
            }
        }

        public void CallBazooka()
        {
            if (gameState == GameState.Gameplay)
            {
                bazooka.Call(
                    Player,
                    camera.ViewBounds,
                    ActiveRoom.Level.Blocks);
            }
        }

        public void FireBazooka()
        {
            if (gameState == GameState.Gameplay)
            {
                bazooka.Fire(Player);
            }
        }

        public void StartGame()
        {
            SetState(GameState.Gameplay, gameplayState);
            audio.StartMusic();
            Window.Title = GameplayTitle;
        }

        public void ResetGame()
        {
            audio.StopMusic();
            barrelStorm?.Reset();
            InitializeGameObjects();
            SetState(GameState.StartMenu, menuState);
            Window.Title = MenuTitle;
        }

        public void NextRoom() => SwitchRoom(roomManager.SwitchNext);

        public void PreviousRoom() => SwitchRoom(roomManager.SwitchPrevious);

        public void GoToRoom(int roomIndex) =>
            SwitchRoom(() => roomManager.SwitchTo(roomIndex));

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

            // Load the repeating background.
            backgroundTexture = Content.Load<Texture2D>("Sprites/Brown");

            bazooka = new BazookaAbility(
                Content.Load<Texture2D>("Sprites/bazooka-helper"),
                Content.Load<Texture2D>("Sprites/bazooka-weapon"));

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

            menuState = new MenuState(
                controlsOverlay,
                GraphicsDevice,
                StartGame,
                ResetGame,
                ToggleMute,
                Exit,
                () => State,
                () => Player,
                MenuColor);

            selfieState = new SelfieState(
                new SelfieScreen(
                    GraphicsDevice,
                    new SelfieSpriteFactory(Content),
                    controlsOverlay,
                    ScreenWidth,
                    ScreenHeight),
                CloseSelfie,
                ToggleMute,
                Exit,
                audio.PlayShutter,
                () => ActiveRoom,
                GameplayColor);

            galleryState = new GalleryState(
                GraphicsDevice,
                uiFont,
                CloseGallery,
                ToggleMute,
                MenuColor);

            Texture2D playerPortrait = Content.Load<Texture2D>(
                "Sprites/dilloSpriteTest2");

            playerHud = new PlayerHud(uiFont, playerPortrait);

            mapState = new MapState(
                GraphicsDevice,
                uiFont,
                fireballs,
                roomFactory,
                CloseMap,
                ToggleMute,
                () => roomManager,
                () => Player,
                MenuColor,
                GameplayColor);

            gameplayState = new GameplayState(
                UpdateActiveGameplay,
                DrawActiveGameplay);

            showcase = new ObjectShowcase();

            barrelStorm?.Dispose();

            barrelStorm = new BarrelStorm(
                GraphicsDevice,
                audio.Play,
                Content.Load<Texture2D>("Sprites/additionalItemSprites"));

            InitializeGameObjects();

            SetState(GameState.StartMenu, menuState);
        }

        protected override void Update(GameTime gameTime)
        {
            currentState.Update(gameTime);
            base.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roomManager?.Unload();

                selfieState?.Dispose();
                selfieState = null;

                galleryState?.Dispose();
                galleryState = null;

                barrelStorm?.Dispose();
                barrelStorm = null;

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
            currentState.Draw(spriteBatch);
            base.Draw(gameTime);
        }

        private void SetState(GameState state, IGameState screen)
        {
            gameState = state;
            currentState = screen;
        }

        private void DrawActiveGameplay()
        {
            GraphicsDevice.Clear(GameplayColor);
            DrawGameplay();
        }

        private void DrawGameplay()
        {
            Room room = roomManager.ActiveRoom;

            // World pass: scrolls with the camera.
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: camera.GetViewMatrix());

            // Draw the repeating background first.
            DrawTiledBackground(camera.ViewBounds, GetRoomBackgroundTint());

            // Draw the world on top of the background.
            room.Draw(spriteBatch);
            barrelStorm.DrawParticles(spriteBatch);
            Player.Draw(spriteBatch);
            bazooka.Draw(spriteBatch, Player);

            spriteBatch.End();

            // UI pass: fixed to the screen, drawn without the camera transform.
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            barrelStorm.DrawOverlay(spriteBatch);
            DrawHud(room);

            spriteBatch.End();
        }

        private void DrawTiledBackground(Rectangle viewBounds, Color tint)
        {
            const int tileSize = 8;

            int startX = (int)Math.Floor(viewBounds.Left / (double)tileSize) * tileSize;
            int startY = (int)Math.Floor(viewBounds.Top / (double)tileSize) * tileSize;

            for (int y = startY; y < viewBounds.Bottom + tileSize; y += tileSize)
            {
                for (int x = startX; x < viewBounds.Right + tileSize; x += tileSize)
                {
                    spriteBatch.Draw(
                        backgroundTexture,
                        new Rectangle(x, y, tileSize, tileSize),
                        tint
                    );
                }
            }
        }

        private void DrawHud(Room room)
        {
            string roomStatus =
                $"{roomManager.ActiveRoomIndex + 1}/{roomManager.RoomCount}";

            playerHud.Draw(
                spriteBatch,
                Player,
                room.Level.Name,
                roomStatus,
                fireballs);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                selfieNeedsSpace ? "F2: Need space" : "F2: Selfie",
                728,
                86,
                false);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                audio.IsMuted ? "M: Audio OFF" : "M: Audio ON",
                820,
                86,
                false);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                "F3: Gallery",
                728,
                100,
                false);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                bazooka.Status(Player),
                440,
                100,
                true);

            if (roomManager.ActiveRoomIndex == 0 &&
                room.Level.HasIntactReinforcedCrate)
            {
                controlsOverlay.DrawSmallText(
                    spriteBatch,
                    "SPACE to transform. Roll into the blue crate.",
                    26,
                    98,
                    false);
            }

            showcase.Draw(spriteBatch, controlsOverlay);

            controlsOverlay.DrawGameplayHelp(
                spriteBatch,
                Player,
                room.Objective);
        }

        private void InitializeGameObjects()
        {
            bazooka.Reset();
            selfieNeedsSpace = false;

            roomManager?.Unload();
            roomManager = null;

            roomFactory.StartNewGame();
            fireballs.Select(0);

            Level initialLevel = roomFactory.LoadLevel(0);

            IProjectileManager initialProjectiles = new ProjectileManager();

            collisionManager = new CollisionManager(
                ScreenWidth,
                ScreenHeight,
                DamagePlayer,
                door => pendingDoor = door);

            player = new Player(
                initialLevel.PlayerStart,
                initialLevel.Blocks,
                initialProjectiles,
                initialLevel.Gravity,
                collisionManager);

            ((Player)player).SoundRequested += audio.Play;

            Room initialRoom = RoomFactory.CreateRoom(
                initialLevel,
                player,
                initialProjectiles,
                collisionManager);

            roomManager = new RoomManager(
                initialRoom,
                roomFactory.RoomCount,
                player,
                index => RoomFactory.CreateRoom(
                    roomFactory.LoadLevel(index),
                    Player,
                    new ProjectileManager(),
                    collisionManager));

            camera = new Camera2D(ScreenWidth, ScreenHeight);

            camera.SnapTo(
                PlayerBounds(),
                roomManager.ActiveRoom.Level.WorldBounds);

            collisionManager.SetWorldBounds(
                roomManager.ActiveRoom.Level.WorldBounds);

            pendingDoor = null;
            unlockedDoors.Clear();

            menuState.Open(false);

            RefreshGameplayController();
        }

        private void UpdateGameplayFrame(GameTime gameTime)
        {
            gameplayController.Update();

            if (State == GameState.Gameplay && !bazooka.Active)
            {
                UpdateGameplay(gameTime);
            }
        }

        private void UpdateActiveGameplay(GameTime gameTime)
        {
            bazooka.SetAim(Keyboard.GetState());

            if (bazooka.Active)
            {
                bazooka.Update(gameTime);
                return;
            }

            selfieShortcut.Update();

            if (State == GameState.Gameplay)
            {
                UpdateGameplayFrame(gameTime);
            }
        }

        private void UpdateGameplay(GameTime gameTime)
        {
            Room activeRoom = roomManager.ActiveRoom;

            activeRoom.Update(gameTime);

            barrelStorm.Update(gameTime, camera.ViewBounds);

            Player.Update(gameTime);

            KeepPlayerInRoom();

            camera.Follow(
                PlayerBounds(),
                activeRoom.Level.WorldBounds,
                gameTime);

            ApplyPowerUps(activeRoom.CollectItems(Player));

            showcase.Update(gameTime);

            collisionManager.BuildTree(
                activeRoom.GetCollisionObjects(Player));

            collisionManager.DetectCollision();

            if (pendingDoor != null && Player.Health > 0)
            {
                Door door = pendingDoor;
                pendingDoor = null;
                UseDoor(door);
            }

            CheckForGameEnd();
        }

        private void ApplyPowerUps(IReadOnlyList<IItem> collected)
        {
            if (collected.Count > 0)
            {
                audio.Play(GameSound.Pickup);
            }

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
                EndGame(
                    GameState.GameOver,
                    "Game Over - R to reset, Q to quit");
            }
            else if (roomManager.ActiveRoom.IsGoalReached)
            {
                EndGame(
                    GameState.Victory,
                    "Mission Complete - R to play again, Q to quit");
            }
        }

        private void EndGame(GameState endState, string title)
        {
            barrelStorm.CancelRoomEffect();

            audio.StopMusic();

            audio.Play(
                endState == GameState.Victory
                    ? GameSound.Victory
                    : GameSound.Defeat);

            SetState(endState, menuState);

            Window.Title = title;

            menuState.Open(true);
        }

        // Resolves a door the player touched:
        // relative (legacy) or targeted, honoring locks and keys.
        private void UseDoor(Door door)
        {
            if (!door.Spec.IsTargeted)
            {
                // Legacy '<' / '>' door: move relative to the current room.
                if (door.Direction > 0)
                {
                    NextRoom();
                }
                else if (door.Direction < 0)
                {
                    PreviousRoom();
                }

                return;
            }

            int target = door.TargetRoomIndex.Value;

            if (target < 0 || target >= roomManager.RoomCount)
            {
                return;
            }

            (int From, int To) id =
                (roomManager.ActiveRoomIndex, target);

            if (door.IsLocked && !unlockedDoors.Contains(id))
            {
                if (!Player.TryUseKey())
                {
                    // No key: leave the door shut.
                    audio.Play(GameSound.Item);
                    return;
                }

                unlockedDoors.Add(id);
                door.IsLocked = false;
            }

            GoToRoom(target);
        }

        private void SwitchRoom(System.Action switchRoom)
        {
            bazooka.ClearSpaceWarning();

            barrelStorm.CancelRoomEffect();

            selfieNeedsSpace = false;

            switchRoom();

            collisionManager.SetWorldBounds(
                roomManager.ActiveRoom.Level.WorldBounds);

            collisionManager.BuildTree(
                roomManager.ActiveRoom.GetCollisionObjects(Player));

            camera.SnapTo(
                PlayerBounds(),
                roomManager.ActiveRoom.Level.WorldBounds);

            RefreshGameplayController();
        }

        public void ActivateBarrelStorm()
        {
            if (gameState == GameState.Gameplay &&
                roomManager?.ActiveRoom != null)
            {
                barrelStorm.TryActivate(
                    roomManager.ActiveRoom.Projectiles,
                    camera.ViewBounds,
                    roomManager.ActiveRoom.Level.WorldBounds);
            }
        }

        private void RefreshGameplayController()
        {
            if (roomManager?.ActiveRoom == null)
            {
                return;
            }

            gameplayController = ControllerFactory.CreateGameplayController(
                this,
                Player,
                showcase.Blocks,
                showcase.Items,
                showcase.Enemies);

            selfieShortcut = ControllerFactory.CreateSelfieShortcut(this);
        }

        private void KeepPlayerInRoom()
        {
            Rectangle bounds =
                roomManager.ActiveRoom.Level.WorldBounds;

            Vector2 minimum = new Vector2(
                bounds.Left,
                bounds.Top);

            Vector2 maximum = new Vector2(
                bounds.Right - Player.Width,
                bounds.Bottom - Player.Height);

            Player.Position = Vector2.Clamp(
                Player.Position,
                minimum,
                maximum);
        }

        private Rectangle PlayerBounds()
        {
            return new Rectangle(
                (int)Player.Position.X,
                (int)Player.Position.Y,
                Player.Width,
                Player.Height);
        }

        private Color GetRoomBackgroundTint()
        {
            return roomManager.ActiveRoomIndex switch
            {
                0 => new Color(190, 150, 110), // Frontier Hub - warm brown
                1 => new Color(220, 145, 90),  // Dusty orange
                2 => new Color(130, 165, 110), // Muted green
                3 => new Color(105, 145, 190), // Blue
                4 => new Color(155, 120, 180), // Purple
                6 => new Color(185, 105, 100), // Red
                5 => new Color(105, 165, 160), // Teal
                7 => new Color(95, 90, 85),    // Fortress - charcoal
                _ => Color.White
            };
        }
    }
}
