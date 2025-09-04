using UnityEngine;
using System.Collections;

public class Pistol : MonoBehaviour
{
	public GameObject bulletPrefab;
	public Transform firePoint;
	public float burstDelay = 0.15f;
	public int burstCount = 4;

	private bool isFiring = false;
	private SpriteRenderer sr;   // <-- 이거 추가

	void Start()
	{
		sr = GetComponent<SpriteRenderer>();   // <-- 이것도 추가
	}

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
			yield return new WaitForSeconds(burstDelay);
		}

		isFiring = false;
	}

	void FireOneBullet()
	{
		GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
		bulletObj.transform.localScale = new Vector3(0.2f, 0.2f, 1f);

		bool isFlipped = sr.flipX;
		int dir = isFlipped ? -1 : 1;

		Rigidbody2D rb = bulletObj.GetComponent<Rigidbody2D>();
		rb.linearVelocity = new Vector2(dir * 15f, 0f);
	}

	void LateUpdate()
	{
		// FirePoint 위치를 총구 위치 좌/우로 수정
		if (firePoint != null)
		{
			float offset = 0.35f;
			float yPos = firePoint.localPosition.y;
			firePoint.localPosition = new Vector3(sr.flipX ? -offset : offset, yPos, 0f);
		}
	}
}
