﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using System.Threading;

namespace Purpose
{
    public class GameManager
    {
        #region Fields
        private Player player;
        private List<Platform> platforms;
        private List<Platform> leftWalls;
        private List<Platform> rightWalls;
        private bool isCrouching;
        private GraphicsDevice graphicsDevice;
        private GameState gameState;
        private Texture2D background;
        private int enemyJumpNum;
        private WaveNumber waveNumber;
        private int gravity = -2;
        private PlatformVersion platformVersion;
        private EnemyManager enemyManager;
        private SoundManager soundManager;
        private GameState prevGameState;

        //private List<Wave> waves;
        private List<Wave> editedWaves;
        private List<Wave> presetWaves;

        private TextureManager textureManager;
        private Texture2D rangeTexture;

        int keyCounter;
        int frameCounter;

        int healthFrameCounter;
        int staminaFrameCounter;

        bool jumping;
        #endregion

        #region Properties
        public Player Player { get { return player; } }
        public List<Platform> Platforms { get { return platforms; } }
        public bool IsCrouching
        {
            get { return isCrouching; }
            set { isCrouching = value; }
        }
        public GraphicsDevice GraphicsDevice { get { return graphicsDevice; } }

        public GameState GameState
        {
            get { return gameState; }
            set
            {
                prevGameState = gameState;
                gameState = value;
            }
        }

        public Texture2D Background
        {
            get { return background; }
            set { background = value; }
        }

        public List<Wave> EditedWaves
        {
            get { return editedWaves; }
            set { editedWaves = value; }
        }

        public List<Wave> PresetWaves
        {
            get { return presetWaves; }
            set { presetWaves = value; }
        }

        public int EnemyJumpNum
        {
            get { return enemyJumpNum; }
            set { enemyJumpNum = value; }
        }

        public Texture2D RangeTexture
        {
            get { return rangeTexture; }
            set { rangeTexture = value; }
        }

        public WaveNumber WaveNumber { get { return waveNumber; } set { waveNumber = value; } }

        public PlatformVersion PlatformVersion
        {
            get { return platformVersion; }
            set { platformVersion = value; }
        }

        public EnemyManager EnemyManager
        {
            get { return enemyManager; }
        }

        public GameState PrevGameState
        {
            get { return prevGameState; }
        }

        public List<Platform> LeftWalls
        {
            get { return leftWalls; }
        }

        public List<Platform> RightWalls
        {
            get { return rightWalls; }
        }
        #endregion

        //constructor
        public GameManager(Player player, List<Platform> platforms, List<Platform> leftWalls, List<Platform> rightWalls, GraphicsDevice graphicsDevice,
            TextureManager textureManager, SoundManager soundManager)
        {
            this.player = player;
            this.platforms = platforms;
            this.leftWalls = leftWalls;
            this.rightWalls = rightWalls;
            isCrouching = false;
            this.graphicsDevice = graphicsDevice;
            this.textureManager = textureManager;
            this.soundManager = soundManager;
            editedWaves = new List<Wave>();
            presetWaves = new List<Wave>();
            rangeTexture = textureManager.RangedEnemyTexture;
            waveNumber = WaveNumber.One;
            platformVersion = PlatformVersion.Easy;
            enemyManager = new EnemyManager(graphicsDevice, textureManager);

            //a boolean representing if the player is on the platform
            jumping = false;
            prevGameState = GameState.Menu;
        }

