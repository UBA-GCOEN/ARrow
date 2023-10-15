using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Neogoma.Stardust.Demo.Navigator
{
    /// <summary>
    /// Compass system points on only one axis towards the target direction 
    /// </summary>
    public class ArrowController : MonoBehaviour
    {
        /// <summary>
        /// this rect transform.
        /// </summary>
        private RectTransform rt;
        /// <summary>
        /// determines if compass can update its angle and point to the target.
        /// </summary>
        private bool canRotate = false;
        /// <summary>
        /// angle to rotate compass.
        /// </summary>
        private float angle;
        /// <summary>
        /// Constant for angle in degrees.
        /// </summary>
        private const float ANGLE_DEGREES = Mathf.Rad2Deg - 90;

        private void Start()
        {
            rt = GetComponent<RectTransform>();
            transform.parent.gameObject.SetActive(false);
        }
        private void Update()
        {
            if(canRotate)
            {             
                rt.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        /// <summary>
        /// Called when the Compass button is pressed. Gets the rect transform component and begins arrow rotation.
        /// </summary>
        /// <param name="target">Vector3 position for the selected target.</param>
        public void Init(Vector3 target)
        {
            Vector3 objScreenPos = Camera.main.WorldToScreenPoint(target);
            Vector3 dir = (objScreenPos - rt.position).normalized;
            angle = Mathf.Atan2(dir.y, dir.x) * ANGLE_DEGREES;
            canRotate = true;
        }
        /// <summary>
        /// called when the object is disabled.
        /// </summary>
        private void OnDisable()
        {
            canRotate = false;
        }
    }
}
