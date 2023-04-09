using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace HoneycombRush;

/// <summary>
/// Represents the beekeeper, the player's avatar.
/// </summary>
public class BeeKeeper : TexturedDrawableGameComponent
{
    /// <summary>
    /// Represents the direction in which the beekeeper is walking.
    /// </summary>
    enum WalkingDirection
    {
        Down = 0,
        Up = 8,
        Left = 16,
        Right = 24,
        LeftDown = 32,
        RightDown = 40,
        LeftUp = 48,
        RightUp = 56
    }

    // Animation name constants
    const string LegAnimationKey = "LegAnimation";
    const string BodyAnimationKey = "BodyAnimation";
    const string SmokeAnimationKey = "SmokeAnimation";
    const string ShootingAnimationKey = "ShootingAnimation";
    const string BeekeeperCollectingHoneyAnimationKey = "BeekeeperCollectingHoney";
    const string BeekeeperDepositingHoneyAnimationKey = "BeekeeperDepositingHoney";

    private Vector2 _bodySize = new Vector2(85, 132);
    private Vector2 _velocity;
    private Vector2 _smokeAdjustment;
    private SpriteEffects _lastEffect;
    private SpriteEffects _currentEffect;

    // Beekeeper state variables
    private bool _needToShootSmoke;
    private bool _isStung;
    //    bool isFlashing;
    //    bool isDrawnLastStungInterval;
    //    bool isDepositingHoney;

    //    TimeSpan stungTime;
    //    TimeSpan stungDuration;
    //    TimeSpan flashingDuration;
    //    TimeSpan depositHoneyUpdatingInterval = TimeSpan.FromMilliseconds(200);
    //    TimeSpan depositHoneyUpdatingTimer = TimeSpan.Zero;
    private TimeSpan _shootSmokePuffTimer = TimeSpan.Zero;
    private readonly TimeSpan _shootSmokePuffTimerInitialValue = TimeSpan.FromMilliseconds(325);

    //private Texture2D _smokeAnimationTexture;
    private Texture2D _smokePuffTexture;
    private const int MaxSmokePuffs = 20;
    /// <summary>
    /// Contains all smoke puffs which are currently active
    /// </summary>
    public Queue<SmokePuff> FiredSmokePuffs { get; private set; }
    /// <summary>
    /// Serves as a pool of available smoke puff objects.
    /// </summary>
    private Stack<SmokePuff> _availableSmokePuffs;

    //    int stungDrawingInterval = 5;
    //    int stungDrawingCounter = 0;
    //    int honeyDepositFrameCount;
    //    int depositHoneyTimerCounter = -1;
    private int _collectingHoneyFrameCounter;

    //    AsyncCallback depositHoneyCallback;

    private WalkingDirection _newDirection = WalkingDirection.Up;
    private WalkingDirection _direction = WalkingDirection.Up;
    private int _lastFrameCounter;

    public bool IsStung
    {
        get
        {
            return _isStung;
        }
    }

    //    public bool IsFlashing
    //    {
    //        get
    //        {
    //            return isFlashing;
    //        }
    //    }

    /// <summary>
    /// Mark the beekeeper as shooting or not shooting smoke.
    /// </summary>        
    public bool IsShootingSmoke
    {
        set
        {
            if (!_isStung)
            {
                _needToShootSmoke = value;
                if (value)
                {
                    Debug.WriteLine("Shooting smoke");
                    //AudioManager.PlaySound("SmokeGun_Loop");
                }
                else
                {
                    _shootSmokePuffTimer = TimeSpan.Zero;
                }
            }
        }
    }

    public override Rectangle Bounds
    {
        get
        {
            int height = (int)_bodySize.Y / 10 * 8;
            int width = (int)_bodySize.X / 10 * 5;

            int offsetY = ((int)_bodySize.Y - height) / 2;
            int offsetX = ((int)_bodySize.X - width) / 2;

            return new Rectangle((int)_position.X + offsetX, (int)_position.Y + offsetY, width, height);
        }

    }

    //    public override Rectangle CentralCollisionArea
    //    {
    //        get
    //        {
    //            Rectangle bounds = Bounds;
    //            int height = (int)bounds.Height / 10 * 5;
    //            int width = (int)bounds.Width / 10 * 8;

