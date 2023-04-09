using Microsoft.Xna.Framework;

namespace HoneycombRush;

public static class ExtensionMethods
{
    /// <summary>
    /// Returns a vector pointing to the rectangle's top left corner.
    /// </summary>
    /// <param name="rect">The rectangle for which to produce the vector.</param>
    /// <returns>A vector pointing to the rectangle's top left corner.</returns>
    public static Vector2 GetVector(this Rectangle rect)
    {
        return new Vector2(rect.X, rect.Y);
    }
}
