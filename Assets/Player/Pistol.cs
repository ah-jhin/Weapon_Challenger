using UnityEngine;
using System.Collections;

/// <summary>
/// 플레이어의 권총 발사 스크립트.
/// 발사 시 생성되는 총알에 씬의 Canvas(Transform)를 런타임에 할당한다.
/// </summary>
public class Pistol : MonoBehaviour
{
    [Header("총알 관련")]
    public GameObject bulletPrefab;     // 발사할 총알 프리팹 (프리팹 에셋)
    public Transform firePoint;         // 총구 위치 (Player의 자식 Transform)
    public float burstDelay = 0.15f;    // 점사 간격 (초)
    public int burstCount = 4;          // 점사 횟수

    [Header("UI 연결 (런타임 할당)")]
    public Transform uiCanvas;          // Inspector에서 씬의 Canvas(Transform)를 드래그

    private bool isFiring = false;      // 발사 중인지 체크
    private SpriteRenderer sr;          // 플레이어 방향 판별 (flipX)
    private Collider2D playerCollider;  // 발사자 충돌 무시용 (Player의 Collider)

    void Start()
    {
        // 컴포넌트 가져오기 및 초기 검사
        sr = GetComponent<SpriteRenderer>();
        playerCollider = GetComponentInParent<Collider2D>();
        if (playerCollider == null)
            Debug.LogError("[Pistol] 플레이어 Collider를 찾지 못했습니다. (Player에 Collider2D 필요)");
    }

    void Update()
    {
        // Z 키를 누르면 점사 시작
        if (Input.GetKeyDown(KeyCode.Z) && !isFiring)
        {
            StartCoroutine(FireBurst());
        }
    }

    IEnumerator FireBurst()
    {
        isFiring = true;
        for (int i = 0; i < burstCount; i++)
        {
            FireOneBullet();
            yield return new WaitForSeconds(burstDelay);
        }
        isFiring = false;
    }

    void FireOneBullet()
    {
        // 1) 총알 인스턴스 생성 (씬에 생성된 오브젝트)
        GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // 2) 방향 계산 (플립 상태에 따라 좌/우 결정)
        int dir = sr != null && sr.flipX ? -1 : 1;

        // 3) Rigidbody2D 가져와서 속도 설정 (총알에 의해 이동)
        Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(dir * 15f, 0f); // 속도는 필요시 조정
        }

        // 4) 총알 스크립트에 발사자 Collider와 Canvas 할당
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            // 발사자 충돌 무시 처리에 사용
            bulletScript.ownerCollider = playerCollider;

            // 씬의 Canvas를 런타임에 할당 -> prefab asset에 Scene Object 참조를 저장하지 않음
            if (uiCanvas != null)
            {
                bulletScript.canvasParent = uiCanvas;
            }
            else
            {
                // uiCanvas가 비어있으면 Scene에서 "Canvas" 이름으로 찾아 할당 시도 (안정장치)
                Transform found = GameObject.Find("Canvas")?.transform;
                if (found != null) bulletScript.canvasParent = found;
            }
        }
    }

    void LateUpdate()
    {
        // FirePoint 위치를 플레이어 바라보는 방향에 맞춰 좌우 오프셋 조정
        if (firePoint != null && sr != null)
        {
            float offset = 0.35f;
            float yPos = firePoint.localPosition.y;
            firePoint.localPosition = new Vector3(sr.flipX ? -offset : offset, yPos, 0f);
        }
    }
}