    //            int offsetY = ((int)bounds.Height - height) / 2;
    //            int offsetX = ((int)bounds.Width - width) / 2;

    //            return new Rectangle((int)bounds.X + offsetX, (int)bounds.Y + offsetY, width, height);
    //        }
    //    }

    //    public bool IsDepositingHoney
    //    {
    //        get
    //        {
    //            return isDepositingHoney;
    //        }
    //    }

    public bool IsCollectingHoney { get; set; }

    //    public Vector2 Position
    //    {
    //        get { return position; }
    //    }

    //    public Rectangle ThumbStickArea { get; set; }

    public bool IsInMotion { get; set; }

    /// <summary>
    /// Creates a new beekeeper instance.
    /// </summary>
    /// <param name="game">The game object.</param>
    /// <param name="gamePlayScreen">The gameplay screen.</param>
    public BeeKeeper(Game game, GameplayScreen gamePlayScreen)
        : base(game, gamePlayScreen)
    {
    }

    public override void Initialize()
    {
        // Initialize the animation 
        AnimationDefinitions[LegAnimationKey].PlayFromFrameIndex(0);
        AnimationDefinitions[BodyAnimationKey].PlayFromFrameIndex(0);
        AnimationDefinitions[SmokeAnimationKey].PlayFromFrameIndex(0);
        AnimationDefinitions[ShootingAnimationKey].PlayFromFrameIndex(0);
        AnimationDefinitions[BeekeeperCollectingHoneyAnimationKey].PlayFromFrameIndex(0);
        AnimationDefinitions[BeekeeperDepositingHoneyAnimationKey].PlayFromFrameIndex(0);

        Point bodyAnimationFrameSize = AnimationDefinitions[LegAnimationKey].frameSize;
        _bodySize = new Vector2(bodyAnimationFrameSize.X, bodyAnimationFrameSize.Y);

        _isStung = false;
        //        stungDuration = TimeSpan.FromSeconds(1);
        //        flashingDuration = TimeSpan.FromSeconds(2);

        _availableSmokePuffs = new Stack<SmokePuff>(MaxSmokePuffs);
        FiredSmokePuffs = new Queue<SmokePuff>(MaxSmokePuffs);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        //_smokeAnimationTexture = Game.Content.Load<Texture2D>("Textures/SmokeAnimationStrip");
        _smokePuffTexture = Game.Content.Load<Texture2D>("Textures/SmokePuff");
        _position = new Vector2(Game.GraphicsDevice.Viewport.Width / 2 - (int)_bodySize.X / 2,
                               Game.GraphicsDevice.Viewport.Height / 2 - (int)_bodySize.Y / 2);

        // Create smoke puffs for the smoke puff pool
        for (int i = 0; i < MaxSmokePuffs; i++)
        {
            _availableSmokePuffs.Push(new SmokePuff(Game, _gamePlayScreen, _smokePuffTexture));
        }

        base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
        if (!(_gamePlayScreen.IsActive && _gamePlayScreen.IsStarted))
        {
            base.Update(gameTime);
            return;
        }

        //        if (IsCollectingHoney)
        //        {
        //            // We want this animation to use a sub animation 
        //            // So must calculate when to call the sub animation
        //            if (_collectingHoneyFrameCounter > 3)
        //            {
        //                AnimationDefinitions[BeekeeperCollectingHoneyAnimationKey].Update(gameTime, true, true);
        //            }
        //            else
        //            {
        //                AnimationDefinitions[BeekeeperCollectingHoneyAnimationKey].Update(gameTime, true, false);
        //            }

        //            _collectingHoneyFrameCounter++;
        //        }
        //        else
        //        {
        _collectingHoneyFrameCounter = 0;
        //        }


        //        if (isDepositingHoney)
        //        {
        //            if (depositHoneyUpdatingTimer == TimeSpan.Zero)
        //            {
        //                depositHoneyUpdatingTimer = gameTime.TotalGameTime;
        //            }

        //            AnimationDefinitions[BeekeeperDepositingHoneyAnimationKey].Update(gameTime, true);
        //        }

        // The oldest smoke puff might have expired and should therefore be recycled
        if ((FiredSmokePuffs.Count > 0) && (FiredSmokePuffs.Peek().IsGone))
        {
            _availableSmokePuffs.Push(FiredSmokePuffs.Dequeue());
        }

        //        // If the beeKeeper is stung by a bee we want to create a flashing 
        //        // effect. 
        //        if (_isStung || isFlashing)
        //        {
        //            stungDrawingCounter++;

        //            if (stungDrawingCounter > stungDrawingInterval)
        //            {
        //                stungDrawingCounter = 0;
        //                isDrawnLastStungInterval = !isDrawnLastStungInterval;
        //            }
        //            // if time is up, end the flashing effect
        //            if (stungTime + stungDuration < gameTime.TotalGameTime)
        //            {
        //                _isStung = false;

        //                if (stungTime + stungDuration + flashingDuration < gameTime.TotalGameTime)
        //                {
        //                    isFlashing = false;
        //                    stungDrawingCounter = -1;
        //                }

        //                AnimationDefinitions[LegAnimationKey].Update(gameTime, IsInMotion);
        //            }
        //        }
        //        else
        //        {
        AnimationDefinitions[LegAnimationKey].Update(gameTime, IsInMotion);
        //        }

        if (_needToShootSmoke)
        {
            AnimationDefinitions[SmokeAnimationKey].Update(gameTime, _needToShootSmoke);

            _shootSmokePuffTimer -= gameTime.ElapsedGameTime;
            if (_shootSmokePuffTimer <= TimeSpan.Zero)
            {
                ShootSmoke();
                _shootSmokePuffTimer = _shootSmokePuffTimerInitialValue;
            }
        }

        base.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
        if (!(_gamePlayScreen.IsActive && _gamePlayScreen.IsStarted))
        {
            base.Draw(gameTime);
            return;
        }

        //        // Make sure not to draw the beekeeper while flashing
        //        if (_isStung || isFlashing)
        //        {
        //            if (stungDrawingCounter != stungDrawingInterval)
        //            {
        //                if (isDrawnLastStungInterval)
        //                {
        //                    return;
        //                }
        //            }
        //        }

        _spriteBatch.Begin();

        //        // if stung we want to show another animation
        //        if (_isStung)
        //        {
        //            scaledSpriteBatch.Draw(Game.Content.Load<Texture2D>("Textures/hit"), position, Color.White);
        //            scaledSpriteBatch.End();
        //            return;
        //        }

        //        // If collecting honey, draw the appropriate animation
        //        if (IsCollectingHoney)
        //        {
        //            AnimationDefinitions[BeekeeperCollectingHoneyAnimationKey].Draw(scaledSpriteBatch, position,
        //                SpriteEffects.None);
        //            scaledSpriteBatch.End();
        //            return;
        //        }


        //        if (isDepositingHoney)
        //        {
        //            if (_velocity != Vector2.Zero)
        //            {
        //                isDepositingHoney = false;
        //                AudioManager.StopSound("DepositingIntoVat_Loop");
        //            }

        //            // We want the deposit duration to sync with the deposit  
        //            // animation
        //            // So we manage the timing ourselves
        //            if (depositHoneyUpdatingTimer != TimeSpan.Zero &&
        //                depositHoneyUpdatingTimer + depositHoneyUpdatingInterval < gameTime.TotalGameTime)
        //            {
        //                depositHoneyTimerCounter++;
        //                depositHoneyUpdatingTimer = TimeSpan.Zero;
        //            }

        //            AnimationDefinitions[BeekeeperDepositingHoneyAnimationKey].Draw(scaledSpriteBatch, position,
        //                SpriteEffects.None);

        //            if (depositHoneyTimerCounter == honeyDepositFrameCount - 1)
        //            {
        //                isDepositingHoney = false;
        //                depositHoneyCallback.Invoke(null);
        //                AnimationDefinitions[BeekeeperDepositingHoneyAnimationKey].PlayFromFrameIndex(0);
        //            }

        //            scaledSpriteBatch.End();
        //            return;
        //        }

        bool hadDirectionChanged = false;

        // See if the direction changed
        if (_newDirection != _direction)
        {
            hadDirectionChanged = true;
            _direction = _newDirection;
        }

        if (hadDirectionChanged)
        {
            // Update the animation
            _lastFrameCounter = 0;
            AnimationDefinitions[LegAnimationKey].PlayFromFrameIndex(_lastFrameCounter + (int)_direction);
            AnimationDefinitions[ShootingAnimationKey].PlayFromFrameIndex(_lastFrameCounter + (int)_direction);
            AnimationDefinitions[BodyAnimationKey].PlayFromFrameIndex(_lastFrameCounter + (int)_direction);
        }
        else
        {
            // Because our animation is 8 cells, but the row is 16 cells,
            // we need to reset the counter after 8 rounds

            if (_lastFrameCounter == 8)
            {
                _lastFrameCounter = 0;
                AnimationDefinitions[LegAnimationKey].PlayFromFrameIndex(_lastFrameCounter + (int)_direction);
                AnimationDefinitions[ShootingAnimationKey].PlayFromFrameIndex(
                    _lastFrameCounter + (int)_direction);
                AnimationDefinitions[BodyAnimationKey].PlayFromFrameIndex(_lastFrameCounter + (int)_direction);
            }
            else
            {
                _lastFrameCounter++;
            }
        }

        AnimationDefinitions[LegAnimationKey].Draw(_spriteBatch, _position, 1f, SpriteEffects.None);


        if (_needToShootSmoke)
        {
            // Draw the body
            AnimationDefinitions[ShootingAnimationKey].Draw(_spriteBatch, _position, 1f, SpriteEffects.None);

            // If true we need to draw smoke
            if (_smokeAdjustment != Vector2.Zero)
            {
                AnimationDefinitions[SmokeAnimationKey].Draw(_spriteBatch, _position + _smokeAdjustment, 1f, _currentEffect);
            }
        }
        else
        {
                AnimationDefinitions[BodyAnimationKey].Draw(_spriteBatch, _position, 1f, SpriteEffects.None);
        }
        _spriteBatch.End();

        base.Draw(gameTime);
    }


