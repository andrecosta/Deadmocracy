using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace UserInterface
{
    class HighScores : MenuScreen
    {
        public static SortedList<string, Button> Buttons { get; private set; }
        private static List<Score> _scores;
        private static string highScoresFile = "highscores.dat";
        private static Random random = new Random();

        public static void Load(ContentManager content)
        {
            // Create buttons
            Buttons = new SortedList<string, Button>();
            Buttons["back"] = new Button("BACK", new Vector2(Game1.Resolution.X / 2, 660), _menuMediumFont);

            _scores = new List<Score>();

            // Populate scores list with some example values
            if (!File.Exists(highScoresFile))
            {
                SaveHighScore(new Score("MICA", random.Next(0, 80000)));
                SaveHighScore(new Score("MIGUEL", random.Next(0, 8000)));
                SaveHighScore(new Score("AMERICO", random.Next(0, 8000)));
                SaveHighScore(new Score("RICARDO", random.Next(0, 8000)));
                SaveHighScore(new Score("JOSE", random.Next(0, 8000)));
            }

            ReadHighScores();
            SortHighScores();
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
            string textHighScores = "HIGH SCORES";
            spriteBatch.DrawString(_menuXLargeFont, textHighScores,
                new Vector2(Game1.Resolution.X / 2 - _menuXLargeFont.MeasureString(textHighScores).X / 2, 80),
                Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

            spriteBatch.Draw(_pixel, new Rectangle(400, 180, 470, 2), Color.White);

            int row = 0;
            if (_scores.Count > 0)
            {
                foreach (Score score in _scores)
                {
                    if (row < 10)
                    {
                        spriteBatch.DrawString(_menuMediumFont, score.PlayerName, new Vector2(400, 200 + 40 * row),
                            Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                        spriteBatch.DrawString(_menuMediumFont, score.TotalScore.ToString(),
                            new Vector2(870 - _menuMediumFont.MeasureString(score.TotalScore.ToString()).X, 200 + 40 * row),
                            Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                        row++;
                    }
                }
            }
            else
            {
                spriteBatch.DrawString(_menuMediumFont, "No High Scores. Be the first!",
                    new Vector2(Game1.Resolution.X / 2 - _menuLargeFont.MeasureString(textHighScores).X / 2, 220),
                    Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
            }

            spriteBatch.Draw(_pixel, new Rectangle(400, 620, 470, 2), Color.White);

            Buttons["back"].Draw(spriteBatch);
        }

        public static void ReadHighScores()
        {
            if (File.Exists(highScoresFile))
            {
                // Open the file
                FileStream stream = File.Open(highScoresFile, FileMode.Open);

                // Convert the object to XML data and put it in the stream
                XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));

                // Read scores from file
                _scores = (List<Score>)serializer.Deserialize(stream);

                // Close the file
                stream.Close();
            }
        }

        public static void SaveHighScore(Score score)
        {
            // Read existing scores from file
            ReadHighScores();

            // Open a new file, overwriting if it already exists
            FileStream stream = File.Open(highScoresFile, FileMode.Create);

            // Convert the object to XML data and put it in the stream
            XmlSerializer serializer = new XmlSerializer(typeof(List<Score>));

            // Append the new player score to the list and save it to the file
            _scores.Add(score);
            serializer.Serialize(stream, _scores);

            // Close the file
            stream.Close();

            // Sort high scores
            SortHighScores();
        }

        private static void SortHighScores()
        {
            _scores = _scores.OrderByDescending(score => score.TotalScore).ToList();
        }
    }
}
