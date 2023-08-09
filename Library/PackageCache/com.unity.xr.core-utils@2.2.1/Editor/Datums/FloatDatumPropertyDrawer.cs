using UnityEditor;

namespace Unity.XR.CoreUtils.Datums.Editor
{
    /// <summary>
    /// Variable reference drawer used to represent a float reference.
    /// </summary>
    /// <seealso cref="FloatDatumProperty"/>
    /// <seealso cref="DatumPropertyDrawer"/>
    /// <summary>
    /// Class used to draw a <see cref="FloatDatumProperty"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(FloatDatumProperty))]
    public class FloatDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}
