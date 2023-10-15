using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.Neogoma.HoboDream.Impl;
using com.Neogoma.Octree;
using com.Neogoma.Stardust.Navigation;
using UnityEngine.Events;
using TMPro;



namespace Neogoma.Stardust.Demo.Navigator
{
    /// <summary>
    /// Responsible for controlling navigation using guide bot.
    /// </summary>
    public class NavControl : MonoBehaviour
    {
        /// <summary>
        /// guide bot controller.
        /// </summary>
        public GameObject robot;
        /// <summary>
        /// panel that holds Dialogue and Target option.
        /// </summary>
        public GameObject worldSpacePanel;
        /// <summary>
        /// Navigation Panel message.
        /// </summary>
        public TMP_Text navPanelMessage;
        /// <summary>
        /// string used to change navPanelMessage when target reached.
        /// </summary>
        public string reachedMessage;
        /// <summary>
        /// Distance in front of the camera the guide bot will spawn.
        /// </summary>
        private const float DISTANCE_FROM_CAMERA = 1.1f;
        /// <summary>
        /// Camera transform reference.
        /// </summary>
        private Transform cameraPosition;
        /// <summary>
        /// GuidebotController component.
        /// </summary>
        private GuideBotController botController;

        private void Start()
        {
            botController = robot.GetComponent<GuideBotController>();
            cameraPosition = Camera.main.transform;
        }

        /// <summary>
        ///Initializes navigation for navigation bot.
        /// </summary>
        public void InitBot()
        {
            worldSpacePanel.gameObject.SetActive(true);
            robot.gameObject.SetActive(true);
            robot.transform.position = cameraPosition.position + cameraPosition.forward * DISTANCE_FROM_CAMERA;
            Vector3 targetPosition = new Vector3(cameraPosition.position.x, transform.position.y, cameraPosition.position.z) - transform.position;
            robot.transform.LookAt(targetPosition);
        }

        /// <summary>
        /// Starts navigation on GuideBotController component.
        /// </summary>
        public void StartMoving()
        {
            botController.StartNavigation();
        }

        /// <summary>
        /// Shows UI panel in worldspace to choose next target witg a default message.
        /// </summary>
        public void ShowFinishedPanel()
        {
            worldSpacePanel.SetActive(true);
            navPanelMessage.text = reachedMessage;
        }

    }
}


