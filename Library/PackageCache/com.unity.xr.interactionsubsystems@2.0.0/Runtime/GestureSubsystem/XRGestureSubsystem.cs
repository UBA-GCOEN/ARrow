using System;
using Unity.Collections;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.InteractionSubsystems
{
    /// <summary>
    /// This class controls the lifecycle of an XR Gesture subsystem.
    /// </summary>
    public class XRGestureSubsystem : SubsystemWithProvider<XRGestureSubsystem, XRGestureSubsystemDescriptor, XRGestureSubsystem.Provider>
    {
        /// <summary>
        /// A collection of all <see cref="ActivateGestureEvent"/> managed by this subsystem.
        /// This is cleared every frame and refreshed with new gesture events.
        /// </summary>
        public NativeArray<ActivateGestureEvent> activateGestureEvents => provider.activateGestureEvents;

        /// <summary>
        /// Do not call this directly. Call create on a valid <see cref="XRGestureSubsystemDescriptor"/> instead.
        /// </summary>
        public XRGestureSubsystem()
        {
        }

        /// <summary>
        /// Trigger the Gesture's update loop.
        /// </summary>
        public void Update()
        {
            provider.Update();
        }

        /// <summary>
        /// Provider to be implemented for <see cref="XRGestureSubsystem"/>.
        /// </summary>
        public abstract class Provider : SubsystemProvider<XRGestureSubsystem>
        {
            /// <inheritdoc />
            public override void Destroy()
            {
                if (m_ActivateGestureEvents.IsCreated)
                    m_ActivateGestureEvents.Dispose();
            }

            /// <summary>
            /// Ensuring that the NativeArray is cleaned up.
            /// </summary>
            ~Provider()
            {
                Destroy();
            }

            /// <summary>
            /// Implement the update method for Update events.
            /// </summary>
            public abstract void Update();

            /// <summary>
            /// Current Gesture Events.
            /// Note: Should we be considering a reallocation if the subsystem is stopped and restarted without destroying?
            ///       The current behaviour is to keep the events in memory. For now, we'll leave it as it was originally
            ///       designed. But we should keep an eye on this.
            /// </summary>
            public NativeArray<ActivateGestureEvent> activateGestureEvents => m_ActivateGestureEvents;

            /// <summary>
            /// Container for the current Gesture Events.
            /// </summary>
            protected NativeArray<ActivateGestureEvent> m_ActivateGestureEvents = new NativeArray<ActivateGestureEvent>(0, Allocator.Persistent);

        }
    }
}
