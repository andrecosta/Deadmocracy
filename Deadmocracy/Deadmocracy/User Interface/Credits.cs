using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprites;
using System.Collections.Generic;

namespace UserInterface
{
    class Credits : MenuScreen
    {
        public static SortedList<string, Button> Buttons { get; private set; }
        private static Texture2D _leftZombie;
        private static Texture2D _rightZombie;
        private static int _animationFrames;
        private static int _currentFrame;
        private static float _animationTimer;

        public static void Load(ContentManager content)
        {
            _leftZombie = content.Load<Texture2D>("zombies/citizen_male_idle");
            _rightZombie = content.Load<Texture2D>("zombies/citizen_female_idle");
            _animationFrames = 8;
            Buttons = new SortedList<string, Button>();
            Buttons["back"] = new Button("BACK", new Vector2(Game1.Resolution.X / 2, 660), _menuMediumFont);
        }

        public static void Update(float time)
        {
            foreach (Button button in Buttons.Values)
            {
                button.Highlighted = true;
                button.Update(time);
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            string textHighScores = "CREDITS";
            spriteBatch.DrawString(_menuXLargeFont, textHighScores,
                new Vector2(Game1.Resolution.X / 2 - _menuXLargeFont.MeasureString(textHighScores).X / 2, 80),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(_pixel, new Rectangle(400, 180, 470, 2), Color.White);

            spriteBatch.DrawString(_menuLargeFont, "Developed by",
                    new Vector2(Game1.Resolution.X / 2 - _menuLargeFont.MeasureString("Developed by").X / 2, 200), Color.White);

            spriteBatch.DrawString(_menuMediumFont, "Diogo Marques",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Diogo Marques").X / 2, 280), Color.White);
            spriteBatch.DrawString(_menuMediumFont, "Fabio Costa",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Fabio Costa").X / 2, 320), Color.White);
            spriteBatch.DrawString(_menuMediumFont, "Guilherme Santos",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Guilherme Santos").X / 2, 360), Color.White);
            spriteBatch.DrawString(_menuMediumFont, "Joao Martins",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Joao Martins").X / 2, 400), Color.White);

            spriteBatch.DrawString(_menuMediumFont, "For Universidade Europeia's Games & Apps Course",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("For Universidade Europeia's Games & Apps Course").X * 0.4f / 2, 450),
                    Color.White, 0, Vector2.Zero, 0.4f, SpriteEffects.None, 0);

            spriteBatch.DrawString(_menuMediumFont, "Music from http://indiegamemusic.com",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Music from http://indiegamemusic.com").X * 0.5f / 2, 500),
                    Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_menuMediumFont, "Sound Effects created using Bfxr",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("Sound Effects created using Bfxr").X * 0.5f / 2, 530),
                    Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

            spriteBatch.DrawString(_menuMediumFont, "FOR EDUCATIONAL PURPOSES ONLY",
                    new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString("FOR EDUCATIONAL PURPOSES ONLY").X * 0.5f / 2, 590),
                    Color.White, 0, Vector2.Zero, 0.5f, SpriteEffects.None, 0);

            spriteBatch.Draw(_pixel, new Rectangle(400, 620, 470, 2), Color.White);

            spriteBatch.Draw(_leftZombie, new Vector2(100, 150),
                new Rectangle(0, _currentFrame * (_leftZombie.Height / _animationFrames), _leftZombie.Width, (_leftZombie.Height / _animationFrames)),
                Color.White, 0, Vector2.Zero, 6, SpriteEffects.None, 0);
            spriteBatch.Draw(_rightZombie, new Vector2(840, 160),
                new Rectangle(0, _currentFrame * (_rightZombie.Height / _animationFrames), _rightZombie.Width, (_rightZombie.Height / _animationFrames)),
                Color.White, 0, Vector2.Zero, 6, SpriteEffects.FlipHorizontally, 0);

            foreach (Button button in Buttons.Values)
                button.Draw(spriteBatch);
        }

        protected void Animate(float time)
        {
            _animationTimer += time;
            if (_animationTimer > 0.15)
            {
                if (_currentFrame < _animationFrames - 1)
                    _currentFrame++;
                else
                    _currentFrame = 0;
                _animationTimer = 0;
            }
        }
    }
}
