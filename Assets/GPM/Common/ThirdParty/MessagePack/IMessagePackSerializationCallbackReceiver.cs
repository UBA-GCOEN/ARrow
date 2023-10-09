using System;
using System.Collections.Generic;
using System.Text;

namespace Gpm.Common.ThirdParty.MessagePack
{
    public interface IMessagePackSerializationCallbackReceiver
    {
        void OnBeforeSerialize();
        void OnAfterDeserialize();
    }
}
