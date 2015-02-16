using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Sprites
{
    public class Sprite // » Base Sprite Class
    {
        protected static Random random = new Random();

        protected Vector2 _position;                        // » Sprite position coordinates
        protected Vector2 _velocity;                        // » Velocity of the sprite
        protected Rectangle _bounds;                        // » Bounds of the sprite
        // Images
        protected Dictionary<string, string> _spritesheets; // » Name of the the spritesheet image resource(s)
        protected Dictionary<string, Texture2D> _animations;// » Loaded spritesheet textures
        protected string _activeAnimation;                  // » Name of currently active animation
        protected Color[] _textureData;                     // » Individual pixel data of the sprite texture (for special collision detection)
        // Animation
        protected int _animationFrames;                     // » Number of animation frames for this sprite (1 means no animation)
        protected int _currentFrame;                        // » Current frame of the sprite animation
        protected float _animationTimer;                    // » Used to control the animation cycle speed
        // Draw variables
        protected Rectangle _sourceRectangle;               // » Area of the image from where the texture will be loaded
        protected Color _color;
        protected float _rotation;
        protected Vector2 _origin;
        protected float _size;
        protected SpriteEffects _spriteEffects;
        protected float _layerDepth;

        // Properties
        public Vector2 Position { get { return _position; } }
        public Vector2 Velocity { get { return _velocity; } set { _velocity = value; } }
        public Texture2D Image { get { return _animations[_activeAnimation]; } set { _animations["default"] = value; } }
        public Rectangle Bounds { get { return _bounds; } }
        // Extra helper properties
        public Vector2 Center { get { return new Vector2(_bounds.Center.X, _bounds.Center.Y); } }
        public Vector2 TopCenter { get { return new Vector2(_bounds.Center.X, _bounds.Top); } }
        public Vector2 BottomCenter { get { return new Vector2(_bounds.Center.X, _bounds.Bottom); } }
        public Vector2 RightCenter { get { return new Vector2(_bounds.Right, _bounds.Center.Y); } }
        public Vector2 LeftCenter { get { return new Vector2(_bounds.Left, _bounds.Center.Y); } }

        // Default constructor
        public Sprite()
        {
            _spritesheets = new Dictionary<string, string>();
            _animations = new Dictionary<string, Texture2D>();
            _velocity = Vector2.Zero;
            _color = Color.White;
            _rotation = 0;
            _origin = Vector2.Zero;
            _size = 1;
            _spriteEffects = SpriteEffects.None;
            _layerDepth = 0;
            _animationFrames = 1; // » If no object sets this number to greater than 1, it means it's a single image (or a "one frame spritesheet")
            _currentFrame = 0;
        }
        // Constructor with position
        public Sprite(Vector2 position)
            : this()
        {
            _position = position;
        }

        // Methods
        public virtual void Load(ContentManager content)
        {
            foreach (KeyValuePair<string, string> imageName in _spritesheets)
                _animations[imageName.Key] = content.Load<Texture2D>(imageName.Value);

            if (String.IsNullOrEmpty(_activeAnimation))
                SetAnimation("default");
            else
                SetAnimation(_activeAnimation);

            _bounds = new Rectangle((int)_position.X - (int)_origin.X, (int)_position.Y - (int)_origin.Y, _sourceRectangle.Width, _sourceRectangle.Height);
            _textureData = new Color[_animations[_activeAnimation].Width * _animations[_activeAnimation].Height];
            _animations[_activeAnimation].GetData(_textureData);
        }

        public virtual void Update(float time)
        {
            // List of conditions that can pause the update for specific situations
            bool updatePaused = false;
            updatePaused = Debug.PlayerTestMode && this is Zombie;

            // Update sprite position
            if (!updatePaused)
                _position += _velocity * time;

            // Offset sprite position if player is moving outside screen bounds (scrolling)
            _position -= Player.ScrollOffset;

            // Update bounds rectangle
            _bounds.X = (int)_position.X - (int)_origin.X;
            _bounds.Y = (int)_position.Y - (int)_origin.Y;

            // Animate the sprite
            Animate(time);
        }

        public virtual void Draw(SpriteBatch sb)
        {
            // Only draw if sprite is inside visible area
            if (_bounds.Right > 0 && _bounds.Left < Game1.Resolution.X)
                sb.Draw(_animations[_activeAnimation], _position, _sourceRectangle, _color, _rotation, _origin, _size, _spriteEffects, _layerDepth);

        }

        protected virtual void CheckBounds(Rectangle area)
        {
            if (_position.Y < area.Top)
                _position.Y = area.Top;
            if (_position.Y + _bounds.Height > area.Bottom)
                _position.Y = area.Bottom - _bounds.Height;
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
            _sourceRectangle.Y = _currentFrame * _sourceRectangle.Height;
        }

        protected void SetAnimation(string animationName)
        {
            _activeAnimation = animationName;
            _sourceRectangle = new Rectangle(0, 0, _animations[_activeAnimation].Width, _animations[_activeAnimation].Height / _animationFrames);
        }

        public bool CollidesWith(Sprite obj)
        {
            // To save CPU, only check for pixel collision when
            if (_bounds.Left > 0 && _bounds.Right < Game1.Resolution.X  // sprite is inside screen
                && _bounds.Intersects(obj._bounds)                      // outer bounds are intersecting
            )
                return IntersectPixels(_bounds, _textureData, obj._bounds, obj._textureData);
            else
                return false;
        }

        // Per pixel collision
        // Returns True if non-transparent pixels overlap; False otherwise
        private bool IntersectPixels(Rectangle rectangleA, Color[] dataA, Rectangle rectangleB, Color[] dataB)
        {
            // Find the bounds of the rectangle intersection
            int top = Math.Max(rectangleA.Top, rectangleB.Top);
            int bottom = Math.Min(rectangleA.Bottom, rectangleB.Bottom);
            int left = Math.Max(rectangleA.Left, rectangleB.Left);
            int right = Math.Min(rectangleA.Right, rectangleB.Right);

            // Check every point within the intersection bounds
            for (int y = top; y < bottom; y++)
            {
                for (int x = left; x < right; x++)
                {
                    // Get the color of both pixels at this point
                    Color colorA = dataA[(x - rectangleA.Left) +
                                         (y - rectangleA.Top) * rectangleA.Width];
                    Color colorB = dataB[(x - rectangleB.Left) +
                                         (y - rectangleB.Top) * rectangleB.Width];

                    // If both pixels are not completely transparent,
                    if (colorA.A != 0 && colorB.A != 0)
                    {
                        // then an intersection has been found
                        return true;
                    }
                }
            }

            // No intersection found
            return false;
        }
    }
}
