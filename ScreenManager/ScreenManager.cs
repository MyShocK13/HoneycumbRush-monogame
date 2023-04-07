using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input.Touch;
using System.Collections.Generic;

namespace HoneycumbRush;

public class ScreenManager : DrawableGameComponent
{
    private List<GameScreen> _screens = new List<GameScreen>();
    private List<GameScreen> _screensToUpdate = new List<GameScreen>();

    private SpriteBatch _spriteBatch;
    private SpriteFont _font;
    private Texture2D _blankTexture;
    private Texture2D _buttonBackground;

    private bool _isInitialized;

    public SpriteBatch SpriteBatch
    {
        get { return _spriteBatch; }
    }

    public SpriteFont Font
    {
        get { return _font; }
    }

    public Texture2D ButtonBackground
    {
        get { return _buttonBackground; }
    }

    //public ContentManager Content
    //{
    //    get { return _content; }
    //}

    public ScreenManager(Game game) : base(game)
    {
    }

    public override void Initialize()
    {
        base.Initialize();
        _isInitialized = true;
    }

    protected override void LoadContent()
    {
        //_content = Game.Content;

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Game.Services.AddService(typeof(SpriteBatch), _spriteBatch);

        _font = Game.Content.Load<SpriteFont>("Fonts/MenuFont");
        _blankTexture = Game.Content.Load<Texture2D>("Textures/Backgrounds/blank");
        _buttonBackground = Game.Content.Load<Texture2D>("Textures/Backgrounds/buttonBackground");

        foreach (GameScreen screen in _screens)
        {
            screen.LoadContent();
        }
    }

    protected override void UnloadContent()
    {
        foreach (GameScreen screen in _screens)
        {
            screen.UnloadContent();
        }
    }

    public override void Update(GameTime gameTime)
    {


        // Make a copy of the master screen list, to avoid confusion if
        // the process of updating one screen adds or removes others.
        _screensToUpdate.Clear();

        foreach (GameScreen screen in _screens)
        {
            _screensToUpdate.Add(screen);
        }

        // Loop as long as there are screens waiting to be updated.
        while (_screensToUpdate.Count > 0)
        {
            GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];
            _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

            screen.Update(gameTime);
        }
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (GameScreen screen in _screens)
        {
            screen.Draw(gameTime);
        }
    }

    public void AddScreen(GameScreen screen)
    {
        screen.ScreenManager = this;

        if (_isInitialized)
        {
            screen.LoadContent();
        }

        _screens.Add(screen);
    }
}
