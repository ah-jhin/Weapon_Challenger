using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("총알 속성")]
    public float speed = 35f;           // 총알 속도
    public float lifetime = 0.7f;       // 총알 수명
    public int minDamage = 1;           // 최소 데미지
    public int maxDamage = 3;           // 최대 데미지

    [Header("UI 연결")]
    public GameObject damageTextPrefab; // 데미지 텍스트 프리팹 (Inspector에서 연결)
    public Transform canvasParent;      // UI용 Canvas (Inspector에서 연결)

    [HideInInspector] public Collider2D ownerCollider; // 발사자 충돌 무시용
    private Rigidbody2D rb;

    void Awake()
    {
        // Rigidbody2D 확인 및 초기화
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    void Start()
    {
        // 총알은 생성 즉시 수평 방향으로 이동 (Pistol에서 방향 설정됨)
        // Rigidbody2D.velocity는 이미 Pistol.cs에서 설정해주므로 여기서는 안 건드림

        // 발사자와 충돌 무시 처리
        if (ownerCollider != null)
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), ownerCollider);
        }

        // lifetime 이후 자동 삭제
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Block"))
        {
            Destroy(gameObject); // 벽에 맞으면 총알 파괴
        }

        // 보스 또는 적과 충돌했을 때만 처리
        if (other.CompareTag("Enemy") || other.CompareTag("Boss") || other.CompareTag("WeakPoint"))
        {
            // 랜덤 데미지 계산
            int damage = Random.Range(minDamage, maxDamage + 1);
            bool isWeak = other.CompareTag("WeakPoint"); // 약점 태그면 true
            
            

            // 데미지 텍스트 생성
            if (damageTextPrefab != null && canvasParent != null)
            {
                // 화면 좌표로 변환 (보스 머리 위)
                Vector3 worldPos = other.transform.position + new Vector3(0, 1.5f, 0);
                Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

                // UI 캔버스 하위에 DamageText 프리팹 생성
                GameObject dmgText = Instantiate(damageTextPrefab, canvasParent);
                dmgText.transform.position = screenPos;

                // DamageText.cs에 데미지 값 전달
                DamageText dt = dmgText.GetComponent<DamageText>();
                if (dt != null)
                {
                    dt.Setup(damage, isWeak);
                }
                else
                {
                    Debug.LogWarning("[Bullet] DamageText 스크립트가 프리팹에 없음");
                }

                Boss boss = other.GetComponentInParent<Boss>();
                if (boss != null)
                {
                    boss.TakeDamage(damage); // 보스 체력 감소
                }
            }
            else
            {
                Debug.LogWarning("[Bullet] DamageTextPrefab 또는 CanvasParent가 Inspector에서 연결되지 않음");
            }

            // 총알 삭제
            Destroy(gameObject);
        }
    }
}
