using Deadmocracy;
using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Sprites
{
    public class Weapon : Sprite
    {
        #region Enums
        public enum Type
        {
            Axe,
            Grenade
        }
        public enum State
        {
            Equipped,
            PreparingThrow,
            Throwing,
            Destroyed
        }
        #endregion

        // Static properties
        public static Weapon ActiveWeapon { get; private set; }     // » Instance of the weapon currently active on the screen

        // Properties
        public Vector2 ThrowInitialPosition { get; private set; }   // » Initial position of weapon throw
        public float Damage { get; protected set; }                 // » Weapon damage value
        public bool CanHit { get; protected set; }                  // » Determines if weapon can hit enemies on direct collision
        public Type type { get; protected set; }                    // » Weapon type
        public State state { get; set; }                            // » Weapon state

        // Attribues
        protected Vector2 _offset;                                  // » Weapon position offset in relation with the player
        // Variables to control power of launch
        protected float _maxStrength;
        protected float _strength;
        protected float _strengthIncrement;
        protected float _initialStrength;
        protected float _rotationAmmount;

        // Weapon Factory method
        public static void Create(Type weaponType)
        {
            switch (weaponType)
            {
                case Type.Axe:
                    ActiveWeapon = new Axe();
                    break;
                case Type.Grenade:
                    ActiveWeapon = new Grenade();
                    break;
            }
        }

        // Constructor
        public Weapon()
            : base()
        {
            ActiveWeapon = this;
            state = State.Equipped;
        }

        public override void Load(ContentManager content)
        {
            base.Load(content);
            _position = Player.CurrentPlayer.Center + _offset;
        }

        public override void Update(float time)
        {
            // Move weapon along with the player
            if (state == State.Equipped || state == State.PreparingThrow)
                _position = Player.CurrentPlayer.Center + _offset;

            if ((Input.IsMouseDown() || Input.IsKeyDown(Keys.Space))
                && (state == State.Equipped || state == State.PreparingThrow))
            {
                // Set initial throw position to player feet to help calculate collision area
                ThrowInitialPosition = Player.CurrentPlayer.BottomCenter;

                // Calculate throw angle
                float angle = (float)Math.Atan2(-(Mouse.GetState().Y - Center.Y), Mouse.GetState().X - Center.X);
                _velocity = new Vector2((float)Math.Cos(angle), -(float)Math.Sin(angle)) * _strength;

                // Throw strength keeps increasing the longer the shoot button is pressed
                if (_strength == 0)
                    _strength = _initialStrength;
                if (_strength <= _maxStrength)
                    _strength += _strengthIncrement;

                state = State.PreparingThrow;
            }
            else if ((Input.IsMouseReleased() || Input.IsKeyReleased(Keys.Space)) && state == State.PreparingThrow)
            {
                // Shoot button was released and weapon is considered to be in the air
                // Each weapon type will calculate its own behaviour in its own class (in the overriden Update() method)
                state = State.Throwing;
            }

            Debug.Track(this);
            base.Update(time);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Change weapon position and rotation according to player direction and throw state
            if (Player.CurrentPlayer.IsFacing == Direction.Left)
            {
                if (state == State.PreparingThrow)
                {
                    //_position.X += 60;
                    _rotation = MathHelper.ToRadians(55);
                }
                _spriteEffects = SpriteEffects.FlipHorizontally;
                _offset.X = Math.Abs(_offset.X) * -1;
            }
            else
            {
                if (state == State.PreparingThrow)
                {
                    //_position.X -= 60;
                    _rotation = MathHelper.ToRadians(-55);
                }
                _spriteEffects = SpriteEffects.None;
                _offset.X = Math.Abs(_offset.X);
            }

            _origin = new Vector2(_sourceRectangle.Width / 2, _sourceRectangle.Height / 2);
            _layerDepth = 1 - Player.CurrentPlayer.BottomCenter.Y / 1000 - 0.00001f;

            base.Draw(spriteBatch);
        }

        // Check for collision with enemy
        public bool HitsEnemy(Sprite enemy)
        {
            // Limit collision checking to a margin of the Y coordinate from where the weapon was initially thrown
            // so it doesn't collide with zombies too far above during the parabolic trajectory
            if (CollidesWith(enemy)
                && enemy.BottomCenter.Y > ThrowInitialPosition.Y - 30
                && enemy.BottomCenter.Y < ThrowInitialPosition.Y + 30)
                return true;
            else
                return false;
        }

        // Perform common gravity and rotation calculations
        protected void CalculateTrajectory(float time)
        {
            // Simulate gravity
            _velocity.Y += 9.8f * 50 * time;

            // Weapon Rotation
            if (_velocity.X < 0)
                _rotation -= _rotationAmmount * time;
            else
                _rotation += _rotationAmmount * time;
        }
    }
}
