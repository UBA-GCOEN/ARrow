using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.InteractionSubsystems
{
    /// <summary>
    /// The event data for a common gesture used to activate world geometry or UI.
    /// </summary>
    /// <seealso cref="XRGestureSubsystem"/>
    [StructLayout(LayoutKind.Sequential)]
    public struct ActivateGestureEvent : IEquatable<ActivateGestureEvent>
    {
        /// <summary>
        /// The <see cref="GestureId"/> associated with this gesture.
        /// </summary>
        public GestureId id { get { return m_Id; } }

        /// <summary>
        /// The <see cref="GestureState"/> of the gesture.
        /// </summary>
        public GestureState state { get { return m_State; } }

        /// <summary>
        /// Gets a default-initialized <see cref="ActivateGestureEvent"/>. 
        /// </summary>
        /// <returns>A default <see cref="ActivateGestureEvent"/>.</returns>
        public static ActivateGestureEvent GetDefault()
        {
            return new ActivateGestureEvent(GestureId.invalidId, GestureState.Invalid);
        }

        /// <summary>
        /// Constructs a new <see cref="ActivateGestureEvent"/>.
        /// </summary>
        /// <param name="id">The <see cref="GestureId"/> associated with the gesture.</param>
        /// <param name="state">The <see cref="GestureId"/> associated with the gesture.</param>
        public ActivateGestureEvent(GestureId id, GestureState state)
        {
            m_Id = id;
            m_State = state;
        }

        /// <summary>
        /// Generates a new string describing the gesture's properties suitable for debugging purposes.
        /// </summary>
        /// <returns>A string describing the gesture's properties.</returns>
        public override string ToString()
        {
            return string.Format(
                "Plane:\n\tid: {0}\n\tstate: {1}\n\t",
                id, state);
        }

        /// <summary>
        /// Comparison operation to determine if the give object is equal to this instace. 
        /// </summary>
        /// <param name="obj">object to compare to</param>
        /// <returns>true if equal, false otherwise</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ActivateGestureEvent && Equals((ActivateGestureEvent)obj);
        }

        /// <summary>
        /// Test if the given ActiveateGestureEvent is equal to this ActivateGestureEvent 
        /// </summary>
        /// <param name="other">The comparison object</param>
        /// <returns>true if they are equal. false otherwise.</returns>
        public bool Equals(ActivateGestureEvent other)
        {
            return
                m_Id.Equals(other.id) &&
                m_State == other.state;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = m_Id.GetHashCode();
                hashCode = (hashCode * 486187739) + ((int)m_State).GetHashCode();
                return hashCode;
            }
        }

        /// <summary>
        /// Comparison operation to determine if the ActivateGestureEvents are equal.
        /// </summary>
        /// <param name="lhs">The first ActivateGestureEvent to compare.</param>
        /// <param name="rhs">The second ActivateGestureEvent to compare.</param>
        /// <returns>true if they are not equal, false otherwise.</returns>
        /// <returns>true if the ActivateGestureEvents are equal. False otherwise.</returns>
        public static bool operator ==(ActivateGestureEvent lhs, ActivateGestureEvent rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// Comparison operation to determine if the ActivateGestureEvents are not equal
        /// </summary>
        /// <param name="lhs">The first ActivateGestureEvent to compare.</param>
        /// <param name="rhs">The second ActivateGestureEvent to compare.</param>
        /// <returns>true if they are not equal, false otherwise.</returns>
        public static bool operator !=(ActivateGestureEvent lhs, ActivateGestureEvent rhs)
        {
            return !lhs.Equals(rhs);
        }

        readonly GestureId m_Id;
        readonly GestureState m_State;
    }
}
