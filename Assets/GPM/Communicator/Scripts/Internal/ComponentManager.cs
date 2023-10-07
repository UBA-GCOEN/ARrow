namespace Gpm.Communicator.Internal
{
    using UnityEngine;

    public class ComponentManager
    {
        public static T AddComponent<T>(GameObjectManager.GameObjectType gameObjectType) where T : Component
        {
            var gameObject = GameObjectManager.GetGameObject(gameObjectType);

            var component = gameObject.GetComponent<T>();

            if (null != component)
            {
                return component;
            }

            return gameObject.AddComponent<T>();
        }

        public static T GetComponent<T>(GameObjectManager.GameObjectType gameObjectType)
        {
            if (false == GameObjectManager.ContainsGameObject(gameObjectType))
            {
                return default(T);
            }

            var gameObject = GameObjectManager.GetGameObject(gameObjectType);

            return gameObject.GetComponent<T>();
        }
    }
}
