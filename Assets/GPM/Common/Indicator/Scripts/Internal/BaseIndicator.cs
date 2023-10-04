using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Networking;

using Gpm.Common.Util;

namespace Gpm.Common.Indicator.Internal
{
    public class BaseIndicator
    {
        protected ICoroutineObject coroutineObject;

        protected virtual void GetLaunchingInfo(Action<LaunchingInfo> callback)
        {
            var launchingRequest = new LaunchingRequest();

            UnityWebRequest request = launchingRequest.Request();
            var helper = new UnityWebRequestHelper(request);

            coroutineObject.StartCoroutine(helper.SendWebRequestAndDispose(result =>
            {
                if (CheckInvalidResult(result) == true)
                {
                    callback(null);
                }
                else
                {
                    var launchingInfo = GpmJsonMapper.ToObject<LaunchingInfo>(result.downloadHandler.text);
                    callback(launchingInfo);
                }
            }));
        }

        protected virtual void ExecuteQueueDelegate()
        {
            coroutineObject.StartCoroutine(ExecuteQueue());
        }

        protected LaunchingInfo.Launching.Indicator.Zone indicatorInfo;
        protected Queue<QueueItem> queue;
        protected QueueItem queueItem;

        protected bool isWaitQueue = true;

        public BaseIndicator()
        {
            queue = new Queue<QueueItem>();
        }

        public void Send(string serviceName, string serviceVersion, string body, Dictionary<string, string> customData, bool ignoreActivation = false)
        {
            queue.Enqueue(new QueueItem(serviceName, serviceVersion, body, customData, ignoreActivation));
        }

        protected void Initialize()
        {
            GetLaunchingInfo((launchingInfo) =>
            {
                if (launchingInfo == null)
                {
                    return;
                }

                if (launchingInfo.header.isSuccessful == true)
                {
                    indicatorInfo = launchingInfo.launching.indicator.real;
                    SetDevelopmentZone(launchingInfo);

                    ExecuteQueueDelegate();
                }
            });
        }

        [Conditional("GPM_INDICATOR_DEVELOPMENT")]
        private void SetDevelopmentZone(LaunchingInfo launchingInfo)
        {
            indicatorInfo = launchingInfo.launching.indicator.alpha;
        }

        protected IEnumerator ExecuteQueue()
        {
            while (true)
            {
                if (IsWaiting() == true)
                {
                    yield return null;
                }
                else
                {
                    queueItem = queue.Dequeue();
                    SetQueueItemStatus();

                    if (CanExecutable(queueItem.ignoreActivation) == true)
                    {
                        LogNCrashRequest Indicator = new LogNCrashRequest(indicatorInfo);
                        var request = Indicator.Request(queueItem);

                        var helper = new UnityWebRequestHelper(request);

                        SendWebRequest(helper);
                    }
                }
            }
        }

        protected virtual void SendWebRequest(UnityWebRequestHelper helper)
        {
            coroutineObject.StartCoroutine(helper.SendWebRequestAndDispose((result) =>
            {
                queueItem.isRunning = false;
            }));
        }

        protected virtual void SetQueueItemStatus()
        {
            queueItem.isRunning = true;
        }

        protected virtual bool IsWaitingInChild()
        {
            if(isWaitQueue ==  true)
            {
                if (queueItem != null && queueItem.isRunning == true)
                {
                    return true;
                }
            }

            return false;
        }

        protected bool CheckInvalidResult(UnityWebRequest result)
        {
            return ((result == null) || (string.IsNullOrEmpty(result.downloadHandler.text) == true));
        }

        private bool IsWaiting()
        {
            if (indicatorInfo == null)
            {
                return true;
            }

            if (queue.Count == 0)
            {
                return true;
            }

            return IsWaitingInChild();
        }

        protected bool CanExecutable(bool ignoreActivation)
        {
            if (indicatorInfo.activation.Equals("off", StringComparison.Ordinal) == true)
            {
                if (ignoreActivation == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
