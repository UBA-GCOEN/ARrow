namespace UnityEngine.XR.ARKit
{
    /// <summary>
    /// Defines the priority of <see cref="ARCollaborationData"/>.
    /// </summary>
    public enum ARCollaborationDataPriority
    {
        /// <summary>
        /// No priority is set.
        /// </summary>
        None,

        /// <summary>
        /// The data is important to the collaborative session and should be sent reliably (for example, using TCP).
        /// </summary>
        Critical,

        /// <summary>
        /// The data is not important to collaborative session quality and may be sent unreliably (for example, using UDP).
        /// </summary>
        Optional
    }
}
