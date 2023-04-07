using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycumbRush;

class BackgroundScreen : GameScreen
{
    private Texture2D _background;

    private string _backgroundName;

    public BackgroundScreen(string backgroundName)
    {
        _backgroundName = backgroundName;
    }

    public override void LoadContent()
    {
        _background = ScreenManager.Game.Content.Load<Texture2D>($"Textures/Backgrounds/{_backgroundName}");

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();
        spriteBatch.Draw(_background, ScreenManager.GraphicsDevice.Viewport.Bounds, Color.White);
        spriteBatch.End();
    }
}
