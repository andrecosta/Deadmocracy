using Microsoft.Xna.Framework;

namespace Sprites
{
    class Axe : Weapon
    {
        public Axe()
            : base()
        {
            _spritesheets["default"] = "weapons/axe";
            _offset = new Vector2(20, -5);
            _initialStrength = 350;
            _maxStrength = 600;
            _strengthIncrement = 5;
            _rotationAmmount = 15f;
            Damage = 10;
            CanHit = true;
            type = Type.Axe;
        }

        public override void Update(float time)
        {
            if (state == State.Throwing)
            {
                CalculateTrajectory(time);

                // Destroy axe when it hits the ground
                if (_position.Y > ThrowInitialPosition.Y)
                    state = State.Destroyed;
            }

            base.Update(time);
        }
    }
}
