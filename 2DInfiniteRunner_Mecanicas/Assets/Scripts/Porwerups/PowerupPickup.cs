using UnityEngine;

public class PowerupPickup : MonoBehaviour
{
    public PowerUpData data;
    private float _lifetime = 12f;

    public void Setup(PowerUpData d)
    {
        data = d;
    }

    void Update()
    {
        _lifetime -= Time.deltaTime;
        if (_lifetime <= 0f) Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var eventBus = GameContainer.Resolve<IEventBus>();

        if (data != null)
        {
            eventBus.Publish(new PowerUpCollectedEvent { powerUpId = data.id });
            var audio = GameContainer.Resolve<IAudioService>();
            audio.PlayOneShot(data.pickupSound);
        }
        else
        {
            Debug.LogWarning("PowerupPickup: data es null en prefab. Asegúrate de asignar PowerUpData o llamar Setup().");
        }

        Destroy(gameObject);
    }
}

