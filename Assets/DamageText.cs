using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
	public float moveUpSpeed = 2f;  // 위로 뜨는 속도
	public float lifetime = 1f;     // 사라지기까지 시간
	private TextMeshProUGUI textMesh;

	void Awake()
	{
		textMesh = GetComponent<TextMeshProUGUI>();
	}

	public void Setup(int damage, bool isWeak)
	{
		textMesh.text = damage.ToString();

		// 약점일 경우 색상 강조
		if (isWeak)
			textMesh.color = Color.yellow;
		else
			textMesh.color = Color.red;

		Destroy(gameObject, lifetime);
	}

	void Update()
	{
		// 위로 이동
		transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
	}
}
