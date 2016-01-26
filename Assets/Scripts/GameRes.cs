//  GameRes.cs
//
//
using UnityEngine;
namespace U3D.Utils
{
    public static class GameRes
    {
        public static GameObject Instance(string path)
        {
            GameObject res = Resources.Load<GameObject>(path);
            if (res)return (GameObject)UnityEngine.Object.Instantiate(res);
            return null;
        }
        public static GameObject Instance(string path, Vector3 vec3, Quaternion qua)
        {
            GameObject res = Resources.Load<GameObject>(path);
            if (res)return (GameObject)UnityEngine.Object.Instantiate(res, vec3, qua);
            return null;
        }
    }
}

