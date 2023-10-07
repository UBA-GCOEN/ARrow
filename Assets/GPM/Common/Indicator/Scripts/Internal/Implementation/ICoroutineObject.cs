using System.Collections;

namespace Gpm.Common.Indicator.Internal
{
    public interface ICoroutineObject
    {
        void StartCoroutine(IEnumerator routine);
    }
}
