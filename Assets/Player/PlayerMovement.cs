using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	[Header("이동 관련")]
	public float moveSpeed = 5f;          // 좌우 이동 속도

	[Header("점프 관련")]
	public float jumpPower = 3.5f;         // 점프할 때의 힘
	public int maxJumpCount = 2;          // 최대 점프 횟수 (2단 점프)
	public float jumpHoldTime = 0.2f;     // 점프 키를 길게 누를 때 점프 지속 시간

	private int currentJumpCount;         // 현재 남은 점프 횟수
	private bool isJumping;               // 점프 버튼을 누르고 있는 중인지
	private float jumpTimeCounter;        // 점프 키 홀드 시간 누적

	private float moveInput;              // ← → 키 입력값
	private Rigidbody2D rb;               // Rigidbody2D 캐시
	private SpriteRenderer sr;            // SpriteRenderer 캐시
	private bool isGrounded = false;
	private bool isCrouching = false;
	private bool isLookingUp = false;
	private bool isLookingDownAir = false;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		sr = GetComponent<SpriteRenderer>();
		currentJumpCount = maxJumpCount;
		// 앉기: ↓ 키를 누르고 있고, 바닥에 있을 때
		if (Input.GetKey(KeyCode.DownArrow) && isGrounded)
		{
			isCrouching = true;
		}
		else
		{
			isCrouching = false;
		}

		// 위 보기: ↑ 키를 누르고 있을 때
		isLookingUp = Input.GetKey(KeyCode.UpArrow);

		// 공중에서 아래 보기
		if (!isGrounded && Input.GetKey(KeyCode.DownArrow))
		{
			isLookingDownAir = true;
		}
		else
		{
			isLookingDownAir = false;
		}
	}

	void Update()
	{
		// ← → 키 입력 (왼쪽 -1, 아무것도 없음 0, 오른쪽 1)
		moveInput = Input.GetAxisRaw("Horizontal");

		// Flip 처리: 왼쪽일 때 뒤집기
		if (moveInput > 0)
			sr.flipX = false; // 오른쪽 보기
		else if (moveInput < 0)
			sr.flipX = true;  // 왼쪽 보기

		// 점프 시작 (X 키 기본 설정)
		if (Input.GetKeyDown(KeyCode.X) && currentJumpCount > 0)
		{
			isJumping = true;
			jumpTimeCounter = jumpHoldTime;
			rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
			currentJumpCount--; // 점프 횟수 감소
		}

		// 점프 키 홀드 중: 일정 시간 동안만 유지
		if (Input.GetKey(KeyCode.X) && isJumping)
		{
			if (jumpTimeCounter > 0)
			{
				rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
				jumpTimeCounter -= Time.deltaTime;
			}
			else
			{
				isJumping = false;
			}
		}

		// 점프 키 뗐을 때
		if (Input.GetKeyUp(KeyCode.X))
		{
			isJumping = false;
		}
	}

	void FixedUpdate()
	{
		// 좌우 이동
		rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		// 바닥에 닿았을 때 점프 횟수 초기화
		if (collision.contacts[0].normal.y > 0.7f) // 바닥에서 닿은 경우
		{
			currentJumpCount = maxJumpCount;
			isGrounded = true;  // 바닥과 닿았을 때
		}
	}
	void OnCollisionExit2D(Collision2D collision)
	{
		isGrounded = false;
	}
	void OnGUI()
	{
		GUI.Label(new Rect(10, 10, 300, 20), "isCrouching: " + isCrouching);
		GUI.Label(new Rect(10, 30, 300, 20), "isLookingUp: " + isLookingUp);
		GUI.Label(new Rect(10, 50, 300, 20), "isLookingDownAir: " + isLookingDownAir);
	}

}
