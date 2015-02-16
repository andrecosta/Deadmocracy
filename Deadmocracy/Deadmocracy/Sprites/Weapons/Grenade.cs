using Microsoft.Xna.Framework;

namespace Sprites
{
    class Grenade : Weapon
    {
        private float _fuseTimer;

        public Grenade()
            : base()
        {
            _spritesheets["default"] = "weapons/grenade";
            _offset = new Vector2(15, 5);
            _initialStrength = 300; // » Minimum throw strength for this weapon type
            _maxStrength = 700;     // » Maximum throw strength 
            _strengthIncrement = 5; // » How much will the throw strength increment by
            _rotationAmmount = 15f;
            Damage = 20;
            CanHit = false;
            type = Type.Grenade;
            _fuseTimer = 2;         // » Number of seconds until explosion
        }

        public override void Update(float time)
        {
            if (state == State.Throwing)
            {
                CalculateTrajectory(time);

                // Grenade hits the floor and bounces
                if (state == State.Throwing && _position.Y > ThrowInitialPosition.Y)
                {
                    _velocity.Y *= -0.3f;
                    _velocity.X *= 0.8f;
                    _position.Y = ThrowInitialPosition.Y;
                    _rotationAmmount *= 0.5f;
                }

                // Decrease grenade fuse timer
                _fuseTimer -= time;

                // Destroy grenade
                if (_fuseTimer <= 0) {
                    state = State.Destroyed;
                }
            }

            base.Update(time);
        }
    }
}
