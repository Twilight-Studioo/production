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
        internal static CameraTargetGroupTag Player()
        {
            return new CameraTargetGroupTag(4, 3);
        }

        public static CameraTargetGroupTag Boss()
        {
            return new CameraTargetGroupTag(3, 3);
        }

        public static CameraTargetGroupTag Enemy()
        {
            return new CameraTargetGroupTag(4, 4);
        }

        public static CameraTargetGroupTag SwapItem()
        {
            return new CameraTargetGroupTag(1, 1);
        }
    }
}