using UnityEditor;

namespace Unity.XR.CoreUtils.Datums.Editor
{
    /// <summary>
    /// Variable reference drawer used to represent an Animation Curve reference.
    /// </summary>
    /// <seealso cref="AnimationCurveDatumProperty"/>
    /// <seealso cref="DatumPropertyDrawer"/>
    /// <summary>
    /// Class used to draw an <see cref="AnimationCurveDatumProperty"/>.
    /// </summary>
    [CustomPropertyDrawer(typeof(AnimationCurveDatumProperty))]
    public class AnimationCurveDatumPropertyDrawer : DatumPropertyDrawer
    {
    }
}
