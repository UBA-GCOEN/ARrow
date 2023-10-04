using System.Collections.Generic;
using UnityEngine;

namespace Gpm.Common.Util
{
    public static class GameObjectContainer
    {
        private static Dictionary<string, GameObject> gameObjectDictionary = new Dictionary<string, GameObject>();

        public static GameObject GetGameObject(string gameObjectName)
        {
            GameObject gameObject;

            if (gameObjectDictionary.TryGetValue(gameObjectName, out gameObject) == true)
            {
                return gameObject;
            }

            return CreateGameObject(gameObjectName);
        }

        public static List<string> GetGeneratedObjectName()
        {
            return new List<string>(gameObjectDictionary.Keys);
        }

        private static GameObject CreateGameObject(string type)
        {
            GameObject gameObject = new GameObject();
            gameObject.name = type;
            gameObject.AddComponent<MonoObject>();
            Object.DontDestroyOnLoad(gameObject);
            gameObjectDictionary.Add(type, gameObject);

            return gameObject;
        }
    }

    public class MonoObject:MonoBehaviour
    {
    }
}