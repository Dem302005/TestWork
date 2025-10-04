using UnityEngine;

namespace Gamekit3D
{
    public class GunnerLightning : MonoBehaviour
    {
        //public Transform start;
        public Transform end;

        public float updateInterval = 0.5f;

        public int pointCount = 10;
        public float randomOffset = 0.5f;
        private Transform[] branch;

        private LineRenderer LR;
        private Vector3[] points;
        private float updateTime;

        // Use this for initialization
        private void Start()
        {
            LR = GetComponent<LineRenderer>();
            points = new Vector3[pointCount];
            LR.positionCount = pointCount;
            LR.useWorldSpace = false;
        }

        private void Update()
        {
            if (Time.time >= updateTime)
            {
                LR.positionCount = pointCount;

                points[0] = Vector3.zero;
                var Segment = (end.position - transform.position) / (pointCount - 1);

                for (var i = 1; i < pointCount - 1; i++)
                {
                    points[i] = Segment * i;
                    points[i].y += Random.Range(-randomOffset, randomOffset);
                    points[i].x += Random.Range(-randomOffset, randomOffset);
                }

                points[pointCount - 1] = end.position - transform.position;
                LR.SetPositions(points);

                updateTime += updateInterval;
            }
        }
    }
}