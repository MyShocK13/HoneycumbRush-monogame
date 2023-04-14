using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// Repesents the base bee component.
/// </summary>
public abstract class Bee : TexturedDrawableGameComponent
{
    protected static Random _random = new Random();

    protected Beehive _relatedBeehive;
    protected Vector2 _velocity;

    protected float _rotation;
    protected bool _isHitBySmoke;
    protected bool _isGotHit;

    protected string AnimationKey { get; set; }

    private TimeSpan _velocityChangeTimer = TimeSpan.Zero;

    /// <summary>
    /// Timespan used to regenerate the be after it is chased away by smoke
    /// </summary>
    private TimeSpan _timeToRegenerate;

    /// <summary>
    /// Time at which the bee was hit by smoke
    /// </summary>
    private TimeSpan _timeHit;

    public bool IsBeeHit
    {
        get
        {
            return _isHitBySmoke;
        }
    }

    public Beehive Beehive
    {
        get
        {
            return _relatedBeehive;
        }
    }

    protected virtual TimeSpan VelocityChangeInterval
    {
        get
        {
            return TimeSpan.FromMilliseconds(500);
        }
    }

    public override Rectangle Bounds
    {
        get
        {
            if (_texture == null)
            {
                return default(Rectangle);
            }
            else
            {
                // The bee's _texture is an animation strip, so we must devide the _texture's width by three 
                // to get the bee's actual width
                return new Rectangle((int)_position.X, (int)_position.Y, (int)(_texture.Width / 3), (int)(_texture.Height));
            }
        }
    }

    abstract protected int MaxVelocity { get; }
    abstract protected float AccelerationFactor { get; }

    /// <summary>
    /// Creates a new bee instance.
    /// </summary>
    /// <param name="game">The game object.</param>
    /// <param name="gamePlayScreen">The gameplay screen.</param>
    /// <param name="beehive">The related beehive.</param>
    public Bee(Game game, GameplayScreen gamePlayScreen, Beehive beehive)
        : base(game, gamePlayScreen)
    {
        _relatedBeehive = beehive;
        DrawOrder = Int32.MaxValue - 20;
    }

    /// <summary>
    /// Initialize the bee's location and animation.
    /// </summary>
    public override void Initialize()
    {
        // Start up _position
        SetStartupPosition();
        if (!string.IsNullOrEmpty(AnimationKey))
        {
            AnimationDefinitions[AnimationKey].PlayFromFrameIndex(0);
        }
        base.Initialize();
    }

