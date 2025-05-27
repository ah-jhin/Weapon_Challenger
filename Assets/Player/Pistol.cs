using System.Collections;
using UnityEngine;

public class Pistol : MonoBehaviour
{
	[Header("총알 프리팹과 총구 위치")]
	public GameObject bulletPrefab; // Inspector에서 연결
	public Transform firePoint;     // 총알 생성 위치 (FirePoint)

	[Header("발사 설정")]
	public float bulletSpeed = 12f;
	public float fireDelay = 0.15f;     // 발사 간격
	public int burstCount = 4;          // 4점사
	public float spreadAngle = 5f;      // 탄퍼짐 각도 (±도)

	private bool isFiring = false;

	void Update()
	{
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
			yield return new WaitForSeconds(fireDelay);
		}

		isFiring = false;
	}

	void FireOneBullet()
	{
		// 탄퍼짐 각도 적용
		float spread = Random.Range(-spreadAngle, spreadAngle);
		Quaternion rotation = Quaternion.Euler(0, 0, transform.localScale.x > 0 ? spread : -spread);

		// 총알 생성
		GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation * rotation);
		Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

		// 총알 속도 적용 (좌/우 방향 자동 적용)
		Vector2 direction = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
		rb.linearVelocity = direction * bulletSpeed;
	}
}
