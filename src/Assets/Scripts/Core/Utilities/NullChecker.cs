namespace Core.Utilities
{
    public static class NullChecker
    {
        public static T CheckNull<T>(this T obj)
        {
            if (obj == null)
            {
                throw new System.ArgumentNullException(nameof(obj));
            }
            return obj;
        }
    }
}