    /// <summary>
    /// Updates the bee's status.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Update(GameTime gameTime)
    {
        if (!(_gamePlayScreen.IsActive && _gamePlayScreen.IsStarted))
        {
            base.Update(gameTime);
            return;
        }

        // This method will handle the regeneration of bees that were hit by smoke
        if (!HandleRegeneration(gameTime))
        {
            return;
        }

        if (!string.IsNullOrEmpty(AnimationKey))
        {
            AnimationDefinitions[AnimationKey].Update(gameTime, true);
        }

        // If a bee is hit by smoke, it doesn't have random movement until  
        //  regeneration
        if (!_isHitBySmoke)
        {
            SetRandomMovement(gameTime);
        }

        // Moving the bee according to its velocity
        _position += _velocity;

        // If the bee is hit by smoke make it bee move faster
        if (_isHitBySmoke)
        {
            _position += _velocity;
        }

        // If the bee is out of screen
        if (_position.X < 0 || _position.X > Game.GraphicsDevice.Viewport.Width - Bounds.Width ||
            _position.Y < 0 || _position.Y > Game.GraphicsDevice.Viewport.Height - Bounds.Height)
        {
            if (_isHitBySmoke)
            {
                // Reset the bee's _position
                SetStartupPositionWithTimer();
            }
            else
            {
                // When hit by the screen bounds, we want the bee to move
                // longer than usual before picking a new direction
                _velocityChangeTimer = TimeSpan.FromMilliseconds(-160);
                if (_position.X < Bounds.Width || _position.X > Game.GraphicsDevice.Viewport.Width - Bounds.Width)
                {
                    _velocity = new Vector2(_velocity.X *= -1, _velocity.Y);
                }
                else
                {
                    _velocity = new Vector2(_velocity.X, _velocity.Y *= -1);
                }
            }
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Renders the bee.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Draw(GameTime gameTime)
    {
        if (_gamePlayScreen.IsActive && _gamePlayScreen.IsStarted)
        {
            _spriteBatch.Begin();

            // If the bee has an animation, draw it
            if (!string.IsNullOrEmpty(AnimationKey))
            {
                AnimationDefinitions[AnimationKey].Draw(_spriteBatch, _position, SpriteEffects.None);
            }
            else
            {
                _spriteBatch.Draw(_texture, _position, null, Color.White, 0, Vector2.Zero, 1f,
                    SpriteEffects.None, 0);
            }

            _spriteBatch.End();
        }

        base.Draw(gameTime);
    }

    /// <summary>
    /// Denotes that the bee has been hit by smoke.
    /// </summary>
    /// <param name="smokePuff">The smoke puff which the be was hit by.</param>
    public void HitBySmoke(SmokePuff smokePuff)
    {
        if (!_isHitBySmoke)
        {
            // Causes the bee to fly away from the smoke puff
            Vector2 escapeVector = Bounds.Center.GetVector() - smokePuff.Bounds.Center.GetVector();
            escapeVector.Normalize();
            escapeVector *= _random.Next(3, 6);

            _velocity = escapeVector;

            _isHitBySmoke = true;
        }
    }

    /// <summary>
    /// Sets the startup _position for the bee.
    /// </summary>
    public virtual void SetStartupPosition()
    {
        if (_relatedBeehive.AllowBeesToGenerate)
        {
            Rectangle rect = _relatedBeehive.Bounds;
            _position = new Vector2(rect.Center.X, rect.Center.Y);
            _velocity = new Vector2(_random.Next(-MaxVelocity * 100, MaxVelocity * 100) / 100,
                                   _random.Next(-MaxVelocity * 100, MaxVelocity * 100) / 100);
            _isHitBySmoke = false;
            _timeToRegenerate = TimeSpan.Zero;
            _timeHit = TimeSpan.Zero;
        }
    }

    /// <summary>
    /// Checks collision with a specified rectangle.
    /// </summary>
    /// <param name="bounds">Rectabgke with which to check for collisions.</param>
    public void Collide(Rectangle bounds)
    {
        // Check if this collision is new
        if (!_isGotHit)
        {
            // Moves to new dircetion calculted by the "wall" that the bee collided 
            // with.
            _velocityChangeTimer = TimeSpan.FromMilliseconds(-300);
            if (_position.X < bounds.X || _position.X > bounds.X + bounds.Width)
            {
                _velocity = new Vector2(_velocity.X *= -1, _velocity.Y);
            }
            else
            {
                _velocity = new Vector2(_velocity.X, _velocity.Y *= -1);
            }

            _isGotHit = true;
        }
    }

    /// <summary>
    /// Set a timer which will cause the be to regenerate when it expires.
    /// </summary>
    private void SetStartupPositionWithTimer()
    {
        _timeToRegenerate = TimeSpan.FromMilliseconds(_random.Next(3000, 5000));
    }

    /// <summary>
    /// This method handles a bee's regeneration.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    /// <returns>True if the bee has regenerated or no regeneration was necessary,
    /// false otherwise.</returns>
    private bool HandleRegeneration(GameTime gameTime)
    {
        // Checks if regeneration is needed
        if (_timeToRegenerate != TimeSpan.Zero)
        {
            // Saves the time the bee was hit
            if (_timeHit == TimeSpan.Zero)
            {
                _timeHit = gameTime.TotalGameTime;
            }

            // If enough time has pass, regenerate the bee
            if (_timeToRegenerate + _timeHit < gameTime.TotalGameTime)
            {
                SetStartupPosition();
            }
            else
            {
                _position = new Vector2(-_texture.Width, -_texture.Height);
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Alter the bee's movement randomly.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    private void SetRandomMovement(GameTime gameTime)
    {
        _velocityChangeTimer += gameTime.ElapsedGameTime;
        if (_velocityChangeTimer >= VelocityChangeInterval)
        {
            _velocity = new Vector2(_random.Next(-MaxVelocity * 100, MaxVelocity * 100) / 100,
                                    _random.Next(-MaxVelocity * 100, MaxVelocity * 100) / 100);

            _velocityChangeTimer = TimeSpan.Zero;

            if (_isGotHit)
            {
                _isGotHit = false;
            }
        }
    }
}
