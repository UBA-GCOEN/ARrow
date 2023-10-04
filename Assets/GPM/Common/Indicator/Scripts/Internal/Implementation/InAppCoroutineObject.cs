using Gpm.Common.Util;
using System.Collections;
using UnityEngine;

namespace Gpm.Common.Indicator.Internal
{
    public class InAppCoroutineObject : ICoroutineObject
    {
        private MonoBehaviour monoObject;

        public InAppCoroutineObject(string name)
        {
            monoObject = GameObjectContainer.GetGameObject(name).GetComponent<MonoBehaviour>();
        }

        public void StartCoroutine(IEnumerator routine)
        {
            monoObject.StartCoroutine(routine);
        }
    }
}