    //    #endregion

    //    #region Public Methods


    //    /// <summary>
    //    /// Checks if a given rectanlge intersects with one of the smoke puffs fired by the beekeeper.
    //    /// </summary>
    //    /// <param name="checkRectangle">The rectangle to check for collisions with smoke puffs.</param>
    //    /// <returns>One of the smoke puffs with which the supplied regtangle collides, or null if it collides with
    //    /// none.</returns>
    //    public SmokePuff CheckSmokeCollision(Rectangle checkRectangle)
    //    {
    //        foreach (SmokePuff smokePuff in FiredSmokePuffs)
    //        {
    //            if (checkRectangle.HasCollision(smokePuff.CentralCollisionArea))
    //            {
    //                return smokePuff;
    //            }
    //        }

    //        return null;
    //    }

    //    /// <summary>
    //    /// Maek the beekeeper as being stung by a bee.
    //    /// </summary>
    //    /// <param name="occurTime">The time at which the beekeeper was stung.</param>
    //    public void Stung(TimeSpan occurTime)
    //    {
    //        if (!_isStung && !isFlashing)
    //        {
    //            _isStung = true;
    //            isFlashing = true;
    //            stungTime = occurTime;
    //            _needToShootSmoke = false;
    //        }
    //    }

    /// <summary>
    /// Updates the beekeeper's position.
    /// </summary>
    /// <param name="movement">A vector which contains the desired adjustment to 
    /// the beekeeper's position.</param>
    public void SetMovement(Vector2 movement)
    {
        if (!_isStung)
        {
            _velocity = movement;
            _position += _velocity;
        }
    }

