/* ===============================================================
 *                     GAME TITLE: DEADMOCRACY 
 * ===============================================================
 *                            CONTROLS
 * ---------------------------------------------------------------
 *  Move player:
 *   - KEYBOARD: W, A, S, D or UP, LEFT, DOWN, RIGHT arrow keys
 *   
 *  Throw weapon / Attack:
 *   - MOUSE: Left Button
 *   - KEYBOARD: SPACE key
 *   
 *  Change weapon:
 *   - 1 KEY: Axe
 *   - 2 KEY: Grenade
 *  
 *  To Aim:
 *   - Move the crosshair in the direction you want to shoot
 *   
 *  To Shoot
 *   - Hold down the shoot key to increase throw strength then
 *     release it to throw weapon
 *  
 *  In-game Debug Console / Immunity
 *   - KEYBOARD: F12 to toggle / F11 to pause enemies
 */

using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Sprites;
using System;
using System.Collections.Generic;
using UserInterface;

namespace Deadmocracy
{
    #region Enums
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver,
        HighScores,
        Credits
    }
    public enum Direction
    {
        Right,
        Left
    }
    #endregion

    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        // Static properties
        public static Vector2 Resolution { get; private set; }
        private static GameState CurrentGameState { get; set; }
        private static GameState PreviousGameState { get; set; }
        public Random r = new Random();

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Background background;
        Player player;
        Crosshair crosshair;
        List<Zombie> zombies;
        List<Explosion> explosions;
        List<BloodParticle> blood;
        Level level;
        float smallWaveTimer;
        float bigWaveTimer;
        Song menuSong, gameSong, scoresSong;
        float timeScale;
        float slowMotionTimer;
        Camera camera;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Resolution = new Vector2(1280, 720);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferWidth = (int)Resolution.X;
            graphics.PreferredBackBufferHeight = (int)Resolution.Y;
            Content.RootDirectory = "Content";
            camera = new Camera(this, new Vector2(Resolution.X / 2, Resolution.Y / 2));
            Components.Add(new Input(this));        // » InputManager Component
            Components.Add(new Debug(this));        // » Debug console Component
            Components.Add(camera);                 // » Camera Component
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            player = new Player();                      // » Initialize player object
            crosshair = new Crosshair();                // » Initialize crosshair object
            zombies = new List<Zombie>();               // » Create empty list for zombies
            explosions = new List<Explosion>();         // » Create empty list for explosions
            blood = new List<BloodParticle>();          // » Create empty list for blood particles
            background = new Background();              // » Initialize background object
            level = new Level(80);                      // » Initialize initial level object

            SetGameState(GameState.MainMenu);           // » Set initial game state
            IsMouseVisible = true;                      // » Set initial mouse visibility
            MediaPlayer.Volume = 0.8f;                  // » Set game music volume
            timeScale = 1.0f;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background.Load(Content);                   // » Load background resources
            player.Load(Content);                       // » Load player resources
            crosshair.Load(Content);                    // » Load crosshair resources
            HUD.Load(Content);                          // » Load Heads Up Display resources
            MenuScreen.Load(Content, GraphicsDevice);   // » Load Heads Up Display resources
            MainMenu.Load(Content);                     // » Load Main Menu screen resources
            GameOver.Load(Content);                     // » Load Game Over screen resources
            HighScores.Load(Content);                   // » Load High Scores screen resources
            Credits.Load(Content);                      // » Load Credits screen resources

            // Load music
            menuSong = Content.Load<Song>("sounds/menu");
            gameSong = Content.Load<Song>("sounds/playing");
            scoresSong = Content.Load<Song>("sounds/scores");

            MediaPlayer.Play(menuSong);

            CreateNewWeapon();                      // » Create and Load a new weapon
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Gets the number of elapsed seconds since the last update (for use in all movement calculations)
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds * timeScale;

            // Slow motion processing
            SlowMotion((float)gameTime.ElapsedGameTime.TotalSeconds);

            switch (CurrentGameState)
            {
                #region Main Menu / Pause Screen
                case GameState.MainMenu:
                case GameState.Paused:

                    MainMenu.Update(time);

                    if (MainMenu.Buttons["start"].Selected)
                    {
                        SetGameState(GameState.Playing);
                        IsMouseVisible = false;
                        MediaPlayer.Stop();
                        MediaPlayer.Play(gameSong);
                    }

                    if (MainMenu.Buttons["highScores"].Selected)
                        SetGameState(GameState.HighScores);

                    if (MainMenu.Buttons["fcredits"].Selected)
                        SetGameState(GameState.Credits);

                    if (MainMenu.Buttons["exit"].Selected)
                        Exit();

                    break;
                #endregion

                #region Game Over screen
                case GameState.GameOver:

                    GameOver.Update(time);

                    if (GameOver.IsScoreSubmitted)
                        SetGameState(GameState.HighScores);

                    break;
                #endregion

                #region High Scores screen
                case GameState.HighScores:

                    HighScores.Update(time);

                    if (HighScores.Buttons["back"].Selected)
                    {
                        if (PreviousGameState == GameState.Paused || PreviousGameState == GameState.MainMenu)
                            SetGameState(GameState.MainMenu);
                        else
                            RestartGame();
                    }

                    break;
                #endregion

                #region Credits screen
                case GameState.Credits:

                    Credits.Update(time);

                    if (Credits.Buttons["back"].Selected)
                        SetGameState(GameState.MainMenu);

                    break;
                #endregion

                #region Main Game State
                case GameState.Playing:

                    HUD.Update(time);                          // » Update HUD
                    player.Update(time);                       // » Update player
                    crosshair.Update(time);                    // » Update crosshair
                    background.Update(time);                   // » Update background
                    Weapon.ActiveWeapon.Update(time);          // » Update active weapon
                    explosions.ForEach(e => e.Update(time));   // » Update explosions
                    blood.ForEach(b => b.Update(time));        // » Update blood particles

                    // Create a new weapon if player changed weapons
                    if (Weapon.ActiveWeapon.type != player.EquippedWeapon)
                        CreateNewWeapon();

                    // Create a new weapon if the previous one was destroyed
                    if (Weapon.ActiveWeapon.state == Weapon.State.Destroyed)
                    {
                        // Spawn an explosion if the destroyed weapon was a grenade
                        if (Weapon.ActiveWeapon is Grenade)
                            CreateExplosion(Weapon.ActiveWeapon.Center);

                        CreateNewWeapon();
                    }

                    // Update all active zombies
                    int zombiesHit = 0;
                    foreach (Zombie z in zombies)
                    {
                        // Collision with player
                        if (z.CollidesWith(player))
                        {
                            if (!player.IsImmune) Bloood(player.Bounds, (player.Center - z.Center) * 5, 50);
                            player.TakeDamage(z.AttackDamage);
                        }

                        // Collision with thrown projectile
                        if (Weapon.ActiveWeapon.state == Weapon.State.Throwing
                            && Weapon.ActiveWeapon.HitsEnemy(z) && Weapon.ActiveWeapon.CanHit)
                        {
                            z.TakeDamage(Weapon.ActiveWeapon.Damage);
                            Bloood(z.Bounds, Weapon.ActiveWeapon.Velocity, 150);
                            Weapon.ActiveWeapon.state = Weapon.State.Destroyed;
                        }

                        // Collision with explosion
                        foreach (Explosion e in explosions)
                            if (z.CollidesWith(e))
                            {
                                z.TakeDamage(Weapon.ActiveWeapon.Damage);
                                Bloood(z.Bounds, (z.Center - e.Center) * 5, 150);
                                zombiesHit++;
                                if (zombiesHit > 3)
                                    EnterSlowMotion(1.5f);
                            }

                        // On zombie death increase player stats
                        if (z.state == Zombie.State.Dead)
                        {
                            player.GainXP(z.XPValue);
                            player.score.Kills++;
                        }

                        // Keep zombie crowds spread out between each other during movement
                        foreach (Zombie y in zombies)
                            if (z != y) z.KeepDistance(y);

                        z.Update(time);
                    }

                    // Periodically generate some zombies
                    GenerateZombieWaves(time);

                    // Remove dead zombies
                    zombies.RemoveAll(z => z.state == Zombie.State.Dead);

                    // Remove expired explosions
                    explosions.RemoveAll(e => e.state == Explosion.State.Ended);

                    // Remove expired blood particles
                    blood.RemoveAll(b => b.Expired);

                    // If the player is dead end the game
                    if (player.HP <= 0)
                    {
                        SetGameState(GameState.GameOver);
                        MediaPlayer.Stop();
                        MediaPlayer.Play(scoresSong);
                    }

                    // If the player has reached the end of the level addtime to the score and end the game
                    if (player.score.TotalDistance > level.DistanceToObjective)
                    {
                        level.IsCompleted = true;
                        player.score.TotalScore += (int)(10000 * (1.34 - Math.Log(player.score.TotalTime, 120)));
                        SetGameState(GameState.GameOver);
                        MediaPlayer.Stop();
                        MediaPlayer.Play(scoresSong);
                    }

                    // Pressing ESC key pauses the game
                    if (Input.IsKeyPressed(Keys.Escape))
                    {
                        SetGameState(GameState.Paused);
                        MainMenu.Buttons["start"].Text = "CONTINUE";
                        IsMouseVisible = true;
                    }

                    break;
                    #endregion
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            if (CurrentGameState == GameState.Playing)
            {
                GraphicsDevice.Clear(new Color(30, 31, 60));    // » Fill background with gray road color
                // Using the PointClamp sampler state the textures will keep the pixelated look when scaling
                spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.GetTransformation());

                background.Draw(spriteBatch);                   // » Draw background
                player.Draw(spriteBatch);                       // » Draw player
                Weapon.ActiveWeapon.Draw(spriteBatch);          // » Draw active weapon
                zombies.ForEach(z => z.Draw(spriteBatch));      // » Draw all active zombies
                explosions.ForEach(e => e.Draw(spriteBatch));   // » Draw all active zombies
                blood.ForEach(b => b.Draw(spriteBatch));        // » Draw all active blood particles

                spriteBatch.End();
            }
            else
                GraphicsDevice.Clear(new Color(0, 107, 169));   // » Fill background with menu blue color

            #region UI elements
            // No camera transform applied in this spritebatch so camera shakes don't affect what's drawn here
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            switch (CurrentGameState)
            {
                case GameState.MainMenu:
                case GameState.Paused:
                    MainMenu.Draw(spriteBatch); break;

                case GameState.GameOver:
                    GameOver.Draw(spriteBatch); break;

                case GameState.HighScores:
                    HighScores.Draw(spriteBatch); break;

                case GameState.Credits:
                    Credits.Draw(spriteBatch); break;

                case GameState.Playing:
                    HUD.Draw(spriteBatch);                  // » Draw User Interface
                    crosshair.Draw(spriteBatch);            // » Draw crosshair
                    break;
            }
            spriteBatch.End();
            #endregion

            base.Draw(gameTime);
        }

        #region Custom Methods
        /// <summary>
        /// Changes the game state and saves the previous one.
        /// </summary>
        /// <param name="newGameState">The new game state to set.</param>
        private void SetGameState(GameState newGameState)
        {
            PreviousGameState = CurrentGameState;
            CurrentGameState = newGameState;
        }

        /// <summary>
        /// Reinitializes all the game elements to their initial value.
        /// </summary>
        private void RestartGame()
        {
            // Calling the Initialize() method results in LoadContent() being called as well
            Initialize();
        }

        /// <summary>
        /// Spawns a zombie wave.
        /// </summary>
        /// <param name="zombieAmount">Number of zombies to spawn.</param>
        /// <param name="includeSpecialZombies">Indicates whether special zombies can also spawn.</param>
        private void SpawnWave(int zombieAmount, bool includeSpecialZombies)
        {
            if (!Debug.PlayerTestMode) // » If player test mode is active don't spawn zombies
            {
                for (int i = 0; i < zombieAmount; i++)
                {
                    Zombie.Type zombieType;
                    if (includeSpecialZombies)
                    {
                        // Chose a random zombie type from all available types
                        Array types = Enum.GetValues(typeof(Zombie.Type));
                        zombieType = (Zombie.Type)types.GetValue(r.Next(0, types.Length));
                    }
                    else
                        zombieType = Zombie.Type.Citizen;

                    // Create zombie object in a random location, load it and add it to the list
                    Zombie z = Zombie.Spawn(zombieType, new Vector2(Resolution.X + r.Next(50, 200),
                        r.Next(Background.RoadBounds.Top, Background.RoadBounds.Bottom)));
                    z.Load(Content);
                    zombies.Add(z);
                }
                Debug.Log("Spawned wave of " + zombieAmount + " zombies");
            }
        }

        private void GenerateZombieWaves(float time)
        {
            // Small waves
            smallWaveTimer += time;
            if (smallWaveTimer >= 3)
            {
                SpawnWave(r.Next(2, 6), false);
                smallWaveTimer = 0;
            }

            // Big waves
            bigWaveTimer += time;
            if (bigWaveTimer >= 30)
            {
                SpawnWave(r.Next(10, 15), true);
                bigWaveTimer = 0;
            }
        }

        private void CreateNewWeapon()
        {
            Weapon.Create(player.EquippedWeapon);
            Weapon.ActiveWeapon.Load(Content);
        }

        /// <summary>
        /// Creates an explosion.
        /// </summary>
        /// <param name="position">The position of the explosion sprite</param>
        private void CreateExplosion(Vector2 position)
        {
            Explosion explosion = new Explosion(position);
            explosions.Add(explosion);
            explosion.Load(Content);
            camera.Shake(0.6f, 5, 0);
        }

        private void Bloood(Rectangle bounds, Vector2 direction, int ammount)
        {
            for (int i = 0; i < ammount; i++)
                blood.Add(new BloodParticle(bounds, direction, GraphicsDevice));
        }

        private void EnterSlowMotion(float duration)
        {
            slowMotionTimer = duration;
            camera.Shake(duration, 8, 0);
        }

        private void SlowMotion(float realTime)
        {
            if (slowMotionTimer > 0)
            {
                slowMotionTimer -= realTime;
                if (timeScale > 0.1f) timeScale -= 0.1f;
            }
            else
            {
                if (timeScale < 1) timeScale += 0.01f;
                else timeScale = 1;
            }
        }
        #endregion
    }
}