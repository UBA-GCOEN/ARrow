using System;
using UnityEngine;

namespace Unity.XR.CoreUtils.Datums
{
    /// <summary>
    /// Serializable container class that holds an animation curve value or container asset reference.
    /// </summary>
    /// <seealso cref="AnimationCurveDatum"/>
    [Serializable]
    public class AnimationCurveDatumProperty : DatumProperty<AnimationCurve, AnimationCurveDatum>
    {
        /// <inheritdoc />
        public AnimationCurveDatumProperty(AnimationCurve value) : base(value)
        {
        }

        /// <inheritdoc />
        public AnimationCurveDatumProperty(AnimationCurveDatum datum) : base(datum)
        {
        }
    }
}