        //methods
        /// <summary>
        /// Allows the player to move and activate abilities
        /// </summary>
        /// <param name="kbState">The current state of teh keyboard</param>
        /// <param name="previouskbState">The previous state of the keyboard</param>
        /// <param name="ms">The current mouse state</param>
        /// <param name="previousMs">The previous mouse state</param>
        public void PlayerMove(KeyboardState kbState, KeyboardState previouskbState, MouseState ms, MouseState previousMs, Camera2D camera, GameTime gameTime)
        {

            #region Health and Stamina Regen
            if(player.CombatStatus == false)
            {
                if (player.Stamina < player.StaminaMax && staminaFrameCounter >= 20)
                {
                    player.Stamina += player.StaminaRegen;
                    staminaFrameCounter = 0;

                    //Make sure stamina doesn't regen more then needed
                    if(player.Stamina > player.StaminaMax)
                    {
                        //If it goes past the max
                        //Puts the stamina to normal
                        player.Stamina = player.StaminaMax;
                    }
                }
                else
                {
                    staminaFrameCounter++;
                }
                if (player.Health < player.HealthMax && healthFrameCounter >= 80)
                {
                    player.Health += player.HealthRegen;
                    healthFrameCounter = 0;

                    //Makes sure health doesn't regen more then it should
                    //Some cases health would end at 101 if health was odd
                    if (player.Health > player.HealthMax)
                    {
                        //If it goes past the max health
                        //Puts health back to max
                        player.Health = player.HealthMax;
                    }
                }
                else
                {
                    healthFrameCounter++;
                }
            }
#endregion

            #region Platform Collisions
            bool onPlatform = false;

            if (player.Velocity > 0)
            {
                foreach (Platform p in platforms)
                {
                    if (new Rectangle(player.X, player.Y + 330, player.Position.Width, 22).Intersects(
                        new Rectangle(p.X, p.Y, p.Position.Width, 25)))
                    {
                        player.Velocity = 0;
                        onPlatform = true;
                        jumping = false;
                        break;
                    }
                }
            }

            //if not, make them fall
            if (!onPlatform)
            {
                player.Y += player.Velocity;
                player.Velocity -= gravity;
                //moving camera with player
                camera.LookAt(new Vector2(player.X, player.Y - 250));
            }

            // Wall collisions
            foreach (Platform w in rightWalls)
            {
                if (player.Position.Intersects(w.Position))
                {
                    player.X = w.Position.X - player.Position.Width;
                }
            }

            foreach (Platform w in leftWalls)
            {
                if (player.Position.Intersects(w.Position))
                {
                    player.X = w.Position.X + w.Position.Width;
                }
            }
#endregion

            #region Attack Animation
            if (player.Texture == textureManager.LeftPlayerAttack1)
            {
                frameCounter++;
                if (frameCounter == 5)
                {
                    player.Texture = textureManager.LeftPlayerAttack2;
                    frameCounter = 0;
                }
                return;
            }
            else if (player.Texture == textureManager.RightPlayerAttack1)
            {
                frameCounter++;
                if (frameCounter == 5)
                {
                    player.Texture = textureManager.RightPlayerAttack2;
                    frameCounter = 0;
                }
                return;
            }
            else if (player.Texture == textureManager.LeftPlayerAttack2)
            {
                frameCounter++;
                if (frameCounter == 5)
                {
                    player.Texture = textureManager.LeftStandingSprite;
                    frameCounter = 0;
                }
                else { return; }
            }
            else if (player.Texture == textureManager.RightPlayerAttack2)
            {
                frameCounter++;
                if (frameCounter == 5)
                {
                    player.Texture = textureManager.RightStandingSprite;
                    frameCounter = 0;
                }
                else { return; }
            }
            #endregion

            #region Keyboard and Mouse Input
            //checking keyboard state to make the player move
            if (kbState.IsKeyDown(Keys.A) || kbState.IsKeyDown(Keys.Left)) //move to the left
            {
                //if neither key was down previously reset keyCounter
                if (previouskbState.IsKeyUp(Keys.A) && previouskbState.IsKeyUp(Keys.Left))
                {
                    keyCounter = 0;
                }
                //add to the key counter if the key has continuously been down
                if (previouskbState.IsKeyDown(Keys.A) || previouskbState.IsKeyDown(Keys.Left))
                {
                    keyCounter++;
                }
                //if keyCounter is 0, change to the appropriate texture
                if (keyCounter == 0)
                {
                    player.Texture = textureManager.LeftStandingSprite;
                }

                // Has the player been pressing the key for long enough?
                if (keyCounter >= 5)
                {
                    keyCounter = 0;
                    // Update the frame and wrap
                    if (player.Texture == textureManager.LeftStandingSprite)
                    {
                        player.Texture = textureManager.LeftMiddleRunningSprite;
                    }
                    else if (player.Texture == textureManager.LeftMiddleRunningSprite)
                    {
                        player.Texture = textureManager.LeftRunningSprite;
                    }
                    else if (player.Texture == textureManager.LeftRunningSprite)
                    {
                        player.Texture = textureManager.LeftStandingSprite;
                    }
                }
                if (Math.Abs(player.PreviousX - player.X) >= player.DashDistance)
                {
                    player.HorizontalVelocity = 8;
                }
                player.X -= player.HorizontalVelocity;
                // moving camera with player
                camera.LookAt(new Vector2(player.X, player.Y - 250));
            }
            if (kbState.IsKeyDown(Keys.D) || kbState.IsKeyDown(Keys.Right)) //move to the right
            {
                //neither key was down previously reset the counter
                if (previouskbState.IsKeyUp(Keys.D) && previouskbState.IsKeyUp(Keys.Right))
                {
                    keyCounter = 0;
                }
                //if either key was down continously add to the counter
                if (previouskbState.IsKeyDown(Keys.D) || previouskbState.IsKeyDown(Keys.Right))
                {
                    keyCounter++;
                }
                //if keyCounter is 0 change to the appropriate texture
                if (keyCounter == 0)
                {
                    player.Texture = textureManager.RightStandingSprite;
                }

                // Has the player been pressing the key for long enough
                if (keyCounter >= 5)
                {
                    keyCounter = 0;
                    // Update the frame and wrap
                    if (player.Texture == textureManager.RightStandingSprite)
                    {
                        player.Texture = textureManager.RightMiddleRunningSprite;
                    }
                    else if (player.Texture == textureManager.RightMiddleRunningSprite)
                    {
                        player.Texture = textureManager.RightRunningSprite;
                    }
                    else if (player.Texture == textureManager.RightRunningSprite)
                    {
                        player.Texture = textureManager.RightStandingSprite;
                    }
                }
                if (Math.Abs(player.PreviousX - player.X) >= player.DashDistance)
                {
                    player.HorizontalVelocity = 8;
                }
                player.X += player.HorizontalVelocity;
                // moving camera with player
                camera.LookAt(new Vector2(player.X, player.Y - 250));
            }
            if (kbState.IsKeyDown(Keys.Space) && !previouskbState.IsKeyDown(Keys.Space) && !isCrouching && !jumping)//jump
            {
                player.Jump();
                jumping = true;
            }
            if (kbState.IsKeyDown(Keys.LeftShift) && !previouskbState.IsKeyDown(Keys.LeftShift)) //dash
            {
                player.Dash();
                camera.LookAt(new Vector2(player.X, player.Y - 250));
            }

            // Player attack done here as well as enemy takeDamage
            if (ms.LeftButton == ButtonState.Pressed && previousMs.LeftButton == ButtonState.Released && !isCrouching)
            {
                for (int i = 0; i < enemyManager.Enemies.Count; i++)
                {
                    int damage = player.Attack(enemyManager.Enemies[i], gameTime, soundManager);
                    enemyManager.Enemies[i].TakeDamage(damage);

                    if (player.Texture == textureManager.LeftJumpSprite || player.Texture == textureManager.LeftRunningSprite
                        || player.Texture == textureManager.LeftStandingSprite || player.Texture == textureManager.LeftMiddleRunningSprite)
                    {
                        player.Texture = textureManager.LeftPlayerAttack1;
                    }
                    else if (player.Texture == textureManager.RightJumpSprite || player.Texture == textureManager.RightRunningSprite
                        || player.Texture == textureManager.RightStandingSprite || player.Texture == textureManager.RightMiddleRunningSprite)
                    {
                        player.Texture = textureManager.RightPlayerAttack1;
                    }
                    
                    if (enemyManager.Enemies[i].IsDead)
                    {
                        enemyManager.Enemies.RemoveAt(i);
                        player.Kills++;
                        player.CombatStatus = false;
                        if (player.Kills != 0 && player.Kills % 5 == 0)
                        {
                            player.UgManager.UpgradePoints++;
                        }
                    }
                }
            }
            if (kbState.IsKeyDown(Keys.E) && previouskbState.IsKeyUp(Keys.E) && ms.LeftButton == ButtonState.Released)
            {
                player.GroundPound(enemyManager.Enemies);
            }
            #endregion

            foreach(Enemy e in enemyManager.Enemies)
            {
                // Call attack methods based on type of enemy and position
                int damage = 0;
                if (e.Ranged && e.IsFacingLeft)
                {
                    damage = e.Attack(player, gameTime, soundManager);
                }

                else if(e.Ranged && !e.IsFacingLeft)
                {
                    damage = e.Attack(player, gameTime, soundManager);
                }

                else if(!e.Ranged && e.IsFacingLeft)
                {
                    damage = e.Attack(player, gameTime, soundManager);
                }

                else if (!e.Ranged && !e.IsFacingLeft)
                {
                    damage = e.Attack(player, gameTime, soundManager);
                }
                player.TakeDamage(damage);

                
            }
        }

