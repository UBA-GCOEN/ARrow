using System;

namespace Unity.XR.CoreUtils.Datums
{
    /// <summary>
    /// Serializable container class that holds an int value or container asset reference.
    /// </summary>
    /// <seealso cref="IntDatum"/>
    [Serializable]
    public class IntDatumProperty : DatumProperty<int, IntDatum>
    {
        /// <inheritdoc/>
        public IntDatumProperty(int value) : base(value)
        {
        }

        /// <inheritdoc/>
        public IntDatumProperty(IntDatum datum) : base(datum)
        {
        }
    }
}
