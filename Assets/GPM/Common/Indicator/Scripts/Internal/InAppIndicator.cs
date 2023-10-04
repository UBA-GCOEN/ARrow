using Gpm.Common.Util;
using System;
using UnityEngine;

namespace Gpm.Common.Indicator.Internal
{
    public sealed class InAppIndicator : BaseIndicator
    {
        public InAppIndicator()
        {
            coroutineObject = new InAppCoroutineObject(GpmIndicator.SERVICE_NAME);

            Initialize();
        }
    }
}