/*using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour
{
    public ObstacleSet set; // ScriptableObject con 3 prefabs
    public GameConfig config;

    private float _timer;

    void Start()
    {
        ResetTimer();
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            Spawn();
            ResetTimer();
        }
    }

    void ResetTimer()
    {
        _timer = Random.Range(config.obstacleMinSpawn, config.obstacleMaxSpawn);
    }

    void Spawn()
    {
        if (set == null || set.obstacles == null || set.obstacles.Length == 0) return;
        var idx = Random.Range(0, set.obstacles.Length);
        var prefab = set.obstacles[idx];
        var pos = new Vector3(config.spawnX, transform.position.y, 0);
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var mover = go.GetComponent<Mover>();
        if (mover != null) mover.Speed = config.baseSpeed;
        // mover.destroyX = config.destroyX; // si expones desde Mover
    }
}*/
