using HoneycumbRush;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace HoneycombRush;

public class GameplayScreen : GameScreen
{
    private const string SmokeText = "Smoke";

    private Dictionary<string, ScaledAnimation> _animations;
    private List<Beehive> _beehives = new List<Beehive>();
    private List<Bee> _bees = new List<Bee>();

    private SpriteFont _font16px;
    private SpriteFont _font36px;
    private Texture2D _arrowTexture;
    private Texture2D _background;
    private Texture2D _controlstickBoundary;
    private Texture2D _controlstick;
    private Texture2D _beehiveTexture;
    private Texture2D _smokeButton;
    private Vector2 _smokeButtonPosition;
    private Vector2 _smokeTextLocation;
    private Vector2 _vatArrowPosition;
    /// <summary>
    /// A vector describing the movement direction according to the current user input.
    /// </summary>
    private Vector2 _movementVector;

    private DifficultyMode _gameDifficultyLevel;
    private BeeKeeper _beeKeeper;
    private Vat _vat;
    private HoneyJar _jar;
    private ScoreBar _smokeButtonScorebar;

    private bool _isAtStartupCountDown;
    private bool _levelEnded;
    private bool _drawArrow;
    private bool _drawArrowInInterval;
    private bool _isSmokebuttonClicked;
    private bool _isInMotion;
    private bool _isLevelEnd;
    private bool _isUserWon;
    private bool _userInputToExit;
    private int _arrowCounter;

    private int _amountOfSoldierBee;
    private int _amountOfWorkerBee;
    private TimeSpan _gameElapsed;
    private TimeSpan _startScreenTime;

    public bool IsStarted
    {
        get
        {
            return !_isAtStartupCountDown && !_levelEnded;
        }
    }

    private bool IsInMotion
    {
        get
        {
            return _isInMotion;
        }
        set
        {
            _isInMotion = value;
            if (_beeKeeper != null)
            {
                _beeKeeper.IsInMotion = _isInMotion;
            }
        }
    }

    //        DebugSystem debugSystem;
    //        bool showDebugInfo = false;
    //        Rectangle deviceUpperRightCorner = new Rectangle(750, 0, 50, 50);

    public static int FinalScore;

    public GameplayScreen(DifficultyMode gameDifficultyMode)
    {
        TransitionOnTime = TimeSpan.FromSeconds(0.0);
        TransitionOffTime = TimeSpan.FromSeconds(0.0);
        _startScreenTime = TimeSpan.FromSeconds(3);

        //            //Loads configuration

        //            //var config =   Path.Combine(NSBundle.MainBundle.ResourcePath,"Content/Configuration/Configuration.xml");
        //            ConfigurationManager.LoadConfiguration(XDocument.Load("Content/Configuration/Configuration.xml"));
        //            //ConfigurationManager.LoadConfiguration(XDocument.Load(config));

        //            ConfigurationManager.DifficultyMode = gameDifficultyMode;

        _gameDifficultyLevel = gameDifficultyMode;
        //            _gameElapsed = ConfigurationManager.ModesConfiguration[gameDifficultyLevel]._gameElapsed;
        _gameElapsed = TimeSpan.FromSeconds(120);

        _amountOfSoldierBee = 4;
        _amountOfWorkerBee = 16;

        _smokeButtonPosition = new Vector2(664, 346);

        IsInMotion = false;
        _isAtStartupCountDown = true;
        _isLevelEnd = false;
    }

    public override void LoadContent()
    {
        base.LoadContent();
    }

    public void LoadAssets()
    {
        // Loads the animation dictionary from an xml file
        _animations = new Dictionary<string, ScaledAnimation>();
        LoadAnimationFromXML();

        // Loads all textures that are required
        LoadTextures();

        // Create all game components
        CreateGameComponents();
    }

    /// <summary>
    /// Unloads game components which are no longer needed once the game ends.
    /// </summary>
    public override void UnloadContent()
    {
        var componentList = ScreenManager.Game.Components;

        for (int index = 0; index < componentList.Count; index++)
        {
            if (componentList[index] is TexturedDrawableGameComponent || componentList[index] is ScoreBar)
            {
                componentList.RemoveAt(index);
                index--;
            }
        }

        base.UnloadContent();
    }

