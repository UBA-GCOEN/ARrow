using System;

namespace Unity.XR.CoreUtils.Datums
{
    /// <summary>
    /// Serializable container class that holds a float value or container asset reference.
    /// </summary>
    /// <seealso cref="FloatDatum"/>
    [Serializable]
    public class FloatDatumProperty : DatumProperty<float, FloatDatum>
    {
        /// <inheritdoc/>
        public FloatDatumProperty(float value) : base(value)
        {
        }

        /// <inheritdoc/>
        public FloatDatumProperty(FloatDatum datum) : base(datum)
        {
        }
    }
}
