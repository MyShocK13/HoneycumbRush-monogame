﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// Represent a single beehive
/// </summary>
public class Beehive : TexturedDrawableGameComponent
{
    private ScoreBar _score;
    private TimeSpan _intervalToAddHoney = TimeSpan.FromMilliseconds(600);
    private TimeSpan _lastTimeHoneyAdded;

    private bool _allowBeesToGenerate = true;
    public bool AllowBeesToGenerate
    {
        get
        {
            return _allowBeesToGenerate;
        }
        set
        {
            _allowBeesToGenerate = value;
        }
    }

    public bool HasHoney
    {
        get
        {
            return _score.CurrentValue > _score.MinValue;
        }
    }

    public override Rectangle Bounds
    {
        get
        {
            Rectangle baseBounds = base.Bounds;
            int widthMargin = baseBounds.Width / 10;
            int width = baseBounds.Width - widthMargin;
            int height = baseBounds.Height / 3;

            return new Rectangle(baseBounds.X + widthMargin, baseBounds.Y + height, width - widthMargin, height);
        }
    }

    public override Rectangle CentralCollisionArea
    {
        get
        {
            Rectangle bounds = Bounds;
            int height = (int)Bounds.Height / 10 * 5;
            int width = (int)Bounds.Width / 10 * 4;

            int offsetY = ((int)Bounds.Height - height) / 2;
            int offsetX = ((int)Bounds.Width - width) / 2;

            return new Rectangle((int)Bounds.X + offsetX, (int)Bounds.Y + offsetY, width, height);
        }
    }

    /// <summary>
    /// Creates a new beehive instance.
    /// </summary>
    /// <param name="game">The game object.</param>
    /// <param name="gamePlayScreen">The gameplay screen.</param>
    /// <param name="texture">The texture representing the beehive.</param>
    /// <param name="score">Score object representing the amount of honey in the
    /// hive.</param>
    /// <param name="position">The beehive's position.</param>
    public Beehive(Game game, GameplayScreen gamePlayScreen, Texture2D texture, ScoreBar score, Vector2 position)
        : base(game, gamePlayScreen)
    {
        _texture = texture;
        _score = score;
        _position = position;

        AllowBeesToGenerate = true;

        DrawOrder = (int)position.Y;
    }

    /// <summary>
    /// Updates the beehive's status.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Update(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Update(gameTime);
            return;
        }

        // Initialize the first time honey was added
        if (_lastTimeHoneyAdded == TimeSpan.Zero)
        {
            _lastTimeHoneyAdded = gameTime.TotalGameTime;
            _score.IncreaseCurrentValue(1);
        }
        else
        {
            // If enough time has passed add more honey
            if (_lastTimeHoneyAdded + _intervalToAddHoney < gameTime.TotalGameTime)
            {
                _lastTimeHoneyAdded = gameTime.TotalGameTime;
                _score.IncreaseCurrentValue(1);
            }
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Render the beehive.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Draw(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Draw(gameTime);
            return;
        }

        _spriteBatch.Begin();
        _spriteBatch.Draw(_texture, _position, Color.White);
        _spriteBatch.End();

        base.Draw(gameTime);
    }

    public void DecreaseHoney(int amount)
    {
        _score.DecreaseCurrentValue(amount);
    }
}
