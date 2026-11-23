
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ArmadilloGame.Collisions;
using ArmadilloGame.Controllers;
using ArmadilloGame.Core;
using ArmadilloGame.Entities;
using ArmadilloGame.Factories;
using ArmadilloGame.Interfaces;
using ArmadilloGame.Sprites;
using ArmadilloGame.States;
using ArmadilloGame.UI;
using ArmadilloGame.Rendering;

namespace ArmadilloGame
{
    internal class Game1 : Game, IPlayerHost
    {
        // Screen configuration
        private const int ScreenWidth = 960;
        private const int ScreenHeight = 540;
        private const int WindowWidth = 1280;
        private const int WindowHeight = 720;
        private const double DoorCooldownDurationMs = 600;

        // Window titles
        private const string MenuTitle = "Armadillo - Press Enter to Start";
        private const string GameplayTitle =
            "Armadillo - A/D Move, W/SPACE Jump, SHIFT Ball, 1-4 Weapon, Click Fire, C/V Items, Tab Map, ` Dev";

        // UI and screen colors
        private static readonly Color MenuColor = new(48, 22, 19);
        private static readonly Color GameplayColor = new(42, 55, 70);

        // Graphics
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private RenderTarget2D sceneTarget;

        private Texture2D backgroundTexture;
        private Texture2D titleTexture;

        private ShadowRenderer shadowRenderer;
        private FilmGrainRenderer gameplayFilmGrain;
        private BackgroundRenderer backgroundRenderer;

        // Game states
        private GameState gameState;
        private IGameState currentState;

        private TitleScreenState titleScreenState;
        private MenuState menuState;
        private GameplayState gameplayState;
        private MapState mapState;
        private SelfieState selfieState;
        private GalleryState galleryState;

        // Gameplay systems
        private readonly RoomFactory roomFactory = new(
            "Content/Levels/hub-00.room",
            "Content/Levels/desert-01.room",
            "Content/Levels/laboratory-02.room",
            "Content/Levels/stone-03.room",
            "Content/Levels/metal-04.room",
            "Content/Levels/crates-05.room",
            "Content/Levels/hazards-06.room",
            "Content/Levels/fortress-07.room");

        private CollisionManager collisionManager;
        private RoomManager roomManager;
        private Camera2D camera;

        private IPlayer player;
        private Door pendingDoor;

        private readonly WeaponSelection weapons = new();
        private ShotgunAbility shotgun;
        private BarrelStorm barrelStorm;
        private readonly GameAudio audio = new();

        // Controllers
        private IController gameplayController;
        private IController devController;
        private IController selfieShortcut;

        // UI
        private ControlsOverlay controlsOverlay;
        private PlayerHud playerHud;
        private AimPreview aimPreview;
        private Controllers.FirePhase lastFirePhase;
        private ObjectShowcase showcase;

        // Gameplay tracking
        private bool devMode;
        private bool selfieNeedsSpace;
        private double doorCooldownMs;

        private readonly HashSet<(int From, int To)> unlockedDoors = new();

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

        // The mouse position in world space: window pixels -> virtual 960x540 -> plus camera offset.
        private Vector2 AimWorldPosition()
        {
            Point virtualMouse = Core.ScreenScale.ToVirtual(Mouse.GetState().Position);
            Rectangle view = camera.ViewBounds;
            return new Vector2(virtualMouse.X + view.Left, virtualMouse.Y + view.Top);
        }

        // Routes the left mouse button's fire phase to the equipped weapon each frame.
        public void HandleFire(Controllers.FirePhase phase)
        {
            if (gameState == GameState.Gameplay)
            {
                lastFirePhase = phase;
                weapons.SetFirePhase(phase);
            }
        }

        // Equips weapon slot 0-3 when its weapon is unlocked.
        public void SelectWeapon(int index)
        {
            if (gameState == GameState.Gameplay)
            {
                weapons.Select(index, Player);
            }
        }

