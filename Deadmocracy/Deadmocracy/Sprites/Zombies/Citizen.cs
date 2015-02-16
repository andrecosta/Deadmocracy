using Microsoft.Xna.Framework;

namespace Sprites
{
    class Citizen : Zombie
    {
        public Citizen(int variation, Vector2 position, bool isProvoked)
            : base(position, isProvoked)
        {
            if (variation == 0)
            {
                _spritesheets["default"] = "zombies/citizen_male_idle";
                _spritesheets["walking"] = "zombies/citizen_male_walking";
            }
            else
            {
                _spritesheets["default"] = "zombies/citizen_female_idle";
                _spritesheets["walking"] = "zombies/citizen_female_walking";
            }
            _speed = new Vector2(40, 30);
            HP = 10;
            AttackDamage = 10;
            XPValue = 10;
            type = Type.Citizen;
        }
    }
}
