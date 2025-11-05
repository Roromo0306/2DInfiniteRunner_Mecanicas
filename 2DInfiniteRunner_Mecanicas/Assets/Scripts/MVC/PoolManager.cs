using System.Collections.Generic;
using UnityEngine;
using RunnerGame.MVC.View;
using RunnerGame.MVC.Model;

namespace RunnerGame.Systems
{
    [DisallowMultipleComponent]
    public class PoolManager : MonoBehaviour
    {
        [Header("Prefab & pool")]
        [SerializeField] private ObstacleView obstaclePrefab; // prefab con ObstacleView
        [SerializeField] private int initialPoolSize = 20;
        [SerializeField] private Transform poolParent;

        private List<ObstacleView> pool = new List<ObstacleView>();

        private void Awake()
        {
            if (poolParent == null) poolParent = this.transform;
            // Si no hay prefab asignado, no intentar crear pool (evitar NRE)
            if (obstaclePrefab == null)
            {
                Debug.LogWarning("[PoolManager] obstaclePrefab no asignado. Asigna el prefab en el Inspector o llama SetObstaclePrefab(prefab) por código antes de usar SyncWithStore.");
                return;
            }
            CreateInitialPool();
        }

        private void CreateInitialPool()
        {
            pool.Clear();
            for (int i = 0; i < initialPoolSize; i++)
            {
                var inst = Instantiate(obstaclePrefab, poolParent);
                inst.Release(); // desactivar y marcar index -1
                pool.Add(inst);
            }
        }

        // Nuevo: permite asignar el prefab por código antes de inicializar el pool
        public void SetObstaclePrefab(ObstacleView prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("[PoolManager] SetObstaclePrefab recibió null.");
                return;
            }

            obstaclePrefab = prefab;
            // si todavía no se ha inicializado el pool (pool vacío), inicializa ahora
            if (pool == null || pool.Count == 0)
            {
                CreateInitialPool();
            }
        }

        public void SyncWithStore(ObstacleStoreSoA store)
        {
            if (store == null) return;
            if (obstaclePrefab == null)
            {
                // nada que hacer si no tenemos prefab
                return;
            }

            if (store.count > pool.Count)
            {
                ExpandPool(store.count - pool.Count);
            }

            int i = 0;
            for (; i < store.count; i++)
            {
                var view = pool[i];
                view.AssignIndex(i);
                Vector2 pos = new Vector2(store.posX[i], store.posY[i]);
                float width = store.widths[i];
                view.SyncToData(pos, width);
            }

            for (; i < pool.Count; i++)
            {
                pool[i].Release();
            }
        }

        private void ExpandPool(int extra)
        {
            for (int e = 0; e < extra; e++)
            {
                var inst = Instantiate(obstaclePrefab, poolParent);
                inst.Release();
                pool.Add(inst);
            }
        }

        public ObstacleView GetViewAt(int index)
        {
            if (index < 0 || index >= pool.Count) return null;
            return pool[index];
        }
    }
}
