//  GameObjectExtends.cs
//
//
using UnityEngine;
using System;

namespace U3D.Utils.Extends
{
    public static class GameObjectExtends
    {
        // Set
        public static void SetComponent<T>(this GameObject obj, Action<T> setter) where T : Component
        {
            setter(obj.GetComponent<T>());
        }
        
        // Get
        public static void GetComponent<T>(this GameObject obj, Action<T> setter) where T : Component
        {
            setter(obj.GetComponent<T>());
        }
        
        // Add
        public static void AddComponent<T>(this GameObject obj, Action<T> setter) where T : Component
        {
            setter(obj.AddComponent<T>());
        }
        public static void AddComponentOnce<T>(this GameObject obj, Action<T> setter) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component != null)setter(component);
            else obj.AddComponent<T>(setter);
        }
        public static T AddComponentOnce<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)component = obj.AddComponent<T>();
            return component;
        }
        
        // Del
        public static void DelComponent<T>(this GameObject obj) where T : Component
        {
            UnityEngine.Object.Destroy(obj.GetComponent<T>());
        }
        
        // Map action to Children component
        public static void MapComponent<T>(this GameObject obj, Action<T> action) where T : Component
        {
            T[] components = obj.GetComponentsInChildren<T>();
            foreach(T c in components)action(c);
        }
    }
}

