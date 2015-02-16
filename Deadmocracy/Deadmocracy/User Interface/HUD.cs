using Deadmocracy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Sprites;

namespace UserInterface
{
    class HUD
    {
        private static Texture2D background;
        private static Texture2D bar;
        private static Texture2D miniAxe;
        private static Texture2D miniGrenade;
        private static Texture2D miniHead;
        private static int level;
        private static float textScale = 2;
        private static Vector2 levelIndicatorPosition;
        private static Color axeSelected, grenadeSelected;
        private static SpriteFont _hudFont;



        public static void Load(ContentManager content)
        {
            background = content.Load<Texture2D>("ui/hud_background");
            bar = content.Load<Texture2D>("ui/bar");
            miniAxe = content.Load<Texture2D>("ui/mini_axe");
            miniGrenade = content.Load<Texture2D>("ui/mini_grenade");
            miniHead = content.Load<Texture2D>("ui/mini_head");
            _hudFont = content.Load<SpriteFont>("fonts/hud");
        }

        public static void Update(float time)
        {
            // Calculate player level to display based on experience gained
            level = Player.CurrentPlayer.XP / 200 + 1;

            // Change weapon indicator icon color based on which weapon is currently equipped
            axeSelected = (Player.CurrentPlayer.EquippedWeapon == Weapon.Type.Axe) ? Color.Yellow : Color.White;
            grenadeSelected = (Player.CurrentPlayer.EquippedWeapon == Weapon.Type.Grenade) ? Color.Yellow : Color.White;

            // Update level selector position
            if (Player.CurrentPlayer.score.TotalDistance >= 0)
                levelIndicatorPosition =
                    new Vector2(450 + Player.CurrentPlayer.score.TotalDistance * 350 / Level.CurrentLevel.DistanceToObjective, 29);
        }

        public static void Draw(SpriteBatch spriteBatch)
        {
            // Draw HUD background and HP and XP bars
            spriteBatch.Draw(background, new Rectangle(0, 0, background.Width, background.Height),
                null, Color.White, 0, Vector2.Zero, SpriteEffects.None, 0.9f);
            spriteBatch.Draw(bar, new Rectangle(84, 14,
                (int)(Player.CurrentPlayer.HP * bar.Width / Player.CurrentPlayer.MaxHP), bar.Height),
                null, Color.Red, 0, Vector2.Zero, SpriteEffects.None, 0.5f);
            spriteBatch.Draw(bar, new Rectangle(84, 35,
                ((Player.CurrentPlayer.XP - (200 * (level - 1))) * bar.Width) / 200, bar.Height),
                null, Color.Yellow, 0, Vector2.Zero, SpriteEffects.None, 0.5f);

            // Draw weapon icons
            spriteBatch.Draw(miniAxe, new Vector2(275, 14), null, axeSelected, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);
            spriteBatch.Draw(miniGrenade, new Vector2(330, 16), null, grenadeSelected, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            // Draw level indicator
            spriteBatch.Draw(miniHead, levelIndicatorPosition, null, Color.White, 0, Vector2.Zero, 1, SpriteEffects.None, 0.1f);

            // Draw player stats
            spriteBatch.DrawString(_hudFont, "LVL " + level, new Vector2(865, 8),
                Color.White, 0, Vector2.Zero, textScale, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(_hudFont,
                "TARGET: " + (int)(Level.CurrentLevel.DistanceToObjective - Player.CurrentPlayer.score.TotalDistance) + " m",
                new Vector2(865, 28), Color.White, 0, Vector2.Zero, textScale, SpriteEffects.None, 0.1f);
            spriteBatch.DrawString(_hudFont, Player.CurrentPlayer.score.Kills + " Kills", new Vector2(963, 8),
                Color.Red, 0, Vector2.Zero, textScale * 0.8f, SpriteEffects.None, 0.1f);
        }
    }
}
