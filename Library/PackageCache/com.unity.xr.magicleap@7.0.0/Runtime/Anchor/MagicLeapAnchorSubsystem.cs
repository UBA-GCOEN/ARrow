using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;

namespace UnityEngine.XR.MagicLeap
{
    using MLLog = UnityEngine.XR.MagicLeap.MagicLeapLogger;

    /// <summary>
    /// The Magic Leap implementation of the <c>XRAnchorSubsystem</c>. Do not create this directly.
    /// Use <c>XRAnchorSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed class MagicLeapAnchorSubsystem : XRAnchorSubsystem
    {
        const string kLogTag = "Unity-Anchors";

        static void DebugLog(string msg)
        {
            MLLog.Debug(kLogTag, msg);
        }

        static void LogWarning(string msg)
        {
            MLLog.Warning(kLogTag, msg);
        }

        static void LogError(string msg)
        {
            MLLog.Error(kLogTag, msg);
        }

        [Conditional("DEVELOPMENT_BUILD")]
        static void DebugError(string msg)
        {
            LogError(msg);
        }

        static class Native
        {
            public const ulong InvalidHandle = ulong.MaxValue;

#if UNITY_ANDROID
            const string Library = "perception.magicleap";
#else
            const string Library = "ml_perception_client";
#endif

            [DllImport(Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLPersistentCoordinateFrameTrackerCreate")]
            public static extern MLApiResult Create(out ulong out_tracker_handle);

            [DllImport(Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLPersistentCoordinateFrameGetClosest")]
            public static unsafe extern MLApiResult GetClosest(ulong tracker_handle, ref Vector3 target, out MLCoordinateFrameUID out_cfuid);

            [DllImport(Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLPersistentCoordinateFrameTrackerDestroy")]
            public static extern MLApiResult Destroy(ulong tracker_handle);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_TryGetPose")]
            public static extern bool TryGetPose(MLCoordinateFrameUID id, out Pose out_pose);
        }

        static Vector3 FlipHandedness(Vector3 position)
        {
            return new Vector3(position.x, position.y, -position.z);
        }

        static Quaternion FlipHandedness(Quaternion rotation)
        {
            return new Quaternion(rotation.x, rotation.y, -rotation.z, -rotation.w);
        }

#if !UNITY_2020_2_OR_NEWER
        /// <summary>
        /// Method for creating the Magic Leap Provider 
        /// </summary>
        /// <returns>A generic <c>Provider</c> representing the Magic Leap Provider.</returns>
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        class MagicLeapProvider : Provider
        {
            ulong m_TrackerHandle = Native.InvalidHandle;

            PerceptionHandle m_PerceptionHandle;

            /// <summary>
            /// The privilege required to access persistent coordinate frames
            /// </summary>
            const uint k_MLPivilegeID_PwFoundObjRead = 201;

            /// <summary>
            /// The squared amount by which a coordinate frame has to move for its anchor to be reported as "updated"
            /// </summary>
            const float k_CoordinateFramePositionEpsilonSquared = .0001f;

            public MagicLeapProvider()
            {
                m_PerceptionHandle = PerceptionHandle.Acquire();
            }

            public override void Start()
            {
                var result = Native.Create(out m_TrackerHandle);
                if (result != MLApiResult.Ok)
                {
                    m_TrackerHandle = Native.InvalidHandle;
                    LogWarning($"Could not create a Magic Leap Persistent Coordinate Frame Tracker because '{result}'. Anchors are unavailable.");
                }
            }

            public override void Stop()
            {
                if (m_TrackerHandle != Native.InvalidHandle)
                {
                    Native.Destroy(m_TrackerHandle);
                    m_TrackerHandle = Native.InvalidHandle;
                }
            }

            public override void Destroy()
            {
                m_PerceptionHandle.Dispose();
            }

            bool PosesAreApproximatelyEqual(Pose lhs, Pose rhs)
            {
                // todo 2019-05-21: consider rotation?
                return (lhs.position - rhs.position).sqrMagnitude <= k_CoordinateFramePositionEpsilonSquared;
            }

            /// <summary>
            /// Checks to see if there is a better PCF for a anchor and updates the
            /// <paramref name="referenceFrame"/> if necessary.
            /// </summary>
            /// <returns>true if the reference frame was updated</returns>
            bool UpdateReferenceFrame(ref ReferenceFrame referenceFrame)
            {
                // See if there is a better coordinate frame we could be using
                var mlTarget = FlipHandedness(referenceFrame.anchorPose.position);
                if (Native.GetClosest(m_TrackerHandle, ref mlTarget, out MLCoordinateFrameUID cfuid) != MLApiResult.Ok)
                {
                    // No coordinate frame could be found, so set tracking state to None
                    // and return whether the tracking state changed.
#if DEVELOPMENT_BUILD
                    DebugError("GetClosest failed.");
#endif
                    return referenceFrame.SetTrackingState(TrackingState.None);
                }
                else if (!Native.TryGetPose(cfuid, out Pose coordinateFrame))
                {
                    // Couldn't get a pose for the coordinate frame, so set tracking state to None
#if DEVELOPMENT_BUILD
                    DebugError($"TryGetPose for cfuid {cfuid} failed.");
#endif
                    return referenceFrame.SetTrackingState(TrackingState.None);
                }
                else if (!cfuid.Equals(referenceFrame.cfuid))
                {
                    // A different coordinate frame has been chosen
#if DEVELOPMENT_BUILD
                    DebugLog($"cfuid's changed. Old: {referenceFrame.coordinateFrame} New: {coordinateFrame}.");
#endif
                    referenceFrame.trackingState = TrackingState.Tracking;
                    referenceFrame.SetCoordinateFrame(cfuid, coordinateFrame);
                    return true;
                }
                // If the CFUIDs are the same, then check to see if the coordinate frame has changed
                else if (!PosesAreApproximatelyEqual(coordinateFrame, referenceFrame.coordinateFrame))
                {
                    // Coordinate frame has changed
#if DEVELOPMENT_BUILD
                    DebugLog($"Coordinate frame updated. Old: {referenceFrame.coordinateFrame} New: {coordinateFrame}.");
#endif
                    referenceFrame.trackingState = TrackingState.Tracking;
                    referenceFrame.coordinateFrame = coordinateFrame;
                    return true;
                }
                else
                {
                    // Common case: pose was retrieved, but nothing has changed.
                    return referenceFrame.SetTrackingState(TrackingState.Tracking);
                }
            }

            /// <summary>
            /// Populates <paramref name="added"/> with all the anchors
            /// which have been added since the last call to <see cref="GetChanges(XRAnchor, Allocator)"/>.
            /// </summary>
            /// <param name="added">An already created array to populate. Its length must match <see cref="m_PendingAdds"/>.</param>
            void GetAdded(NativeArray<XRAnchor> added)
            {
                if (!added.IsCreated)
                    throw new ArgumentException("Array has not been created.", nameof(added));

                if (added.Length != m_PendingAdds.Count)
                    throw new ArgumentException($"Array is not the correct size. Should be {m_PendingAdds.Count} but is {added.Length}.", nameof(added));

                for (int i = 0; i < m_PendingAdds.Count; ++i)
                {
                    var referenceFrame = m_PendingAdds[i];
                    UpdateReferenceFrame(ref referenceFrame);

                    // Store it in the list of changes for this frame.
                    added[i] = referenceFrame.anchor;

                    // Add it to persistent storage for subsequent frames.
                    m_ReferenceFrames.Add(referenceFrame);
                }

                m_PendingAdds.Clear();
            }

            /// <summary>
            /// Returns an array containing all the updated anchors since the last
            /// call to <see cref="GetChanges(XRAnchor, Allocator)"/>.
            /// This method considers all <see cref="m_ReferenceFrames"/>, so it should
            /// be called before <see cref="GetAdded(NativeArray{XRAnchor})"/> since
            /// that method will add elements to <see cref="m_ReferenceFrames"/>.
            /// </summary>
            /// <param name="allocator">The allocator to use for the returned array.</param>
            /// <param name="length">The number of updated anchors.</param>
            /// <returns>An array of updated anchors. Note the array's length
            /// will always be the total number of anchors. Use <paramref name="length"/>
            /// for the true number of updated anchors.</returns>
            NativeArray<XRAnchor> GetUpdated(Allocator allocator, out int length)
            {
                var updated = new NativeArray<XRAnchor>(m_ReferenceFrames.Count, allocator);
                length = 0;

                for (int i = 0; i < m_ReferenceFrames.Count; ++i)
                {
                    var referenceFrame = m_ReferenceFrames[i];
                    if (UpdateReferenceFrame(ref referenceFrame))
                    {
                        // Update the version in our persistent storage container
                        m_ReferenceFrames[i] = referenceFrame;
                        updated[length++] = referenceFrame.anchor;
                    }
                }

                return updated;
            }

            /// <summary>
            /// Populates <paramref name="removed"/> with the ids of the anchors
            /// removed since the last call to <see cref="GetChanges(XRAnchor, Allocator)"/>.
            /// </summary>
            /// <param name="removed">An already created array to populate. Its length must match <see cref="m_PendingRemoves"/>.</param>
            void GetRemoved(NativeArray<TrackableId> removed)
            {
                if (!removed.IsCreated)
                    throw new ArgumentException("Array has not been created.", nameof(removed));

                if (removed.Length != m_PendingRemoves.Count)
                    throw new ArgumentException($"Array is not the correct size. Should be {m_PendingRemoves.Count} but is {removed.Length}.", nameof(removed));

                for (int i = 0; i < removed.Length; ++i)
                {
                    removed[i] = m_PendingRemoves[i];
                }

                m_PendingRemoves.Clear();
            }

            public override unsafe TrackableChanges<XRAnchor> GetChanges(
                XRAnchor defaultAnchor,
                Allocator allocator)
            {
                using (var updated = GetUpdated(Allocator.Temp, out int updatedCount))
                {
                    var changes = new TrackableChanges<XRAnchor>(
                        m_PendingAdds.Count,
                        updatedCount,
                        m_PendingRemoves.Count,
                        allocator);

                    GetAdded(changes.added);
                    NativeArray<XRAnchor>.Copy(updated, changes.updated, updatedCount);
                    GetRemoved(changes.removed);

                    return changes;
                }
            }

            public override unsafe bool TryAddAnchor(Pose pose, out XRAnchor anchor)
            {
                if (m_TrackerHandle == Native.InvalidHandle)
                {
                    anchor = default;
                    return false;
                }

                // Get the pose position in right-handed coordinates
                var mlTarget = FlipHandedness(pose.position);

                // Get the ID of the closest PCF to our target position
                var getClosestResult = Native.GetClosest(m_TrackerHandle, ref mlTarget, out MLCoordinateFrameUID cfuid);
                if (getClosestResult != MLApiResult.Ok)
                {
                    LogWarning($"Could not create anchor because MLPersistentCoordinateFrameGetClosest returned {getClosestResult}.");
                    anchor = default;
                    return false;
                }

                // Get the pose of the PCF
                if (!Native.TryGetPose(cfuid, out Pose closetCoordinateFrame))
                {
                    LogWarning($"Could not create anchor because no pose could be determined for coordinate frame {cfuid}.");
                    anchor = default;
                    return false;
                }

                var referenceFrame = new ReferenceFrame(new ReferenceFrame.Cinfo
                {
                    closetCoordinateFrame = closetCoordinateFrame,
                    cfuid = cfuid,
                    trackingState = TrackingState.Tracking,
                    initialAnchorPose = pose
                });

                m_PendingAdds.Add(referenceFrame);
                anchor = referenceFrame.anchor;

                return true;
            }

            /// <summary>
            /// Removes <paramref name="trackableId"/> from <paramref name="referenceFrames"/>
            /// or returns false if the reference frame with <paramref name="trackableId"/>
            /// is not found. If <paramref name="trackableId"/> is found, the dictionary
            /// of coordinate frames <see cref="m_CoordinateFrames"/> is also updated.
            /// </summary>
            /// <param name="trackableId">The id of the reference frame to remove.</param>
            /// <param name="referenceFrames">The list of reference frames to search.</param>
            /// <returns><c>true</c> if found, <c>false</c> otherwise.</returns>
            bool Remove(TrackableId trackableId, List<ReferenceFrame> referenceFrames)
            {
                // Removal is uncommon and we don't expect that many anchors,
                // so a linear search should do.
                for (int i = 0; i < referenceFrames.Count; ++i)
                {
                    var referenceFrame = referenceFrames[i];
                    if (referenceFrame.trackableId.Equals(trackableId))
                    {
                        referenceFrames.RemoveAt(i);
                        return true;
                    }
                }

                return false;
            }

            public override bool TryRemoveAnchor(TrackableId trackableId)
            {
                if (Remove(trackableId, m_PendingAdds))
                {
                    // Since it was pending, we have never reported it as added.
                    // That means we should not report it as removed, so take no action.
                    return true;
                }
                else if (Remove(trackableId, m_ReferenceFrames))
                {
                    // We must remember that we removed it here so that
                    // we can report it as removed in the next call to GetChanges
                    m_PendingRemoves.Add(trackableId);
                    return true;
                }
                else
                {
                    // We don't know about this anchor
                    return false;
                }
            }

            List<ReferenceFrame> m_PendingAdds = new List<ReferenceFrame>();
            List<ReferenceFrame> m_ReferenceFrames = new List<ReferenceFrame>();
            List<TrackableId> m_PendingRemoves = new List<TrackableId>();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_ANDROID
            XRAnchorSubsystemDescriptor.Create(new XRAnchorSubsystemDescriptor.Cinfo
            {
                id = "MagicLeap-Anchor",
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapAnchorSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(MagicLeapAnchorSubsystem),
#else
                subsystemImplementationType = typeof(MagicLeapAnchorSubsystem),
#endif
                supportsTrackableAttachments = false
            });
#endif // UNITY_ANDROID
        }
    }
}