        public void CallShotgun()
        {
            if (gameState == GameState.Gameplay)
            {
                shotgun.Call(
                    Player,
                    camera.ViewBounds,
                    ActiveRoom.Level.Blocks);
            }
        }

        public void FireShotgun()
        {
            if (gameState == GameState.Gameplay)
            {
                shotgun.Fire(Player);
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
            graphics.PreferredBackBufferWidth = WindowWidth;
            graphics.PreferredBackBufferHeight = WindowHeight;
            graphics.ApplyChanges();

            Core.ScreenScale.Configure(ScreenWidth, ScreenHeight, WindowWidth, WindowHeight);

            gameState = GameState.StartMenu;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sceneTarget = new RenderTarget2D(GraphicsDevice, ScreenWidth, ScreenHeight);

            titleTexture = Content.Load<Texture2D>("Sprites/backgrounds/dilloStartTest");

            gameplayFilmGrain = new FilmGrainRenderer(GraphicsDevice)
            {
                Intensity = 0.02f
            };

            // Load the repeating background.
            backgroundTexture = Content.Load<Texture2D>("Sprites/Brown");
            backgroundRenderer = new BackgroundRenderer(backgroundTexture);

            shotgun = new ShotgunAbility(
                Content.Load<Texture2D>("Sprites/Shotgun-helper"),
                Content.Load<Texture2D>("Sprites/Shotgun-weapon"));

            audio.Load(Content);

            SpriteAtlas.LoadAllTextures(Content);
            PlayerSpriteFactory.Instance.LoadAllTextures(Content);
            EnemySpriteFactory.Instance.LoadAllTextures(Content);
            ProjectileSpriteFactory.Instance.LoadAllTextures(Content);

            weapons.LoadPreviews();

            BlockSpriteFactory.Instance.LoadAllTextures(Content);
            ItemSpriteFactory.Instance.LoadAllTextures(Content);

            SpriteFont uiFont = Content.Load<SpriteFont>("Fonts/Controls");
            SpriteFont uiFontLarge = Content.Load<SpriteFont>("Fonts/Controls-Large");

            controlsOverlay = new ControlsOverlay(uiFontLarge, GraphicsDevice);

            shadowRenderer = new ShadowRenderer(
            GraphicsDevice,
            spriteBatch,
            ScreenWidth,
            ScreenHeight);
            shadowRenderer.Opacity = 0.02f;
            shadowRenderer.Spread = 10;
            shadowRenderer.Offset = new Vector2(0f, 5f);

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

            titleScreenState = new TitleScreenState(
                titleTexture,
                GraphicsDevice,
                uiFontLarge,
                () =>
                {
                    menuState.Open(false);
                    SetState(GameState.StartMenu, menuState);
                });

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

            playerHud = new PlayerHud(uiFontLarge, playerPortrait);
            aimPreview = new AimPreview(GraphicsDevice);

            mapState = new MapState(
                GraphicsDevice,
                uiFont,
                weapons,
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

            SetState(GameState.StartMenu, titleScreenState);
        }

        protected override void Update(GameTime gameTime)
        {
            currentState.Update(gameTime);
            base.Update(gameTime);
        }

        private void CaptureGameplayShadows(Room room)
        {
            shadowRenderer.Capture(spriteBatch =>
            {
                // Blocks
                foreach (IBlock block in room.Level.Blocks)
                {
                    block.Draw(spriteBatch);
                }

                // Items
                foreach (IItem item in room.Level.Items)
                {
                    item.Draw(spriteBatch);
                }

                // Enemies
                foreach (IGameObject enemy in room.Enemies.Objects)
                {
                    if (enemy is not IEnemy combatant || !combatant.IsDestroyed)
                    {
                        enemy.Draw(spriteBatch);
                    }
                }

                // Player
                Player.Draw(spriteBatch);

            }, camera.GetViewMatrix());
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                roomManager?.Unload();

                // States
                selfieState?.Dispose();
                galleryState?.Dispose();
                titleScreenState?.Dispose();

                // Rendering
                shadowRenderer?.Dispose();
                gameplayFilmGrain?.Dispose();

                // Gameplay systems
                barrelStorm?.Dispose();
                audio.Dispose();
                controlsOverlay?.Dispose();
                aimPreview?.Dispose();

                // Graphics resources
                sceneTarget?.Dispose();
                spriteBatch?.Dispose();
            }

            base.Dispose(disposing);
        }

        protected override void Draw(GameTime gameTime)
{
    // Title and menu screens render directly at window resolution.
    if (State == GameState.StartMenu ||
        State == GameState.GameOver ||
        State == GameState.Victory)
    {
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        currentState.Draw(spriteBatch);

        base.Draw(gameTime);
        return;
    }

    // Gameplay and other screens retain virtual-resolution rendering.
    GraphicsDevice.SetRenderTarget(sceneTarget);
    currentState.Draw(spriteBatch);

    GraphicsDevice.SetRenderTarget(null);
    GraphicsDevice.Clear(Color.Black);

    spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    spriteBatch.Draw(
        sceneTarget,
        new Rectangle(0, 0, WindowWidth, WindowHeight),
        Color.White);
    spriteBatch.End();

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

            CaptureGameplayShadows(room);

            // Background
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: camera.GetViewMatrix());

            backgroundRenderer.Draw(
                spriteBatch,
                camera.ViewBounds,
                roomManager.ActiveRoomIndex);

            spriteBatch.End();

            // Shadows
            shadowRenderer.Draw();

            // World objects
            spriteBatch.Begin(
                samplerState: SamplerState.PointClamp,
                transformMatrix: camera.GetViewMatrix());

            room.Draw(spriteBatch);
            barrelStorm.DrawParticles(spriteBatch);
            DrawAimPreview();
            Player.Draw(spriteBatch);
            shotgun.Draw(spriteBatch, Player);

            spriteBatch.End();

            // Screen effects
            gameplayFilmGrain.Draw(
                spriteBatch,
                new Rectangle(0, 0, ScreenWidth, ScreenHeight));

            // HUD and overlays
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            barrelStorm.DrawOverlay(spriteBatch);
            DrawHud(room);

            spriteBatch.End();
        }
        private void DrawAimPreview()
        {
            bool active = lastFirePhase == Controllers.FirePhase.Pressed
                || lastFirePhase == Controllers.FirePhase.Held;
            aimPreview.Draw(spriteBatch, Player, weapons.SelectedKind, weapons.MolotovCharge, active, IsAimPointBlocked);
        }

