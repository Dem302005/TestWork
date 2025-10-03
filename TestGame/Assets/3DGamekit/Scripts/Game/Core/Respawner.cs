using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class Respawner : MonoBehaviour
    {
        public GameObject player;
        public float savePeriod = 5;

        public List<SaveState> savedStates = new List<SaveState>();

        private float lastCheck;
        private bool paused;

        private void Start()
        {
            lastCheck = Time.time - savePeriod;
        }

        private void Update()
        {
            if (!paused && Time.time - lastCheck > savePeriod)
            {
                lastCheck = Time.time;
                savedStates.Add(new SaveState
                    { position = player.transform.position, rotation = player.transform.rotation });
                savedStates.RemoveRange(0, Mathf.Max(0, savedStates.Count - 8));
            }
        }

        public void Pause()
        {
            paused = true;
        }

        public void Resume()
        {
            paused = false;
        }

        public void RestoreLast()
        {
            if (savedStates.Count > 0)
            {
                var ss = savedStates[savedStates.Count - 1];
                savedStates.RemoveAt(savedStates.Count - 1);
                player.transform.position = ss.position;
                player.transform.rotation = ss.rotation;
            }
        }

        [Serializable]
        public class SaveState
        {
            public Vector3 position;
            public Quaternion rotation;
        }
    }
}