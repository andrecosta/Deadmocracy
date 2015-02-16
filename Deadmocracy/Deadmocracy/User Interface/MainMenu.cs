using Deadmocracy;
using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace UserInterface
{
    class MainMenu : MenuScreen
    {
        public static SortedList<string, Button> Buttons { get; private set; }
        private static Texture2D _logoImage;
        private static float _logoScale = 0.8f;
        private static float _logoScaleAmmount;
        private static float _logoRotation = 0;
        private static int _selectedButtonIndex;
        
        public static void Load(ContentManager content)
        {
            _logoImage = content.Load<Texture2D>("deadmocracy_logo");
            Buttons = new SortedList<string, Button>();
            Buttons["start"] = new Button("NEW GAME", new Vector2(Game1.Resolution.X / 2, 420), _menuMediumFont);
            Buttons["highScores"] = new Button("HIGH SCORES", new Vector2(Game1.Resolution.X / 2, 480), _menuMediumFont);
            Buttons["fcredits"] = new Button("CREDITS", new Vector2(Game1.Resolution.X / 2, 540), _menuMediumFont);
            Buttons["exit"] = new Button("EXIT", new Vector2(Game1.Resolution.X / 2, 600), _menuMediumFont);
            _selectedButtonIndex = Buttons.Count - 1;
            
        }

        public static void Update(float time)
        {
            ChangeSelectedButton();
            UpdateButtons(time);
            AnimateLogo(time);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_logoImage, new Vector2(Game1.Resolution.X - _logoImage.Width / 2, 240),
                null, Color.White, _logoRotation, new Vector2(_logoImage.Width / 2, _logoImage.Height / 2), _logoScale, SpriteEffects.None, 0);

            foreach (Button button in Buttons.Values)
                button.Draw(spriteBatch);
        }

        // Change between the next and previous menu buttons using keyboard
        private static void ChangeSelectedButton()
        {
            if (Input.IsKeyPressed(Keys.Down) && _selectedButtonIndex > 0)
                _selectedButtonIndex--;
            if (Input.IsKeyPressed(Keys.Up) && _selectedButtonIndex < Buttons.Count - 1)
                _selectedButtonIndex++;
        }

        // Update and highlight the menu buttons
        private static void UpdateButtons(float time)
        {
            foreach (Button button in Buttons.Values)
            {
                button.Highlighted = false;

                if (button.MouseOver)
                    _selectedButtonIndex = Buttons.IndexOfValue(button);

                if (Buttons.IndexOfValue(button) == _selectedButtonIndex)
                    button.Highlighted = true;

                button.Update(time);
            }
        }

        // Animate the logo
        private static void AnimateLogo(float time)
        {
            if (_logoScale >= 0.85f) _logoScaleAmmount = -0.02f;
            else if (_logoScale <= 0.8f) _logoScaleAmmount = 0.02f;
            _logoScale += _logoScaleAmmount * time;
            _logoRotation += _logoScaleAmmount / 4 * time;
        }
    }
}
