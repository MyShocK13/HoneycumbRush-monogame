using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// A game component that represent the Honey Jar
/// </summary>
public class HoneyJar : TexturedDrawableGameComponent
{
    const string HoneyText = "Honey";

    private SpriteFont _font16px;
    private Vector2 _honeyTextSize;

    private ScoreBar _score;

    public bool CanCarryMore
    {
        get
        {
            return _score.CurrentValue < _score.MaxValue;
        }
    }

    public bool HasHoney
    {
        get
        {
            return _score.CurrentValue > _score.MinValue;
        }
    }

    /// <summary>
    /// Creates a new instance of the component.
    /// </summary>
    /// <param name="game">The associated game object.</param>
    /// <param name="gamePlayScreen">The gameplay screen where the component will be rendered.</param>
    /// <param name="position">The position of the component.</param>
    /// <param name="score">Scorebar representing the amount of honey in the jar.</param>
    public HoneyJar(Game game, GameplayScreen gamePlayScreen, Texture2D texture, Vector2 position, ScoreBar score)
        : base(game, gamePlayScreen)
    {
        _texture = texture;
        _position = position;
        _score = score;
    }

    protected override void LoadContent()
    {
        _font16px = Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
        
        _honeyTextSize = _font16px.MeasureString(HoneyText);

        base.LoadContent();
    }

    public override void Draw(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Draw(gameTime);
            return;
        }

        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, _position, Color.White);
        _spriteBatch.DrawString(_font16px, HoneyText, 
            _position + new Vector2(Bounds.Width / 2 - _honeyTextSize.X / 2, Bounds.Height * 4 / 3), Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Increases honey stored in the jar by the specified amount.
    /// </summary>
    /// <param name="value">The amount of honey to add to the jar.</param>
    public void IncreaseHoney(int value)
    {
        _score.IncreaseCurrentValue(value);
    }

    /// <summary>
    /// Decreases honey stored in the jar by the specified amount.
    /// </summary>
    /// <param name="value">The amount of honey to remove from the jar.</param>
    public void DecreaseHoney(int value)
    {
        _score.DecreaseCurrentValue(value);
    }

    /// <summary>
    /// Decrease the amount of honey in the jar by a specified percent of the jar's total capacity.
    /// </summary>
    /// <param name="percent">The percent of the jar's capacity by which to decrease the current amount
    /// of honey. If the jar's capacity is 100 and this value is 20, then the amount of honey will be reduced
    /// by 20.</param>
    public int DecreaseHoneyByPercent(int percent)
    {
        return _score.DecreaseCurrentValue(percent * _score.MaxValue / 100, true);
    }
}
