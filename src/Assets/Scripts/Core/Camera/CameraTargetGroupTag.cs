namespace Core.Camera
{
    public class CameraTargetGroupTag
    {
        private CameraTargetGroupTag(int weight, int radius)
        {
            Weight = weight;
            Radius = radius;
        }

        public int Weight { get; }
        public int Radius { get; }


        /// <summary>
        ///     [SetPlayer] でしか使用しないのでinternal
        /// </summary>
        /// <returns>
        ///     A new instance of CameraTargetGroupTag with default weight and radius set for a player.
        /// </returns>
        internal static CameraTargetGroupTag Player() => new(11, 8);

        public static CameraTargetGroupTag Boss() => new(8, 5);

        public static CameraTargetGroupTag Enemy() => new(3, 4);

        public static CameraTargetGroupTag SwapItem() => new(4, 3);
    }
}