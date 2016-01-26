//  ComponentExtends.cs
//
//
using UnityEngine;
using System;

namespace U3D.Utils.Extends
{
    public static class ComponentExtends
    {
        public static void SetComponent<T>(this Component obj, Action<T> setter) where T : Component
        {
            setter(obj.GetComponent<T>());
        }
    }
}

