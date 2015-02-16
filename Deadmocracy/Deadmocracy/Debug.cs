using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Sprites;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Deadmocracy
{
    // +==============+
    // |  HOW TO USE  |
    // +==============+
    //
    // Press F12 to toggle the in-game debug console
    // Press F11 to toggle player test mode (pauses enemies and gives immunity)
    //
    // To output some message to the console use:
    //   Debug.Log(<message>)
    //

    class Debug : DrawableGameComponent
    {
        public static bool IsOpen { get; set; }
        public static bool PlayerTestMode { get; set; }
        private static List<string> _messageLog;
        private static List<Sprite> _tracking;
        private SpriteBatch spriteBatch;
        private Texture2D _consoleWindow;
        private Rectangle _consoleBounds;
        private SpriteFont _spriteFont;
        protected static Texture2D _pixel;
        private int _fps;
        private Random r = new Random();

        public Debug(Game game)
            : base(game)
        { }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            _spriteFont = Game.Content.Load<SpriteFont>("fonts/debug");

            // Create single pixel texture
            _pixel = new Texture2D(GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.White });

            _consoleWindow = new Texture2D(GraphicsDevice, 1, 1);
            _consoleWindow.SetData(new[] { new Color(0, 0, 0, 0.7f) });

            _consoleBounds = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, (int)(GraphicsDevice.Viewport.Height * 0.37));

            _messageLog = new List<string>();
            _tracking = new List<Sprite>();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            float time = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // F12 Toggle
            if (Input.IsKeyPressed(Keys.F12))
                IsOpen = !IsOpen;

            // F11 Toggle
            if (Input.IsKeyPressed(Keys.F11))
                PlayerTestMode = !PlayerTestMode;

            _fps = (int)Math.Round(1 / time);

            // If debug window is open show the mouse
            if (IsOpen) Game.IsMouseVisible = true;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend);
            if (IsOpen)
            {
                // Draw console window
                spriteBatch.Draw(_consoleWindow, _consoleBounds, null, Color.Gold, 0, Vector2.Zero, SpriteEffects.None, 0.0001f);

                // Draw FPS counter
                spriteBatch.DrawString(_spriteFont, "FPS: " + _fps,
                    new Vector2(_consoleBounds.Right - 70, _consoleBounds.Top + 10), Color.Red, 0,
                    Vector2.Zero, 1, SpriteEffects.None, 0);

                // Draw playerTestMode tag
                spriteBatch.DrawString(_spriteFont, "[F11] Player Test Mode: " + (PlayerTestMode ? "ON" : "OFF"),
                    new Vector2(_consoleBounds.Center.X - 150, _consoleBounds.Top + 10),
                    PlayerTestMode ? Color.LimeGreen : Color.Red, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                // Draw log messages
                for (int i = 0; i < _messageLog.Count; i++)
                {
                    if (_consoleBounds.Bottom - i * 14 > 0)
                    {
                        spriteBatch.DrawString(_spriteFont, _messageLog[_messageLog.Count - i - 1],
                            new Vector2(10, (_consoleBounds.Bottom - 18) - i * 14), Color.Gold);
                    }
                }

                // Draw tracking header
                spriteBatch.DrawString(_spriteFont, "[TRACKING]",
                        new Vector2(_consoleBounds.Right - _spriteFont.MeasureString("[TRACKING]").X - 80,
                            _consoleBounds.Top + 10), Color.Gold);

                // Draw tracking entries
                for (int i = 0; i < _tracking.Count; i++)
                {
                    // Set text content
                    string text = "[" + _tracking[i].GetType() + "] " +
                        new Vector2((int)_tracking[i].Position.X, (int)_tracking[i].Position.Y);

                    // Set text position
                    Vector2 textPos = new Vector2(_consoleBounds.Right - _spriteFont.MeasureString(text).X - 21,
                            _consoleBounds.Top + 16 + (i + 1) * 18);

                    // Generate unique color for this object
                    var md5 = MD5.Create();
                    var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(_tracking[i].GetHashCode().ToString()));
                    Color color = new Color(hash[0], hash[1], hash[2]);

                    // Draw text and background highlight
                    spriteBatch.Draw(_consoleWindow, new Rectangle((int)textPos.X, (int)textPos.Y,
                        (int)_spriteFont.MeasureString(text).X, (int)_spriteFont.MeasureString(text).Y),
                        null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.0001f);
                    spriteBatch.DrawString(_spriteFont, text, textPos, color, 0, Vector2.Zero, 1, SpriteEffects.None, 0);

                    // Change rectangle color if object is colliding with player
                    if (_tracking[i].CollidesWith(Player.CurrentPlayer)) color = Color.Red;

                    // Draw bounding box of object
                    DrawBoundingBox(spriteBatch, _tracking[i].Bounds, 1, color);

                    // Draw vector line
                    DrawVector(spriteBatch, _tracking[i], 1, Color.Gold);

                    // Draw parabolic trajectory
                    DrawTrajectory(spriteBatch, _tracking[i], 1, Color.White, (float)gameTime.ElapsedGameTime.TotalSeconds);
                }
            }
            spriteBatch.End();

            _tracking.Clear();
            base.Draw(gameTime);
        }

        private void DrawBoundingBox(SpriteBatch spriteBatch, Rectangle r, int borderSize, Color borderColor)
        {
            // Draw top border
            spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, r.Width, borderSize), borderColor);

            // Draw left border
            spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y, borderSize, r.Height), borderColor);

            // Draw right border
            spriteBatch.Draw(_pixel, new Rectangle((r.X + r.Width - borderSize), r.Y, borderSize, r.Height), borderColor);

            // Draw bottom border
            spriteBatch.Draw(_pixel, new Rectangle(r.X, r.Y + r.Height - borderSize, r.Width, borderSize), borderColor);
        }

        private void DrawVector(SpriteBatch spriteBatch, Sprite sprite, int borderSize, Color borderColor)
        {
            // Calculate line size and angle
            Vector2 begin = new Vector2(sprite.Bounds.Center.X, sprite.Bounds.Center.Y);
            Vector2 end = begin + sprite.Velocity;
            Rectangle r = new Rectangle((int)begin.X, (int)begin.Y, (int)(end - begin).Length() + borderSize, borderSize);
            Vector2 v = Vector2.Normalize(begin - end);
            float angle = (float)Math.Acos(Vector2.Dot(v, -Vector2.UnitX));
            if (begin.Y > end.Y) angle = MathHelper.TwoPi - angle;

            // Calculate text and background sizes
            Vector2 XtextSize = _spriteFont.MeasureString("X:" + (int)sprite.Velocity.X);
            Vector2 YtextSize = _spriteFont.MeasureString("Y:" + (int)sprite.Velocity.Y);
            int maxWidth = (int)XtextSize.X;
            if (YtextSize.X > maxWidth) maxWidth = (int)YtextSize.X;
            Rectangle textSize = new Rectangle((int)end.X - maxWidth / 2, (int)end.Y - (int)XtextSize.Y + 4,
                maxWidth + 4, (int)XtextSize.Y + 9);

            // Draw vector line and component values
            spriteBatch.Draw(_pixel, r, null, borderColor, angle, Vector2.Zero, SpriteEffects.None, 0.0002f);
            spriteBatch.DrawString(_spriteFont, "X:" + (int)sprite.Velocity.X, new Vector2(textSize.X + 2, end.Y - 12), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
            spriteBatch.DrawString(_spriteFont, "Y:" + (int)sprite.Velocity.Y, new Vector2(textSize.X + 2, end.Y - 2), Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            // Draw text background
            spriteBatch.Draw(_consoleWindow, textSize, null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.0001f);
        }

        private void DrawTrajectory(SpriteBatch spriteBatch, Sprite sprite, int borderSize, Color borderColor, float time)
        {
            if (sprite is Weapon && sprite.Velocity.Length() > 0)
            {
                Vector2 initialPosition = ((Weapon)sprite).ThrowInitialPosition;
                Vector2 position = sprite.Position;
                Vector2 velocity = sprite.Velocity;
                while (position.Y <= initialPosition.Y)
                {
                    velocity.Y += 9.8f * 50 * time;
                    position += velocity * time;
                    Rectangle r = new Rectangle((int)position.X, (int)position.Y, 2, 2);
                    spriteBatch.Draw(_pixel, r, null, borderColor, 0, Vector2.Zero, SpriteEffects.None, 0);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public static void Log(string message)
        {
            _messageLog.Add(message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        public static void Track(Sprite obj)
        {
            _tracking.Add(obj);
        }
    }
}
