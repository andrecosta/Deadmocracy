using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sprites;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Deadmocracy
{
    class Background
    {
        // Static variables
        public static int BuildingNumber { get; set; }
        public static Rectangle ScrollMargin { get; set; }
        public static Rectangle RoadBounds { get; set; }

        private Texture2D[] _textureBuildings;
        private Texture2D[] _textureRoads;
        private List<Texture2D> _drawnBuildings;
        private List<Texture2D> _drawnRoads;
        private Vector2 _buildingSize;
        private Vector2 _roadSize;
        private int _gapSize;
        private float _buildingStart;
        private int _roadNumber;
        private float _roadStart;
        private Random r = new Random();

        public Background()
        {
            _drawnBuildings = new List<Texture2D>();
            _drawnRoads = new List<Texture2D>();
            _textureBuildings = new Texture2D[11];
            _textureRoads = new Texture2D[9];
            _buildingStart = 0;
            _roadNumber = 0;
            _roadStart = 0;
            _roadNumber = 0;
            BuildingNumber = 0;

            // Set the horizontal limits outside which the player will cause the background to scroll
            ScrollMargin = new Rectangle((int)(Game1.Resolution.X * 0.2), 0, (int)(Game1.Resolution.X * 0.4), 0);

            // Set the area where entities can move on foot (limited only on the vertical road edges)
            RoadBounds = new Rectangle(0, (int)(Game1.Resolution.Y * 0.38), 0, (int)(Game1.Resolution.Y * 0.6));
        }

        public void Load(ContentManager content)
        {
            // Load textures
            for (int i = 0; i < 11; i++)
                _textureBuildings[i] = content.Load<Texture2D>("background/" + (i + 1));

            for (int i = 0; i < 9; i++)
                _textureRoads[i] = content.Load<Texture2D>("background/" + (i + 13));

            // Set scaled sizes
            _buildingSize = new Vector2(_textureBuildings[0].Width * 3, _textureBuildings[0].Height * 3);
            _roadSize = new Vector2(_textureRoads[0].Width * 3, _textureRoads[0].Height * 3);
            _gapSize = 50 * 3;

            // Create initial set of buildings and roads
            for (int i = 0; i < 5; i++)
                _drawnBuildings.Add(_textureBuildings[r.Next(0, _textureBuildings.Count())]);

            for (int i = 0; i < 11; i++)
                _drawnRoads.Add(_textureRoads[r.Next(0, _textureRoads.Count())]);
        }

        public void Update(float time)
        {
            // Offset player velocity and altering the number of the variables
            // "buildingNumber" and "roadNumber" that alter the buildings and roads that are being drawn
            _buildingStart -= Player.ScrollOffset.X;
            _roadStart -= Player.ScrollOffset.X;

            if (_buildingStart <= -(_buildingSize.X) + _gapSize)
            {
                Debug.Log("_buildingStart: " + (int)_buildingStart + " px | _buildingNumber: " + BuildingNumber);
                BuildingNumber++;
                _buildingStart = 0;
                _drawnBuildings.Add(_textureBuildings[r.Next(0, _textureBuildings.Count())]);
            }
            if (_buildingStart > _buildingSize.X - _gapSize)
            {
                Debug.Log("_buildingStart: " + (int)_buildingStart + " px | _buildingNumber: " + BuildingNumber);
                BuildingNumber--;
                _buildingStart = 0;
            }

            if (_roadStart <= -_roadSize.X)
            {
                _roadNumber++;
                _roadStart = 0;
                _drawnRoads.Add(_textureRoads[r.Next(0, _textureRoads.Count())]);
            }
            if (_roadStart > _roadSize.X)
            {
                _roadNumber--;
                _roadStart = 0;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            // only Drawing 5 instances of "buildings", if "buildingNumber" changes a new building will be drawn and one will disapear
            // "buildingStart" is the variable we need to change to make the buildings scroll left or right in acordance with the movement of the player
            // if the "buildingStart" reaches a certain number it would reset to 0 and change the "buildingNumber"
            for (int i = 0; i < 5; i++)
            {
                spriteBatch.Draw(_drawnBuildings[BuildingNumber + i],
                    new Rectangle((int)_buildingStart + (i - 1) * ((int)_buildingSize.X - _gapSize), 0, (int)_buildingSize.X, (int)_buildingSize.Y),
                    null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1 - i * 0.001f);
            }

            for (int i = 0; i < 11; i++)
            {
                spriteBatch.Draw(_drawnRoads[i + _roadNumber],
                    new Rectangle((int)_roadStart + (i - 1) * (int)_roadSize.X, (int)_roadSize.Y, (int)_roadSize.X, (int)_roadSize.Y),
                    null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 1 - i * 0.001f);
            }
        }
    }
}