    /// <summary>
    /// Makes sure the beekeeper's direction matches his movement direction.
    /// </summary>
    /// <param name="movementDirection">A vector indicating the beekeeper's movement direction.</param>
    public void SetDirection(Vector2 movementDirection)
    {
        DetermineDirection(movementDirection, ref _newDirection, ref _smokeAdjustment);
        _currentEffect = GetSpriteEffect(movementDirection);
    }

    //    /// <summary>
    //    /// Starts the process of transferring honey to the honey vat.
    //    /// </summary>
    //    /// <param name="honeyDepositFrameCount">The amount of frames in the honey
    //    /// depositing animation.</param>
    //    /// <param name="callback">Callback to invoke once the process is 
    //    /// complete.</param>
    //    public void StartTransferHoney(int honeyDepositFrameCount, AsyncCallback callback)
    //    {
    //        depositHoneyCallback = callback;
    //        this.honeyDepositFrameCount = honeyDepositFrameCount;
    //        isDepositingHoney = true;
    //        depositHoneyTimerCounter = 0;


    //        AudioManager.PlaySound("DepositingIntoVat_Loop");
    //    }

    //    /// <summary>
    //    /// Marks the honey transfer process as complete.
    //    /// </summary>
    //    public void EndTransferHoney()
    //    {
    //        isDepositingHoney = false;
    //    }


