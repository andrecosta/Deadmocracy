using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;

namespace Sprites
{
    class Zombie : Sprite
    {
        #region Enums
        public enum State
        {
            Alive,
            Dead
        }
        public enum Type
        {
            Citizen,
            Dog,
            Scooter
        }
        #endregion

        public State state { get; set; }
        public Type type { get; protected set; }
        public float HP { get; protected set; }
        public int XPValue { get; protected set; }
        public float AttackDamage { get; protected set; }

        protected bool _aggro;
        protected Vector2 _speed;
        protected SoundEffect _hitSound;

        // Zombie Factory method
        public static Zombie Spawn(Type zombieType, Vector2 position)
        {
            switch (zombieType)
            {
                case Type.Citizen: return new Citizen(random.Next(2), position, Convert.ToBoolean(random.Next(2)));
                case Type.Dog: return new Dog(position, true);
                case Type.Scooter: return new Scooter(position, true);
            }
            return null;
        }

        // Constructor
        public Zombie(Vector2 position, bool isProvoked)
            : base(position)
        {
            state = State.Alive;
            _aggro = isProvoked;
            _animationFrames = 8;
            if (random.Next(2) == 0) _spriteEffects = SpriteEffects.FlipHorizontally;
        }

        public override void Load(ContentManager content)
        {
            _hitSound = content.Load<SoundEffect>("sounds/hit");
            base.Load(content);
        }

        public override void Update(float time)
        {
            // Zombie moves towards the player if he has aggression
            if (_aggro)
            {
                // Keep a minimum distance between the zombie and the player
                if (Vector2.Distance(BottomCenter, Player.CurrentPlayer.BottomCenter) > 20)
                {
                    // Get direction vector to player
                    Vector2 directionToPlayer = Player.CurrentPlayer.BottomCenter - BottomCenter;
                    directionToPlayer.Normalize();
                    _velocity = directionToPlayer * _speed;
                }
                else
                    _velocity = Vector2.Zero;
            }

            // If zombie is at a certain distance from the player, he gains aggression
            if (Vector2.Distance(BottomCenter, Player.CurrentPlayer.BottomCenter) < 400)
                _aggro = true;

            // Switch to running animation
            if (_aggro)
                SetAnimation("walking");

            // Execute common code in base (Sprite) class
            base.Update(time);

            // Limit enemy movement to road area
            CheckBounds(Background.RoadBounds);

            // Track sprite properties on debug window
            Debug.Track(this);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            // Flip sprite according to its movement direction
            if (_velocity.X < 0) _spriteEffects = SpriteEffects.FlipHorizontally;
            else if (_velocity.X > 0) _spriteEffects = SpriteEffects.None;
            _layerDepth = 1 - BottomCenter.Y / 1000;

            base.Draw(spriteBatch);
        }

        // Zombies will keep a certain distance between each other, pushing other zombies aside during movement
        public void KeepDistance(Zombie zombie)
        {
            if (Vector2.Distance(BottomCenter, zombie.BottomCenter) < 40)
            {
                Vector2 distanceToZombie = zombie.BottomCenter - BottomCenter;
                distanceToZombie.Normalize();
                Vector2 distanceToPlayer = Player.CurrentPlayer.BottomCenter - BottomCenter;

                if (Math.Abs(distanceToPlayer.X) >= Math.Abs(distanceToPlayer.Y))
                    _position.Y += -distanceToZombie.Y;
                else
                    _position.X += -distanceToZombie.X;
            }
        }
        public void TakeDamage(float ammount)
        {
            HP -= ammount;
            if (HP <= 0)
                state = State.Dead;
            _hitSound.Play();
        }
    }
}
