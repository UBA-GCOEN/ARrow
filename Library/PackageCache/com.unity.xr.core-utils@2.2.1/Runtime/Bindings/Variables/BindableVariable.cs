using System;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
    /// <summary>
    /// Generic class which contains a member variable of type <typeparamref name="T"/> and provides a binding API to data changes.
    /// </summary>
    /// <typeparam name="T">The type of the variable value.</typeparam>
    /// <remarks>
    /// <typeparamref name="T"/> is <c>IEquatable</c> to avoid GC alloc that would occur with <c>object.Equals</c> in the base class.
    /// </remarks>
    public class BindableVariable<T> : BindableVariableBase<T> where T : IEquatable<T>
    {
        /// <inheritdoc />
        public BindableVariable(T initialValue = default, bool checkEquality = true, Func<T, T, bool> equalityMethod = null, bool startInitialized = false)
            : base(initialValue, checkEquality, equalityMethod, startInitialized)
        {
        }

        /// <inheritdoc />
        // Uses IEquatable<T>.Equals rather than object.Equals done in the base class to avoid GC alloc
        public override bool ValueEquals(T other) => Value.Equals(other);
    }
}
