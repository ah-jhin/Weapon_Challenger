using UnityEngine;

public class Bullet : MonoBehaviour
{
	public float lifetime = 0.7f;  // 시간이 지나면 삭제
	public int minDamage = 1;
	public int maxDamage = 3;
	public GameObject damageTextPrefab;  // Inspector에 넣기

	void Start()
	{
		Destroy(gameObject, lifetime);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		// 약점 쪽인지 먼저 판별
		bool isWeak = false;

		if (other.CompareTag("WeakPoint"))
		{
			isWeak = true;
			// 약점의 부모 객체가 Boss.cs 를 가지게 만들어두면 Parent에서 Boss를 찾는다
			other = other.transform.parent.GetComponent<Collider2D>();
		}

		Boss boss = other.GetComponent<Boss>();
		if (boss != null)
		{
			int damage = Random.Range(minDamage, maxDamage + 1);
			boss.TakeDamage(damage, isWeak);
			Destroy(gameObject); // 총알은 1회 타격 후 제거
		}

		if (boss != null)
		{
			int damage = Random.Range(minDamage, maxDamage + 1);
			boss.TakeDamage(damage, isWeak);

			// 🔽 데미지 텍스트 표시
			if (damageTextPrefab != null)
			{
				// 보스 위치에서 살짝 위로
				Vector3 spawnPos = other.transform.position + Vector3.up * 1.5f;
				GameObject dmgText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

				dmgText.transform.SetParent(GameObject.Find("Canvas").transform, false);
				dmgText.GetComponent<DamageText>().Setup(damage, isWeak);
			}
			Destroy(gameObject); // 총알 제거
		}
	}

}
