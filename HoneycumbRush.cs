using Microsoft.Xna.Framework;

namespace HoneycumbRush;

public class HoneycumbRush : Game
{
    private GraphicsDeviceManager _graphics;
    private ScreenManager _screenManager;

    public HoneycumbRush()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";

        _graphics.PreferredBackBufferWidth = 800;
        _graphics.PreferredBackBufferHeight = 480;

        _graphics.IsFullScreen = false;
        IsMouseVisible = true;

        _screenManager = new ScreenManager(this);

        _screenManager.AddScreen(new BackgroundScreen("titleScreen"), null);
        _screenManager.AddScreen(new MainMenuScreen(), PlayerIndex.One);
        Components.Add(_screenManager);
    }

    protected override void Initialize()
    {
        base.Initialize();
    }

    protected override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        base.Draw(gameTime);
    }
}