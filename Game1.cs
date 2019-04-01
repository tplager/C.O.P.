﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;
using MonoGame.Extended;
using MonoGame.Extended.ViewportAdapters;
using Microsoft.Xna.Framework.Media;

namespace Purpose
{
    /// <summary>
    /// It's ya boy, Alex
    /// Nicholas needs a medic
    /// Overlord Trent has arrived.
    /// Vansh is (no longer) here
    /// </summary>

    #region Enums
    public enum Level
    {
        One,
        Two,
        Three,
    }

    public enum WaveNumber
    {
        One,
        Two, 
        Three, 
        Four, 
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Eleven,
        Twelve,
        Thirteen,
        Fourteen,
        Fifteen
    }

    public enum GameState
    {
        Menu,
        Controls,
        PresetGame,
        EditorGame,
        Pause,
        UpgradeMenu,
        UnlockablesMenu,
        NextWave,
        GameOver,
        YouWin
    }
    #endregion

    public class Game1 : Game
    {
        #region Fields
        //Fields for MonoGame
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;
        private Camera2D camera;

        //Fields for Key,Mouse States, etc.
        private KeyboardState kbState;
        private MouseState ms;

        
        private List<Platform> leftWalls;
        private List<Platform> rightWalls;

        //Fields for World Size
        private int worldLeftEndWidth;
        private int worldRightEndWidth;
        private int worldTopHeight;
        private int worldBottomHeight;

        //Fields for Buttons/GameObjects
        private GameObject editedGameButton;
        private GameObject presetGameButton;
        private GameObject toGameButton;
        private GameObject returnToNewWaveButton;
        private GameObject groundPoundButton;
        private GameObject attackUpButton;
        private GameObject staminaUpButton;
        private GameObject healthUpButton;
        private GameObject dashButton;
        private GameObject dashDistanceUpButton;
        private GameObject returnToGameButton;
        private GameObject exitGameButton;
        private GameObject goOnButton;
        private GameObject upgradesButton;
        private GameObject returnToMenuButton;
        private GameObject returnToMainButton;
        private GameObject unlockablesReturnToPauseButton;
        private GameObject startUnlockablesButton;
        private GameObject pauseUnlockablesButton;
        private GameObject nextWaveUnlockablesButton;

        private GameObject groundPoundTip;
        private GameObject damageUpTip;
        private GameObject staminaUpTip;
        private GameObject healthUpTip;
        private GameObject dashTip;
        private GameObject dashUpTip;

        private GameObject sombreroButton;
        private GameObject ushankaButton;
        private GameObject fezButton;
        private GameObject cowboyButton;
        private GameObject beretButton;
        private GameObject defaultButton;

        //Field for other Classes
        private Reader reader;
        private Player player;
        private GameManager gameManager;
        private GameTime gameTime;
        private PresetWaves presetWaves;
        private TextureManager textureManager;
        private PlatformManager platformManager;

        private Texture2D background;
        private Texture2D tempTexture;
        private Texture2D currentTexture;
        private Rectangle healthBar;
        private Rectangle staminaBar;
        private Rectangle staminaBack;
        private Rectangle healthBack;
        private SpriteFont agency30;
        private SpriteFont agency20;
        private Random rng;
        private Vector2 waveIndicator;

        //Field for sound
        private SoundManager soundManager;

        //Field for the Unlockables
        private Unlockables unlockables;
        private UnlockablesReadWrite savedUnlockables;


        private bool editedGame;
        #endregion

        #region Properties
        public Random Rng
        {
            get { return rng; }
            set { rng = value; }
        }

        public int WorldLeftEndWidth
        {
            get { return worldLeftEndWidth; }
            set { worldLeftEndWidth = value; }
        }

        public int WorldRightEndWidth
        {
            get { return worldRightEndWidth; }
            set { worldRightEndWidth = value; }
        }

        public GameTime GameTime
        {
            get { return gameTime; }
            set { gameTime = value; }
        }

        public List<Platform> LeftWalls
        {
            get { return leftWalls; }
            set { leftWalls = value; }
        }

        public List<Platform> RightWalls
        {
            get { return rightWalls; }
            set { rightWalls = value; }
        }
        #endregion

        #region Constructor
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // Set screen size
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 845;
            graphics.PreferredBackBufferWidth = 1350;

            //graphics.IsFullScreen = true;
            //graphics.PreferredBackBufferHeight = 1080;
            //graphics.PreferredBackBufferWidth = 1920;

            // Temp coding stuffs
            rng = new Random();

            // Set size of world width and height
            worldLeftEndWidth = -3000;
            worldRightEndWidth = 3000;
            worldTopHeight = -2500;
            worldBottomHeight = 1000;

            // Initialize gameTime and platformManager
            gameTime = new GameTime();
            platformManager = new PlatformManager(GraphicsDevice);
            
