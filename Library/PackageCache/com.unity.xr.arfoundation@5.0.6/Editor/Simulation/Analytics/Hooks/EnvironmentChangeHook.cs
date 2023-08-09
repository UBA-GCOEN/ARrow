namespace UnityEditor.XR.Simulation
{
    static class EnvironmentChangeHook
    {
        [InitializeOnLoadMethod]
        static void RegisterForEnvironmentChange()
        {
            var environmentAssetsManager = SimulationEnvironmentAssetsManager.Instance;
            environmentAssetsManager.activeEnvironmentChanged += OnEnvironmentChange;
        }

        static void OnEnvironmentChange()
        {
            AREditorAnalytics.simulationUIAnalyticsEvent.Send(
                new SimulationUIAnalyticsArgs(
                    eventName: SimulationUIAnalyticsArgs.EventName.EnvironmentCycle,
                    environmentGuid: SimulationEnvironmentAssetsManager.GetActiveEnvironmentAssetGuid()));
        }
    }
}
