using UnityEngine;

// 挂到左右两个面板（Quad）上：都会自旋；按住任意一个暂停两者，松手恢复。
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]  // Quad 需要有 Collider 才能响应 OnMouseDown/Up
public class PanelSpinHold : MonoBehaviour
{
    [Header("Spin")]
    public float speedDegPerSec = 60f;     // 旋转速度（度/秒）
    public Vector3 axis = new Vector3(0, 1, 0); // 旋转轴，默认绕 Y 转

    [Header("Visual (optional)")]
    public Color playingColor = Color.white;
    public Color pausedColor = new Color(0.85f, 0.85f, 0.85f);

    static int holdCount = 0;              // 当前被“按住”的面板数量（>0 即全局暂停）
    bool isHolding = false;                // 本面板是否正被按住
    Renderer rend;

    static bool Paused => holdCount > 0;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        // 确保有可点击的 Collider（如没加，自动补一个 BoxCollider）
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    void Update()
    {
        // 释放在面板外也能恢复（防止“卡住暂停”）
        if (isHolding && !Input.GetMouseButton(0))
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }

        if (!Paused)
        {
            transform.Rotate(axis * (speedDegPerSec * Time.deltaTime), Space.Self);
        }

        if (rend) rend.material.color = Paused ? pausedColor : playingColor;
    }

    void OnMouseDown()
    {
        if (!isHolding)
        {
            isHolding = true;
            holdCount++;
        }
    }

    void OnMouseUp()
    {
        if (isHolding)
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }
    }

    void OnDisable()        // 物体被禁用/销毁时，别把全局暂停留住
    {
        if (isHolding)
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus && isHolding)   // 切出窗口时也恢复
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }
    }
}
