using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Deadmocracy
{
    class MenuScreen
    {
        protected static SpriteFont _menuMediumFont;
        protected static SpriteFont _menuLargeFont;
        protected static SpriteFont _menuXLargeFont;
        protected static SpriteFont _debugFont;
        protected static Texture2D _pixel;
        protected static SoundEffect _buttonSelect;

        public static void Load(ContentManager content, GraphicsDevice device)
        {
            // Create single pixel texture
            _pixel = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            // Load fonts
            _menuMediumFont = content.Load<SpriteFont>("fonts/menu_medium");
            _menuLargeFont = content.Load<SpriteFont>("fonts/menu_large");
            _menuXLargeFont = content.Load<SpriteFont>("fonts/menu_xlarge");
            _debugFont = content.Load<SpriteFont>("fonts/debug");

            // Load sounds
            _buttonSelect = content.Load<SoundEffect>("sounds/button_select");
        }
    }
}
