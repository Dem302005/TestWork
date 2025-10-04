using UnityEngine;

namespace Gamekit3D
{
    [RequireComponent(typeof(Collider))]
    public class InventoryItem : MonoBehaviour, IDataPersister
    {
        public string inventoryKey = "";
        public LayerMask layers;
        public bool disableOnEnter;

        [HideInInspector] public new Collider collider;

        public AudioClip clip;
        public DataSettings dataSettings;

        private void Reset()
        {
            layers = LayerMask.NameToLayer("Everything");
            collider = GetComponent<Collider>();
            collider.isTrigger = true;
            dataSettings = new DataSettings();
        }

        private void OnEnable()
        {
            collider = GetComponent<Collider>();
            PersistentDataManager.RegisterPersister(this);
        }

        private void OnDisable()
        {
            PersistentDataManager.UnregisterPersister(this);
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawIcon(transform.position, "InventoryItem", false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (layers.Contains(other.gameObject))
            {
                var ic = other.GetComponent<InventoryController>();
                ic.AddItem(inventoryKey);
                if (disableOnEnter)
                {
                    gameObject.SetActive(false);
                    Save();
                }

                if (clip) AudioSource.PlayClipAtPoint(clip, transform.position);
            }
        }

        public DataSettings GetDataSettings()
        {
            return dataSettings;
        }

        public void SetDataSettings(string dataTag, DataSettings.PersistenceType persistenceType)
        {
            dataSettings.dataTag = dataTag;
            dataSettings.persistenceType = persistenceType;
        }

        public Data SaveData()
        {
            return new Data<bool>(gameObject.activeSelf);
        }

        public void LoadData(Data data)
        {
            var inventoryItemData = (Data<bool>)data;
            gameObject.SetActive(inventoryItemData.value);
        }

        public void Save()
        {
            PersistentDataManager.SetDirty(this);
        }
    }
}