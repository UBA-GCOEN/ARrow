using UnityEditor;

namespace Unity.XR.CoreUtils.Datums.Editor
{
    /// <summary>
    /// Variable reference drawer used to represent an int reference.
    /// </summary>
    /// <seealso cref="IntDatumProperty"/>
    /// <seealso cref="DatumPropertyDrawer"/>
    /// <summary>
    /// Class used to draw an <see cref="IntDatumProperty"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(IntDatumProperty))]
    public class IntDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}
