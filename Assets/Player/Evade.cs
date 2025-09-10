using UnityEngine;
using UnityEngine.UI; // ← Slider 사용하려면 반드시 필요

public class Evade : MonoBehaviour
{
	public float dodgeSpeed = 12f;
	public float dodgeDuration = 0.18f;
	public float cooldown = 0.7f;

	private Rigidbody2D rb;
	private bool isDodging = false;
	private float dodgeTimer = 0f;
	private float cooldownTimer = 0f;
	private Vector2 dodgeVelocity = Vector2.zero;

	public bool IsDodging => isDodging;

	// UI 연동용
	public Slider cooldownSlider; // Inspector에 드래그해서 연결

	void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
	}

	void Update()
	{
		if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;

		// UI 갱신 (0 = 쿨다운 중, 1 = 사용 가능)
		if (cooldownSlider != null)
			cooldownSlider.value = 1f - (cooldownTimer / cooldown);

		if (isDodging)
		{
			dodgeTimer -= Time.deltaTime;
			if (dodgeTimer <= 0f) isDodging = false;
			return;
		}

		if (Input.GetKeyDown(KeyCode.C) && cooldownTimer <= 0f)
		{
			StartDodge();
		}
	}

	void FixedUpdate()
	{
		if (isDodging && rb != null)
		{
			rb.linearVelocity = dodgeVelocity;
		}
	}

	private void StartDodge()
	{
		float h = Input.GetAxisRaw("Horizontal");
		float v = Input.GetAxisRaw("Vertical");

		if (Mathf.Abs(v) > 0.5f)
			dodgeVelocity = (v > 0f ? Vector2.up : Vector2.down) * dodgeSpeed;
		else if (Mathf.Abs(h) > 0.5f)
			dodgeVelocity = new Vector2(Mathf.Sign(h), 0f) * dodgeSpeed;
		else
			dodgeVelocity = (transform.localScale.x >= 0f ? Vector2.right : Vector2.left) * dodgeSpeed;

		isDodging = true;
		dodgeTimer = dodgeDuration;
		cooldownTimer = cooldown;
	}
}