    //    #endregion

    //    #region Private Methods


    /// <summary>
    /// Shoots a puff of smoke. If too many puffs of smoke have already been fired, the oldest one vanishes and
    /// is replaced with a new one.        
    /// </summary>
    private void ShootSmoke()
    {
        SmokePuff availableSmokePuff;

        if (_availableSmokePuffs.Count > 0)
        {
            // Take a smoke puff from the pool
            availableSmokePuff = _availableSmokePuffs.Pop();
        }
        else
        {
            // Take the oldest smoke puff and use it
            availableSmokePuff = FiredSmokePuffs.Dequeue();
        }

        Vector2 beeKeeperCenter = Bounds.Center.GetVector();
        Vector2 smokeInitialPosition = beeKeeperCenter;

        availableSmokePuff.Fire(smokeInitialPosition, GetSmokeVelocityVector());
        FiredSmokePuffs.Enqueue(availableSmokePuff);
    }

    /// <summary>
    /// Used to return a vector which will serve as shot smoke velocity.
    /// </summary>
    /// <returns>A vector which serves as the initial velocity of smoke puffs being shot.</returns>
    private Vector2 GetSmokeVelocityVector()
    {
        Vector2 initialVector;

        switch (_direction)
        {
            case WalkingDirection.Down:
                initialVector = new Vector2(0, 1);
                break;
            case WalkingDirection.Up:
                initialVector = new Vector2(0, -1);
                break;
            case WalkingDirection.Left:
                initialVector = new Vector2(-1, 0);
                break;
            case WalkingDirection.Right:
                initialVector = new Vector2(1, 0);
                break;
            case WalkingDirection.LeftDown:
                initialVector = new Vector2(-1, 1);
                break;
            case WalkingDirection.RightDown:
                initialVector = new Vector2(1, 1);
                break;
            case WalkingDirection.LeftUp:
                initialVector = new Vector2(-1, -1);
                break;
            case WalkingDirection.RightUp:
                initialVector = new Vector2(1, -1);
                break;
            default:
                throw new InvalidOperationException("Determining the vector for an invalid walking direction");
        }

        return initialVector * 2f + _velocity * 1f;
    }

    /// <summary>
    /// Returns an effect appropriate to the supplied vector which either does nothing or flips the beekeeper horizontally.
    /// </summary>
    /// <param name="movementDirection">A vector depicting the beekeeper's movement.</param>
    /// <returns>A sprite effect that should be applied to the beekeeper.</returns>
    private SpriteEffects GetSpriteEffect(Vector2 movementDirection)
    {
        // Checks if there is any movement input
        if (movementDirection != Vector2.Zero)
        {
            // If beekeeper is facing left
            if (movementDirection.X < 0)
            {
                _lastEffect = SpriteEffects.FlipHorizontally;
            }
            else if (movementDirection.X > 0)
            {
                _lastEffect = SpriteEffects.None;
            }
        }

        return _lastEffect;
    }

    /// <summary>
    /// Returns movement information according to the current virtual thumbstick input.
    /// </summary>
    /// <param name="movement">Vector indicating the current beekeeper movement.</param>
    /// <param name="tempDirection">Enum describing the inpot direction.</param>
    /// <param name="_smokeAdjustment">Adjustment to smoke position according to input direction.</param>
    private void DetermineDirection(Vector2 movement, ref WalkingDirection tempDirection, ref Vector2 _smokeAdjustment)
    {
        if (movement == Vector2.Zero)
        {
            return;
        }

        if (Math.Abs(movement.X) > Math.Abs(movement.Y))
        {
            DetermineDirectionDominantX(movement, ref tempDirection, ref _smokeAdjustment);
        }
        else
        {
            DetermineDirectionDominantY(movement, ref tempDirection, ref _smokeAdjustment);
        }
    }

