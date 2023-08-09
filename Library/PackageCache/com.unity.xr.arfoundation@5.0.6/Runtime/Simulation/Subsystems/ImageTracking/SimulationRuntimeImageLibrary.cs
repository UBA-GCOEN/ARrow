using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.Simulation
{
    /// <summary>
    /// Minimal implementation of <see cref="RuntimeReferenceImageLibrary"/> for simulation use.
    /// </summary>
    class SimulationRuntimeImageLibrary : RuntimeReferenceImageLibrary
    {
        XRReferenceImageLibrary m_Library;

        public override int count => m_Library.count;

        public SimulationRuntimeImageLibrary(XRReferenceImageLibrary library)
        {
            m_Library = library;
        }

        protected override XRReferenceImage GetReferenceImageAt(int index) => m_Library[index];

        /// <summary>
        /// Given a texture, returns an <see cref="XRReferenceImage"/> from the library with a matching texture,
        /// or <c>null</c> if no match was found.
        /// </summary>
        /// <param name="texture">The texture whose <see cref="XRReferenceImage"/> we are seeking.</param>
        /// <returns>An <see cref="XRReferenceImage"/> with a matching texture, or <c>null</c> if not found.</returns>
        public XRReferenceImage? GetReferenceImageWithTexture(Texture2D texture)
        {
            foreach (var referenceImage in m_Library)
            {
                if (referenceImage.texture == texture)
                    return referenceImage;
            }

            return null;
        }
    }
}
