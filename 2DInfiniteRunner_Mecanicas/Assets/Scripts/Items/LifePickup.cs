using UnityEngine;

public class LifePickup : MonoBehaviour
{
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        var eventBus = GameContainer.Resolve<IEventBus>();
        eventBus.Publish(new LifeCollectedEvent { newLives = 1 });
        var audio = GameContainer.Resolve<IAudioService>();
        audio.PlayOneShot(pickupSound);
        Destroy(gameObject);
    }
}
