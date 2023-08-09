using System;
using System.Linq;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.Simulation;

namespace UnityEditor.XR.Simulation
{
    class SimulationEditorUtilities
    {
        internal static event Action simulationSubsystemLoaderAddedOrRemoved;

        static bool s_SimulationSubsystemEnabled;

        internal static bool simulationSubsystemEnabled => CheckIsSimulationSubsystemEnabled();

        internal static bool CheckIsSimulationSubsystemEnabled()
        {
            if (Application.isPlaying)
                return XRGeneralSettings.Instance.Manager.activeLoader is SimulationLoader;

            var generalSettings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);

            var simActive = false;
            if (generalSettings != null && generalSettings.AssignedSettings != null)
            {
                var activeLoaders = generalSettings.AssignedSettings.activeLoaders;
                simActive = activeLoaders != null && activeLoaders.Any(loader => loader.GetType() == typeof(SimulationLoader));
            }

            if (s_SimulationSubsystemEnabled != simActive)
            {
                s_SimulationSubsystemEnabled = simActive;
                simulationSubsystemLoaderAddedOrRemoved?.Invoke();
            }

            return s_SimulationSubsystemEnabled;
        }
    }
}
