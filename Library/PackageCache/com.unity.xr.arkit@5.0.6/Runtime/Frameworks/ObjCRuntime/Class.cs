using System;

namespace UnityEngine.XR.ARKit
{
    struct Class : IEquatable<Class>
    {
        IntPtr m_Self;

        public override int GetHashCode() => m_Self.GetHashCode();

        public override bool Equals(object obj) => obj is Class data && Equals(data);

        public bool Equals(Class other) => m_Self == other.m_Self;

        public static bool operator ==(Class lhs, Class rhs) => lhs.Equals(rhs);

        public static bool operator !=(Class lhs, Class rhs) => !lhs.Equals(rhs);

        public static bool operator ==(Class? lhs, Class? rhs)
        {
            // Both have a value, so compare values
            if (lhs.HasValue && rhs.HasValue)
                return lhs.Value.m_Self == rhs.Value.m_Self;

            // lhs has a value; rhs is null
            if (lhs.HasValue)
                return lhs.Value.m_Self == IntPtr.Zero;

            // rhs has a value; lhs is null
            if (rhs.HasValue)
                return rhs.Value.m_Self == IntPtr.Zero;

            // Neither has a value
            return true;
        }

        public static bool operator !=(Class? lhs, Class? rhs) => !(lhs == rhs);
    }
}
