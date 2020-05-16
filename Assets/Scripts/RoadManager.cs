using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SpyHunter.Roads
{
    public class RoadManager : MonoBehaviour
    {
        [System.Serializable]
        public struct RoadSet
        {
            public GameObject[] roadStraightPrefab;
            public GameObject[] roadLeftPrefab;
            public GameObject[] roadRightPrefab;
        }

        public RoadSet[] roadSets;
        public GameObject startingTunnelPrefab;
        public double roadLength;

        int previousRoadObj;
        float roadAngle;

        void Start()
        {

        }

        void Update()
        {

        }
    }
}