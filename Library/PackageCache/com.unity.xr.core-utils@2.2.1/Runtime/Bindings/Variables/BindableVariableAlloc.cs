using System;

namespace Unity.XR.CoreUtils.Bindings.Variables
{
    /// <summary>
    /// Generic class which contains a member variable of type <typeparamref name="T"/> and provides a binding API to data changes.
    /// If <typeparamref name="T"/> is <c>IEquatable</c>, use <see cref="BindableVariable{T}"/> instead.
    /// </summary>
    /// <typeparam name="T">The type of the variable value.</typeparam>
    /// <remarks>
    /// This class can be used for types which are not <c>IEquatable</c>.
    /// Since <typeparamref name="T"/> is not <c>IEquatable</c>, when setting the value,
    /// it calls <c>object.Equals</c> and will GC alloc.
    /// </remarks>
    /// <seealso cref="BindableVariable{T}"/>
    public class BindableVariableAlloc<T> : BindableVariableBase<T>
    {
        /// <inheritdoc />
        public BindableVariableAlloc(T initialValue = default, bool checkEquality = true, Func<T, T, bool> equalityMethod = null, bool startInitialized = false)
            : base(initialValue, checkEquality, equalityMethod, startInitialized)
        {
        }
    }
}
