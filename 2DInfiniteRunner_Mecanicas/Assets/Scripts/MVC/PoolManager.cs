using System.Collections.Generic;
using UnityEngine;
using RunnerGame.MVC.View;
using RunnerGame.MVC.Model; // Ajusta si tu ObstacleStoreSoA está en otro namespace

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
            CreateInitialPool();
        }

        private void CreateInitialPool()
        {
            pool.Clear();
            for (int i = 0; i < initialPoolSize; i++)
            {
                var inst = Instantiate(obstaclePrefab, poolParent);
                inst.Release(); // método de ObstacleView: desactiva y marca index -1
                pool.Add(inst);
            }
        }

        /// <summary>
        /// Sincroniza las primeras store.count entradas con los primeros elementos del pool.
        /// Llama a este método DESPUÉS de actualizar la lógica (MovementSystem.Update).
        /// </summary>
        public void SyncWithStore(ObstacleStoreSoA store)
        {
            if (store == null) return;

            // Si hay más obstáculos que objetos disponibles, expandimos
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

            // Desactivar el resto del pool
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

        // (Opcional) método para obtener un view por índice si quieres control adicional
        public ObstacleView GetViewAt(int index)
        {
            if (index < 0 || index >= pool.Count) return null;
            return pool[index];
        }
    }
}