    /// <summary>
    /// Returns movement information according to the current virtual thumbstick input, given that advancement
    /// along the X axis is greater than along the Y axis.
    /// </summary>
    /// <param name="movement">Vector indicating the current beekeeper movement.</param>
    /// <param name="tempDirection">Enum describing the input direction.</param>
    /// <param name="_smokeAdjustment">Adjustment to smoke position according to input direction.</param>
    private void DetermineDirectionDominantX(Vector2 movement, ref WalkingDirection tempDirection, ref Vector2 _smokeAdjustment)
    {
        if (movement.X > 0)
        {
            if (movement.Y > 0.25f)
            {
                tempDirection = WalkingDirection.RightDown;
                _smokeAdjustment = new Vector2(UIConstants.SprayRightOffset, UIConstants.SprayDownOffset);
            }
            else if (movement.Y < -0.25f)
            {
                tempDirection = WalkingDirection.RightUp;
                _smokeAdjustment = new Vector2(UIConstants.SprayRightOffset, UIConstants.SprayUpOffset);
            }
            else
            {
                tempDirection = WalkingDirection.Right;
                _smokeAdjustment = new Vector2(UIConstants.SprayRightOffset, UIConstants.SprayMiddleOffset);
            }
        }
        else
        {
            if (movement.Y > 0.25f)
            {
                tempDirection = WalkingDirection.LeftDown;
                _smokeAdjustment = new Vector2(-UIConstants.SprayLeftOffset, UIConstants.SprayDownOffset);
            }
            else if (movement.Y < -0.25f)
            {
                tempDirection = WalkingDirection.LeftUp;
                _smokeAdjustment = new Vector2(-UIConstants.SprayLeftOffset, UIConstants.SprayUpOffset);
            }
            else
            {
                tempDirection = WalkingDirection.Left;
                _smokeAdjustment = new Vector2(-UIConstants.SprayLeftOffset, UIConstants.SprayMiddleOffset);
            }
        }
    }

    /// <summary>
    /// Returns movement information according to the current virtual thumbstick input, given that advancement
    /// along the Y axis is greater than along the X axis.
    /// </summary>
    /// <param name="movement">Vector indicating the current beekeeper movement.</param>      
    /// <param name="tempDirection">Enum describing the input direction.</param>
    /// <param name="_smokeAdjustment">Adjustment to smoke position according to input direction.</param>
    private void DetermineDirectionDominantY(Vector2 movement, ref WalkingDirection tempDirection, ref Vector2 _smokeAdjustment)
    {
        if (movement.Y > 0)
        {
            if (movement.X > 0.25f)
            {
                tempDirection = WalkingDirection.RightDown;
                _smokeAdjustment = new Vector2(UIConstants.SprayRightOffset, UIConstants.SprayDownOffset);
            }
            else if (movement.X < -0.25f)
            {
                tempDirection = WalkingDirection.LeftDown;
                _smokeAdjustment = new Vector2(-UIConstants.SprayLeftOffset, UIConstants.SprayDownOffset);
            }
            else
            {
                tempDirection = WalkingDirection.Down;
                _smokeAdjustment = Vector2.Zero;
            }
        }
        else
        {
            if (movement.X > 0.25f)
            {
                tempDirection = WalkingDirection.RightUp;
                _smokeAdjustment = new Vector2(UIConstants.SprayRightOffset, UIConstants.SprayUpOffset);
            }
            else if (movement.X < -0.25f)
            {
                tempDirection = WalkingDirection.LeftUp;
                _smokeAdjustment = new Vector2(-UIConstants.SprayLeftOffset, UIConstants.SprayUpOffset);
            }
            else
            {
                tempDirection = WalkingDirection.Up;
                _smokeAdjustment = Vector2.Zero;
            }
        }
    }
}
