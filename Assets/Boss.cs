using UnityEngine;
using UnityEngine.UI;   // Slider 사용을 위해 필요

public class Boss : MonoBehaviour
{
	[Header("Boss Stats")]
	public int maxHP = 100;
	private int currentHP;

	[Header("UI")]
	public Slider hpSlider;   // Inspector에서 Slider 연결

	void Start()
	{
		// HP 초기화
		currentHP = maxHP;

		if (hpSlider != null)
		{
			hpSlider.maxValue = 1; // Slider는 0~1로 쓰는게 직관적
			hpSlider.value = 1;    // 시작은 가득 찬 상태
		}
	}

	// 데미지 받는 함수
	public void TakeDamage(int damage, bool isWeak)
	{
		// 약점 공격이면 1.5배
		if (isWeak)
			damage = Mathf.RoundToInt(damage * 1.5f);

		// 체력 감소
		currentHP -= damage;
		currentHP = Mathf.Clamp(currentHP, 0, maxHP);

		// HP바 업데이트
		if (hpSlider != null)
		{
			hpSlider.value = (float)currentHP / maxHP;
		}

		// 보스 사망 처리
		if (currentHP <= 0)
		{
			Die();
		}
	}

	private void Die()
	{
		// 추후 연출 추가 가능
		Debug.Log("Boss Defeated!");
		Destroy(gameObject);
	}
}
