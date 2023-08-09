using System;

namespace Unity.XR.CoreUtils.Datums
{
    /// <summary>
    /// Serializable container class that holds a string value or container asset reference.
    /// </summary>
    /// <seealso cref="StringDatum"/>
    [Serializable]
    public class StringDatumProperty : DatumProperty<string, StringDatum>
    {
        /// <inheritdoc/>
        public StringDatumProperty(string value) : base(value)
        {
        }

        /// <inheritdoc/>
        public StringDatumProperty(StringDatum datum) : base(datum)
        {
        }
    }
}
