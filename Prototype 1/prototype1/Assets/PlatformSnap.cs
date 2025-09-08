using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlatformSnap : MonoBehaviour
{
    public string platformName = "Left";                 // Left / Right / Pool
    public ClipItem.Category acceptCategory = ClipItem.Category.A;
    public Transform anchor;                             // LeftAnchor / RightAnchor
    public float cell = 0.25f;                           // 网格尺寸
    public int cols = 6;                                 // 每行数量

    int nextIndex = 0;
    BoxCollider box;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;                            // 作为放置面
    }

    // 只按 XZ 判断是否在平台范围内（忽略很薄的Y厚度问题）
    public bool IsInside(Vector3 worldPos)
    {
        var b = box.bounds;
        return (worldPos.x >= b.min.x && worldPos.x <= b.max.x) &&
               (worldPos.z >= b.min.z && worldPos.z <= b.max.z);
    }

    // 按行列顺序吸附（左到右、再到下一行）
    public Vector3 GetSnapPosition()
    {
        if (!anchor) return transform.position + Vector3.up * 0.06f;
        int row = nextIndex / cols;
        int col = nextIndex % cols;
        nextIndex++;
        return anchor.position + new Vector3(col * cell, 0f, row * cell);
    }
}
