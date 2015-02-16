using Microsoft.Xna.Framework;
using System;

namespace Deadmocracy
{
    public class Camera : GameComponent
    {
        public Vector2 Position { get; set; }   // » Camera Position
        public float Rotation { get; set; }     // » Camera Rotation
        public float Zoom { get; set; }         // » Camera Zoom
        private Random _random;
        private Vector2 _originalPosition;
        private float _originalRotation;
        // Camera shake variables
        private float _shakeTimer;
        private float _positionShakeAmount;
        private float _rotationShakeAmount;

        public Camera(Game game, Vector2 position) : base(game)
        {
            Position = position;
            Zoom = 1.0f;
            Rotation = 0.0f;
            _random = new Random();
        }

        public override void Update(GameTime gameTime)
        {
            // Perform a camera shake
            if (_shakeTimer > 0)
            {
                // Restore the original position and rotation so we do not go far from the center point
                Position = _originalPosition;
                Rotation = _originalRotation;

                float time = (float)gameTime.ElapsedGameTime.TotalSeconds;
                _shakeTimer -= time;
                if (_shakeTimer > 0)
                {
                    _positionShakeAmount *= 1 - time; // » Decrease shake amount little by little each update
                    Position += new Vector2((float)((_random.NextDouble() * 2) - 1) * _positionShakeAmount,
                        (float)((_random.NextDouble() * 2) - 1) * _positionShakeAmount);
                    Rotation += (float)((_random.NextDouble() * 2) - 1) * _rotationShakeAmount;
                }
            }

            base.Update(gameTime);
        }

        public Matrix GetTransformation()
        {
            return (Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                    Matrix.CreateRotationZ(Rotation) *
                    Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                    Matrix.CreateTranslation(new Vector3(Game1.Resolution.X / 2, Game1.Resolution.Y / 2, 0)));
        }

        /// <summary>
        /// Move the camera by a specified amount.
        /// </summary>
        /// <param name="displacement">The amount of displacement to offset the camera.</param>
        public void Move(Vector2 displacement)
        {
            Position += displacement;
        }

        /// <summary>
        /// Perform a camera shake.
        /// </summary>
        /// <param name="duration">The amount of time to shake the camera.</param>
        /// <param name="positionAmount">The maximum position amount to offset the camera.</param>
        /// <param name="rotationAmount">The maximum amount the camera can rotate.</param>
        public void Shake(float duration, float positionAmount, float rotationAmount)
        {
            //if (_shakeTimer <= 0)
            //{
                _shakeTimer = duration;
                _positionShakeAmount = positionAmount;
                _rotationShakeAmount = rotationAmount;

                _originalPosition = Position;
                _originalRotation = Rotation;
            //}
        }
    }
}
