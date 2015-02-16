using Deadmocracy;
using InputManager;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace UserInterface
{
    class Button
    {
        private Vector2 _position;
        private Color _color;
        private Vector2 _origin;
        private float _scale;
        private float _scaleAnimation;
        private SpriteFont _font;
        public string Text { get; set; }
        public bool Highlighted { get; set; }

        // Properties
        private Rectangle MousePointer
        {
            get
            {
                return new Rectangle(Mouse.GetState().X, Mouse.GetState().Y, 1, 1);
            }
        }
        public bool MouseOver
        {
            get
            {
                return MousePointer.Intersects(new Rectangle((int)(_position.X - _origin.X * _scale), (int)(_position.Y - _origin.Y * _scale),
                    (int)(_font.MeasureString(Text).X * _scale), (int)(_font.MeasureString(Text).Y * _scale)));
            }
        }
        public bool Selected
        {
            get
            {
                if (Highlighted && ((MouseOver && Input.IsMousePressed()) || Input.IsKeyPressed(Keys.Enter)))
                {
                    //_buttonSelect.Play(0.1f, 0.6f, 0);
                    return true;
                }
                else return false;
            }
        }

        // Constructor
        public Button(string text, Vector2 position, SpriteFont font)
        {
            Text = text;
            _font = font;
            _position = position;
            _color = Color.White;
            _scale = 1;
        }

        public void Update(float time)
        {
            if (MouseOver) Highlighted = true;
            HighlightAnimation(time);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _origin = new Vector2(_font.MeasureString(Text).X / 2, _font.MeasureString(Text).Y / 2);
            spriteBatch.DrawString(_font, Text, _position, _color, 0, _origin, _scale, SpriteEffects.None, 0);
        }

        private void HighlightAnimation(float time)
        {
            if (Highlighted)
            {
                if (_scale >= 1.1f) _scaleAnimation = -0.5f;
                else if (_scale <= 1f) _scaleAnimation = +0.5f;
                _scale += _scaleAnimation * time;
                _color = Color.Orange;
            }
            else
            {
                _scale = 1;
                _color = Color.White;
            }
        }
    }
}
