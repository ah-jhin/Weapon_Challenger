using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("이동 및 점프")]
	public float moveSpeed = 5f;
	public float jumpForce = 10f;
	public float highJumpForce = 16f;
	public float jumpTime = 0.3f;
	public float jumpBufferTime = 0.15f;
	public float fallMultiplier = 0.5f;

	[Header("회피")]
	public float dodgeForce = 14f;           // ← 여기 수치를 키우면 더 멀리 도약함 (기본 12~15 추천)
	public float dodgeCooldown = 1.2f;       // 쿨타임
	private float lastDodgeTime = -999f;     // 마지막 회피 시간
	private bool isDodging = false;          // 회피 중 여부

	[Header("상태")]
	public bool isGrounded = false;
	public bool isInWater = false;

	private bool isJumping = false;
	private bool hasAirJumped = false;
	private bool hasExtraJump = false;
	private bool useHighJump = false;

	private float moveInput;
	private float jumpBufferCounter;
	private float jumpTimeCounter;

	private Rigidbody2D rb;
	private SpriteRenderer sr;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
	}

	void Update()
	{
		// ▶ 이동 입력
		moveInput = Input.GetAxisRaw("Horizontal");

		// ▶ Flip 처리 (좌우 방향)
		if (moveInput > 0) sr.flipX = false;
		else if (moveInput < 0) sr.flipX = true;

		// ▶ 점프 입력 버퍼
		if (Input.GetKeyDown(KeyCode.X))
			jumpBufferCounter = jumpBufferTime;
		else
			jumpBufferCounter -= Time.deltaTime;

		// ▶ 점프 조건 체크
		if (jumpBufferCounter > 0)
		{
			if (isGrounded || !hasAirJumped || hasExtraJump || isInWater)
			{
				float force = useHighJump ? highJumpForce : jumpForce;
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, force);
				isJumping = true;
				jumpTimeCounter = jumpTime;
				jumpBufferCounter = 0;

				if (!isGrounded && !isInWater)
				{
					if (hasExtraJump)
					{
						hasExtraJump = false;
						useHighJump = false;
					}
					hasAirJumped = true;
				}
			}
		}

		// ▶ 점프 높이 조절
		if (Input.GetKey(KeyCode.X) && isJumping && jumpTimeCounter > 0)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, useHighJump ? highJumpForce : jumpForce);
			jumpTimeCounter -= Time.deltaTime;
		}

		if (Input.GetKeyUp(KeyCode.X))
		{
			isJumping = false;
		}

		// ▶ 회피 키 입력 (C 키 + 쿨타임 체크)
		if (Input.GetKeyDown(KeyCode.C) && Time.time - lastDodgeTime >= dodgeCooldown)
		{
			PerformDodge();
		}
	}

	void FixedUpdate()
	{
		// ▶ 회피 중이 아닐 때만 이동 처리
		if (!isDodging)
		{
			rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
		}

		// ▶ 물 속 낙하 감속
		if (isInWater && rb.linearVelocity.y < 0)
		{
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * fallMultiplier);
		}
	}

	void PerformDodge()
	{
		// ▶ 방향 판단: 입력 중이면 그 방향, 없으면 바라보는 방향
		float direction = moveInput != 0 ? Mathf.Sign(moveInput) : (sr.flipX ? -1 : 1);

		// ▶ 회피 도약 실행
		rb.linearVelocity = new Vector2(direction * dodgeForce, rb.linearVelocity.y);
		lastDodgeTime = Time.time;

		isDodging = true;
		Invoke(nameof(EndDodge), 0.1f); // 0.1초 후 회피 종료
	}

	void EndDodge()
	{
		isDodging = false;
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.contacts[0].normal.y > 0.7f)
		{
			isGrounded = true;
			hasAirJumped = false;
			isJumping = false;
		}
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		isGrounded = false;
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Water"))
		{
			isInWater = true;
		}

		if (other.CompareTag("BlueJumpOrb"))
		{
			hasExtraJump = true;
			useHighJump = false;
			other.gameObject.SetActive(false);
			Invoke(nameof(ReactivateOrb), 3f);
		}

		if (other.CompareTag("RedJumpOrb"))
		{
			hasExtraJump = true;
			useHighJump = true;
			other.gameObject.SetActive(false);
			Invoke(nameof(ReactivateOrb), 3f);
		}
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Water"))
		{
			isInWater = false;
		}
	}

	void ReactivateOrb()
	{
		// 나중에 OrbManager로 변경 가능
	}
}
