using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush;

/// <summary>
/// Supports animation playback.
/// </summary>
public class ScaledAnimation
{
    private Point _sheetSize;
    private Texture2D _animatedCharacter;

    private TimeSpan _lastestChangeTime;
    private TimeSpan _timeInterval = TimeSpan.Zero;

    private bool _drawWasAlreadyCalledOnce = false;
    private int _startFrame;
    private int _endFrame;
    private int _lastSubFrame = -1;

    public Point currentFrame;
    public Point frameSize;

    public Vector2 Offset { get; set; }
    public bool IsActive { get; private set; }

    public int FrameCount
    {
        get
        {
            return _sheetSize.X * _sheetSize.Y;
        }
    }

    public int FrameIndex
    {
        get
        {
            return _sheetSize.X * currentFrame.Y + currentFrame.X;
        }
        set
        {
            if (value >= _sheetSize.X * _sheetSize.Y + 1)
            {
                throw new InvalidOperationException("Specified frame index exceeds available frames");
            }

            currentFrame.Y = value / _sheetSize.X;
            currentFrame.X = value % _sheetSize.X;
        }
    }

    /// <summary>
    /// Creates a new instance of the animation class
    /// </summary>
    /// <param name="frameSheet">Texture which is a sheet containing the animation frames.</param>
    /// <param name="size">The size of a single frame.</param>
    /// <param name="frameSheetSize">The size of the entire animation sheet.</param>
    public ScaledAnimation(Texture2D frameSheet, Point size, Point frameSheetSize)
    {
        _animatedCharacter = frameSheet;
        frameSize = size;
        _sheetSize = frameSheetSize;
        Offset = Vector2.Zero;
    }

    /// <summary>
    /// Updates the animation's progress.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    /// <param name="isInMotion">Whether or not the animation element itself is currently in motion.</param>
    public void Update(GameTime gameTime, bool isInMotion)
    {
        Update(gameTime, isInMotion, false);
    }

    /// <summary>
    /// Updates the animation's progress.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    /// <param name="isInMotion">Whether or not the animation element itself is currently in motion.</param>
    /// <param name="runSubAnimation"></param>
    public void Update(GameTime gameTime, bool isInMotion, bool runSubAnimation)
    {
        if (IsActive && gameTime.TotalGameTime != _lastestChangeTime)
        {
            // See if a time interval between frames is defined
            if (_timeInterval != TimeSpan.Zero)
            {
                // Do nothing until an interval passes
                if (_lastestChangeTime + _timeInterval > gameTime.TotalGameTime)
                {
                    return;
                }
            }

            _lastestChangeTime = gameTime.TotalGameTime;
            if (FrameIndex >= FrameCount)
            {
                FrameIndex = 0; // Reset the animation
            }
            else
            {
                // Only advance the animation if the animation element is moving
                if (isInMotion)
                {
                    if (runSubAnimation)
                    {
                        // Initialize the animation
                        if (_lastSubFrame == -1)
                        {
                            _lastSubFrame = _startFrame;
                        }

                        // Calculate the currentFrame, which depends on the current
                        // frame in the parent animation
                        currentFrame.Y = _lastSubFrame / _sheetSize.X;
                        currentFrame.X = _lastSubFrame % _sheetSize.X;

                        // Move to the next Frame
                        _lastSubFrame += 1;
                        // Loop the animation
                        if (_lastSubFrame > _endFrame)
                        {
                            _lastSubFrame = _startFrame;
                        }
                    }
                    else
                    {
                        // Do not advance frames before the first draw operation
                        if (_drawWasAlreadyCalledOnce)
                        {
                            currentFrame.X++;
                            if (currentFrame.X >= _sheetSize.X)
                            {
                                currentFrame.X = 0;
                                currentFrame.Y++;
                            }
                            if (currentFrame.Y >= _sheetSize.Y)
                                currentFrame.Y = 0;

                            if (_lastSubFrame != -1)
                            {
                                _lastSubFrame = -1;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Render the animation.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch with which the current frame will be rendered.</param>
    /// <param name="position">The position to draw the current frame.</param>
    /// <param name="spriteEffect">SpriteEffect to apply to the current frame.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, SpriteEffects spriteEffect)
    {
        Draw(spriteBatch, position, 1.0f, spriteEffect);
    }

    /// <summary>
    /// Render the animation.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch with which the current frame will be rendered.</param>
    /// <param name="position">The position to draw the current frame.</param>
    /// <param name="scale">Scale factor to apply to the current frame.</param>
    /// <param name="spriteEffect">SpriteEffect to apply to the 
    /// current frame.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, SpriteEffects spriteEffect)
    {
        _drawWasAlreadyCalledOnce = true;

        spriteBatch.Draw(_animatedCharacter, position + Offset,
            new Rectangle(frameSize.X * currentFrame.X, frameSize.Y * currentFrame.Y, frameSize.X, frameSize.Y),
            Color.White, 0f, Vector2.Zero, scale, spriteEffect, 0);
    }

    /// <summary>
    /// Causes the animation to start playing from a specified frame index.
    /// </summary>
    /// <param name="frameIndex">Frame index to play the animation from.</param>
    public void PlayFromFrameIndex(int frameIndex)
    {
        FrameIndex = frameIndex;
        IsActive = true;
        _drawWasAlreadyCalledOnce = false;
    }

    /// <summary>
    /// Sets the range of frames which serves as the sub animation.
    /// </summary>
    /// <param name="startFrame">Start frame for the sub-animation.</param>
    /// <param name="endFrame">End frame for the sub-animation.</param>
    public void SetSubAnimation(int startFrame, int endFrame)
    {
        _startFrame = startFrame;
        _endFrame = endFrame;
    }

    /// <summary>
    /// Used to set the interval between frames.
    /// </summary>
    /// <param name="interval">The interval between frames.</param>
    public void SetFrameInterval(TimeSpan interval)
    {
        _timeInterval = interval;
    }
}
