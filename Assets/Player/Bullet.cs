using System.Xml;
using UnityEngine;
using TMPro; // TextMeshProUGUI 쓰려면 필요
using DamageText; // DamageText가 속한 네임스페이스

public class Bullet : MonoBehaviour
{
	public float lifetime = 0.7f;            // 총알이 살아있는 시간 (초)
	public int minDamage = 1;                // 최소 데미지
	public int maxDamage = 3;                // 최대 데미지
	public GameObject damageTextPrefab;      // 데미지 텍스트 프리팹 (Inspector에서 연결)
	public Transform canvasParent;

	void Start()
	{
		// lifetime이 지나면 자동 삭제
		Destroy(gameObject, lifetime);


	}

	void OnTriggerEnter2D(Collider2D other)
	{

		if (canvasParent != null)
		{
			dmgText.transform.SetParent(canvasParent, false);
		}
		// 약점 여부 판별
		bool isWeak = false;

		if (other.CompareTag("WeakPoint"))
		{
			isWeak = true;
			// 약점은 자식 Collider일 수 있으므로, 부모 Boss 오브젝트에서 Collider 가져오기
			other = other.transform.parent.GetComponent<Collider2D>();
		}

		// Boss 스크립트 가져오기
		Boss boss = other.GetComponent<Boss>();

		if (boss != null)
		{
			// 랜덤 데미지 계산
			int damage = Random.Range(minDamage, maxDamage + 1);

			// 보스에게 데미지 전달
			boss.TakeDamage(damage, isWeak);

			// 데미지 텍스트 표시
			if (damageTextPrefab != null)
			{
				// 보스 머리 위 좌표 (살짝 위로 띄움)
				Vector3 spawnPos = other.transform.position + Vector3.up * 1.5f;

				// 프리팹 생성
				GameObject dmgText = Instantiate(damageTextPrefab, spawnPos, Quaternion.identity);

				// Canvas 하위에 배치 (UI로 보이게 함)
				Transform canvas = GameObject.Find("Canvas")?.transform;
				if (canvas != null)
				{
					dmgText.transform.SetParent(canvas, false);
				}

				// DamageText 스크립트 가져와서 세팅
				DamageText dt = dmgText.GetComponent<DamageText>();
				if (dt != null)
				{
					dt.Setup(damage, isWeak);
				}
				else
				{
					Debug.LogWarning("DamageText 스크립트가 프리팹에 붙어 있지 않습니다.");
				}
			}
			else
			{
				Debug.LogWarning("DamageTextPrefab이 연결되지 않았습니다.");
			}

			// 총알은 1회 타격 후 삭제
			Destroy(gameObject);
		}
	}
}
