using Microsoft.Xna.Framework;

namespace Sprites
{
    class Scooter : Zombie
    {
        public Scooter(Vector2 position, bool isProvoked)
            : base(position, isProvoked)
        {
            _spritesheets["default"] = "zombies/scooter_idle";
            _spritesheets["walking"] = "zombies/scooter_driving";
            _speed = new Vector2(40, 30);
            HP = 20;
            AttackDamage = 20;
            XPValue = 20;
            type = Type.Scooter;
        }
    }
}
