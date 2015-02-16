using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sprites
{
    class BloodParticle : Sprite
    {
        public bool Expired { get; private set; }
        private int _ground;
        private float _expirationTimer;
        private Texture2D _pixel;

        public BloodParticle(Rectangle bounds, Vector2 splashDirection, GraphicsDevice device)
        {
            _pixel = new Texture2D(device, 1, 1, false, SurfaceFormat.Color);
            _pixel.SetData(new[] { Color.DarkRed });
            _animations.Add("blood", _pixel);
            _activeAnimation = "blood";
            _position = new Vector2(bounds.Center.X, bounds.Center.Y);
            _size = random.Next(1, 6);
            // Give each particle a random velocity in the direction of the hit
            _velocity = new Vector2(
                random.Next(
                    (int)Math.Abs(splashDirection.X * 0.1f),
                    (int)Math.Abs(splashDirection.X * 0.8f))
                    * Math.Sign(splashDirection.X) + random.Next(-150, 150),
                random.Next(
                    (int)splashDirection.Y,
                    (int)splashDirection.Y)
                    + random.Next(-200, 100));
            _ground = bounds.Bottom + random.Next(-30, 30);
            _expirationTimer = random.Next(-15, 0);
            _textureData = new Color[_animations[_activeAnimation].Width * _animations[_activeAnimation].Height];
            _animations[_activeAnimation].GetData(_textureData);
            //_origin = new Vector2(_size / 2, _size / 2);
        }
        public override void Update(float time)
        {
            // Delete particles that existed for over 15-30 seconds
            _expirationTimer += time;
            if (_expirationTimer > 15)
                Expired = true;

            // Falling movement for blood particles
            if (_position.Y < _ground)
                _velocity.Y += 9.8f * 50 * time - (5 - _size) / 2; // » Smaller particles fall slower
            else
            {
                // Particle is on the ground and grows a little each time
                _velocity = Vector2.Zero;
                _size += 0.1f * time;
            }

            base.Update(time);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            _layerDepth = 1 - _ground / 1000.0f;
            _sourceRectangle = _animations[_activeAnimation].Bounds;
            base.Draw(spriteBatch);
        }
    }
}
