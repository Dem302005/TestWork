using UnityEditor;
using UnityEngine;

namespace Gamekit3D
{
    public class RangeWeapon : MonoBehaviour
    {
        public Vector3 muzzleOffset;

        public Projectile projectile;

        protected Projectile m_LoadedProjectile;
        protected ObjectPooler<Projectile> m_ProjectilePool;

        public Projectile loadedProjectile => m_LoadedProjectile;

        private void Start()
        {
            m_ProjectilePool = new ObjectPooler<Projectile>();
            m_ProjectilePool.Initialize(20, projectile);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            var worldOffset = transform.TransformPoint(muzzleOffset);
            Handles.color = Color.yellow;
            Handles.DrawLine(worldOffset + Vector3.up * 0.4f, worldOffset + Vector3.down * 0.4f);
            Handles.DrawLine(worldOffset + Vector3.forward * 0.4f, worldOffset + Vector3.back * 0.4f);
        }
#endif

        public void Attack(Vector3 target)
        {
            AttackProjectile(target);
        }

        public void LoadProjectile()
        {
            if (m_LoadedProjectile != null)
                return;

            m_LoadedProjectile = m_ProjectilePool.GetNew();
            m_LoadedProjectile.transform.SetParent(transform, false);
            m_LoadedProjectile.transform.localPosition = muzzleOffset;
            m_LoadedProjectile.transform.localRotation = Quaternion.identity;
        }

        private void AttackProjectile(Vector3 target)
        {
            if (m_LoadedProjectile == null) LoadProjectile();

            m_LoadedProjectile.transform.SetParent(null, true);
            m_LoadedProjectile.Shot(target, this);
            m_LoadedProjectile = null; //once shot, we don't own the projectile anymore, it does it's own life.
        }
    }
}