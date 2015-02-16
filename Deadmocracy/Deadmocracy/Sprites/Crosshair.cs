using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Sprites;

namespace Sprites
{
    class Crosshair : Sprite
    {
        public Crosshair()
            : base(new Vector2(Mouse.GetState().X, Mouse.GetState().Y))
        {
            _spritesheets["default"] = "crosshair";
        }

        public override void Update(float time)
        {
            _position = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            _origin = new Vector2((Image.Width / 2), (Image.Height / 2));
            _rotation += 0.01f;
            base.Update(time);
        }
    }
}
