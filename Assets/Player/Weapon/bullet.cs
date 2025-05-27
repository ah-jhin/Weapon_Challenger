using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float lifetime = 0.8f; // 자동 삭제 시간
	public float baseDamage = 1f; // 기본 피해 (1~3 랜덤)

	void Start()
	{
		Destroy(gameObject, lifetime); // 일정 시간 후 자동 삭제
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// 적 또는 약점 태그 감지
		if (other.CompareTag("Enemy") || other.CompareTag("WeakPoint"))
		{
			float damage = Random.Range(1f, 4f); // 1~3 랜덤 피해

			// 약점이면 +50%
			if (other.CompareTag("WeakPoint"))
			{
				damage *= 1.5f;
			}

			// 데미지 전달
			other.SendMessage("TakeDamage", damage, SendMessageOptions.DontRequireReceiver);

			Destroy(gameObject); // 총알 제거
		}

		// 벽에 부딪혀도 제거 가능 (선택)
		if (other.CompareTag("Wall"))
		{
			Destroy(gameObject);
		}
	}
}
