using System;

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
        private partial Destination<T> FindScreen(T route)
        {
            foreach (var screen in screens)
            {
                if (screen.Route.Equals(route))
                {
                    return screen;
                }
            }

            throw new Exception($"Screen not found for route {route}");
        }
    }
}