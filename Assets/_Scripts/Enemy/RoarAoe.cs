using UnityEngine;

public class RoarAOE : MonoBehaviour
{
    [Header("Life")]
    [SerializeField] float lifeTime = 0.5f;

    [Header("Knockback")]
    [SerializeField] float knockbackPower = 6f;
    [SerializeField] bool useDistanceReduce = true;

    SphereCollider col;

    void Awake()
    {
        col = GetComponent<SphereCollider>();
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        IHitResponder responder = other.GetComponent<IHitResponder>();
        if (responder == null) return;

        Vector3 dir = other.transform.position - transform.position;
        float distance = dir.magnitude;
        dir.Normalize();

        float power = knockbackPower;

        if (useDistanceReduce)
        {
            float ratio = Mathf.Clamp01(1f - (distance / col.radius));
            power *= ratio;
        }

        DamageData data = new DamageData(
            0f,                 // 데미지 없음
            gameObject,         // 출처
            dir * power         // 넉백 벡터
        );

        responder.TakeDamage(data);
    }
}

