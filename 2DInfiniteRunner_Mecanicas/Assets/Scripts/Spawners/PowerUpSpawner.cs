using UnityEngine;

public class PowerupSpawner : MonoBehaviour
{
    public PowerUpData[] powerups;
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
            SpawnRandom();
            ResetTimer();
        }
    }

    void ResetTimer() => _timer = config != null ? config.powerupSpawnInterval : 10f;

    void SpawnRandom()
    {
        if (powerups == null || powerups.Length == 0) return;
        var idx = Random.Range(0, powerups.Length);
        var data = powerups[idx];
        if (data == null || data.prefab == null) return;

        var pos = new Vector3(transform.position.x, transform.position.y + Random.Range(-1f, 2f), 0f);
        var go = Instantiate(data.prefab, pos, Quaternion.identity);
        var powerComp = go.GetComponent<PowerupPickup>();
        if (powerComp != null) powerComp.Setup(data);
    }
}
