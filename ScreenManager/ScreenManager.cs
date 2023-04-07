using HoneycombRush;
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

    private InputState _input = new InputState();
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
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        Game.Services.AddService(typeof(SpriteBatch), _spriteBatch);

        // Load content belonging to the screen manager.
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
        // Read the keyboard and gamepad.
        _input.Update();

        // Make a copy of the master screen list, to avoid confusion if
        // the process of updating one screen adds or removes others.
        _screensToUpdate.Clear();

        foreach (GameScreen screen in _screens)
        {
            _screensToUpdate.Add(screen);
        }

        bool otherScreenHasFocus = !Game.IsActive;
        bool coveredByOtherScreen = false;

        // Loop as long as there are screens waiting to be updated.
        while (_screensToUpdate.Count > 0)
        {
            // Pop the topmost screen off the waiting list.
            GameScreen screen = _screensToUpdate[_screensToUpdate.Count - 1];
            _screensToUpdate.RemoveAt(_screensToUpdate.Count - 1);

            // Update the screen.
            screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (screen.ScreenState == ScreenState.TransitionOn ||
                screen.ScreenState == ScreenState.Active)
            {
                // If this is the first active screen we came across,
                // give it a chance to handle input.
                if (!otherScreenHasFocus)
                {
                    screen.HandleInput(gameTime, _input);

                    otherScreenHasFocus = true;
                }

                // If this is an active non-popup, inform any subsequent
                // screens that they are covered by it.
                if (!screen.IsPopup)
                {
                    coveredByOtherScreen = true;
                }
            }
        }
    }

    public override void Draw(GameTime gameTime)
    {
        foreach (GameScreen screen in _screens)
        {
            if (screen.ScreenState == ScreenState.Hidden)
            {
                continue;
            }

            screen.Draw(gameTime);
        }
    }

    public void AddScreen(GameScreen screen, PlayerIndex? controllingPlayer)
    {
        screen.ControllingPlayer = controllingPlayer;
        screen.ScreenManager = this;
        screen.IsExiting = false;

        // If we have a graphics device, tell the screen to load content.
        if (_isInitialized)
        {
            screen.LoadContent();
        }

        _screens.Add(screen);
    }

    public void RemoveScreen(GameScreen screen)
    {
        // If we have a graphics device, tell the screen to unload content.
        if (_isInitialized)
        {
            screen.UnloadContent();
        }

        _screens.Remove(screen);
        _screensToUpdate.Remove(screen);
    }
}
