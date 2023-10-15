using com.Neogoma.HoboDream.Impl;
using com.Neogoma.Octree;
using com.Neogoma.Stardust.Navigation;
using com.Neogoma.Stardust.Navigation.Rendering;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Neogoma.Stardust.Demo.Navigator
{
    /// <summary>
    /// Responsible for overriding the path displayed when doing navigation. Allows for customization of the generated path to target.
    /// </summary>
    public class BotPathRenderer : AbstractNonMonoInteractive, IPathRenderer
    {
       
        /// <summary>
        /// Encapsulated path renderer
        /// </summary>
        private PathRenderer normalPathRenderer;

        /// <summary>
        /// Called when new list of points is calculated.
        /// </summary>
        public UnityEvent<List<Vector3>> OnCalculatedPointList = new UnityEvent<List<Vector3>>();
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="pathPrefab">path prefab gameObject</param>
        public BotPathRenderer(GameObject pathPrefab)
        {
            normalPathRenderer = new PathRenderer(pathPrefab,PathFindingManager.Instance.transform);
        }

        /// <summary>
        /// Called by PathFindingManager, clears the current path
        /// </summary>
        public void ClearPath()
        {
            normalPathRenderer.ClearPath();
        }

        public void DisplayPath(Vector3 playerPosition, List<IOctreeCoordinateObject> allNavigationsPoint)
        {

            List<Vector3> positionList= new List<Vector3>();

            for (int i = 0; i < allNavigationsPoint.Count; i++)
            {
                positionList.Add(allNavigationsPoint[i].GetCoordinates());
            }


            normalPathRenderer.DisplayPath(playerPosition,allNavigationsPoint);
            OnCalculatedPointList.Invoke(positionList);
        }
    }
}

