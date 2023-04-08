using HoneycombRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Threading;

namespace HoneycumbRush;

class LoadingAndInstructionScreen : GameScreen
{
    private GameplayScreen _gameplayScreen;
    private Thread _thread;

    private bool _isLoading;

    public LoadingAndInstructionScreen()
    {
        TransitionOnTime = TimeSpan.FromSeconds(0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    public override void LoadContent()
    {
        // Create a new instance of the gameplay screen
        _gameplayScreen = new GameplayScreen(DifficultyMode.Easy);
    }

    public override void HandleInput(GameTime gameTime, InputState input)
    {
        if (!_isLoading)
        {
            PlayerIndex player;

            if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) ||
                input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player) ||
                input.IsNewMouseClick(InputState.MouseButton.Left, ControllingPlayer, out player))
            {
                LoadResources();
            }
        }
            
        base.HandleInput(gameTime, input);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        // If additional thread is running, do nothing
        if (null != _thread)
        {
            // If additional thread finished loading and the screen is not exiting
            if (_thread.ThreadState == ThreadState.Stopped && !IsExiting)
            {
                // Move on to the game play screen once highscore data is loaded
                foreach (GameScreen screen in ScreenManager.GetScreens())
                {
                    screen.ExitScreen();
                }

                ScreenManager.AddScreen(_gameplayScreen, null);
            }
        }

        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
        SpriteFont font = ScreenManager.Font;

        spriteBatch.Begin();

        // If loading game play screen resource in the background, show "Loading..." text
        if (_isLoading)
        {
            string text = "Loading...";
            Vector2 size = font.MeasureString(text);
            Vector2 position = new Vector2(
                (ScreenManager.GraphicsDevice.Viewport.Width - size.X) / 2,
                (ScreenManager.GraphicsDevice.Viewport.Height - size.Y) / 2);
            spriteBatch.DrawString(font, text, position, Color.White);
        }

        spriteBatch.End();
    }

    private void LoadResources()
    {
        // Start loading the resources in an additional thread
        _thread = new Thread(new ThreadStart(_gameplayScreen.LoadAssets));
        _thread.Start();

        _isLoading = true;
    }
}