        /// <summary>
        /// Resets game to beginning
        /// </summary>
        public void ResetOnPlayerDeathEdited(Camera2D camera, Random rng, int worldLeftEndWidth, int worldRightEndWidth, GameTime gameTime, 
            PlatformManager platformManager)
        {
            player.Health = 100;
            player.HealthMax = player.Health;
            camera.Zoom = 1.0f;
            player.IsDead = false;
            player.X = 175;
            player.Y = 175;
            isCrouching = false;
            EnemyManager.Enemies.Clear();
            enemyManager.FillEnemyList(rng, editedWaves[0].NumberOfMelee, editedWaves[0].Difficulty, gameTime, PlatformVersion.Easy);
            enemyManager.FillRangedList(rng, editedWaves[0].NumberOfRanged, editedWaves[0].Difficulty, gameTime, PlatformVersion.Easy);
            platformManager.ClearPlatformLists();
            platformManager.MakePlatforms(PlatformVersion.Easy, graphicsDevice, textureManager);
            player.UgManager.ResetUpgrades();

        }

        public void ResetForNextWaveEdited(Camera2D camera, Random rng, int worldLeftEndWidth, int worldRightEndWidth, GameTime gameTime, 
            int waveNumber, PlatformManager platformManager)
        {
            camera.Zoom = 1.0f;
            player.X = 175;
            player.Y = 175;
            isCrouching = false;
            enemyManager.Enemies.Clear();
            enemyManager.FillEnemyList(rng, editedWaves[waveNumber].NumberOfMelee, editedWaves[waveNumber].Difficulty, gameTime, platformVersion);
            enemyManager.FillRangedList(rng, editedWaves[waveNumber].NumberOfRanged, editedWaves[waveNumber].Difficulty, gameTime, platformVersion);
            platformManager.ClearPlatformLists();
            platformManager.MakePlatforms(platformVersion, graphicsDevice, textureManager);
        }

