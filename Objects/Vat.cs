using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// A game component that represents the vat.
/// </summary>
public class Vat : TexturedDrawableGameComponent
{
    const string EmptyString = "Empty";
    const string FullString = "Full";
    const string TimeLeftString = "Time Left";

    private SpriteFont _font14px;
    private SpriteFont _font16px;
    private SpriteFont _font36px;
    private Vector2 _emptyStringSize;
    private Vector2 _fullStringSize;
    private Vector2 _timeDigStringSize;
    private Vector2 _timeleftStringSize;

    private ScoreBar _score;

    private string _timeLeftString = string.Empty;
    private TimeSpan _timeLeft;

    public Vector2 Position
    {
        get
        {
            return _position;
        }
    }

    public Texture2D Texture
    {
        get
        {
            return _texture;
        }
    }

    public int MaxVatCapacity
    {
        get
        {
            return _score.MaxValue;
        }
    }

    public int CurrentVatCapacity
    {
        get
        {
            return _score.CurrentValue;
        }
    }

    public override Rectangle CentralCollisionArea
    {
        get
        {
            Rectangle bounds = Bounds;
            int height = (int)bounds.Height / 10 * 5;
            int width = (int)bounds.Width / 10 * 8;

            int offsetY = ((int)bounds.Height - height) / 2;
            int offsetX = ((int)bounds.Width - width) / 2;


            return new Rectangle((int)bounds.X + offsetX, (int)bounds.Y + offsetY, width, height);
        }
    }

    public Rectangle VatDepositArea
    {
        get
        {
            Rectangle bounds = Bounds;

            float sizeFactor = 0.75f;
            float marginFactor = (1 - sizeFactor) / 2;
            int x = bounds.X + (int)(marginFactor * bounds.Width);
            int y = bounds.Y + (int)(marginFactor * bounds.Height);
            int width = (int)(bounds.Width * sizeFactor);
            int height = (int)(bounds.Height * sizeFactor);

            return new Rectangle(x, y, width, height);
        }
    }

    /// <summary>
    /// Creates a new vat instance.
    /// </summary>
    /// <param name="game">The associated game object.</param>
    /// <param name="gamePlayScreen">Gameplay screen where the vat will be displayed.</param>
    /// <param name="texture">The vat's texture.</param>
    /// <param name="position">The position of the vat.</param>
    /// <param name="score">An associated score bar.</param>
    public Vat(Game game, GameplayScreen gamePlayScreen, Texture2D texture, Vector2 position, ScoreBar score)
        : base(game, gamePlayScreen)
    {
        _texture = texture;
        _position = position;
        _score = score;

        DrawOrder = (int)(position.Y + Bounds.Height);
    }

    protected override void LoadContent()
    {
        _font14px = Game.Content.Load<SpriteFont>("Fonts/GameScreenFont14px");
        _font16px = Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
        _font36px = Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");

        _fullStringSize = _font14px.MeasureString(FullString);
        _emptyStringSize = _font14px.MeasureString(EmptyString);
        _timeleftStringSize = _font16px.MeasureString(TimeLeftString);

        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Draw(gameTime);
            return;
        }

        // Draws the texture
        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, _position, Color.White);

        // Draws the "time left" text
        _spriteBatch.DrawString(_font16px, TimeLeftString,
            _position + new Vector2(Bounds.Width / 2 - _timeleftStringSize.X / 2, _timeleftStringSize.Y - 8),
            Color.White, 0, Vector2.Zero, 0, SpriteEffects.None, 2f);

        // Draws how much time is left
        _timeDigStringSize = _font36px.MeasureString(_timeLeftString);
        Color colorToDraw = Color.White;

        if (_timeLeft.Minutes == 0 && (_timeLeft.Seconds == 30 || _timeLeft.Seconds <= 10))
        {
            colorToDraw = Color.Red;
        }

        _spriteBatch.DrawString(_font36px, _timeLeftString,
            _position + new Vector2(
                Bounds.Width / 2 - _timeDigStringSize.X / 2,
                Bounds.Height / 2 - _timeDigStringSize.Y / 2),
            colorToDraw);

        // Draws the "full" and "empty" strings
        _spriteBatch.DrawString(_font14px, EmptyString,
            new Vector2(_position.X, _position.Y + Bounds.Height - _emptyStringSize.Y), Color.White);

        _spriteBatch.DrawString(_font14px, FullString,
            new Vector2(_position.X + Bounds.Width - _fullStringSize.X,
                        _position.Y + Bounds.Height - _emptyStringSize.Y), Color.White);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Translates time left in the game to a internal representation string.
    /// </summary>
    /// <param name="timeLeft">Time left before the current level ends.</param>
    public void DrawTimeLeft(TimeSpan timeLeft)
    {
        _timeLeft = timeLeft;
        _timeLeftString = String.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
    }

    /// <summary>
    /// Adds honey to the amount stored in the vat.
    /// </summary>
    /// <param name="value">Amount of honey to add.</param>
    public void IncreaseHoney(int value)
    {
        _score.IncreaseCurrentValue(value);
    }
}
