using Gpm.Common.Util;
using System;

namespace Gpm.Common.Indicator.Internal
{
    public class EditorIndicator : BaseIndicator
    {
        public EditorIndicator()
        {
            coroutineObject = new EditorCoroutineObject();
            isWaitQueue = false;

            Initialize();
        }
    }
}