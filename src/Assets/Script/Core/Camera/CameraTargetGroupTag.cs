namespace Core.Camera
{
    public class CameraTargetGroupTag
    {
        public int Weight { get; }
        public int Radius { get; }
        private CameraTargetGroupTag(int weight, int radius)
        {
            Weight = weight;
            Radius = radius;
        }
        
        public static CameraTargetGroupTag Player() =>
            new CameraTargetGroupTag(5, 3);
        
        public static CameraTargetGroupTag Boss() =>
            new CameraTargetGroupTag(3, 3);
        
        public static CameraTargetGroupTag Enemy() =>
            new CameraTargetGroupTag(1, 1);
    }
}