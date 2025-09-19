using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveUpSpeed = 4.5f;  // 위로 뜨는 속도
    public float lifetime = 0.5f;     // 사라지기까지 시간
    private TextMeshProUGUI textMesh;

    void Awake()
    {
        // 자식 포함해서 TMP Text 찾아오기
        textMesh = GetComponentInChildren<TextMeshProUGUI>();

        if (textMesh == null)
            Debug.LogError("[DamageText] TextMeshProUGUI 컴포넌트를 찾지 못했습니다.");
    }

    public void Setup(int damage, bool isWeak)
    {
        if (textMesh == null) return;

		if(isWeak)
			textMesh.color = Color.yellow; // 약점
		else
			textMesh.color = Color.white;  // 일반

        Destroy(gameObject, lifetime);
    }

    void Update()
    {
		// 위로이동
        transform.position += Vector3.up * moveUpSpeed * Time.deltaTime;
    }
}
