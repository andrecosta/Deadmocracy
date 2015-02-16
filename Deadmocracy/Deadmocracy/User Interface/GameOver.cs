using Deadmocracy;
using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprites;
using System;

namespace UserInterface
{
    class GameOver : MenuScreen
    {
        public static bool IsScoreSubmitted { get; private set; }
        private static char[] _playerName;
        private static char[] _allowedChars;
        private static int _inputPosition;
        private static int _selectedChar;
        
        public static void Load(ContentManager content)
        {
            _playerName = new char[] { '_', '_', '_', '_', '_' };
            _allowedChars = new char[] { '_', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            _inputPosition = 0;
            _selectedChar = 0;
            IsScoreSubmitted = false;
        }

        public static void Update(float time)
        {
            if (Input.IsKeyDown(Keys.Up, 100))
                if (_selectedChar < _allowedChars.Length - 1)
                    _playerName[_inputPosition] = _allowedChars[++_selectedChar];

            if (Input.IsKeyDown(Keys.Down, 100))
                if (_selectedChar > 0)
                    _playerName[_inputPosition] = _allowedChars[--_selectedChar];

            if (Input.IsKeyDown(Keys.Right, 100))
                if (_inputPosition < _playerName.Length - 1)
                    _selectedChar = Array.IndexOf(_allowedChars, _playerName[++_inputPosition]);

            if (Input.IsKeyDown(Keys.Left, 100))
                if (_inputPosition > 0)
                    _selectedChar = Array.IndexOf(_allowedChars, _playerName[--_inputPosition]);

            foreach (Keys key in Keyboard.GetState().GetPressedKeys())
            {
                if (Input.IsKeyPressed(key))
                {
                    // ENTER key moves the selection to the next character until it reaches last position, then submits
                    if (key == Keys.Enter)
                    {
                        _selectedChar = 0;
                        _inputPosition++;
                        if (_inputPosition == _playerName.Length)
                        {
                            Player.CurrentPlayer.score.PlayerName = String.Join("", _playerName).Replace('_', ' ');
                            HighScores.SaveHighScore(Player.CurrentPlayer.score);
                            HighScores.Buttons["back"].Text = "MAIN MENU";
                            IsScoreSubmitted = true;
                        }
                    }

                    // Get direct input from keyboard if any character key was pressed
                    if (key >= Keys.A && key <= Keys.Z)
                        _playerName[_inputPosition++] = Convert.ToChar(key);

                    // SPACE key insert an empty character
                    if (key == Keys.Space)
                        _playerName[_inputPosition++] = '_';

                    // BACKSPACE key deletes previous character
                    if (key == Keys.Back)
                    {
                        if (_inputPosition > 0)
                        {
                            if (_inputPosition == _playerName.Length - 1 && _playerName[_playerName.Length - 1] != '_')
                                _inputPosition++;
                            _playerName[--_inputPosition] = '_';
                        }
                    }

                    // Keep input position from going forward after the last character
                    if (_inputPosition >= _playerName.Length)
                        _inputPosition = _playerName.Length - 1;
                }
            }
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            string textGameOver = (Level.CurrentLevel.IsCompleted) ? "LEVEL FINISHED!" : "GAME OVER";
            spriteBatch.DrawString(_menuXLargeFont, textGameOver,
                new Vector2(Game1.Resolution.X / 2 - _menuXLargeFont.MeasureString(textGameOver).X / 2, 80),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Draw score breakdown table
            spriteBatch.DrawString(_menuMediumFont, "TOTAL XP", new Vector2(400, 230), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            string xp = Player.CurrentPlayer.score.TotalXP.ToString();
            spriteBatch.DrawString(_menuMediumFont, xp + " xp", new Vector2(810 - _menuMediumFont.MeasureString(xp).X, 230),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.DrawString(_menuMediumFont, "HP LOST", new Vector2(400, 280), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            string hpLost = Player.CurrentPlayer.score.TotalHpLost.ToString();
            spriteBatch.DrawString(_menuMediumFont, hpLost + " hp", new Vector2(810 - _menuMediumFont.MeasureString(hpLost).X, 280),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            if (Level.CurrentLevel.IsCompleted)
            {
                spriteBatch.DrawString(_menuMediumFont, "TOTAL TIME", new Vector2(400, 330), Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                string time = ((int)Player.CurrentPlayer.score.TotalTime).ToString();
                spriteBatch.DrawString(_menuMediumFont, time + " s ", new Vector2(810 - _menuMediumFont.MeasureString(time).X, 330),
                    Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(_pixel, new Rectangle(400, 395, 470, 2), Color.White);

            spriteBatch.DrawString(_menuLargeFont, "SCORE", new Vector2(400, 410), Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            string totalScore = Player.CurrentPlayer.score.TotalScore.ToString();
            spriteBatch.DrawString(_menuLargeFont, totalScore, new Vector2(870 - _menuLargeFont.MeasureString(totalScore).X, 410),
                Color.Orange, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            string enterNameText = "Enter your name";
            spriteBatch.DrawString(_menuMediumFont, enterNameText,
                new Vector2(Game1.Resolution.X / 2 - _menuMediumFont.MeasureString(enterNameText).X / 2, 520),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            // Draw player name characters
            spriteBatch.DrawString(_menuMediumFont, _playerName[0].ToString(),
                new Vector2(550, 580), _inputPosition == 0 ? Color.Orange : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(_menuMediumFont, _playerName[1].ToString(),
                new Vector2(590, 580), _inputPosition == 1 ? Color.Orange : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(_menuMediumFont, _playerName[2].ToString(),
                new Vector2(630, 580), _inputPosition == 2 ? Color.Orange : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(_menuMediumFont, _playerName[3].ToString(),
                new Vector2(670, 580), _inputPosition == 3 ? Color.Orange : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            spriteBatch.DrawString(_menuMediumFont, _playerName[4].ToString(),
                new Vector2(710, 580), _inputPosition == 4 ? Color.Orange : Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
        }
    }
}
