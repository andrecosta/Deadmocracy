using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Sprites
{
    public class Explosion : Sprite
    {
        public enum State
        {
            Exploding,
            Ended
        }

        public State state { get; private set; }
        private SoundEffect _explosionSound;

        public Explosion(Vector2 position)
            : base(position)
        {
            _spritesheets["default"] = "explosion";
            _animationFrames = 6;
            state = State.Exploding;
        }

        public override void Load(ContentManager content)
        {
            _explosionSound = content.Load<SoundEffect>("sounds/explosion");
            _explosionSound.Play();
            base.Load(content);
        }

        public override void Update(float time)
        {
            if (_currentFrame == _animationFrames - 1)
                state = State.Ended;

            base.Update(time);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            _origin = new Vector2(_sourceRectangle.Width / 2, _sourceRectangle.Height / 2);

            base.Draw(spriteBatch);
        }
    }
}