        // True when a world point sits inside a solid block or a live enemy; the aim guide stops there.
        private bool IsAimPointBlocked(Point worldPoint)
        {
            Room room = roomManager.ActiveRoom;
            foreach (IBlock block in room.Level.Blocks)
            {
                if (block.IsSolid && block.Bounds.Contains(worldPoint))
                {
                    return true;
                }
            }
            foreach (IGameObject gameObject in room.Enemies.Objects)
            {
                if (gameObject is IEnemy { IsDestroyed: false } enemy && enemy.Bounds.Contains(worldPoint))
                {
                    return true;
                }
            }
            return false;
        }

        private void DrawHud(Room room)
        {
            playerHud.Draw(spriteBatch, Player, weapons);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                selfieNeedsSpace ? "F2: Need space" : "F2: Selfie",
                728, 86, false);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                audio.IsMuted ? "M: Audio OFF" : "M: Audio ON",
                820, 86, false);

            controlsOverlay.DrawSmallText(
                spriteBatch,
                "F3: Gallery",
                728, 100, false);

            if (roomManager.ActiveRoomIndex == 0 &&
                room.Level.HasIntactReinforcedCrate)
            {
                controlsOverlay.DrawSmallText(
                    spriteBatch,
                    "SHIFT to transform. Roll into the blue crate.",
                    26, 98, false);
            }

