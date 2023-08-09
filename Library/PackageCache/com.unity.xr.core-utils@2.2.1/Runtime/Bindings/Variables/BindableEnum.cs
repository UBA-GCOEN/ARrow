using System;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
    /// <summary>
    /// Class which contains an <see langword="enum"/> member variable of type <typeparamref name="T"/> and provides a binding API to data changes.
    /// </summary>
    /// <remarks>
    /// Uses <c>GetHashCode</c> for comparison since <c>Equals</c> on an <c>enum</c> causes GC alloc.
    /// </remarks>
    /// <typeparam name="T">The type of the variable enum.</typeparam>
    public class BindableEnum<T> : BindableVariableBase<T> where T : struct, IConvertible
    {
        /// <inheritdoc/>
        public BindableEnum(T initialValue = default, bool checkEquality = true, Func<T, T, bool> equalityMethod = null, bool startInitialized = false)
            : base(initialValue, checkEquality, equalityMethod, startInitialized)
        {
        }

        /// <summary>
        /// Performs equal operation by comparing hash codes.
        /// </summary>
        /// <param name="other">Other enum to compare with.</param>
        /// <returns>Returns <see langword="true"/> if equal, returns <see langword="false"/> otherwise.</returns>
        public override bool ValueEquals(T other) => Value.GetHashCode() == other.GetHashCode();
    }
}
