using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Threading;
using Microsoft.Xna.Framework.Input;
using HoneycumbRush;

namespace HoneycombRush;

class LevelOverScreen : GameScreen
{
    private SpriteFont _font36px;
    private SpriteFont _font16px;

    private Rectangle _safeArea;

    private string _text;
    private bool _isLoading;
    private Vector2 _textSize;

    private DifficultyMode? _difficultyMode;

    private Thread _thread;
    private GameplayScreen _gameplayScreen;

    private bool _assetsLoaded = false;

    /// <summary>
    /// Ctor.
    /// </summary>
    /// <param name="text">The text to display</param>
    /// <param name="difficultyMode">The next level</param>
    public LevelOverScreen(string text, DifficultyMode? difficultyMode)
    {
        _text = text;
        _difficultyMode = difficultyMode;
    }

    /// <summary>
    /// Load screen resources
    /// </summary>
    public override void LoadContent()
    {
        if (_difficultyMode.HasValue)
        {
            _gameplayScreen = new GameplayScreen(_difficultyMode.Value);
            _gameplayScreen.ScreenManager = ScreenManager;
        }
        _font36px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");
        _font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
        _textSize = _font36px.MeasureString(_text);
        _safeArea = SafeArea;

        base.LoadContent();
    }

    /// <summary>
    /// Update the screen
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    /// <param name="otherScreenHasFocus">Whether another screen has the focus.</param>
    /// <param name="coveredByOtherScreen">Whether this screen is covered by another.</param>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        // If null don't do anything
        if (null != _thread)
        {
            // If we finishedloading the assets, add the game play screen
            if (_thread.ThreadState == ThreadState.Stopped)
            {
                // Exit all the screen
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }

                // Add the gameplay screen
                if (_difficultyMode.HasValue)
                {
                    ScreenManager.AddScreen(_gameplayScreen, null);
                }
            }
        }
        else if (_assetsLoaded)
        {
            // Screen is not exiting
            if (!IsExiting)
            {
                // Move on to the game play screen once highscore data is loaded                    
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }

                // Add the gameplay screen
                if (_difficultyMode.HasValue)
                {
                    ScreenManager.AddScreen(_gameplayScreen, null);
                }
            }
        }
        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    /// <summary>
    /// Handle any input from the user
    /// </summary>
    /// <param name="gameTime"></param>
    /// <param name="input"></param>
    public override void HandleInput(GameTime gameTime, InputState input)
    {
        if (input == null)
        {
            throw new ArgumentNullException("input");
        }

        PlayerIndex player;

        // Handle keyboard
        if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) ||
            input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player) ||
            input.IsNewMouseClick(InputState.MouseButton.Left, ControllingPlayer, out player))
        {
            StartNewLevelOrExit(input);
        }

        base.HandleInput(gameTime, input);
    }

    /// <summary>
    /// Renders the screen
    /// </summary>
    /// <param name="gameTime"></param>
    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();

        // Draw the footer text

        if (_difficultyMode.HasValue)
        {
            string actionText = "Press space to start next level";

            spriteBatch.DrawString(_font16px, actionText,
                new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 -
                    _font16px.MeasureString(actionText).X / 2,
                    _safeArea.Bottom - _font16px.MeasureString(actionText).Y - 4),
                Color.Black);
        }
        else
        {
            string actionText = "Press space to end game";

            spriteBatch.DrawString(_font16px, actionText,
                new Vector2(_safeArea.Left + _safeArea.Width / 2 - _font16px.MeasureString(actionText).X / 2,
                    _safeArea.Top + _safeArea.Height - _font16px.MeasureString(actionText).Y - 4),
                Color.Black);
        }

        spriteBatch.End();
    }

    /// <summary>
    /// Starts new level or exit to High Score
    /// </summary>
    /// <param name="input"></param>
    private void StartNewLevelOrExit(InputState input)
    {
        // If there is no next level - go to high score screen
        if (!_difficultyMode.HasValue)
        {
            // If is in high score, gets is name
            //if (GameplayScreen.FinalScore != 0 && HighScoreScreen.IsInHighscores(GameplayScreen.FinalScore))
            //{
            //    Guide.BeginShowKeyboardInput(PlayerIndex.One,
            //        "Player Name", "What is your name (max 15 characters)?", "Player",
            //        AfterPlayerEnterName, null);
            //}
            //else
            //{
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }

                ScreenManager.AddScreen(new BackgroundScreen("highScoreScreen"), null);
                //ScreenManager.AddScreen(new HighScoreScreen(), null);
            //}
        }
        // If not already loading
        else if (!_isLoading)
        {
            // Start loading the resources in an additional thread
            _thread = new Thread(new ThreadStart(_gameplayScreen.LoadAssets));

            _isLoading = true;
            _thread.Start();
        }
    }

    ///// <summary>
    ///// A handler invoked after the user has enter his name.
    ///// </summary>
    ///// <param name="result"></param>
    //private void AfterPlayerEnterName(IAsyncResult result)
    //{
    //    // Gets the name entered
    //    string playerName = Guide.EndShowKeyboardInput(result);
    //    if (!string.IsNullOrEmpty(playerName))
    //    {
    //        // Ensure that it is valid
    //        if (playerName != null && playerName.Length > 15)
    //            playerName = playerName.Substring(0, 15);

    //        // Puts it in high score
    //        HighScoreScreen.PutHighScore(playerName, GameplayScreen.FinalScore);
    //        HighScoreScreen.HighScoreChanged();
    //    }

    //    // Moves to the next screen
    //    foreach (GameScreen screen in ScreenManager.GetScreens())
    //    {
    //        screen.ExitScreen();
    //    }

    //    ScreenManager.AddScreen(new BackgroundScreen("highScoreScreen"), null);
    //    ScreenManager.AddScreen(new HighScoreScreen(), null);
    //}
}
