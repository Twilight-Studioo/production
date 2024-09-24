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

        public static CameraTargetGroupTag Player() => new(7, 3);

        public static CameraTargetGroupTag Boss() => new(3, 3);

        public static CameraTargetGroupTag Enemy() => new(3, 4);
    }
}