using Gpm.Common.Util;
using System.Collections;
using UnityEngine;

namespace Gpm.Common.Indicator.Internal
{
    public class EditorCoroutineObject : ICoroutineObject
    {
        public void StartCoroutine(IEnumerator routine)
        {
            EditorCoroutine.Start(routine);
        }
    }
}
