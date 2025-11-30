using UnityEngine;

public class Mover : MonoBehaviour
{
    public float Speed = 5f;
    public float DestroyX = -15f;

    void Update()
    {
        transform.Translate(Vector3.left * Speed * Time.deltaTime);
        if (transform.position.x < DestroyX) Destroy(gameObject);
    }
}

