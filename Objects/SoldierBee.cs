﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// This is a game component that implements IUpdateable.
/// </summary>
public class SoldierBee : Bee
{
    protected float chaseDistance = 70f;
    bool isChaseMode = false;

    public float DistanceFromBeeKeeper { get; set; }

    public Vector2 BeeKeeperVector { get; set; }

    protected override int MaxVelocity
    {
        get
        {
            //return (int)ConfigurationManager.ModesConfiguration[ConfigurationManager.DifficultyMode.Value].MaxSoldierBeeVelocity;
            return 4;
        }
    }

    protected override float AccelerationFactor
    {
        get
        {
            return 20;
        }
    }

    /// <summary>
    /// Creates a new soldier bee instance.
    /// </summary>
    /// <param name="game">Associated game object.</param>
    /// <param name="gamePlayScreen">Gameplay screen where the bee will appear.</param>
    /// <param name="beehive">The bee's associated beehive.</param>
    public SoldierBee(Game game, GameplayScreen gamePlayScreen, Beehive beehive)
        : base(game, gamePlayScreen, beehive)
    {
        AnimationKey = "SoldierBee";
    }

    /// <summary>
    /// Loads the content used by this component.
    /// </summary>
    protected override void LoadContent()
    {
        _texture = Game.Content.Load<Texture2D>("Textures/SoldierBeeWingFlap");

        base.LoadContent();
    }

    /// <summary>
    /// Updates the component.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    public override void Update(GameTime gameTime)
    {
        if (!_gamePlayScreen.IsActive)
        {
            base.Update(gameTime);
            return;
        }

        // If the bee was hit by smoke use the base behavior
        if (_isHitBySmoke)
        {
            base.Update(gameTime);
            // Bee can not chase when it has been hit by smoke
            isChaseMode = false;
        }
        else
        {
            // The bee is chasing the beekeeper
            if (isChaseMode)
            {
                // Move the bee closer to the beekeeper
                _velocity = BeeKeeperVector / AccelerationFactor;
                _position += _velocity;
                AnimationDefinitions[AnimationKey].Update(gameTime, true);

                // The chase is over
                if (DistanceFromBeeKeeper <= 10f)
                {
                    isChaseMode = false;
                    SetStartupPosition();
                }
            }
            else
            {
                // If close enough, start chasing
                if (DistanceFromBeeKeeper != 0f && DistanceFromBeeKeeper <= chaseDistance)
                {
                    isChaseMode = true;
                }
                else
                {
                    base.Update(gameTime);
                }
            }
        }
    }
}
