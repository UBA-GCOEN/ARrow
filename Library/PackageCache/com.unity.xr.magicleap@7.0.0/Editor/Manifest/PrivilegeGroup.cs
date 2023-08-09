using System;

using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    /// <summary>
    /// Class representing privilege Grouping 
    /// </summary>
    [Serializable]
    public class PrivilegeGroup
    {
        [SerializeField]
        private string m_Name;

        [SerializeField]
        internal Privilege[] m_Privileges;

        /// <summary>
        /// Privilege group name
        /// </summary>
        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        /// <summary>
        /// List of Privileges in this PrivilegeGroup
        /// </summary>
        public Privilege[] Privileges
        {
            get => m_Privileges;
            set => m_Privileges = value;
        }

        /// <summary>
        /// Equality comparison
        /// </summary>
        /// <param name="obj">Object to compare this PrivilegeGroup with</param>
        /// <returns>true if they are equal</returns>
        public override bool Equals(object obj)
        {
            var p2 = obj as PrivilegeGroup;
            if (p2 == null)
                return false;
            return Name == p2.Name
                && System.Object.ReferenceEquals(Privileges, p2.Privileges);
        }

        /// <summary>
        /// Generate a hash code of this class
        /// </summary>
        /// <returns>Integer representing the hash code of this object</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (!string.IsNullOrEmpty(Name))
                    hash = hash * 486187739 + Name.GetHashCode();
                hash = hash * 486187739 + Privileges.GetHashCode();
                return hash;
            }
        }
    }
}