using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PlatformSnap : MonoBehaviour
{
    public string platformName = "Left";                 // Left / Right / Pool
    public ClipItem.Category acceptCategory = ClipItem.Category.A;
    public Transform anchor;                             // LeftAnchor / RightAnchor
    public float cell = 0.25f;                           // ����ߴ�
    public int cols = 6;                                 // ÿ������

    int nextIndex = 0;
    BoxCollider box;

    void Awake()
    {
        box = GetComponent<BoxCollider>();
        box.isTrigger = true;                            // ��Ϊ������
    }

    // ֻ�� XZ �ж��Ƿ���ƽ̨��Χ�ڣ����Ժܱ���Y������⣩
    public bool IsInside(Vector3 worldPos)
    {
        var b = box.bounds;
        return (worldPos.x >= b.min.x && worldPos.x <= b.max.x) &&
               (worldPos.z >= b.min.z && worldPos.z <= b.max.z);
    }

    // ������˳�����������ҡ��ٵ���һ�У�
    public Vector3 GetSnapPosition()
    {
        if (!anchor) return transform.position + Vector3.up * 0.06f;
        int row = nextIndex / cols;
        int col = nextIndex % cols;
        nextIndex++;
        return anchor.position + new Vector3(col * cell, 0f, row * cell);
    }
}
