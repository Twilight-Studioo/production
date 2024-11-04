using System;
using System.Collections.Generic;

namespace Core.Navigation
{
    public partial class ScreenController<T> where T : Enum
    {
        public delegate AScreen CreateScreen(Destination<T> destination);

        private readonly Queue<DestinationStack<T>> backStack = new();

        private DestinationStack<T> currentDestination;

        public Destination<T> CurrentDestination => currentDestination;

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
    }
}