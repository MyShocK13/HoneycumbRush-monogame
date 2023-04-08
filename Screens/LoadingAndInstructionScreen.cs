using HoneycombRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace HoneycumbRush;

class LoadingAndInstructionScreen : GameScreen
{
    private bool _isLoading;

    public LoadingAndInstructionScreen()
    {
        TransitionOnTime = TimeSpan.FromSeconds(0);
        TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    public override void LoadContent()
    {
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
        _isLoading = true;
    }
}
