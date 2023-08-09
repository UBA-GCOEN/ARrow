using UnityEditor;

namespace Unity.XR.CoreUtils.Datums.Editor
{
    /// <summary>
    /// Variable reference drawer used to represent string references.
    /// </summary>
    /// <seealso cref="DatumPropertyDrawer"/>
    /// <summary>
    /// Class used to draw an <see cref="StringDatumProperty"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(StringDatumProperty))]
    public class StringDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}
