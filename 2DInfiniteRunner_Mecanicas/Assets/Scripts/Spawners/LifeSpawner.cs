using UnityEngine;

public class LifeSpawner : MonoBehaviour
{
    public GameObject lifePrefab;
    public GameConfig config;
    private float _timer;

    void Start()
    {
        _timer = config.lifeSpawnInterval;
    }

    void Update()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
        {
            Spawn();
            _timer = config.lifeSpawnInterval;
        }
    }

    void Spawn()
    {
        var pos = new Vector3(transform.position.x, transform.position.y + Random.Range(-1f, 2f), 0);
        Instantiate(lifePrefab, pos, Quaternion.identity);
    }
}
