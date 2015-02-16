using Deadmocracy;
using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Microsoft.Xna.Framework.Content;

namespace Sprites
{
    public class Player : Sprite
    {
        // Static properties
        public static Player CurrentPlayer { get; private set; }    // » Holds the current player instance
        public static Vector2 ScrollOffset { get; private set; }    // » Velocity offset when player is scrolling the background

        // Properties
        public Weapon.Type EquippedWeapon { get; private set; }     // » The weapon currently equipped by the player
        public Direction IsFacing { get; private set; }             // » Direction in which the player is facing
        public Score score { get; private set; }                    // » Player score elements
        public float MaxHP { get; private set; }                    // » Maximum player hitpoints
        public float HP { get; private set; }                       // » Current player hitpoints
        public int XP { get; private set; }                         // » Current player experience points
        public bool IsImmune { get; private set; }


        // Attributes
        private Vector2 _speed;
        private Vector2 _direction;
        private float _immunityTimer;
        private bool _isFlashing;
        private float _flashTimer;
        private SoundEffect _hitSound;

        // Constructor
        public Player()
            : base()
        {
            CurrentPlayer = this;
            _spritesheets["default"] = "player_idle";
            _spritesheets["walking"] = "player_walking";
            _animationFrames = 8;
            _position = new Vector2(Game1.Resolution.X / 2, 400);
            _speed = new Vector2(120, 100);
            IsFacing = Direction.Right;
            MaxHP = 100;
            HP = MaxHP;
            EquippedWeapon = Weapon.Type.Axe;
            IsImmune = false;
            _color = Color.Yellow;
            score = new Score();
        }

        public override void Load(ContentManager content)
        {
            _hitSound = content.Load<SoundEffect>("sounds/hit");
            base.Load(content);
        }

        public override void Update(float time)
        {
            _direction = Vector2.Zero;

            if (Input.IsKeyDown(Keys.W) || Input.IsKeyDown(Keys.Up))
                _direction.Y = -1;

            if (Input.IsKeyDown(Keys.S) || Input.IsKeyDown(Keys.Down))
                _direction.Y = 1;

            if ((Input.IsKeyDown(Keys.A) || Input.IsKeyDown(Keys.Left)))
                _direction.X = -1;

            if (Input.IsKeyDown(Keys.D) || Input.IsKeyDown(Keys.Right))
                _direction.X = 1;

            if (Input.IsKeyDown(Keys.D1) && Weapon.ActiveWeapon.state != Weapon.State.Throwing)
                EquippedWeapon = Weapon.Type.Axe;

            if (Input.IsKeyDown(Keys.D2) && Weapon.ActiveWeapon.state != Weapon.State.Throwing)
                EquippedWeapon = Weapon.Type.Grenade;

            _velocity = _direction * _speed;

            if (_direction.X == -1) IsFacing = Direction.Left;
            if (_direction.X == 1) IsFacing = Direction.Right;

            if (Input.IsMouseDown())
            {
                if (Mouse.GetState().X > Center.X)
                    IsFacing = Direction.Right;
                else
                    IsFacing = Direction.Left;
            }

            // If player is out of bounds set a velocity that will be used to offset the other entities' positions
            // When scrolling, the player will also offset itself so it will remain "stopped"
            // removing the need to check for horizontal bounds on game area
            if (_direction.X < 0 && _bounds.Left <= Background.ScrollMargin.Left && Background.BuildingNumber > 0 ||
                _direction.X > 0 && _bounds.Right >= Background.ScrollMargin.Right)
            {
                ScrollOffset = new Vector2(_velocity.X, 0) * time;
            }
            else
                ScrollOffset = Vector2.Zero;

            // Switch player animation based on its movement
            if (_velocity.Length() > 0)
                SetAnimation("walking");
            else
                SetAnimation("default");

            FlashAnimation(time);   // » If player was damaged perform flash animation
            CalculateScore(time);      // » Update player score

            // Limit player movement to road area
            CheckBounds(Background.RoadBounds);

            // Track sprite properties on debug window
            Debug.Track(this);

            // Execute common code in base (Sprite) class
            base.Update(time);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Flip sprite according to the direction it's facing
            _spriteEffects = SpriteEffects.None;
            if (IsFacing == Direction.Left) _spriteEffects = SpriteEffects.FlipHorizontally;
            _layerDepth = 1 - BottomCenter.Y / 1000;

            base.Draw(spriteBatch);
        }

        protected override void CheckBounds(Rectangle area)
        {
            // Player can't go out of the screen to the left
            if (_position.X < 0) _position.X = 0;
            base.CheckBounds(area);
        }

        protected void FlashAnimation(float time)
        {
            if (IsImmune)
            {
                _flashTimer += time;
                if (_flashTimer > 0.1f)
                {
                    _isFlashing = !_isFlashing;
                    _flashTimer = 0;
                }
                if (!_isFlashing) _color *= 0f;
                else _color = Color.Yellow;
                _immunityTimer += time;
                if (_immunityTimer > 1)
                {
                    IsImmune = false;
                    _color = Color.Yellow;
                }
            }
        }

        public void TakeDamage(float ammount)
        {
            if (!IsImmune)
            {
                if (!Debug.PlayerTestMode && !Debug.IsOpen)
                    HP -= ammount;
                IsImmune = true;
                _immunityTimer = 0;
                score.TotalHpLost += (int)ammount;
                _hitSound.Play();
            }
        }

        public void GainXP(int ammount)
        {
            XP += ammount;
            score.TotalXP += XP;
        }

        private void CalculateScore(float time)
        {
            score.TotalTime += time;
            score.TotalDistance += (_velocity.X * time) / 50;
            score.TotalScore = score.TotalXP - (score.TotalHpLost * 2);
            if (score.TotalScore < 0)
                score.TotalScore = 0;
        }
    }
}