            showcase.Draw(spriteBatch, controlsOverlay);

            controlsOverlay.DrawGameplayHelp(
                spriteBatch,
                Player,
                room.Objective);

            if (devMode)
            {
                controlsOverlay.DrawDevControls(
                    spriteBatch,
                    GraphicsDevice.Viewport);
            }
        }

        private void InitializeGameObjects()
        {
            shotgun.Reset();
            selfieNeedsSpace = false;

            roomManager?.Unload();
            roomManager = null;

            roomFactory.StartNewGame();

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

            // Default to the boomerang now that the player (weapon loadout) exists.
            weapons.Select(0, player);

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
            doorCooldownMs = 0;
            unlockedDoors.Clear();

            RefreshGameplayController();
        }

        private void UpdateGameplayFrame(GameTime gameTime)
        {
            gameplayController.Update();

            if (devMode)
            {
                devController.Update();
            }

            if (State == GameState.Gameplay && !shotgun.Active)
            {
                UpdateGameplay(gameTime);
            }
        }

        public void ToggleDevMode()
        {
            devMode = !devMode;
            // Entering dev mode grants every weapon so they can be tested without finding pickups.
            if (devMode)
            {
                Player.UnlockAllWeapons();
            }
        }

        public bool IsDevMode => devMode;

        private void UpdateActiveGameplay(GameTime gameTime)
        {
            gameplayFilmGrain.Update(gameTime);
            shotgun.SetAim(Keyboard.GetState());

            if (shotgun.Active)
            {
                shotgun.Update(gameTime);
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

            // Point the player's aim at the mouse before anything reads it this frame.
            Player.SetAimTarget(AimWorldPosition());

            // Advance the equipped weapon's fire behavior (windup, charge, autofire).
            weapons.Update(Player, gameTime);

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

            Door touchedDoor = pendingDoor;
            pendingDoor = null;

            if (doorCooldownMs > 0)
            {
                doorCooldownMs -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else if (touchedDoor != null && Player.Health > 0)
            {
                UseDoor(touchedDoor);
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

            if (door.LockedBehind && !unlockedDoors.Contains(id))
            {
                // The door was locked behind the player on entry. Passing back through requires
                // holding a key but does not spend one.
                if (Player.KeyCount <= 0)
                {
                    audio.Play(GameSound.Item);
                    return;
                }

                door.LockedBehind = false;
                door.IsLocked = false;
                unlockedDoors.Add(id);
                // Persist so the door stays open (not relocked) on later entries to this level.
                roomManager.ActiveRoom.Level.UnlockDoorTo(target);
            }
            else if (door.IsLocked && !unlockedDoors.Contains(id))
            {
                if (!Player.TryUseKey())
                {
                    // No key: leave the door shut.
                    audio.Play(GameSound.Item);
                    return;
                }

                unlockedDoors.Add(id);
                door.IsLocked = false;
                // Persist so this door stays open when its room is rebuilt (e.g. back in the hub).
                roomManager.ActiveRoom.Level.UnlockDoorTo(target);
            }

            GoToRoom(target);
        }

        private void SwitchRoom(System.Action switchRoom)
        {
            shotgun.ClearSpaceWarning();

            barrelStorm.CancelRoomEffect();

            selfieNeedsSpace = false;

            switchRoom();

            // Arm the door cooldown so the player does not re-trigger a door on arrival.
            doorCooldownMs = DoorCooldownDurationMs;
            pendingDoor = null;

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

            devController = ControllerFactory.CreateDevController(
                this,
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

        private Rectangle PlayerBounds() =>
            new Rectangle(
                (int)Player.Position.X,
                (int)Player.Position.Y,
                Player.Width,
                Player.Height);
    }
}
