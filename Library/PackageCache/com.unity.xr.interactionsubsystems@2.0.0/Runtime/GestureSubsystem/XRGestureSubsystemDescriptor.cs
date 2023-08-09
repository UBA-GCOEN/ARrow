using System;
using UnityEngine.SubsystemsImplementation;

namespace UnityEngine.XR.InteractionSubsystems
{
    /// <summary>
    /// Descriptor for the <see cref="XRGestureSubsystem"/> describing capabilities which may vary by implementation.
    /// </summary>
    public sealed class XRGestureSubsystemDescriptor : SubsystemDescriptorWithProvider<XRGestureSubsystem, XRGestureSubsystem.Provider>
    {
        /// <summary>
        /// Used in conjunction with <see cref="RegisterDescriptor(Cinfo)"/> to register a provider.
        /// This should only be used by subsystem implementors.
        /// </summary>
        public struct Cinfo : IEquatable<Cinfo>
        {
            /// <summary>
            /// The string used to identify this subsystem implementation.
            /// This will be available when enumerating the available descriptors at runtime.
            /// </summary>
            public string id { get; set; }

            /// <summary>
            /// Specifies the provider implementation type to use for instantiation.
            /// </summary>
            /// <value>
            /// The provider implementation type to use for instantiation.
            /// </value>
            public Type providerType { get; set; }

            /// <summary>
            /// Specifies the <c>XRGestureSubsystemDescriptor</c>-derived type that forwards casted calls to its provider.
            /// </summary>
            /// <value>
            /// The type of the subsystem to use for instantiation. If null, <c>XRGestureSubsystemDescriptor</c> will be instantiated.
            /// </value>
            public Type subsystemTypeOverride { get; set; }

            /// <summary>
            /// The <c>Type</c> of the implementation.
            /// </summary>
            [Obsolete("XRGestureSubsystemDescriptor no longer supports the deprecated set of base classes for subsystems as of Unity 2020.2. Use providerType and, optionally, subsystemTypeOverride instead.", true)]
            public Type subsystemImplementationType { get; set; }

            /// <inheritdoc  />
            public override int GetHashCode()
            {
                unchecked
                {
                    var hash = (id != null) ? id.GetHashCode() : 0;
                    hash = hash * 486187739 + ((providerType != null) ? providerType.GetHashCode() : 0);
                    hash = hash * 486187739 + ((subsystemTypeOverride != null) ? subsystemTypeOverride.GetHashCode() : 0);
                    return hash;
                }
            }

            /// <inheritdoc  />
            public override bool Equals(object obj)
            {
                if (!(obj is Cinfo))
                    return false;

                return Equals((Cinfo)obj);
            }

            /// <summary>
            /// Equivalency test
            /// </summary>
            /// <param name="other">Object instance to test against</param>
            /// <returns>true if equal. false otherwise.</returns>
            public bool Equals(Cinfo other)
            {
                return
                    string.Equals(id, other.id) &&
                    (providerType == other.providerType) &&
                    (subsystemTypeOverride == other.subsystemTypeOverride);
            }

            /// <summary>
            /// Equivalency test against two <c>Cinfo</c> instances
            /// </summary>
            /// <param name="lhs">First object instance to test against</param>
            /// <param name="rhs">Second object instance to test against</param>
            /// <returns>true if equal. false otherwise.</returns>
            public static bool operator ==(Cinfo lhs, Cinfo rhs)
            {
                return lhs.Equals(rhs);
            }

            /// <summary>
            /// Inequality test against two <c>Cinfo</c> instances
            /// </summary>
            /// <param name="lhs">First object instance to test against</param>
            /// <param name="rhs">Second object instance to test against</param>
            /// <returns>true if NOT equal. false otherwise.</returns>
            public static bool operator !=(Cinfo lhs, Cinfo rhs)
            {
                return !lhs.Equals(rhs);
            }
        }

        /// <summary>
        /// Register a subsystem implementation.
        /// This should only be used by subsystem implementors.
        /// </summary>
        /// <param name="cinfo">Information used to construct the descriptor.</param>
        public static void RegisterDescriptor(Cinfo cinfo)
        {
            SubsystemDescriptorStore.RegisterDescriptor(new XRGestureSubsystemDescriptor(cinfo));
        }

        XRGestureSubsystemDescriptor(Cinfo cinfo)
        {
            id = cinfo.id;
            providerType = cinfo.providerType;
            subsystemTypeOverride = cinfo.subsystemTypeOverride;
        }
    }
}
