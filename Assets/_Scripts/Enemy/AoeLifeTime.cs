using UnityEngine;

public class AoeLifeTime : MonoBehaviour
{
    [SerializeField] float lifeTime = 0.5f;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

