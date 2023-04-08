using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// This abstract class represent a component which has a texture that represents it visually.
/// </summary>
public abstract class TexturedDrawableGameComponent : DrawableGameComponent
{
    protected SpriteBatch _spriteBatch;
    protected Texture2D _texture;
    protected Vector2 _position;
    protected GameplayScreen _gamePlayScreen;

    /// <summary>
    /// Represents the bounds of the component.
    /// </summary>
    public virtual Rectangle Bounds
    {
        get
        {
            if (_texture == null)
            {
                return default(Rectangle);
            }
            else
            {
                return new Rectangle((int)_position.X, (int)_position.Y, (int)(_texture.Width), (int)(_texture.Height));
            }
        }
    }

    /// <summary>
    /// Represents an area used for collision calculations.
    /// </summary>
    public virtual Rectangle CentralCollisionArea
    {
        get
        {
            return default(Rectangle);
        }
    }

    public Dictionary<string, ScaledAnimation> AnimationDefinitions { get; set; }

    /// <summary>
    /// Creates a new instance.
    /// </summary>
    /// <param name="game">Associated game object.</param>
    /// <param name="gamePlayScreen">Gameplay screen where the component will be presented.</param>
    public TexturedDrawableGameComponent(Game game, GameplayScreen gamePlayScreen)
        : base(game)
    {
        _gamePlayScreen = gamePlayScreen;

        _spriteBatch = (SpriteBatch)game.Services.GetService(typeof(SpriteBatch));
    }
}
