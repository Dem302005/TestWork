using System.Collections.Generic;
using UnityEngine;

namespace Gamekit3D
{
    public class BatchProcessor : MonoBehaviour
    {
        public delegate void BatchProcessing();

        protected static BatchProcessor s_Instance;
        protected static List<BatchProcessing> s_ProcessList;

        static BatchProcessor()
        {
            s_ProcessList = new List<BatchProcessing>();
        }

        // Update is called once per frame
        private void Update()
        {
            for (var i = 0; i < s_ProcessList.Count; ++i) s_ProcessList[i]();
        }

        public static void RegisterBatchFunction(BatchProcessing function)
        {
            s_ProcessList.Add(function);
        }

        public static void UnregisterBatchFunction(BatchProcessing function)
        {
            s_ProcessList.Remove(function);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void Init()
        {
            if (s_Instance != null)
                return;

            var obj = new GameObject("BatchProcessor");
            DontDestroyOnLoad(obj);
            s_Instance = obj.AddComponent<BatchProcessor>();
        }
    }
}