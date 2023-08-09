using System;
using System.Collections.Generic;
using MagicLeapXRStats = UnityEngine.XR.Provider.XRStats;

namespace UnityEngine.XR.MagicLeap
{
    public static class MagicLeapPerformance
    {
        public static float GetGPULastFrameTime()
        {
            return TryGetFloatStat(kGPUAppLastFrameTime);
        }

        public static float GetGPUCompositorLastFrameTime()
        {
            return TryGetFloatStat(kGPUCompositorLastFrameTime);
        }

        public static float GetDisplayRefreshRate()
        {
            return TryGetFloatStat(kdisplayRefreshRate);
        }

        public static float GetDroppedFrameCount()
        {
            return TryGetFloatStat(kdroppedFrameCount);
        }

        public static float GetFrameMotionToPhoton()
        {
            return TryGetFloatStat(kmotionToPhoton);
        }

        public static float GetFramePresentCount()
        {
            return TryGetFloatStat(kframePresentCount);
        }

        public static float GetFrameStartCPUCompAcquire()
        {
            return TryGetFloatStat(kFrameStartCPUCompAcquireCPU);
        }

        public static float GetFrameStartCPUFrameEndGPU()
        {
            return TryGetFloatStat(kFrameStartCPUFrameEndGPU);
        }

        public static float GetFrameStartCPUFrameStartCPU()
        {
            return TryGetFloatStat(kFrameStartCPUFrameStartCPU);
        }

        public static float GetFrameDurationCPU()
        {
            return TryGetFloatStat(kFrameDurationCPU);
        }

        public static float GetFrameDurationGPU()
        {
            return TryGetFloatStat(kFrameDurationGPU);
        }

        public static float GetFrameInternalDurationCPU()
        {
            return TryGetFloatStat(kFrameInternalDurationCPU);
        }

        public static float GetFrameInternalDurationGPU()
        {
            return TryGetFloatStat(kFrameInternalDurationGPU);
        }

        static float TryGetFloatStat(string tag)
        {
            if (!MagicLeapXRStats.TryGetStat(GetMagicLeapDisplaySubsystem(), tag, out var value))
                Debug.LogError($"[XR::ML] Failed to get the {tag}");
            return value;
        }

        static IntegratedSubsystem GetMagicLeapDisplaySubsystem()
        {
            if (m_Display != null)
            {
                return m_Display;
            }

            var displays = new List<XRDisplaySubsystem>();
            SubsystemManager.GetInstances(displays);

            foreach (var xrDisplaySubsystem in displays)
            {
                if (xrDisplaySubsystem.SubsystemDescriptor.id != MagicLeapConstants.kDisplaySubsytemId ||
                    !xrDisplaySubsystem.running) continue;

                m_Display = xrDisplaySubsystem;
                return m_Display;
            }

            return m_Display;
        }

        static IntegratedSubsystem m_Display;

        const string kGPUAppLastFrameTime = "GPUAppLastFrameTime";
        const string kGPUCompositorLastFrameTime = "GPUCompositorLastFrameTime";
        const string kdisplayRefreshRate = "displayRefreshRate";
        const string kmotionToPhoton = "motionToPhoton";
        const string kdroppedFrameCount = "droppedFrameCount";
        const string kframePresentCount = "framePresentCount";

        const string kFrameStartCPUCompAcquireCPU = "frameStartCPUCompAcquireCPU";
        const string kFrameStartCPUFrameEndGPU = "frameStartCPUFrameEndGPU";
        const string kFrameStartCPUFrameStartCPU = "frameStartCPUFrameStartCPU";
        const string kFrameDurationCPU = "frameDurationCPU";
        const string kFrameDurationGPU = "frameDurationGPU";
        const string kFrameInternalDurationCPU = "frameInternalDurationCPU";
        const string kFrameInternalDurationGPU = "frameInternalDurationGPU";
    }
}
