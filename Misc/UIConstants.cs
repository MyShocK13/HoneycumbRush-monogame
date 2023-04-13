using Microsoft.Xna.Framework;

namespace HoneycombRush;

/// <summary>
/// Defines constants for positioning UI elements.
/// </summary>
static class UIConstants
{
    /// <summary>
    /// Updates all constants returned by the class according to a specified scale factor.
    /// Always call this method at least once before retrieving constants from the class.
    /// </summary>
    /// <param name="scaleVector">Vector depicting the scale used.</param>
    public static void SetScale(Vector2 scaleVector)
    {
        BeehiveLeftMargin = 130 * scaleVector.X;
        BeehiveRightMargin = 130 * scaleVector.X;
        BeehiveTopMargin = 30 * scaleVector.Y;
        BeehiveMiddleOffset = 70 * scaleVector.Y;

        HoneyJarTopMargin = 12 * scaleVector.Y;
        HoneyJarLeftMargin = 30 * scaleVector.Y;

        VatArrowOffset = -35 * scaleVector.Y;
        VatBottomMargin = 50 * scaleVector.Y;
        VatScorebarHeight = (int)(20 * scaleVector.Y);
        VatScorebarWidth = (int)(210 * scaleVector.X);

        SmokeButtonSize = 137;
        SmokeButtonRightAbsoluteMargin = 170 * scaleVector.X;
        SmokeButtonBottomAbsoluteMargin = 200 * scaleVector.Y;

        SprayUpOffset = 24 * scaleVector.Y;
        SprayMiddleOffset = 65 * scaleVector.Y;
        SprayDownOffset = 110 * scaleVector.Y;
        SprayRightOffset = 230 * scaleVector.X;
        SprayLeftOffset = 130 * scaleVector.X;

        //            HighScorePlaceLeftMargin = 50 * scaleVector.X;
        //            HighScoreNameLeftMargin = 300 * scaleVector.X;
        //            HighScoreScoreLeftMargin = 960 * scaleVector.X;
        //            HighScoreTopMargin = 147 * scaleVector.Y;
        //            HighScoreOddVerticalJump = 74 * scaleVector.Y;
        //            HighScoreEvenVerticalJump = 69 * scaleVector.Y;
    }

    public static float BeehiveLeftMargin { get; private set; }
    public static float BeehiveRightMargin { get; private set; }
    public static float BeehiveTopMargin { get; private set; }
    /// <summary>
    /// An additional offset used for positioning the bottom two beehives.
    /// </summary>
    public static float BeehiveMiddleOffset { get; private set; }

    public static float HoneyJarTopMargin { get; private set; }
    public static float HoneyJarLeftMargin { get; private set; }

    public static float VatArrowOffset { get; private set; }
    public static float VatBottomMargin { get; private set; }
    public static int VatScorebarHeight { get; private set; }
    public static int VatScorebarWidth { get; private set; }

    /// <summary>
    /// Sets the size of the smoke button. This does not take the scale factor into account.
    /// </summary>
    public static float SmokeButtonSize { get; private set; }

    /// <summary>
    /// This margin takes the button's own size in consideration.
    /// </summary>
    public static float SmokeButtonRightAbsoluteMargin { get; private set; }
    /// <summary>
    /// This margin takes the button's own size in consideration.
    /// </summary>
    public static float SmokeButtonBottomAbsoluteMargin { get; private set; }

    public static float SprayUpOffset { get; private set; }
    public static float SprayMiddleOffset { get; private set; }
    public static float SprayDownOffset { get; private set; }
    public static float SprayRightOffset { get; private set; }
    public static float SprayLeftOffset { get; private set; }

    //        public static float HighScorePlaceLeftMargin { get; private set; }
    //        public static float HighScoreNameLeftMargin { get; private set; }
    //        public static float HighScoreScoreLeftMargin { get; private set; }
    //        public static float HighScoreTopMargin { get; private set; }
    //        public static float HighScoreOddVerticalJump { get; private set; }
    //        public static float HighScoreEvenVerticalJump { get; private set; }
}
