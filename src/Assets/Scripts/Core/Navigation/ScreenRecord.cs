using System;
using UnityEngine;

namespace Core.Navigation
{
    public record Destination<T>(T Route, GameObject Content) where T : Enum
    {
        public T Route { get; } = Route;

        public GameObject Content { get; } = Content;
    }
    
    internal record DestinationStack<T>(T Route, GameObject Content, AScreen Instance): Destination<T>(Route, Content) where T : Enum {
        public AScreen Instance { get; } = Instance;
        
        internal bool IsHidden { get; private set; } = true;
        
        public void Show()
        {
            if (!IsHidden)
            {
                return;
            }
            Instance.gameObject.SetActive(true);
            Instance.OnShow();
            IsHidden = false;
        }
        
        public void Hide()
        {
            if (IsHidden)
            {
                return;
            }
            Instance.gameObject.SetActive(false);
            Instance.OnHide();
            IsHidden = true;
        }
    }

}