    /// <summary>
    /// Handle the player's input.
    /// </summary>
    /// <param name="input"></param>
    public override void HandleInput(GameTime gameTime, InputState input)
    {
        if (IsActive)
        {
            if (input == null)
            {
                throw new ArgumentNullException("input");
            }

            if (input.IsPauseGame(null))
            {
                PauseCurrentGame();
            }
        }

        _isSmokebuttonClicked = false;

        PlayerIndex player;

        if (_isLevelEnd)
        {
            if (input.IsNewKeyPress(Keys.Enter, ControllingPlayer, out player) ||
                input.IsNewKeyPress(Keys.Space, ControllingPlayer, out player))
            {
                _userInputToExit = true;
            }
        }

        if (!IsStarted)
        {
            return;
        }

        // Handle keyboard
        //            if (input.IsNewKeyPress(Keys.Y, ControllingPlayer, out player))
        //            {
        //                showDebugInfo = !showDebugInfo;
        //            }

        if ((input.IsKeyDown(Keys.Space, ControllingPlayer, out player) ||
              input.IsNewMouseClick(InputState.MouseButton.Right, ControllingPlayer, out player))
            && !_beeKeeper.IsCollectingHoney
            && !_beeKeeper.IsStung)
        {
            _isSmokebuttonClicked = true;
        }

        _movementVector = SetMotion(input);
        _beeKeeper.SetDirection(_movementVector);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
        // When the game starts the first thing the user sees is the count down before the game actually begins
        if (_isAtStartupCountDown)
        {
            _startScreenTime -= gameTime.ElapsedGameTime;
        }

        // Check for and handle a game over
        if (CheckIfCurrentGameFinished())
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            return;
        }

