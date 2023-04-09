using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// Represents a puff of smoke fired from the beekeeper's smoke gun.
/// </summary>
/// <remarks>Smoke puffs add and remove themselves from the list of game components as appropriate.</remarks>
public class SmokePuff : TexturedDrawableGameComponent
{
    private readonly TimeSpan _growthTimeInterval = TimeSpan.FromMilliseconds(50);
    private const float GrowthStep = 0.05f;

    private TimeSpan _lifeTime;
    private TimeSpan _growthTimeTrack;

    /// <summary>
    /// Used to scale the smoke puff
    /// </summary>
    private float _spreadFactor;
    private Vector2 _initialVelocity;
    private Vector2 _velocity;
    private Vector2 _acceleration;

    private Vector2 _drawOrigin;

    private Random _random = new Random();

    public bool IsGone
    {
        get
        {
            return _lifeTime <= TimeSpan.Zero;
        }
    }

    bool isInGameComponents;

    /// <summary>
    /// Represents an area used for collision calculations.
    /// </summary>
    public override Rectangle CentralCollisionArea
    {
        get
        {
            int boundsWidth = (int)(_texture.Width * _spreadFactor * 1.5f);
            int boundsHeight = (int)(_texture.Height * _spreadFactor * 1.5f);

            return new Rectangle((int)_position.X - boundsWidth / 4, (int)_position.Y - boundsHeight / 4,
                boundsWidth, boundsHeight);
        }
    }

    /// <summary>
    /// Creates a new puff of smoke.
    /// </summary>
    /// <param name="game">Associated game object.</param>
    /// <param name="gameplayScreen">The gameplay screen where the smoke puff will be displayed.</param>
    /// <param name="texture">The texture which represents the smoke puff.</param>        
    public SmokePuff(Game game, GameplayScreen gameplayScreen, Texture2D texture)
        : base(game, gameplayScreen)
    {
        _texture = texture;

        _drawOrigin = new Vector2(texture.Width / 2, texture.Height / 2);

        DrawOrder = Int32.MaxValue - 15;
    }

    /// <summary>
    /// Fires the smoke puff from a specified position and at a specified velocity. This also adds the smoke puff
    /// to the game component collection.
    /// </summary>
    /// <param name="origin">The position where the smoke puff should first appear.</param>
    /// <param name="initialVelocity">A vector indicating the initial velocity for this new smoke puff.</param>
    /// <remarks>The smoke puff's acceleration is internaly derived from 
    /// <paramref name="initialVelocity"/>.
    /// This method is not thread safe and calling it from another thread while the smoke puff expires (via
    /// its <see cref="Update"/> method) might have undesired effects.</remarks>
    public void Fire(Vector2 origin, Vector2 initialVelocity)
    {
        _spreadFactor = 0.05f;

        _lifeTime = TimeSpan.FromSeconds(5);
        _growthTimeTrack = TimeSpan.Zero;

        _position = origin;
        _velocity = initialVelocity;
        _initialVelocity = initialVelocity;
        initialVelocity.Normalize();

        _acceleration = -(initialVelocity) * 6;

        if (!isInGameComponents)
        {
            Game.Components.Add(this);
            isInGameComponents = true;
        }
    }

    /// <summary>
    /// Performs update logic for the smoke puff. The smoke puff slows down while growing and eventually evaporates.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Update(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Update(gameTime);
            return;
        }

        _lifeTime -= gameTime.ElapsedGameTime;

        // The smoke puff needs to vanish
        if (_lifeTime <= TimeSpan.Zero)
        {
            Game.Components.Remove(this);
            isInGameComponents = false;
            base.Update(gameTime);
            return;
        }

        _growthTimeTrack += gameTime.ElapsedGameTime;

        // See if it is time for the smoke puff to grow
        if ((_spreadFactor < 1) && (_growthTimeTrack >= _growthTimeInterval))
        {
            _growthTimeTrack = TimeSpan.Zero;
            _spreadFactor += GrowthStep;
        }

        // Stop the smoke once it starts moving in the other direction
        if (Vector2.Dot(_initialVelocity, _velocity) > 0)
        {
            _position += _velocity;
            _velocity += _acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Draws the smoke puff.
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

        Vector2 offset = GetRandomOffset();

        _spriteBatch.Draw(_texture, _position + offset, null, Color.White, 0, _drawOrigin, _spreadFactor, SpriteEffects.None, 0);

        _spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Used to make the smoke puff shift randomly.
    /// </summary>
    /// <returns>An offset which should be added to the smoke puff's position.</returns>
    private Vector2 GetRandomOffset()
    {
        return new Vector2(_random.Next(2) - 4, _random.Next(2) - 4);
    }
}
