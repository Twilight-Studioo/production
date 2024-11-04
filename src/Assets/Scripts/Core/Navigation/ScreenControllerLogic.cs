#region

using System;
using System.Linq;

#endregion

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
        private void NavigateNew(T route)
        {
            var destination = FindScreen(route);
            var instance = createScreen(destination);
            if (instance == null)
            {
                throw new("Screen must be derived from AScreen");
            }

            currentDestination = new(destination.Route, destination.Content, instance);
            currentDestination.Instance.OnCreate();
            currentDestination.Show();
        }

        private void NavigateBackstack(T route)
        {
            var destination = backStack.FirstOrDefault(x => x.Route.Equals(route));
            currentDestination = destination ?? throw new($"Screen not found for route {route}");
            currentDestination.Show();
        }

        private void Dismiss()
        {
            if (currentDestination == null)
            {
                return;
            }

            currentDestination.Hide();
            currentDestination.Instance.OnDestroy();
            currentDestination = null;
        }

        private partial void PopBackstack_Internal()
        {
            Dismiss();
            if (backStack.Count == 0)
            {
                currentDestination = null;
                return;
            }

            currentDestination = backStack.Pop();
            currentDestination.Show();
        }

        private partial void Navigate_Internal(T route)
        {
            if (currentDestination?.Route.Equals(route) == true)
            {
                return;
            }

            if (currentDestination != null)
            {
                currentDestination.Hide();
                backStack.Push(currentDestination);
            }

            if (backStack.FirstOrDefault(x => x.Route.Equals(route)) != null)
            {
                NavigateBackstack(route);
                return;
            }

            NavigateNew(route);
        }

        private partial void Replace_Internal(T route)
        {
            Dismiss();
            NavigateNew(route);
        }

        private partial void Hide_Internal()
        {
            currentDestination?.Hide();
        }

        private partial void Show_Internal()
        {
            currentDestination?.Show();
        }
    }
}