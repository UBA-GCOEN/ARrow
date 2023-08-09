using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// A serializable class representing Magic Leap privilege settings
    /// </summary>
    [Serializable]
    public class Privilege
    {
        /// <summary>
        /// Category of the privilege. It can be one of `Invalid`, `Sensitive`, `Reality`, `Autogranted` or `Trusted`
        /// </summary>
        public enum Category
        {
            /// <summary>
            /// Invalid
            /// </summary>
            Invalid,
            /// <summary>
            /// Sensitive
            /// </summary>
            Sensitive,
            /// <summary>
            /// Reality
            /// </summary>
            Reality,
            /// <summary>
            /// Autogranted
            /// </summary>
            Autogranted,
            /// <summary>
            /// Trusted
            /// </summary>
            Trusted
        }

        [SerializeField]
        private uint m_ApiLevel;

        [FormerlySerializedAs("enabled")]
        [SerializeField]
        private bool m_Enabled;

        [FormerlySerializedAs("name")]
        [SerializeField]
        private string m_Name;

        /// <summary>
        /// Getter/Setter for the API level
        /// </summary>
        public uint ApiLevel
        {
            get => m_ApiLevel;
            set => m_ApiLevel = value;
        }

        /// <summary>
        /// TODO: delete this
        /// </summary>
        [Obsolete("Use Privilege.Enabled instead")]
        public bool enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        /// <summary>
        /// Getter/Setter on if the privilege is enabled.
        /// </summary>
        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        /// <summary>
        /// TODO: delete this
        /// </summary>
        [Obsolete("Use Privilege.Name instead")]
        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// Privilege Name
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// Equals check
        /// </summary>
        /// <param name="obj">Object to compare against</param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var p2 = obj as Privilege;
            if (p2 == null)
                return false;
            return Name == p2.Name
                && Enabled == p2.Enabled;
        }

        /// <summary>
        /// Get a Hash Code from the privilege.
        /// </summary>
        /// <returns>A Generated Hash from the Privilege</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (!string.IsNullOrEmpty(Name))
                    hash = hash * 486187739 + Name.GetHashCode();
                hash = hash * 486187739 + Enabled.GetHashCode();
                return hash;
            }
        }
    }
}