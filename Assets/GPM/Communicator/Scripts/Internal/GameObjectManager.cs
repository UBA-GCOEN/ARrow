namespace Gpm.Communicator.Internal
{
    using System.Collections.Generic;
    using UnityEngine;

    public class GameObjectManager
    {

        public enum GameObjectType
        {
            CORE_TYPE,
        }

        static private Dictionary<GameObjectType, GameObject> gameObjectDictionary = new Dictionary<GameObjectType, GameObject>();


        public static bool ContainsGameObject(GameObjectType gameObjectType)
        {
            return gameObjectDictionary.ContainsKey(gameObjectType);
        }

        public static GameObject GetGameObject(GameObjectType gameObjectType)
        {
            if (false == ContainsGameObject(gameObjectType))
            {
                return CreateGameObject(gameObjectType);
            }

            return gameObjectDictionary[gameObjectType].gameObject;
        }

        private static GameObject CreateGameObject(GameObjectType gameObjectType)
        {
            if (true == gameObjectDictionary.ContainsKey(gameObjectType))
            {
                return gameObjectDictionary[gameObjectType];
            }

            GameObject gameObject = new GameObject();
            gameObject.name = gameObjectType.ToString();
            GameObject.DontDestroyOnLoad(gameObject);
            gameObjectDictionary.Add(gameObjectType, gameObject);

            return gameObject;
        }
    }
}
