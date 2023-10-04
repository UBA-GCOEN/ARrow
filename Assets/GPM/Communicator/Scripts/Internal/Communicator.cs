namespace Gpm.Communicator.Internal
{
    using Gpm.Communicator.Internal.Log;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class Communicator : MonoBehaviour
    {
        private INativeMessageSender messageSender = null;        
        private string methodName = "OnAsyncEvent";
        private const string DELIMITER = "${gpm_communicator}";

        private static Dictionary<string, GpmCommunicatorCallback.CommunicatorCallback> receiverDictionary = new Dictionary<string, GpmCommunicatorCallback.CommunicatorCallback>();

        private Communicator()
        {
#if UNITY_ANDROID
            messageSender = Android.AndroidMessageSender.Instance;
#elif UNITY_IOS
            messageSender = Ios.IosMessageSender.Instance;
#endif
        }

        public void Initialize()
        {
            if (messageSender == null)
            {
                CommunicatorLogger.Error("MessageSender is null", "GpmCommunicator", GetType(), "Initialize");
                return;
            }

            messageSender.Initialize(GameObjectManager.GameObjectType.CORE_TYPE.ToString(), methodName);
        }

        public void InitializeClass(GpmCommunicatorVO.Configuration configuration)
        {
            if (messageSender == null)
            {
                CommunicatorLogger.Error("MessageSender is null", "GpmCommunicator", GetType(), "InitializeClass");
                return;
            }

            messageSender.InitializeClass(configuration.className);
        }

        public void AddReceiver(string domain, GpmCommunicatorCallback.CommunicatorCallback callback)
        {
            if(receiverDictionary.ContainsKey(domain) == true)
            {
                CommunicatorLogger.Error(
                    string.Format(
                        "The receiver is already registered : {0}",
                        domain),
                    "GpmCommunicator",
                    GetType(),
                    "AddReceiver");
                return;
            }

            receiverDictionary.Add(domain, callback);
        }

        public GpmCommunicatorVO.Message CallSync(GpmCommunicatorVO.Message message)
        {
            if (messageSender == null)
            {
                CommunicatorLogger.Error("MessageSender is null", "GpmCommunicator", GetType(), "CallSync");
                return null;
            }

            string responseString = messageSender.CallSync(message.domain, message.data, message.extra);

            GpmCommunicatorVO.Message responseMessage = null;
            
            if(string.IsNullOrEmpty(responseString) == true)
            {
                return responseMessage;
            }

            string[] messageData = responseString.Split(new string[] { DELIMITER }, StringSplitOptions.None);

            if (messageData == null || messageData.Length == 0)
            {
                return responseMessage;
            }

            responseMessage = new GpmCommunicatorVO.Message();
            responseMessage.domain = messageData[0];
            responseMessage.data = messageData[1];
            responseMessage.extra = messageData[2];

            return responseMessage;
        }

        public void CallAsync(GpmCommunicatorVO.Message message)
        {
            if (messageSender == null)
            {
                CommunicatorLogger.Error("MessageSender is null", "GpmCommunicator", GetType(), "CallAsync");
                return;
            }

            messageSender.CallAsync(message.domain, message.data, message.extra);
        }

        public void OnAsyncEvent(string message)
        {
            string[] messageData = message.Split(new string[] { DELIMITER }, StringSplitOptions.None);

            if(messageData == null || messageData.Length == 0)
            {
                return;
            }
            
            string domain = messageData[0];
            string data = string.Empty;
            string extra = string.Empty;

            if (receiverDictionary.ContainsKey(domain) == false)
            {
                CommunicatorLogger.Warn(
                    string.Format(
                        "There is no registered receiver : {0}",
                        domain), 
                    "GpmCommunicator", 
                    GetType(), 
                    "OnAsyncEvent");
                return;
            }

            GpmCommunicatorCallback.CommunicatorCallback callback = receiverDictionary[domain];

            if(messageData.Length > 1)
            {
                data = messageData[1];
            }

            if(messageData.Length > 2)
            {
                extra = messageData[2];
            }

            callback(new GpmCommunicatorVO.Message()
            {
                domain = domain,
                data = data,
                extra = extra
            });
        }        
    }
}