        if (!(IsActive && IsStarted))
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            return;
        }

        //            // Show all diagnostic counters
        //            debugSystem.FpsCounter.Visible = showDebugInfo;
        //            debugSystem.TimeRuler.Visible = showDebugInfo;
        //            debugSystem.TimeRuler.ShowLog = showDebugInfo;

        _gameElapsed -= gameTime.ElapsedGameTime;

        HandleSmoke();

        HandleCollision(gameTime);

        HandleVatHoneyArrow();

        _beeKeeper.DrawOrder = 1;
        int beeKeeperY = (int)(_beeKeeper.Position.Y + _beeKeeper.Bounds.Height - 2);

        // We want to determine the draw order of the beekeeper,
        // if the beekeeper is under half the height of the beehive 
        // it should be drawn over the beehive.
        foreach (Beehive beehive in _beehives)
        {
            if (beeKeeperY > beehive.Bounds.Y)
            {
                if (beehive.Bounds.Y + beehive.Bounds.Height / 2 < beeKeeperY)
                {
                    _beeKeeper.DrawOrder = Math.Max(_beeKeeper.DrawOrder, beehive.Bounds.Y + 1);
                }
            }
        }

        //            if (_gameElapsed.Minutes == 0 && _gameElapsed.Seconds == 10)
        //            {
        //                AudioManager.PlaySound("10SecondCountDown");
        //            }
        //            if (_gameElapsed.Minutes == 0 && _gameElapsed.Seconds == 30)
        //            {
        //                AudioManager.PlaySound("30SecondWarning");
        //            }

        // Update the time remaining displayed on the vat
        _vat.DrawTimeLeft(_gameElapsed);

        base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
        SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

        spriteBatch.Begin();

        Rectangle bounds = ScreenManager.GraphicsDevice.Viewport.Bounds;
        spriteBatch.Draw(_background, bounds, null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 1);


        // Draw count down screen
        if (_isAtStartupCountDown)
        {
            DrawStartupString();
        }

        if (IsActive && IsStarted)
        {
            DrawSmokeButton();
            spriteBatch.DrawString(_font16px, SmokeText, _smokeTextLocation, Color.White);
            DrawVatHoneyArrow();
        }

        DrawLevelEndIfNecessary();

        spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// If the level is over, draws text describing the level's outcome.
    /// </summary>
    private void DrawLevelEndIfNecessary()
    {
        if (_isLevelEnd)
        {
            string stringToDisplay = string.Empty;

            if (_isUserWon)
            {
                //if (FinalScore != 0 && HighScoreScreen.IsInHighscores(FinalScore))
                //{
                //    stringToDisplay = "It's a new\nHigh-Score!";
                //}
                //else
                //{
                    stringToDisplay = "You Win!";
                //}
            }
            else
            {
                stringToDisplay = "Time Is Up!";
            }

            Vector2 stringVector = _font36px.MeasureString(stringToDisplay);

            ScreenManager.SpriteBatch.DrawString(_font36px, stringToDisplay,
                            new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2 - stringVector.X / 2,
                                        ScreenManager.GraphicsDevice.Viewport.Height / 2 - stringVector.Y / 2),
                            Color.White);
        }
    }

    /// <summary>
    /// Advances to the next screen based on the current difficulty and whether or not the user has won.
    /// </summary>
    /// <param name="isWon">Whether or not the user has won the current level.</param>
    private void MoveToNextScreen(bool isWon)
    {
        ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);

        if (isWon)
        {
            switch (_gameDifficultyLevel)
            {
                case DifficultyMode.Easy:
                case DifficultyMode.Medium:
                    ScreenManager.AddScreen(
                            new LevelOverScreen("You Finished Level: " + _gameDifficultyLevel.ToString(),
                                ++_gameDifficultyLevel), null);
                    break;
                case DifficultyMode.Hard:
                    ScreenManager.AddScreen(new LevelOverScreen("You Win", null), null);
                    break;
            }
        }
        else
        {
            ScreenManager.AddScreen(new LevelOverScreen("You Lose", null), null);
        }

        //AudioManager.StopMusic();
        //AudioManager.StopSound("BeeBuzzing_Loop");
    }

    /// <summary>
    /// Pause the game.
    /// </summary>
    private void PauseCurrentGame()
    {
        //debugSystem.FpsCounter.Visible = false;
        //debugSystem.TimeRuler.Visible = false;
        //debugSystem.TimeRuler.ShowLog = false;

        //// Pause sounds
        //AudioManager.PauseResumeSounds(false);

        // Set pause screen
        ScreenManager.AddScreen(new BackgroundScreen("pauseBackground"), null);
        ScreenManager.AddScreen(new PauseScreen(), null);
    }

    /// <summary>
    /// Loads animation settings from an xml file.
    /// </summary>
    private void LoadAnimationFromXML()
    {
        XDocument doc = XDocument.Load("Content/Textures/AnimationsDefinition.xml");
        XName name = XName.Get("Definition");
        var definitions = doc.Document.Descendants(name);

        // Loop over all definitions in the XML
        foreach (XElement animationDefinition in definitions)
        {
            // Get the name of the animation
            string animationAlias = animationDefinition.Attribute("Alias").Value;
            Texture2D texture = ScreenManager.Game.Content.Load<Texture2D>(animationDefinition.Attribute("SheetName").Value);

            // Get the frame size (width & height)
            Point frameSize = new Point();
            frameSize.X = int.Parse(animationDefinition.Attribute("FrameWidth").Value);
            frameSize.Y = int.Parse(animationDefinition.Attribute("FrameHeight").Value);

            // Get the frames sheet dimensions
            Point sheetSize = new Point();
            sheetSize.X = int.Parse(animationDefinition.Attribute("SheetColumns").Value);
            sheetSize.Y = int.Parse(animationDefinition.Attribute("SheetRows").Value);

            ScaledAnimation animation = new ScaledAnimation(texture, frameSize, sheetSize);

            // Checks for sub-animation definition
            if (animationDefinition.Element("SubDefinition") != null)
            {
                int startFrame = int.Parse(animationDefinition.Element("SubDefinition").Attribute("StartFrame").Value);

                int endFrame = int.Parse(animationDefinition.Element("SubDefinition").Attribute("EndFrame").Value);

                animation.SetSubAnimation(startFrame, endFrame);
            }

            if (animationDefinition.Attribute("Speed") != null)
            {
                animation.SetFrameInterval(TimeSpan.FromMilliseconds(
                    double.Parse(animationDefinition.Attribute("Speed").Value)));
            }

            // If the definition has an offset defined - it should be  
            // rendered relative to some element/animation

            if (null != animationDefinition.Attribute("OffsetX") &&
                null != animationDefinition.Attribute("OffsetY"))
            {
                animation.Offset = new Vector2(int.Parse(animationDefinition.Attribute("OffsetX").Value),
                    int.Parse(animationDefinition.Attribute("OffsetY").Value));
            }

            _animations.Add(animationAlias, animation);
        }
    }

    /// <summary>
    /// Create all the game components.
    /// </summary>
    private void CreateGameComponents()
    {
        Rectangle safeArea = SafeArea;

        Texture2D jarTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/honeyJar");
        Vector2 honeyJarLocation = safeArea.GetVector() + new Vector2(UIConstants.HoneyJarLeftMargin, UIConstants.HoneyJarTopMargin);

        Vector2 jarBarLocation = honeyJarLocation + new Vector2(0, jarTexture.Height + 7);

        ScoreBar scoreBar = new ScoreBar(ScreenManager.Game, 0, 100, jarBarLocation, (int)(jarTexture.Height / 6), (int)(jarTexture.Width), Color.Blue,
            ScoreBar.ScoreBarOrientation.Horizontal, 0, this, true);
        ScreenManager.Game.Components.Add(scoreBar);

        // Create the honey jar
        _jar = new HoneyJar(ScreenManager.Game, this, jarTexture, honeyJarLocation, scoreBar);
        ScreenManager.Game.Components.Add(_jar);

        // Create all the beehives and the bees
        CreateBeehives(safeArea, _jar);

        // We only initialize the smoke button position here since we need access
        // to the screen manager in order to do so (and it is null in the 
        // constructor)
        _smokeButtonPosition = new Vector2(
            safeArea.Right - UIConstants.SmokeButtonRightAbsoluteMargin,
            safeArea.Bottom - UIConstants.SmokeButtonBottomAbsoluteMargin);

        // Create the smoke gun's score bar
        //int totalSmokeAmount = ConfigurationManager.ModesConfiguration[_gameDifficultyLevel].TotalSmokeAmount;
        int totalSmokeAmount = 100;
        Vector2 smokeBarLocation = _smokeButtonPosition + new Vector2(UIConstants.SmokeButtonSize / 8, UIConstants.SmokeButtonSize);

        _smokeButtonScorebar = new ScoreBar(ScreenManager.Game, 0, totalSmokeAmount, smokeBarLocation, (int)(UIConstants.SmokeButtonSize / 10),
            (int)(UIConstants.SmokeButtonSize * 3 / 4), Color.White, ScoreBar.ScoreBarOrientation.Horizontal, totalSmokeAmount, this, false);

        _smokeTextLocation = _smokeButtonPosition + new Vector2(
            UIConstants.SmokeButtonSize / 2 - _font16px.MeasureString(SmokeText).X / 2,
            UIConstants.SmokeButtonSize * 11 / 10);

        ScreenManager.Game.Components.Add(_smokeButtonScorebar);

        // Creates the BeeKeeper
        _beeKeeper = new BeeKeeper(ScreenManager.Game, this);
        _beeKeeper.AnimationDefinitions = _animations;
        ScreenManager.Game.Components.Add(_beeKeeper);

        // Creates the vat
        Texture2D vatTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/vat");

        Vector2 vatLocation = new Vector2(
            safeArea.Center.X - vatTexture.Width / 2,
            safeArea.Bottom - vatTexture.Height - UIConstants.VatBottomMargin);
        Vector2 vatScorebarLocation = vatLocation + new Vector2(
            (vatTexture.Width - UIConstants.VatScorebarWidth) / 2,
            vatTexture.Height * 7 / 10);

        scoreBar = new ScoreBar(ScreenManager.Game, 0, 300, vatScorebarLocation, UIConstants.VatScorebarHeight,
                UIConstants.VatScorebarWidth, Color.White, ScoreBar.ScoreBarOrientation.Horizontal, 0, this, true);

        _vat = new Vat(ScreenManager.Game, this, vatTexture, vatLocation, scoreBar);
        ScreenManager.Game.Components.Add(_vat);

        _vatArrowPosition = vatLocation + new Vector2(vatTexture.Width / 2 - _arrowTexture.Width / 2, UIConstants.VatArrowOffset);

        ScreenManager.Game.Components.Add(scoreBar);
        scoreBar.DrawOrder = _vat.DrawOrder + 1;
    }

    /// <summary>
    /// Creates all the beehives and bees.
    /// </summary>
    private void CreateBeehives(Rectangle safeArea, HoneyJar jar)
    {
        // Init position parameters
        Vector2 scorebarPosition = new Vector2(_beehiveTexture.Width / 4, _beehiveTexture.Height * 9 / 10);

        Vector2[] beehivePositions = new Vector2[5]
        {
            // top left
            new Vector2(safeArea.Left + UIConstants.BeehiveLeftMargin, safeArea.Top + UIConstants.BeehiveTopMargin),
            // top middle
            new Vector2(safeArea.Center.X - _beehiveTexture.Width / 2, safeArea.Top + UIConstants.BeehiveTopMargin),
            // top right
            new Vector2(safeArea.Right - _beehiveTexture.Width - UIConstants.BeehiveRightMargin, safeArea.Top + UIConstants.BeehiveTopMargin),
            // left
            new Vector2(safeArea.Left + UIConstants.BeehiveLeftMargin, safeArea.Center.Y - _beehiveTexture.Height / 2 + UIConstants.BeehiveMiddleOffset),
            // right
            new Vector2(safeArea.Right - _beehiveTexture.Width - UIConstants.BeehiveRightMargin, safeArea.Center.Y - _beehiveTexture.Height / 2  + UIConstants.BeehiveMiddleOffset)
        };

        // Create the beehives
        for (int beehiveCounter = 0; beehiveCounter < beehivePositions.Length; beehiveCounter++)
        {
            ScoreBar scoreBar = new ScoreBar(ScreenManager.Game, 0, 100, beehivePositions[beehiveCounter] +
                                    scorebarPosition, (int)(_beehiveTexture.Height / 10),
                                    (int)(_beehiveTexture.Width / 2), Color.Green,
                                    ScoreBar.ScoreBarOrientation.Horizontal, 100, this, false);
            ScreenManager.Game.Components.Add(scoreBar);

            Beehive beehive = new Beehive(ScreenManager.Game, this, _beehiveTexture, scoreBar, beehivePositions[beehiveCounter]);

            beehive.AnimationDefinitions = _animations;

            ScreenManager.Game.Components.Add(beehive);
            _beehives.Add(beehive);
            scoreBar.DrawOrder = beehive.DrawOrder;
        }

        for (int beehiveIndex = 0; beehiveIndex < beehivePositions.Length; beehiveIndex++)
        {
            // Create the Soldier bees
            for (int SoldierBeeCounter = 0; SoldierBeeCounter < _amountOfSoldierBee; SoldierBeeCounter++)
            {
                SoldierBee bee = new SoldierBee(ScreenManager.Game, this, _beehives[beehiveIndex]);
                bee.AnimationDefinitions = _animations;
                ScreenManager.Game.Components.Add(bee);
                _bees.Add(bee);
            }

            // Creates the worker bees
            for (int workerBeeCounter = 0; workerBeeCounter < _amountOfWorkerBee; workerBeeCounter++)
            {
                WorkerBee bee = new WorkerBee(ScreenManager.Game, this, _beehives[beehiveIndex]);
                bee.AnimationDefinitions = _animations;
                ScreenManager.Game.Components.Add(bee);
                _bees.Add(bee);
            }
        }
    }

    /// <summary>
    /// Loads all the necessary textures.
    /// </summary>
    private void LoadTextures()
    {
        _font16px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont16px");
        _font36px = ScreenManager.Game.Content.Load<SpriteFont>("Fonts/GameScreenFont36px");
        
        _beehiveTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/beehive");
        _smokeButton = ScreenManager.Game.Content.Load<Texture2D>("Textures/smokeBtn");
        _arrowTexture = ScreenManager.Game.Content.Load<Texture2D>("Textures/arrow");

        _background = ScreenManager.Game.Content.Load<Texture2D>("Textures/Backgrounds/GamePlayBackground");
        
        _controlstickBoundary = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstickBoundary");
        _controlstick = ScreenManager.Game.Content.Load<Texture2D>("Textures/controlstick");
    }

    /// <summary>
    /// Moves the beekeeper.
    /// </summary>
    /// <returns>Returns a vector indicating the beekeeper's movement direction.
    /// </returns>
    private Vector2 SetMotion(InputState input)
    {
        // Calculate the beekeeper location, if allow moving
        Rectangle safeArea = SafeArea;

        PlayerIndex playerIndex;

        Vector2 leftThumbstick = Vector2.Zero;

        // Move on to keyboard input if we still have nothing
        if (leftThumbstick == Vector2.Zero)
        {
            float vecX = 0;
            float vecY = 0;

            if (input.IsKeyDown(Keys.Left, ControllingPlayer, out playerIndex))
            {
                vecX--;
            }
            if (input.IsKeyDown(Keys.Right, ControllingPlayer, out playerIndex))
            {
                vecX++;
            }
            if (input.IsKeyDown(Keys.Up, ControllingPlayer, out playerIndex))
            {
                vecY--;
            }
            if (input.IsKeyDown(Keys.Down, ControllingPlayer, out playerIndex))
            {
                vecY++;
            }
            if (input.IsMouseDown(InputState.MouseButton.Left, ControllingPlayer, out playerIndex))
            {
                vecX = input.CurrentMouseStates[(int)playerIndex].X - _beeKeeper.Bounds.X;
                vecY = input.CurrentMouseStates[(int)playerIndex].Y - _beeKeeper.Bounds.Y;
            }
            leftThumbstick = new Vector2(vecX, vecY);
            leftThumbstick.Normalize();
        }

        Vector2 _movementVector = leftThumbstick * 12f;

        Rectangle futureBounds = _beeKeeper.Bounds;
        futureBounds.X += (int)_movementVector.X;
        futureBounds.Y += (int)_movementVector.Y;

        if (futureBounds.Left <= safeArea.Left || futureBounds.Right >= safeArea.Right)
        {
            _movementVector.X = 0;
        }
        if (futureBounds.Top <= safeArea.Top || futureBounds.Bottom >= safeArea.Bottom)
        {
            _movementVector.Y = 0;
        }

        if (_movementVector == Vector2.Zero)
        {
            IsInMotion = false;
            _beeKeeper.SetMovement(Vector2.Zero);
        }
        else
        {
            Vector2 beekeeperCalculatedPosition = new Vector2(_beeKeeper.CentralCollisionArea.X, _beeKeeper.CentralCollisionArea.Y) + _movementVector;

            if (!CheckBeehiveCollision(beekeeperCalculatedPosition))
            {
                _beeKeeper.SetMovement(_movementVector);
                IsInMotion = true;
            }
        }

        return _movementVector;
    }

    /// <summary>
    /// Checks if the beekeeper collides with a beehive.
    /// </summary>
    /// <param name="beekeeperPosition">The beekeeper's position.</param>
    /// <returns>True if the beekeeper collides with a beehive and false otherwise.</returns>
    private bool CheckBeehiveCollision(Vector2 beekeeperPosition)
    {
        // We do not use the beekeeper's collision area property as he has not actually moved at this point and
        // is still in his previous position
        Rectangle beekeeperTempCollisionArea = new Rectangle((int)beekeeperPosition.X, (int)beekeeperPosition.Y,
            _beeKeeper.CentralCollisionArea.Width, _beeKeeper.CentralCollisionArea.Height);

        foreach (Beehive currentBeehive in _beehives)
        {
            if (beekeeperTempCollisionArea.Intersects(currentBeehive.CentralCollisionArea))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Check for any of the possible collisions.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    private void HandleCollision(GameTime gameTime)
    {
        bool isCollectingHoney = HandleBeeKeeperBeehiveCollision();

        HandleSmokeBeehiveCollision();

        bool hasCollisionWithVat = HandleVatCollision();

        HandleBeeInteractions(gameTime, hasCollisionWithVat, isCollectingHoney);
    }

    /// <summary>
    /// Handle the interaction of the bees with other game components.
    /// </summary>
    /// <param name="gameTime">Game time information.</param>
    /// <param name="isBeeKeeperCollideWithVat">Whether the beekeeper is currently touching the vat.</param>
    /// <param name="isBeeKeeperCollideWithBeehive">Whether the beekeeper is currently touching a beehive.</param>
    private void HandleBeeInteractions(GameTime gameTime, bool isBeeKeeperCollideWithVat, bool isBeeKeeperCollideWithBeehive)
    {
        // Goes over all the bees
        foreach (Bee bee in _bees)
        {
            // Check for smoke collisions
            SmokePuff intersectingPuff = _beeKeeper.CheckSmokeCollision(bee.Bounds);

            if (intersectingPuff != null)
            {
                bee.HitBySmoke(intersectingPuff);
            }

            // Check for vat collision
            if (_vat.Bounds.HasCollision(bee.Bounds))
            {
                bee.Collide(_vat.Bounds);
            }
            // Check for beekeeper collision
            if (_beeKeeper.Bounds.HasCollision(bee.Bounds))
            {
                if (!bee.IsBeeHit && !isBeeKeeperCollideWithVat && !_beeKeeper.IsStung && !_beeKeeper.IsFlashing && !isBeeKeeperCollideWithBeehive)
                {
                    _jar.DecreaseHoneyByPercent(20);
                    _beeKeeper.Stung(gameTime.TotalGameTime);
                    //AudioManager.PlaySound("HoneyPotBreak");
                    //AudioManager.PlaySound("Stung");
                }

                bee.Collide(_beeKeeper.Bounds);
            }
            // Soldier bee chase logic
            if (bee is SoldierBee)
            {
                SoldierBee SoldierBee = bee as SoldierBee;
                SoldierBee.DistanceFromBeeKeeper = (Vector2.Distance(_beeKeeper.Bounds.GetVector(), SoldierBee.Bounds.GetVector()));

                SoldierBee.BeeKeeperVector = _beeKeeper.Bounds.GetVector() - SoldierBee.Bounds.GetVector();
            }
        }
    }

    /// <summary>
    /// Handle the beekeeper's collision with the vat component.
    /// </summary>
    /// <returns>True if the beekeeper collides with the vat and false otherwise.</returns>
    private bool HandleVatCollision()
    {
        if (_beeKeeper.Bounds.HasCollision(_vat.VatDepositArea))
        {
            if (_jar.HasHoney && !_beeKeeper.IsStung && !_beeKeeper.IsDepositingHoney && _movementVector == Vector2.Zero)
            {
                _beeKeeper.StartTransferHoney(4, EndHoneyDeposit);
            }

            return true;
        }

        _beeKeeper.EndTransferHoney();
        return false;
    }

    /// <summary>
    /// Handler for finalizing the honey deposit to the vat.
    /// </summary>
    /// <param name="result"></param>
    public void EndHoneyDeposit(IAsyncResult result)
    {
        int HoneyAmount = _jar.DecreaseHoneyByPercent(100);
        _vat.IncreaseHoney(HoneyAmount);
        //AudioManager.StopSound("DepositingIntoVat_Loop");
    }

    /// <summary>
    /// Handle the beekeeper's collision with beehive components.
    /// </summary>
    /// <returns>True if the beekeeper collides with a beehive and false otherwise.</returns>
    /// <remarks>This method is also responsible for allowing bees to regenerate when the beekeeper is not
    /// intersecting with a specific hive.</remarks>
    private bool HandleBeeKeeperBeehiveCollision()
    {
        bool isCollidingWithBeehive = false;

        Beehive collidedBeehive = null;

        // Goes over all the beehives
        foreach (Beehive beehive in _beehives)
        {
            // If the beekeeper intersects with the beehive
            if (_beeKeeper.Bounds.HasCollision(beehive.Bounds))
            {
                if (_movementVector == Vector2.Zero)
                {
                    collidedBeehive = beehive;
                    isCollidingWithBeehive = true;
                }
            }
            else
            {
                beehive.AllowBeesToGenerate = true;
            }
        }

        if (collidedBeehive != null)
        {
            // The beehive has honey, the jar can carry more honey, and the beekeeper is not stung
            if (collidedBeehive.HasHoney && _jar.CanCarryMore && !_beeKeeper.IsStung)
            {
                // Take honey from the beehive and put it in the jar
                collidedBeehive.DecreaseHoney(1);
                _jar.IncreaseHoney(1);
                _beeKeeper.IsCollectingHoney = true;
                //AudioManager.PlaySound("FillingHoneyPot_Loop");
            }
            else
            {
                _beeKeeper.IsCollectingHoney = false;
            }

            // Bees are not allowed to regenerate while the beekeeper is colliding with their beehive
            isCollidingWithBeehive = true;
            collidedBeehive.AllowBeesToGenerate = false;
        }
        else
        {
            _beeKeeper.IsCollectingHoney = false;
            //AudioManager.StopSound("FillingHoneyPot_Loop");
        }

        return isCollidingWithBeehive;
    }

    /// <summary>
    /// Handle the smoke puff collision with beehive components.
    /// </summary>
    /// <remarks>Only disables bee regeneration, as it assumes that it will be enabled by 
    /// <see cref="HandleBeeKeeperBeehiveCollision"/></remarks>
    private void HandleSmokeBeehiveCollision()
    {
        foreach (Beehive beehive in _beehives)
        {
            foreach (SmokePuff smokePuff in _beeKeeper.FiredSmokePuffs)
            {
                if (beehive.Bounds.HasCollision(smokePuff.CentralCollisionArea))
                {
                    beehive.AllowBeesToGenerate = false;
                }
            }
        }
    }

    /// <summary>
    /// Sets an internal value which determines whether or not to display an arrow above the vat.
    /// </summary>
    private void HandleVatHoneyArrow()
    {
        if (_jar.HasHoney)
        {
            _drawArrow = true;
        }
        else
        {
            _drawArrow = false;
        }
    }

    /// <summary>
    /// Handle smoke logic.
    /// </summary>
    private void HandleSmoke()
    {
        // If not currently shooting, refill the gun
        if (!_isSmokebuttonClicked)
        {
            //_smokeButtonScorebar.IncreaseCurrentValue(ConfigurationManager.ModesConfiguration[_gameDifficultyLevel].IncreaseAmountSpeed);
            _smokeButtonScorebar.IncreaseCurrentValue(1);

            _beeKeeper.IsShootingSmoke = false;
        }
        else
        {
            // Check that the gun is not empty
            if (_smokeButtonScorebar.CurrentValue <= _smokeButtonScorebar.MinValue)
            {
                _beeKeeper.IsShootingSmoke = false;
            }
            else
            {
                _beeKeeper.IsShootingSmoke = true;

                //_smokeButtonScorebar.DecreaseCurrentValue(ConfigurationManager.ModesConfiguration[_gameDifficultyLevel].DecreaseAmountSpeed);
                _smokeButtonScorebar.DecreaseCurrentValue(2);
            }
        }
    }

    /// <summary>
    /// Checks whether the current game is over, and if so performs the necessary actions.
    /// </summary>
    /// <returns>True if the current game is over and false otherwise.</returns>
    private bool CheckIfCurrentGameFinished()
    {
        _levelEnded = false;
        _isUserWon = _vat.CurrentVatCapacity >= _vat.MaxVatCapacity;

        // If the vat is full, the player wins
        if (_isUserWon || _gameElapsed <= TimeSpan.Zero)
        {
            _levelEnded = true;

            //if (_gameDifficultyLevel == DifficultyMode.Hard)
            //{
            //    FinalScore = ConfigurationManager.ModesConfiguration[gameDifficultyLevel].HighScoreFactor
            //        * (int)_gameElapsed.TotalMilliseconds;
            //}
            //else
            //{
                FinalScore = 0;
            //}
        }

        // if true, game is over
        if (_gameElapsed <= TimeSpan.Zero || _levelEnded)
        {
            _isLevelEnd = true;

            if (_userInputToExit)
            {
                ScreenManager.RemoveScreen(this);

                //if (_isUserWon) // True - the user won
                //{
                //    AudioManager.PlaySound("Victory");
                //}
                //else
                //{
                //    AudioManager.PlaySound("Defeat");
                //}

                MoveToNextScreen(_isUserWon);
            }
        }

        return false;
    }

    /// <summary>
    /// Draws the arrow in intervals of 20 game update loops.        
    /// </summary>
    private void DrawVatHoneyArrow()
    {
        // If the arrow needs to be drawn, and it is not invisible during the current interval
        if (_drawArrow && _drawArrowInInterval)
        {
            ScreenManager.SpriteBatch.Draw(_arrowTexture, _vatArrowPosition, Color.White);

            if (_arrowCounter == 20)
            {
                _drawArrowInInterval = false;
                _arrowCounter = 0;
            }
            _arrowCounter++;
        }
        else
        {
            if (_arrowCounter == 20)
            {
                _drawArrowInInterval = true;
                _arrowCounter = 0;
            }
            _arrowCounter++;
        }
    }

    /// <summary>
    /// Draws the smoke button.
    /// </summary>
    private void DrawSmokeButton()
    {
        ScreenManager.SpriteBatch.Draw(
            _smokeButton,
            new Rectangle(
                (int)_smokeButtonPosition.X,
                (int)_smokeButtonPosition.Y,
                (int)(UIConstants.SmokeButtonSize),
                (int)(UIConstants.SmokeButtonSize)),
            Color.White);
    }

    /// <summary>
    /// Draws the count down string.
    /// </summary>
    private void DrawStartupString()
    {
        // If needed
        if (_isAtStartupCountDown)
        {
            string text = string.Empty;

            // If countdown is done
            if (_startScreenTime.Seconds == 0)
            {
                text = "Go!";
                _isAtStartupCountDown = false;
            }
            else
            {
                text = _startScreenTime.Seconds.ToString();
            }

            Vector2 viewportSize = new Vector2(
                ScreenManager.GraphicsDevice.Viewport.Width,
                ScreenManager.GraphicsDevice.Viewport.Height);
            Vector2 size = _font16px.MeasureString(text);

            Vector2 textPosition = (viewportSize - size) / 2f;

            ScreenManager.SpriteBatch.DrawString(_font36px, text, textPosition, Color.White);
        }
    }
}
