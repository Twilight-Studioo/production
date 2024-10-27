namespace Core.Utilities
{
    public static class Easing
    {
        public static float Linear(float t) => t;

        public static float EaseIn(float t) => t * t;

        public static float EaseOut(float t) => t * (2 - t);

        public static float EaseInOut(float t) => t < 0.5f ? 2 * t * t : -1 + (4 - 2 * t) * t;
    }
}