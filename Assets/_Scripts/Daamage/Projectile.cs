using UnityEngine;

/// <summary>
/// 화살과 같은 투사체의 이동 및 수명 관리를 담당하는 스크립트입니다.
/// </summary>
[RequireComponent(typeof(DamageDealer))] // DamageDealer 컴포넌트 필수
public class Projectile : MonoBehaviour
{
    // === Inspector 설정 변수 ===
    [Header("Projectile Settings")]
    [Tooltip("투사체의 초당 이동 속도")]
    public float launchSpeed = 2f;

    [Tooltip("투사체가 자동으로 파괴되기까지의 시간 (수명)")]
    public float lifetime = 10f;

    // === 내부 상태 변수 ===
    private bool isLaunched = false;
    private float lifeTimer;
    private DamageDealer damageDealer;

    void Awake()
    {
        // DamageDealer를 미리 가져와 참조합니다.
        damageDealer = GetComponent<DamageDealer>();
        lifeTimer = lifetime;

        // DamageDealer가 없다면 오류 방지 (RequireComponent로 사실상 방지됨)
        if (damageDealer == null)
        {
            Debug.LogError("Projectile에는 DamageDealer 컴포넌트가 필요합니다.");
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// PlayerControll.fireArrow()에서 호출되어 발사 프로세스를 시작합니다.
    /// </summary>
    public void Launch()
    {
        isLaunched = true;
        lifeTimer = lifetime;
    }

    void Update()
    {
        if (!isLaunched) return;

        // 1. 이동 처리 (자신의 정면 방향으로 이동)
        transform.Translate(Vector3.right * launchSpeed * Time.deltaTime);

        // 2. 수명 타이머 처리 (자동 파괴)
        lifeTimer -= Time.deltaTime;
        if (lifeTimer <= 0f)
        {
            DestroyProjectile();
        }
    }

    // Note: OnTriggerEnter 로직은 DamageDealer에서 처리하므로, 
    // Projectile은 수명 관리에 집중합니다. 
    // DamageDealer가 충돌 시 스스로 Destroy(gameObject)를 호출하는 구조이므로, 
    // 여기에는 별도의 충돌 로직을 넣지 않습니다.

    /// <summary>
    /// 투사체를 파괴하고 필요한 정리 작업을 수행합니다.
    /// </summary>
    private void DestroyProjectile()
    {
        // Object Pooling을 사용하게 되면 여기서 Destroy 대신 Pool로 반환하는 로직을 넣을 수 있습니다.
        Destroy(gameObject);
    }

    // ⚠️ 참고: DamageDealer가 충돌 시 스스로 Destroy(gameObject)를 호출하고 있으므로, 
    // 이 스크립트의 DestroyProjectile()은 오직 수명(lifetime)이 다했을 때만 호출됩니다.
}