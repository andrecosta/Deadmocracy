using Microsoft.Xna.Framework;

namespace Sprites
{
    class Dog : Zombie
    {
        public Dog(Vector2 position, bool isProvoked)
            : base(position, isProvoked)
        {
            _spritesheets["default"] = "zombies/dog_idle";
            _spritesheets["walking"] = "zombies/dog_walking";
            _speed = new Vector2(100, 80);
            HP = 5;
            AttackDamage = 5;
            XPValue = 5;
            type = Type.Dog;
        }
    }
}
