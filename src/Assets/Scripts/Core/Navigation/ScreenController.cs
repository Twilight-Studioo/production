using System;
using System.Collections.Generic;
using System.Linq;
using Object = UnityEngine.Object;

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
        public delegate AScreen CreateScreen(Destination<T> destination);
        private Queue<DestinationStack<T>> backStack = new();
        
        private DestinationStack<T> currentDestination;
        
        private Destination<T>[] screens;
        
        private CreateScreen createScreen;
        private partial void PopBackstack_Internal();

        private partial void Navigate_Internal(T route);

        private partial void Replace_Internal(T route);

        private partial void Hide_Internal();
        
        private partial void Show_Internal();

        private partial Destination<T> FindScreen(T route);

        private ScreenController()
        {
            
        }
        
        public static ScreenController<T> Create(CreateScreen createScreen, params Destination<T>[] args)
        {
            var controller = new ScreenController<T>
            {
                screens = args,
                createScreen = createScreen,
            };
            return controller;
        }

        public void PopBackstack()
        {
            PopBackstack_Internal();
        }

        public void Navigate(T route)
        {
            Navigate_Internal(route);
        }
        
        public void Replace(T route)
        {
            Replace_Internal(route);
        }
        
        public void Reset()
        {
            Dismiss();
            while (backStack.Count > 0)
            {
                Dismiss();
                currentDestination = backStack.Dequeue();
            }
        }
        
        

        public void Hide()
        {
            Hide_Internal();
        }
        
        public void Show()
        {
            Show_Internal();
        }
        
        public bool IsShowing => currentDestination is { IsHidden: false };
        
        public T CurrentRoute => currentDestination.Route;
    }

    public partial class ScreenController<T> where T : Enum
    {
        private void NavigateNew(T route)
        {
            var destination = FindScreen(route);
            var instance = createScreen(destination);
            if (instance == null)
            {
                throw new ("Screen must be derived from AScreen");
            }
            currentDestination = new(destination.Route, destination.Content, instance);
            currentDestination.Instance.OnCreate();
            currentDestination.Show();
        }

        private void NavigateBackstack(T route)
        {
            var destination = backStack.FirstOrDefault(x => x.Route.Equals(route));
            currentDestination = destination ?? throw new ($"Screen not found for route {route}");
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
            Object.Destroy(currentDestination.Instance.gameObject);
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
            else
            {
                currentDestination = backStack.Dequeue();
                currentDestination.Show();
            }
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
                backStack.Enqueue(currentDestination);
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