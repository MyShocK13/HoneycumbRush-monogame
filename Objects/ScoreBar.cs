using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace HoneycombRush
{
    /// <summary>
    /// Used by other components to display their status.
    /// </summary>
    public class ScoreBar : DrawableGameComponent
    {
        /// <summary>
        /// Used to determine the component's orientation.
        /// </summary>
        public enum ScoreBarOrientation
        {
            Vertical,
            Horizontal
        }

        private SpriteBatch _spriteBatch;
        private Texture2D _backgroundTexture;
        private Texture2D _redTexture;
        private Texture2D _greenTexture;
        private Texture2D _yellowTexture;

        private GameplayScreen _gameplayScreen;
        private ScoreBarOrientation _scoreBarOrientation;

        private bool _isAppearAtCountDown;
        private int _currentValue;
        private int _height;
        private int _width;

        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public Vector2 Position { get; set; }
        public Color ScoreBarColor { get; set; }
        public int CurrentValue
        {
            get
            {
                return _currentValue;
            }
        }

        /// <summary>
        /// Creates a new score bar instance.
        /// </summary>
        /// <param name="game">The associated game object.</param>
        /// <param name="minValue">The score bar's minimal value.</param>
        /// <param name="maxValue">The score bar's maximal value.</param>
        /// <param name="position">The score bar's position.</param>
        /// <param name="height">The score bar's height.</param>
        /// <param name="width">The score bar's width.</param>
        /// <param name="scoreBarColor">Color to tint the scorebar's background with.</param>
        /// <param name="scoreBarOrientation">The score bar's orientation.</param>
        /// <param name="initialValue">The score bar's initial value.</param>
        /// <param name="screen">Gameplay screen where the score bar will appear.</param>
        /// <param name="isAppearAtCountDown">Whether or not the score bar will appear during the game's initial
        /// countdown phase.</param>
        public ScoreBar(Game game,
                        int minValue,
                        int maxValue,
                        Vector2 position,
                        int height,
                        int width,
                        Color scoreBarColor,
                        ScoreBarOrientation scoreBarOrientation,
                        int initialValue,
                        GameplayScreen screen,
                        bool isAppearAtCountDown)
            : base(game)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Position = position;
            ScoreBarColor = scoreBarColor;
            _scoreBarOrientation = scoreBarOrientation;
            _currentValue = initialValue;
            _width = width;
            _height = height;
            _gameplayScreen = screen;
            _isAppearAtCountDown = isAppearAtCountDown;

            _spriteBatch = (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            GetSpaceFromBorder();
        }

        protected override void LoadContent()
        {
            _backgroundTexture = Game.Content.Load<Texture2D>("Textures/barBlackBorder");
            _greenTexture = Game.Content.Load<Texture2D>("Textures/barGreen");
            _yellowTexture = Game.Content.Load<Texture2D>("Textures/barYellow");
            _redTexture = Game.Content.Load<Texture2D>("Textures/barRed");

            base.LoadContent();
        }

        public override void Draw(GameTime gameTime)
        {
            if (!_gameplayScreen.IsActive)
            {
                base.Draw(gameTime);
                return;
            }
            if (!_isAppearAtCountDown && !_gameplayScreen.IsStarted)
            {
                base.Draw(gameTime);
                return;
            }

            float rotation;
            // Determine the orientation of the component
            if (_scoreBarOrientation == ScoreBar.ScoreBarOrientation.Horizontal)
            {
                rotation = 0f;
            }
            else
            {
                rotation = 1.57f;
            }

            _spriteBatch.Begin();

            // Draws the background of the score bar
            _spriteBatch.Draw(_backgroundTexture, new Rectangle((int)Position.X, (int)Position.Y, _width, _height),
                null, ScoreBarColor, rotation, new Vector2(0, 0), SpriteEffects.None, 0);

            // Gets the margin from the border
            decimal spaceFromBorder = GetSpaceFromBorder();

            spaceFromBorder += 4;
            Texture2D coloredTexture = GetTextureByCurrentValue(_currentValue);

            if (_scoreBarOrientation == ScoreBarOrientation.Horizontal)
            {

                _spriteBatch.Draw(coloredTexture, new Rectangle((int)Position.X + 2, (int)Position.Y + 2,
                    _width - (int)spaceFromBorder, _height - 4), null, Color.White, rotation, new Vector2(0, 0),
                    SpriteEffects.None, 0);
            }
            else
            {
                _spriteBatch.Draw(coloredTexture, new Rectangle((int)Position.X + 2 - _height,
                    (int)Position.Y + _width + -2, _width - (int)spaceFromBorder, _height - 4), null, ScoreBarColor,
                    -rotation, new Vector2(0, 0), SpriteEffects.None, 0);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        /// <summary>
        /// Increases the current value of the score bar.
        /// </summary>
        /// <param name="valueToIncrease">Number to increase the value by</param>
        /// <remarks>Negative numbers will have no effect.</remarks>
        public void IncreaseCurrentValue(int valueToIncrease)
        {
            // Make sure that the target value does not exceed the max value
            if (valueToIncrease >= 0 && _currentValue < MaxValue && _currentValue + valueToIncrease <= MaxValue)
            {
                _currentValue += valueToIncrease;
            }
            // If the target value exceeds the max value, clamp the value to the maximum
            else if (_currentValue + valueToIncrease > MaxValue)
            {
                _currentValue = MaxValue;
            }
        }

        /// <summary>
        /// Decreases the current value of the score bar.
        /// </summary>
        /// <param name="valueToDecrease">Number to decrease the value by.</param>
        public void DecreaseCurrentValue(int valueToDecrease)
        {
            DecreaseCurrentValue(valueToDecrease, false);
        }

        /// <summary>
        /// Decreases the current value of the score bar.
        /// </summary>
        /// <param name="valueToDecrease">Number to decrease the value by.</param>
        /// <param name="clampToMinimum">If true, then decreasing by an amount which will cause the bar's value
        /// to go under its minimum will set the bar to its minimal value. If false, performing such an operation
        /// as just described will leave the bar's value unchanged.</param>
        /// <returns>The actual amount which was subtracted from the bar's value.</returns>
        public int DecreaseCurrentValue(int valueToDecrease, bool clampToMinimum)
        {
            // Make sure that the target value does not exceed the min value
            int valueThatWasDecreased = 0;
            if (valueToDecrease >= 0 && _currentValue > MinValue && _currentValue - valueToDecrease >= MinValue)
            {
                _currentValue -= valueToDecrease;
                valueThatWasDecreased = valueToDecrease;
            }
            // If the target value exceeds the min value, clamp the value to the minimum (or do nothing)
            else if (_currentValue - valueToDecrease < MinValue && clampToMinimum)
            {
                valueThatWasDecreased = _currentValue - MinValue;
                _currentValue = MinValue;
            }

            return valueThatWasDecreased;
        }

        /// <summary>
        /// Calculate the empty portion of the score bar according to its current value.
        /// </summary>
        /// <returns>Width of the bar portion which should be empty to represent the bar's current score.</returns>
        private decimal GetSpaceFromBorder()
        {
            int textureSize;
            textureSize = _width;

            decimal valuePercent = Decimal.Divide(_currentValue, MaxValue) * 100;
            return textureSize - ((decimal)textureSize * valuePercent / (decimal)100);
        }

        /// <summary>
        /// Returns a texture for the score bar's "fill" according to its value.
        /// </summary>
        /// <param name="value">Current value of the score bar.</param>
        /// <returns>A texture the color of which indicates how close to the bar's maximum the value is.</returns>
        private Texture2D GetTextureByCurrentValue(int value)
        {
            Texture2D selectedTexture;

            decimal valuePercent = Decimal.Divide(_currentValue, MaxValue) * 100;
            if (valuePercent > 50)
            {
                selectedTexture = _greenTexture;
            }
            else if (valuePercent > 25)
            {
                selectedTexture = _yellowTexture;
            }
            else
            {
                selectedTexture = _redTexture;
            }
            return selectedTexture;
        }
    }
}