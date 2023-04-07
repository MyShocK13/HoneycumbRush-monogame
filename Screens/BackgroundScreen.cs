using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace HoneycumbRush;

class BackgroundScreen : GameScreen
{
    private Texture2D _background;

    private string _backgroundName;

    public BackgroundScreen(string backgroundName)
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);

        _backgroundName = backgroundName;
    }

    public override void LoadContent()
    {
        _background = ScreenManager.Game.Content.Load<Texture2D>($"Textures/Backgrounds/{_backgroundName}");

        base.LoadContent();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        base.Update(gameTime, otherScreenHasFocus, false);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        // Draw background
        spriteBatch.Begin();
        spriteBatch.Draw(_background, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White * TransitionAlpha);
        spriteBatch.End();
    }
}