            editedGame = false;
        }
#endregion

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Make mouse visible
            this.IsMouseVisible = true;
            //Initialize the Window Form
            base.Initialize();
            //bottomPlatforms = new List<Platform>();
            ViewportAdapter viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);
            camera = new Camera2D(viewportAdapter);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load in textures
            agency30 = Content.Load<SpriteFont>("Agency30");
            agency20 = Content.Load<SpriteFont>("Agency20");

            //Loads in the current texture which can be changed with unlockables
            currentTexture = Content.Load<Texture2D>("pineapple2");
            tempTexture = Content.Load<Texture2D>("pineapple2");

            //Loading in the sound effects
            soundManager = new SoundManager(Content.Load<SoundEffect>("Sound/Grunts"), Content.Load<SoundEffect>("Sound/Punch"), Content.Load<SoundEffect>("Sound/PlayerScream"), Content.Load<Song>("Sound/Song"));

            textureManager = new TextureManager(Content.Load<Texture2D>("Player/LeftStandingSprite"), Content.Load<Texture2D>("Player/RightStandingSprite"),
                Content.Load<Texture2D>("Player/LeftMiddleRunningSprite"), Content.Load<Texture2D>("Player/RightMiddleRunningSprite"),
                Content.Load<Texture2D>("Player/LeftRunningSprite"), Content.Load<Texture2D>("Player/RightRunningSprite"),
                Content.Load<Texture2D>("Player/RightPlayerAttackSprite1"), Content.Load<Texture2D>("Player/LeftPlayerAttackSprite1"),
                Content.Load<Texture2D>("Player/RightPlayerAttackSprite2"), Content.Load<Texture2D>("Player/LeftPlayerAttackSprite2"),
                Content.Load<Texture2D>("Enemy/RightMeleePineapple1"), Content.Load<Texture2D>("Enemy/RightMeleePineapple2"),
                Content.Load<Texture2D>("Enemy/RightMeleePineapple3"), Content.Load<Texture2D>("Enemy/LeftMeleePineapple1"),
                Content.Load<Texture2D>("Enemy/LeftMeleePineapple2"), Content.Load<Texture2D>("Enemy/LeftMeleePineapple3"), tempTexture, Content.Load<Texture2D>("StartMenu"),
                Content.Load<Texture2D>("buttonFrame2"), Content.Load<Texture2D>("roundedFrame"), Content.Load<Texture2D>("UpgradeUI"),
                Content.Load<Texture2D>("PauseMenu"), Content.Load<Texture2D>("NextWaveMenu"), Content.Load<Texture2D>("GameOver"),
                Content.Load<Texture2D>("YouWin"), Content.Load<Texture2D>("Controls"), Content.Load<Texture2D>("PlatformTest"),
                Content.Load<Texture2D>("PlatformTest2"), Content.Load<Texture2D>("background"), Content.Load<Texture2D>("HealthBar/staminabar"),
                Content.Load<Texture2D>("HealthBar/healthbar"), Content.Load<Texture2D>("GroundPoundToolTip"), Content.Load<Texture2D>("DamageIncreaseToolTip"),
                Content.Load<Texture2D>("StaminaIncreaseToolTip"), Content.Load<Texture2D>("HealthIncreaseToolTip"), Content.Load<Texture2D>("DashToolTip"), Content.Load<Texture2D>("DashUpToolTip"),
                Content.Load<Texture2D>("Unlockables"), Content.Load<Texture2D>("Unlockables/SombreroSprites/RangedPineappleSombrero"), Content.Load<Texture2D>("Unlockables/SombreroSprites/RightMeleePineappleSombrero1"),
                Content.Load<Texture2D>("Unlockables/SombreroSprites/RightMeleePineappleSombrero2"), Content.Load<Texture2D>("Unlockables/SombreroSprites/RightMeleePineappleSombrero3"),
                Content.Load<Texture2D>("Unlockables/SombreroSprites/LeftMeleePineappleSombrero1"), Content.Load<Texture2D>("Unlockables/SombreroSprites/LeftMeleePineappleSombrero2"), Content.Load<Texture2D>("Unlockables/SombreroSprites/LeftMeleePineappleSombrero3"),
                Content.Load<Texture2D>("Unlockables/UshankaSprites/RangedPineappleUshanka"), Content.Load<Texture2D>("Unlockables/UshankaSprites/RightMeleePineappleUshanka1"),
                Content.Load<Texture2D>("Unlockables/UshankaSprites/RightMeleePineappleUshanka2"), Content.Load<Texture2D>("Unlockables/UshankaSprites/RightMeleePineappleUshanka3"),
                Content.Load<Texture2D>("Unlockables/UshankaSprites/LeftMeleePineappleUshanka1"), Content.Load<Texture2D>("Unlockables/UshankaSprites/LeftMeleePineappleUshanka2"), Content.Load<Texture2D>("Unlockables/UshankaSprites/LeftMeleePineappleUshanka3"),
                Content.Load<Texture2D>("Unlockables/FezSprites/RangedPineappleFez"), Content.Load<Texture2D>("Unlockables/FezSprites/RightMeleePineappleFez1"),
                Content.Load<Texture2D>("Unlockables/FezSprites/RightMeleePineappleFez2"), Content.Load<Texture2D>("Unlockables/FezSprites/RightMeleePineappleFez3"),
                Content.Load<Texture2D>("Unlockables/FezSprites/LeftMeleePineappleFez1"), Content.Load<Texture2D>("Unlockables/FezSprites/LeftMeleePineappleFez2"), Content.Load<Texture2D>("Unlockables/FezSprites/LeftMeleePineappleFez3"),
                Content.Load<Texture2D>("Unlockables/CowboySprites/RangedPineappleCowboy"), Content.Load<Texture2D>("Unlockables/CowboySprites/RightMeleePineappleCowboy1"),
                Content.Load<Texture2D>("Unlockables/CowboySprites/RightMeleePineappleCowboy2"), Content.Load<Texture2D>("Unlockables/CowboySprites/RightMeleePineappleCowboy3"),
                Content.Load<Texture2D>("Unlockables/CowboySprites/LeftMeleePineappleCowboy1"), Content.Load<Texture2D>("Unlockables/CowboySprites/LeftMeleePineappleCowboy2"), Content.Load<Texture2D>("Unlockables/CowboySprites/LeftMeleePineappleCowboy3"),
                Content.Load<Texture2D>("Unlockables/BeretSprites/RangedPineappleBeret"), Content.Load<Texture2D>("Unlockables/BeretSprites/RightMeleePineappleBeret1"),
                Content.Load<Texture2D>("Unlockables/BeretSprites/RightMeleePineappleBeret2"), Content.Load<Texture2D>("Unlockables/BeretSprites/RightMeleePineappleBeret3"),
                Content.Load<Texture2D>("Unlockables/BeretSprites/LeftMeleePineappleBeret1"), Content.Load<Texture2D>("Unlockables/BeretSprites/LeftMeleePineappleBeret2"), Content.Load<Texture2D>("Unlockables/BeretSprites/LeftMeleePineappleBeret3"));
                

            //Creating the unlockables
            unlockables = new Unlockables(textureManager);

            editedGameButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 308, 349, 155));
            presetGameButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 479, 349, 155));
            startUnlockablesButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 649, 349, 155));
            toGameButton = new GameObject(textureManager.ButtonFrame, new Rectangle(965, 668, 349, 155));
            returnToNewWaveButton = new GameObject(textureManager.ButtonFrame, new Rectangle(10,340,242,109));
            returnToGameButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 272, 349, 160));
            exitGameButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 457, 349, 160));
            pauseUnlockablesButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 643, 349, 155));
            goOnButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 272, 349, 160));
            upgradesButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 457, 349, 160));
            nextWaveUnlockablesButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 643, 349, 155));
            returnToMenuButton = new GameObject(textureManager.ButtonFrame, new Rectangle(500, 343, 349, 160));
            returnToMainButton = new GameObject(textureManager.ButtonFrame, new Rectangle(35, 35, 243, 108));
            unlockablesReturnToPauseButton = new GameObject(textureManager.ButtonFrame, new Rectangle(35, 34, 242, 109));

            groundPoundTip = new GameObject(textureManager.GroundPoundTip, new Rectangle(0, 0, 250, 250));
            damageUpTip = new GameObject(textureManager.DamageUpTip, new Rectangle(0, 0, 250, 250));
            staminaUpTip = new GameObject(textureManager.StaminaUpTip, new Rectangle(0, 0, 250, 250));
            healthUpTip = new GameObject(textureManager.HealthUpTip, new Rectangle(0, 0, 250, 250));
            dashTip = new GameObject(textureManager.DashTip, new Rectangle(0, 0, 250, 250));
            dashUpTip = new GameObject(textureManager.DashUpTip, new Rectangle(0, 0, 250, 250));

            groundPoundButton = new GameObject(textureManager.RoundedFrame, new Rectangle(337,200,118,118));
            attackUpButton = new GameObject(textureManager.RoundedFrame, new Rectangle(510, 292, 118, 118));
            staminaUpButton = new GameObject(textureManager.RoundedFrame, new Rectangle(725, 292, 118, 118));
            healthUpButton = new GameObject(textureManager.RoundedFrame, new Rectangle(889, 200, 118, 118));
            dashButton = new GameObject(textureManager.RoundedFrame, new Rectangle(620, 490, 118, 118));
            dashDistanceUpButton = new GameObject(textureManager.RoundedFrame, new Rectangle(620, 680, 118, 118));

            sombreroButton = new GameObject(textureManager.ButtonFrame, new Rectangle(158, 203, 252, 252));
            ushankaButton = new GameObject(textureManager.ButtonFrame, new Rectangle(544, 203, 252, 252));
            fezButton = new GameObject(textureManager.ButtonFrame, new Rectangle(930, 203, 252, 252));
            cowboyButton = new GameObject(textureManager.ButtonFrame, new Rectangle(158, 531, 252, 252));
            beretButton = new GameObject(textureManager.ButtonFrame, new Rectangle(544, 531, 252, 252));
            defaultButton = new GameObject(textureManager.ButtonFrame, new Rectangle(930, 531, 252, 252));

            // Makes platforms & walls
            platformManager.MakePlatforms(PlatformVersion.Easy, GraphicsDevice, textureManager);
            leftWalls = new List<Platform>();
            rightWalls = new List<Platform>();

            

            // Makes player, gameManager object
            background = textureManager.MetalBack;
            player = new Player("Dude", new Rectangle(50, GraphicsDevice.Viewport.Height - 452, 139, 352), textureManager, gameTime, soundManager);
            gameManager = new GameManager(player, platformManager.TotalPlatforms, platformManager.LeftWalls, platformManager.RightWalls, GraphicsDevice, 
                textureManager, soundManager);
            //wave = new Wave(gameManager, game1 = new Game1());

            gameManager.GameState = GameState.Menu;

            //Initializing Reader
            reader = new Reader(gameManager);
            //Runs the Reader method to vcreate enemies needed
            reader.ReadEditor();

            //Intializing the PresetWaves
            presetWaves = new PresetWaves(gameManager);
            //Creates all the waves
            presetWaves.CreateWaves();

            savedUnlockables = new UnlockablesReadWrite();
            List<bool[]> unlockedEquipped = savedUnlockables.Load();

            for (int i = 0; i < unlockables.ItemsList.Count; i++)
            {
                if (unlockedEquipped.Count > 0)
                {
                    if (unlockedEquipped[i][0])
                    {
                        unlockables.ItemsList[i].Unlocked = true;
                    }

                    if (unlockedEquipped[i][1])
                    {
                        unlockables.ItemsList[i].Equipped = true;
                    }
                }
            }
            unlockables.UnlockPoints = savedUnlockables.UnlockPoints;
        }
        
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            KeyboardState previouskbState = kbState;
            kbState = Keyboard.GetState();

            MouseState previousMs = ms;
            ms = Mouse.GetState();

            groundPoundTip.Position = new Rectangle(ms.X, ms.Y, 300, 300);
            damageUpTip.Position = new Rectangle(ms.X, ms.Y, 300, 300);
            staminaUpTip.Position = new Rectangle(ms.X, ms.Y, 300, 300);
            healthUpTip.Position = new Rectangle(ms.X, ms.Y, 300, 300);
            dashTip.Position = new Rectangle(ms.X, ms.Y-250, 300, 300);
            dashUpTip.Position = new Rectangle(ms.X, ms.Y-250, 300, 300);

            // Checks for the GameState which is determined by user
            // If they want to use editor tool they get game based of input from text file
            // If they pick regular start they get preset waves
            //if (gameManager.GameState == GameState.Game)
            //{
            if (!editedGame)
            {
                switch (gameManager.WaveNumber)
                {
                    case WaveNumber.One:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 0, gameTime);
                        break;
                    case WaveNumber.Two:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 1, gameTime);
                        break;
                    case WaveNumber.Three:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 2, gameTime);
                        break;
                    case WaveNumber.Four:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 3, gameTime);
                        break;
                    case WaveNumber.Five:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 4, gameTime);
                        break;
                    case WaveNumber.Six:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 5, gameTime);
                        break;
                    case WaveNumber.Seven:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 6, gameTime);
                        break;
                    case WaveNumber.Eight:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 7, gameTime);
                        break;
                    case WaveNumber.Nine:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 8, gameTime);
                        break;
                    case WaveNumber.Ten:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 9, gameTime);
                        break;
                    case WaveNumber.Eleven:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 10, gameTime);
                        break;
                    case WaveNumber.Twelve:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 11, gameTime);
                        break;
                    case WaveNumber.Thirteen:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 12, gameTime);
                        break;
                    case WaveNumber.Fourteen:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 13, gameTime);
                        break;
                    case WaveNumber.Fifteen:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 14, gameTime);
                        break;
                    default:
                        break;
                }
            }
            else if (editedGame)
            {
                switch (gameManager.WaveNumber)
                {
                    case WaveNumber.One:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 0, gameTime);
                        break;
                    case WaveNumber.Two:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 1, gameTime);
                        break;
                    case WaveNumber.Three:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 2, gameTime);
                        break;
                    case WaveNumber.Four:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 3, gameTime);
                        break;
                    case WaveNumber.Five:
                        UpdateHelper(kbState, previouskbState, ms, previousMs, 4, gameTime);
                        break;
                    default:
                        break;
                }
            }
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            var transformMatrix = camera.GetViewMatrix();
            spriteBatch.Begin(transformMatrix: transformMatrix);

            // GameState drawing stuffs
            switch (gameManager.GameState)
            {
                #region Menu Draw State
                case GameState.Menu:

                    spriteBatch.Draw(textureManager.StartScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    if (editedGameButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(editedGameButton.Texture, editedGameButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(editedGameButton.Texture, editedGameButton.Position, Color.White);
                    }

                    if (presetGameButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(presetGameButton.Texture, presetGameButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(presetGameButton.Texture, presetGameButton.Position, Color.White);
                    }

                    if (startUnlockablesButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(startUnlockablesButton.Texture, startUnlockablesButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(startUnlockablesButton.Texture, startUnlockablesButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region Controls Draw State
                case GameState.Controls:
                    spriteBatch.Draw(textureManager.ControlScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    if (toGameButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(toGameButton.Texture, toGameButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(toGameButton.Texture, toGameButton.Position, Color.White);
                    }

                    if (returnToMainButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(returnToMainButton.Texture, returnToMainButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(returnToMainButton.Texture, returnToMainButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region Preset Game Draw State
                case GameState.PresetGame:
                    // Background
                    for (int x = worldLeftEndWidth; x < worldRightEndWidth; x += background.Width)
                    {
                        for (int y = worldTopHeight; y < worldBottomHeight; y += background.Height)
                        {
                            spriteBatch.Draw(background, new Rectangle(x, y, background.Width, background.Height), Color.White);
                        }
                    }

                    // Platforms
                    foreach (Platform p in platformManager.TotalPlatforms)
                    {
                        spriteBatch.Draw(p.Texture, p.Position, Color.White);
                    }

                    // Walls
                    foreach (Platform w in platformManager.LeftWalls)
                    {
                        spriteBatch.Draw(w.Texture, w.Position, Color.White);
                    }
                    foreach (Platform w in platformManager.RightWalls)
                    {
                        spriteBatch.Draw(w.Texture, w.Position, Color.White);
                    }

                    spriteBatch.DrawString(agency30,"Wave: " + gameManager.WaveNumber.ToString(), waveIndicator, Color.Red);

                    foreach (Enemy e in gameManager.EnemyManager.Enemies)
                    {
                            spriteBatch.Draw(e.Texture, e.Position, e.Color);
                        
                        // Drawing bullet bois
                        if(e.HasBullet)
                        {
                            spriteBatch.Draw(e.Texture, e.Bullet, Color.White);
                        }
                    }

                    // Player
                    spriteBatch.Draw(gameManager.Player.Texture, new Rectangle(gameManager.Player.X, gameManager.Player.Y, player.Position.Width, player.Position.Height),
                        Color.White);

                    //Draws the health bar and stamina bar and their backgrounds
                    spriteBatch.Draw(textureManager.Healthbar, healthBack, Color.Black);
                    spriteBatch.Draw(textureManager.Healthbar, healthBar, Color.White);

                    if(player.UgManager.DashActive == true || player.UgManager.GroundPoundActive == true)
                    {

                        spriteBatch.Draw(textureManager.Staminabar, staminaBack, Color.Black);
                        spriteBatch.Draw(textureManager.Staminabar, staminaBar, Color.White);
                    }

                    break;
                #endregion

                #region Edited Game Draw State
                case GameState.EditorGame:
                    // Background
                    for (int x = worldLeftEndWidth; x < worldRightEndWidth; x += background.Width)
                    {
                        for (int y = worldTopHeight; y < worldBottomHeight; y += background.Height)
                        {
                            spriteBatch.Draw(background, new Rectangle(x, y, background.Width, background.Height), Color.White);
                        }
                    }

                    // Platforms
                    foreach (Platform p in platformManager.TotalPlatforms)
                    {
                        spriteBatch.Draw(p.Texture, p.Position, Color.White);
                    }

                    // Walls
                    foreach (Platform w in platformManager.LeftWalls)
                    {
                        spriteBatch.Draw(w.Texture, w.Position, Color.White);
                    }
                    foreach (Platform w in platformManager.RightWalls)
                    {
                        spriteBatch.Draw(w.Texture, w.Position, Color.White);
                    }

                    spriteBatch.DrawString(agency30, "Wave: " + gameManager.WaveNumber.ToString(), waveIndicator, Color.Red);

                    foreach (Enemy e in gameManager.EnemyManager.Enemies)
                    {
                        spriteBatch.Draw(e.Texture, e.Position, e.Color);

                        // Drawing bullet bois
                        if (e.HasBullet)
                        {
                            spriteBatch.Draw(e.Texture, e.Bullet, Color.White);
                        }
                    }

                    // Player
                    spriteBatch.Draw(gameManager.Player.Texture, new Rectangle(gameManager.Player.X, gameManager.Player.Y, player.Position.Width, player.Position.Height),
                        Color.White);

                    //Draws the health bar and stamina bar and their backgrounds
                    spriteBatch.Draw(textureManager.Healthbar, healthBack, Color.Black);
                    spriteBatch.Draw(textureManager.Healthbar, healthBar, Color.White);

                    if (player.UgManager.DashActive == true || player.UgManager.GroundPoundActive == true)
                    {
                        spriteBatch.Draw(textureManager.Staminabar, staminaBack, Color.Black);
                        spriteBatch.Draw(textureManager.Staminabar, staminaBar, Color.White);
                    }
                    break;
                #endregion

                #region Pause Draw State
                case GameState.Pause:
                    spriteBatch.Draw(textureManager.PauseScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                    if (returnToGameButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(returnToGameButton.Texture, returnToGameButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(returnToGameButton.Texture, returnToGameButton.Position, Color.White);
                    }

                    if (exitGameButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(exitGameButton.Texture, exitGameButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(exitGameButton.Texture, exitGameButton.Position, Color.White);
                    }

                    if (pauseUnlockablesButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(pauseUnlockablesButton.Texture, pauseUnlockablesButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(pauseUnlockablesButton.Texture, pauseUnlockablesButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region Upgrade Menu Draw State
                case GameState.UpgradeMenu:
                    spriteBatch.Draw(textureManager.UpgradeScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    spriteBatch.DrawString(agency30, "Upgrade Points: " + gameManager.Player.UgManager.UpgradePoints.ToString(), new Vector2(1090,730), Color.White);

                    if (returnToNewWaveButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(returnToNewWaveButton.Texture, returnToNewWaveButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(returnToNewWaveButton.Texture, returnToNewWaveButton.Position, Color.White);
                    }

                    if (healthUpButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(healthUpButton.Texture, healthUpButton.Position, Color.Black);
                        spriteBatch.Draw(healthUpTip.Texture, healthUpTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.HealthMax.ToString(), new Vector2(ms.X + 150, ms.Y + 125), Color.Black);
                        spriteBatch.DrawString(agency20, player.HealthRegen.ToString(), new Vector2(ms.X + 115, ms.Y + 156), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.HealthUpCost.ToString(), new Vector2(ms.X + 168, ms.Y + 188), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(healthUpButton.Texture, healthUpButton.Position, Color.White);
                    }

                    if (staminaUpButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(staminaUpButton.Texture, staminaUpButton.Position, Color.Black);
                        spriteBatch.Draw(staminaUpTip.Texture, staminaUpTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.StaminaMax.ToString(), new Vector2(ms.X + 165, ms.Y + 152), Color.Black);
                        spriteBatch.DrawString(agency20, player.StaminaRegen.ToString(), new Vector2(ms.X + 117, ms.Y + 184), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.StaminaUpCost.ToString(), new Vector2(ms.X + 173, ms.Y + 215), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(staminaUpButton.Texture, staminaUpButton.Position, Color.White);
                    }

                    if (dashButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(dashButton.Texture, dashButton.Position, Color.Black);
                        spriteBatch.Draw(dashTip.Texture, dashTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.DashDistance.ToString(), new Vector2(ms.X + 175, ms.Y - 72), Color.Black);
                        spriteBatch.DrawString(agency20, player.DashStaminaCost.ToString(), new Vector2(ms.X + 170, ms.Y - 40), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.DashCost.ToString(), new Vector2(ms.X + 175, ms.Y - 8), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(dashButton.Texture, dashButton.Position, Color.White);
                    }

                    if (dashDistanceUpButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(dashDistanceUpButton.Texture, dashDistanceUpButton.Position, Color.Black);
                        spriteBatch.Draw(dashUpTip.Texture, dashUpTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.DashDistance.ToString(), new Vector2(ms.X + 175, ms.Y - 98), Color.Black);
                        spriteBatch.DrawString(agency20, player.DashStaminaCost.ToString(), new Vector2(ms.X + 170, ms.Y - 67), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.DashUpCost.ToString(), new Vector2(ms.X + 170, ms.Y - 35), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(dashDistanceUpButton.Texture, dashDistanceUpButton.Position, Color.White);
                    }

                    if (attackUpButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(attackUpButton.Texture, attackUpButton.Position, Color.Black);
                        spriteBatch.Draw(damageUpTip.Texture, damageUpTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.Damage.ToString(), new Vector2(ms.X + 133, ms.Y + 157), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.DamageUpCost.ToString(), new Vector2(ms.X + 173, ms.Y + 189), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(attackUpButton.Texture, attackUpButton.Position, Color.White);
                    }

                    if (groundPoundButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(groundPoundButton.Texture, groundPoundButton.Position, Color.Black);
                        spriteBatch.Draw(groundPoundTip.Texture, groundPoundTip.Position, Color.White);
                        spriteBatch.DrawString(agency20, player.GroundPoundDamage.ToString(), new Vector2(ms.X + 130, ms.Y + 150), Color.Black);
                        spriteBatch.DrawString(agency20, player.GroundPoundStaminaCost.ToString(), new Vector2(ms.X + 170, ms.Y + 182), Color.Black);
                        spriteBatch.DrawString(agency20, player.UgManager.GroundPoundCost.ToString(), new Vector2(ms.X + 170, ms.Y + 213), Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(groundPoundButton.Texture, groundPoundButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region Unlockables Menu
                case GameState.UnlockablesMenu:
                    spriteBatch.Draw(textureManager.UnlockablesUI, new Rectangle(0,0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                    spriteBatch.DrawString(agency30, "SPEND 5 UPGRADE POINTS \nTO UNLOCK A NEW HAT!", new Vector2(1000, 45), Color.Black);
                    spriteBatch.DrawString(agency30, "Unlock Points: " + unlockables.UnlockPoints, new Vector2(1000, 135), Color.Black);

                    if (unlockablesReturnToPauseButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(unlockablesReturnToPauseButton.Texture, unlockablesReturnToPauseButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(unlockablesReturnToPauseButton.Texture, unlockablesReturnToPauseButton.Position, Color.White);
                    }

                    if (sombreroButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(sombreroButton.Texture, sombreroButton.Position, Color.Black);
                    }
                    else
                    {
                        //checks if sombrero is equipped
                        if(unlockables.Sombrero.Equipped == true)
                        {
                            //If it is draws button as green
                            spriteBatch.Draw(sombreroButton.Texture, sombreroButton.Position, Color.Green);
                        }
                        //checks if the item is unlocked
                        else if (unlockables.Sombrero.Unlocked == true)
                        {
                            //If it is, makes button red to indicate its unlocked and can be equipped
                            spriteBatch.Draw(sombreroButton.Texture, sombreroButton.Position, Color.Red);
                        }
                        else
                        {
                            spriteBatch.Draw(sombreroButton.Texture, sombreroButton.Position, Color.White);
                        }
                    }

                    if (ushankaButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(ushankaButton.Texture, ushankaButton.Position, Color.Black);
                    }
                    else
                    {
                        //checks if ushanka is equipped
                        if (unlockables.Ushanka.Equipped == true)
                        {
                            //If it is, draws button as green
                            spriteBatch.Draw(ushankaButton.Texture, ushankaButton.Position, Color.Green);
                        }
                        //checks if the item is unlocked
                        else if (unlockables.Ushanka.Unlocked == true)
                        {
                            //If it is, makes button red to indicate its unlocked and can be equipped
                            spriteBatch.Draw(ushankaButton.Texture, ushankaButton.Position, Color.Red);
                        }
                        else
                        {
                            spriteBatch.Draw(ushankaButton.Texture, ushankaButton.Position, Color.White);
                        }
                    }

                    if (fezButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(fezButton.Texture, fezButton.Position, Color.Black);
                    }
                    else
                    {
                        //checks if fez is equipped
                        if(unlockables.Fez.Equipped == true)
                        {
                            //If it is, draws button as green
                            spriteBatch.Draw(fezButton.Texture, fezButton.Position, Color.Green);
                        }
                        //checks if the item is unlocked
                        else if (unlockables.Fez.Unlocked == true)
                        {
                            //If it is, makes button red to indicate its unlocked and can be equipped
                            spriteBatch.Draw(fezButton.Texture, fezButton.Position, Color.Red);
                        }
                        else
                        {
                            spriteBatch.Draw(fezButton.Texture, fezButton.Position, Color.White);
                        }
                    }

                    if (cowboyButton.Intersects(ms.Position))
                    {                        
                            spriteBatch.Draw(cowboyButton.Texture, cowboyButton.Position, Color.Black);                        
                    }
                    else
                    {
                        //checks if the cowboy hat is equipped
                        if (unlockables.Cowboy.Equipped == true)
                        {
                            //If it is, draws button as green
                            spriteBatch.Draw(cowboyButton.Texture, cowboyButton.Position, Color.Green);
                        }
                        //checks if the item is unlocked
                        else if (unlockables.Cowboy.Unlocked == true)
                        {
                            //If it is, makes button red to indicate its unlocked and can be equipped
                            spriteBatch.Draw(cowboyButton.Texture, cowboyButton.Position, Color.Red);
                        }
                        else
                        {                            
                            spriteBatch.Draw(cowboyButton.Texture, cowboyButton.Position, Color.White);
                        }
                    
                    }

                    if (beretButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(beretButton.Texture, beretButton.Position, Color.Black);
                    }
                    else
                    {   
                        //checks if the beret is equipped
                        if(unlockables.Beret.Equipped == true)
                        {
                            //If it is, draw the button as green
                            spriteBatch.Draw(beretButton.Texture, beretButton.Position, Color.Green);
                        }
                        //checks if the item is unlocked
                        else if(unlockables.Beret.Unlocked == true)
                        {
                            //If it is, makes button red to indicate its unlocked and can be equipped
                            spriteBatch.Draw(beretButton.Texture, beretButton.Position, Color.Red);
                        }
                        else
                        {
                            spriteBatch.Draw(beretButton.Texture, beretButton.Position, Color.White);
                        }
                    }

                    if (defaultButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(defaultButton.Texture, defaultButton.Position, Color.Black);
                    }
                    else
                    {
                        //Checks that all other unlockables are not equipped
                        if (unlockables.Beret.Equipped == false && unlockables.Cowboy.Equipped == false && unlockables.Ushanka.Equipped == false 
                            && unlockables.Sombrero.Equipped == false && unlockables.Fez.Equipped == false)
                        {
                            //If they are all default button is turned green
                            spriteBatch.Draw(defaultButton.Texture, defaultButton.Position, Color.Green);
                        }
                        else
                        {
                            //Otherwise shows red to indicate its usable
                            spriteBatch.Draw(defaultButton.Texture, defaultButton.Position, Color.Red);
                        }
                    }

                    break;
                #endregion

                #region Next Wave Draw State
                case GameState.NextWave:
                    spriteBatch.Draw(textureManager.NextWaveScreen, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                    if (goOnButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(goOnButton.Texture, goOnButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(goOnButton.Texture, goOnButton.Position, Color.White);
                    }


                    if (upgradesButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(upgradesButton.Texture, upgradesButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(upgradesButton.Texture, upgradesButton.Position, Color.White);
                    }

                    if (nextWaveUnlockablesButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(nextWaveUnlockablesButton.Texture, nextWaveUnlockablesButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(nextWaveUnlockablesButton.Texture, nextWaveUnlockablesButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region Game Over Draw State
                case GameState.GameOver:
                    spriteBatch.Draw(textureManager.GameOver, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);

                    if (returnToMenuButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(returnToMenuButton.Texture, returnToMenuButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(returnToMenuButton.Texture, returnToMenuButton.Position, Color.White);
                    }
                    break;
                #endregion

                #region You Win Draw State
                case GameState.YouWin:
                    spriteBatch.Draw(textureManager.YouWin, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White);
                    if (returnToMenuButton.Intersects(ms.Position))
                    {
                        spriteBatch.Draw(returnToMenuButton.Texture, returnToMenuButton.Position, Color.Black);
                    }
                    else
                    {
                        spriteBatch.Draw(returnToMenuButton.Texture, returnToMenuButton.Position, Color.White);
                    }
                    break;
                    #endregion
            }

            spriteBatch.End();
            base.Draw(gameTime);
        }

        private void UpdateHelper(KeyboardState kbState, KeyboardState previouskbState, MouseState msState, MouseState previousMs, int waveNumber, GameTime gameTime)
        {
            // GameState finite state machine
            switch (gameManager.GameState)
            {
                #region Menu State
                case GameState.Menu:
                    MediaPlayer.Play(soundManager.Song);
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.2f;

                    if (editedGameButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.Controls;
                        editedGame = true;
                    }

                    if (presetGameButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.Controls;
                        editedGame = false;
                    }

                    if (startUnlockablesButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.UnlockablesMenu;
                    }

                    //Reset Game
                    if (editedGame)
                    {
                        gameManager.ResetOnPlayerDeathEdited(camera, rng, worldLeftEndWidth, worldRightEndWidth, gameTime, platformManager);
                    }
                    else
                    {
                        gameManager.ResetOnPlayerDeathPreset(camera, rng, worldLeftEndWidth, worldRightEndWidth, gameTime, platformManager);
                    }

                    unlockables.UnlockableUpdate(gameManager.EnemyManager);

                    break;
                #endregion

                #region Controls State
                case GameState.Controls:
                    if (toGameButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        if (editedGame)
                        {
                            gameManager.GameState = GameState.EditorGame;
                        }
                        else
                        {
                            gameManager.GameState = GameState.PresetGame;
                        }
                    }

                    if (returnToMainButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.Menu;
                    }
                    break;
                #endregion

                #region Preset Game State
                case GameState.PresetGame:
                    //arenaManagerCounter = 0;
                    camera.MinimumZoom = 0.1f;
                    camera.MaximumZoom = 1.0f;
                    camera.Zoom = 0.5f;

                    //Used for the locations of the health bar and stamina bar
                    healthBar = new Rectangle(player.Position.X - 1245, player.Position.Y - 1000, player.Health * 4, textureManager.Healthbar.Height);
                    staminaBar = new Rectangle(player.Position.X - 1245, player.Position.Y - 900, player.Stamina * 4, textureManager.Staminabar.Height);
              
                    healthBack = new Rectangle(player.Position.X - 1250, player.Position.Y - 1006, (player.HealthMax * 4) + 10, textureManager.Healthbar.Height + 11);
                    staminaBack = new Rectangle(player.Position.X - 1250, player.Position.Y - 906, (player.StaminaMax * 4) + 10, textureManager.Staminabar.Height + 11);

                    //Used to Indicate and let the player know what wave they are on
                    waveIndicator = new Vector2(player.Position.X - 1250, player.Position.Y - 775);


                    // Stuff for moving player and enemy, as well as player attack
                    ms = Mouse.GetState();
                    gameManager.PlayerMove(kbState, previouskbState, ms, previousMs, camera, gameTime);

                    gameManager.EnemyManager.EnemyMove(gameTime, gameManager.Platforms, gameManager.Player, gameManager.LeftWalls, gameManager.RightWalls);
                    if (kbState.IsKeyDown(Keys.P))
                    {
                        gameManager.GameState = GameState.Pause;
                    }

                    if (player.IsDead || player.Y >= GraphicsDevice.Viewport.Height)
                    {
                        gameManager.GameState = GameState.GameOver;
                        soundManager.PlayerDeath.Play();
                    }

                        //When all enemies are dead calls in the next wave method
                    if (gameManager.EnemyManager.Enemies.Count == 0)
                    {
                        if (gameManager.WaveNumber == WaveNumber.One)
                        {
                            gameManager.WaveNumber = WaveNumber.Two;
                            gameManager.GameState = GameState.NextWave;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Two)
                        {
                            gameManager.WaveNumber = WaveNumber.Three;
                            gameManager.GameState = GameState.NextWave;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Three)
                        {
                            gameManager.WaveNumber = WaveNumber.Four;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Medium;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Four)
                        {
                            gameManager.WaveNumber = WaveNumber.Five;
                            gameManager.GameState = GameState.NextWave;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Five)
                        {
                            gameManager.WaveNumber = WaveNumber.Six;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Six)
                        {
                            gameManager.WaveNumber = WaveNumber.Seven;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Easy;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Seven)
                        {
                            gameManager.WaveNumber = WaveNumber.Eight;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Medium;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Eight)
                        {
                            gameManager.WaveNumber = WaveNumber.Nine;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Nine)
                        {
                            gameManager.WaveNumber = WaveNumber.Ten;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Medium;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Ten)
                        {
                            gameManager.WaveNumber = WaveNumber.Eleven;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Eleven)
                        {
                            gameManager.WaveNumber = WaveNumber.Twelve;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Easy;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Twelve)
                        {
                            gameManager.WaveNumber = WaveNumber.Thirteen;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Thirteen)
                        {
                            gameManager.WaveNumber = WaveNumber.Fourteen;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Medium;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Fourteen)
                        {
                            gameManager.WaveNumber = WaveNumber.Fifteen;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                            unlockables.UnlockPoints++;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Fifteen)
                        {
                            gameManager.GameState = GameState.YouWin;
                            unlockables.UnlockPoints++;
                            break;
                        }
                    }
                    break;
                #endregion

                #region Editor Game State
                case GameState.EditorGame:
                    //arenaManagerCounter = 0;
                    camera.MinimumZoom = 0.1f;
                    camera.MaximumZoom = 1.0f;
                    camera.Zoom = 0.5f;

                    //Gives location of the health bar and stamina bar and their backgrounds
                    healthBar = new Rectangle(player.Position.X - 1245, player.Position.Y - 1000, player.Health * 4, textureManager.Healthbar.Height);
                    staminaBar = new Rectangle(player.Position.X - 1245, player.Position.Y - 900, player.Stamina * 4, textureManager.Staminabar.Height);

                    healthBack = new Rectangle(player.Position.X - 1250, player.Position.Y - 1006, (player.HealthMax * 4) + 10, textureManager.Healthbar.Height + 11);
                    staminaBack = new Rectangle(player.Position.X - 1250, player.Position.Y - 906, (player.StaminaMax * 4) + 10, textureManager.Staminabar.Height + 11);

                    //Used to Indicate and let the player know what wave they are on
                    waveIndicator = new Vector2(player.Position.X - 1250, player.Position.Y - 775);


                    // Stuff for moving player and enemy, as well as player attack
                    ms = Mouse.GetState();
                    gameManager.PlayerMove(kbState, previouskbState, ms, previousMs, camera, gameTime);

                    gameManager.EnemyManager.EnemyMove(gameTime, gameManager.Platforms, gameManager.Player, gameManager.LeftWalls, gameManager.RightWalls);
                    if (kbState.IsKeyDown(Keys.P))
                    {
                        gameManager.GameState = GameState.Pause;
                    }

                    if (player.IsDead || player.Y >= GraphicsDevice.Viewport.Height)
                    {
                        gameManager.GameState = GameState.GameOver;
                        soundManager.PlayerDeath.Play();
                    }

                    //When all enemies are dead calls in the next wave method
                    if (gameManager.EnemyManager.Enemies.Count == 0)
                    {
                        if (gameManager.WaveNumber == WaveNumber.One)
                        {
                            gameManager.WaveNumber = WaveNumber.Two;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Medium;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Two)
                        {
                            gameManager.WaveNumber = WaveNumber.Three;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Hard;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Three)
                        {
                            gameManager.WaveNumber = WaveNumber.Four;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Mean;
                        }
                        else if (gameManager.WaveNumber == WaveNumber.Four)
                        {
                            gameManager.WaveNumber = WaveNumber.Five;
                            gameManager.GameState = GameState.NextWave;
                            gameManager.PlatformVersion = PlatformVersion.Life;

                        }
                        else if (gameManager.WaveNumber == WaveNumber.Five)
                        {
                            gameManager.GameState = GameState.YouWin;
                        }
                        
                    }
                    break;
                #endregion

                #region Pause State
                case GameState.Pause:
                    camera.Zoom = 1.0f;
                    camera.Position = new Vector2(0, 0);

                    if (returnToGameButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed)
                    {
                        gameManager.GameState = GameState.PresetGame;
                    }
                    else if (exitGameButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        Exit();
                    }
                    else if (pauseUnlockablesButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.UnlockablesMenu;
                    }

                    break;
                #endregion

                #region Upgrade State
                case GameState.UpgradeMenu:
                    if (returnToNewWaveButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed)
                    {
                        gameManager.GameState = GameState.NextWave;
                    }

                    if (groundPoundButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.UgManager.ActivateGroundPound();
                    }
                    else if (attackUpButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.Damage = gameManager.Player.UgManager.AttackUpgrade(gameManager.Player.Damage);
                    }
                    else if (staminaUpButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.UgManager.StaminaUpgrade(gameManager.Player);
                    }
                    else if (healthUpButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.UgManager.HealthUpgrade(gameManager.Player);
                    }
                    else if (dashButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.UgManager.ActivateDash();
                    }
                    else if (dashDistanceUpButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.Player.DashDistance = gameManager.Player.UgManager.DashDistanceUpgrade(gameManager.Player.DashDistance);
                    }
                    break;
                #endregion

                #region Unlockables Menu State
                case GameState.UnlockablesMenu:

                    if (unlockablesReturnToPauseButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = gameManager.PrevGameState;
                        savedUnlockables.Save(unlockables.UnlockPoints, unlockables.ItemsList);
                    }

                    //Deals with the Fez hat
                    if (fezButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Checks to see if the Fez is not unlocked
                        if (unlockables.Fez.Unlocked == false)
                        {
                            //If not unlocked will buy the Fez
                            unlockables.Buy(unlockables.Fez);
                        }

                        //If it is unlocked will check to see if it is not equipped
                        if (unlockables.Fez.Equipped == false && unlockables.Fez.Unlocked == true)
                        {
                            //If not it will equip the item
                            unlockables.Equip(unlockables.Fez);
                        }                       
                        else if(unlockables.Fez.Equipped == true)
                        {
                            //If it's unlocked and equipped, it will unequip it
                            unlockables.Unequip(unlockables.Fez);
                        }
                    }

                    //Deals with the Cowboy hat
                    if (cowboyButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Checks to see if the cowboy hat is not unlocked
                        if (unlockables.Cowboy.Unlocked == false)
                        {
                            //If not unlocked will buy the Cowboy hat
                            unlockables.Buy(unlockables.Cowboy);
                        }

                        //If it is unlocked will check to see if it is not equipped
                        if (unlockables.Cowboy.Equipped == false && unlockables.Cowboy.Unlocked == true)
                        {
                            //If not it will equip the item
                            unlockables.Equip(unlockables.Cowboy);
                        }
                        else if (unlockables.Cowboy.Equipped == true)
                        {
                            //If it's unlocked and equipped, it will unequip it
                            unlockables.Unequip(unlockables.Cowboy);
                        }
                    }

                    //Deals with the ushanka hat
                    if (ushankaButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Checks to see if the ushanka is not unlocked
                        if (unlockables.Ushanka.Unlocked == false)
                        {
                            //If not unlocked will buy the ushanka
                            unlockables.Buy(unlockables.Ushanka);
                        }

                        //If it is unlocked will check to see if it is not equipped
                        if (unlockables.Ushanka.Equipped == false && unlockables.Ushanka.Unlocked == true)
                        {
                            //If not it will equip the item
                            unlockables.Equip(unlockables.Ushanka);
                        }
                        else if (unlockables.Ushanka.Equipped == true)
                        {
                            //If it's unlocked and equipped, it will unequip it
                            unlockables.Unequip(unlockables.Ushanka);
                        }
                    }

                    //Deals with the Beret
                    if (beretButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Checks to see if the beret is not unlocked
                        if (unlockables.Beret.Unlocked == false)
                        {
                            //If not unlocked will buy the beret
                            unlockables.Buy(unlockables.Beret);
                        }

                        //If it is unlocked will check to see if it is not equipped
                        if (unlockables.Beret.Equipped == false && unlockables.Beret.Unlocked == true)
                        {
                            //If not it will equip the item
                            unlockables.Equip(unlockables.Beret);
                        }
                        else if (unlockables.Beret.Equipped == true)
                        {
                            //If it's unlocked and equipped, it will unequip it
                            unlockables.Unequip(unlockables.Beret);
                        }
                    }

                    //Deals with the sombrero hat
                    if (sombreroButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Checks to see if the sombrero is not unlocked
                        if (unlockables.Sombrero.Unlocked == false)
                        {
                            //If not unlocked will buy the sombrero
                            unlockables.Buy(unlockables.Sombrero);
                        }

                        //If it is unlocked will check to see if it is not equipped
                        if (unlockables.Sombrero.Equipped == false && unlockables.Sombrero.Unlocked == true)
                        {
                            //If not it will equip the item
                            unlockables.Equip(unlockables.Sombrero);
                        }
                        else if (unlockables.Sombrero.Equipped == true)
                        {
                            //If it's unlocked and equipped, it will unequip it
                            unlockables.Unequip(unlockables.Sombrero);
                        }
                    }

                    //Deals with the default option
                    if (defaultButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        //Makes the skin now Default
                        unlockables.MakeDefault();
                    }

                    unlockables.UnlockableUpdate(gameManager.EnemyManager);
                    break;
                #endregion

                #region Next Wave State
                case GameState.NextWave:
                    camera.Zoom = 1.0f;
                    camera.Position = new Vector2(0, 0);

                    savedUnlockables.Save(unlockables.UnlockPoints, unlockables.ItemsList);

                    //Reset Game
                    if (editedGame)
                    {
                        gameManager.ResetForNextWaveEdited(camera, rng, worldLeftEndWidth, worldRightEndWidth, gameTime, waveNumber, platformManager);
                    }
                    else
                    {
                        gameManager.ResetForNextWavePreset(camera, rng, worldLeftEndWidth, worldRightEndWidth, gameTime, waveNumber, platformManager);
                    }

                    if (goOnButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released && editedGame)
                    {
                        gameManager.GameState = GameState.EditorGame;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(soundManager.Song);
                    }
                    else if (goOnButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released && !editedGame)
                    {
                        gameManager.GameState = GameState.PresetGame;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(soundManager.Song);
                    }
                    else if (upgradesButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.UpgradeMenu;
                    }
                    else if (nextWaveUnlockablesButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.UnlockablesMenu;
                    }
                    break;
                #endregion

                #region Game Over State
                case GameState.GameOver:

                    
                     
                    camera.Zoom = 1.0f;
                    camera.Position = new Vector2(0, 0);
                    gameManager.WaveNumber = WaveNumber.One;

                    // Press enter to go back to menu
                    if (returnToMenuButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.Menu;
                        MediaPlayer.Stop();
                    }
                    break;
                #endregion

                #region Win State
                case GameState.YouWin:
                    camera.Zoom = 1.0f;
                    camera.Position = new Vector2(0, 0);
                    gameManager.WaveNumber = WaveNumber.One;

                    // Press enter to go back to menu
                    if (returnToMenuButton.Intersects(ms.Position) && ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released)
                    {
                        gameManager.GameState = GameState.Menu;
                        MediaPlayer.Stop();
                    }
                    break;
#endregion
            }
        }
    }
}