        public void ResetOnPlayerDeathPreset(Camera2D camera, Random rng, int worldLeftEndWidth, int worldRightEndWidth, GameTime gameTime, 
            PlatformManager platformManager)
        {
            player.Health = 100;
            player.HealthMax = player.Health;
            player.Stamina = 100;
            player.StaminaMax = player.Stamina;
            camera.Zoom = 1.0f;
            player.IsDead = false;
            player.X = 175;
            player.Y = 175;
            player.HealthRegen = 2;
            player.StaminaRegen = 1;
            player.Damage = 10;
            isCrouching = false;
            EnemyManager.Enemies.Clear();
            enemyManager.FillEnemyList(rng, presetWaves[0].NumberOfMelee, presetWaves[0].Difficulty, gameTime, PlatformVersion.Easy);
            enemyManager.FillRangedList(rng, presetWaves[0].NumberOfRanged, presetWaves[0].Difficulty, gameTime, PlatformVersion.Easy);
            platformManager.ClearPlatformLists();
            platformManager.MakePlatforms(PlatformVersion.Easy, graphicsDevice, textureManager);
            player.UgManager.ResetUpgrades();
        }

        public void ResetForNextWavePreset(Camera2D camera, Random rng, int worldLeftEndWidth, int worldRightEndWidth, GameTime gameTime, 
            int waveNumber, PlatformManager platformManager)
        {
            camera.Zoom = 1.0f;
            player.X = 175;
            player.Y = 175;
            isCrouching = false;
            EnemyManager.Enemies.Clear();
            enemyManager.FillEnemyList(rng, presetWaves[waveNumber].NumberOfMelee, presetWaves[waveNumber].Difficulty, gameTime,platformVersion);
            enemyManager.FillRangedList(rng, presetWaves[waveNumber].NumberOfRanged, presetWaves[waveNumber].Difficulty, gameTime, platformVersion);
            platformManager.ClearPlatformLists();
            platformManager.MakePlatforms(platformVersion, graphicsDevice, textureManager);
        }
    }
}
