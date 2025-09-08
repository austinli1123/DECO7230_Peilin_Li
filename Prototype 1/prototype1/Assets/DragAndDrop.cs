using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DragAndDrop : MonoBehaviour
{
    Camera cam;
    Plane dragPlane;
    Vector3 offset;
    bool dragging;

    ClipItem clip;

    void Start()
    {
        cam = Camera.main;
        clip = GetComponent<ClipItem>();
        // 与平台同高的拖拽平面（平台中心 y=0.5）
        dragPlane = new Plane(Vector3.up, new Vector3(0f, 0.5f, 0f));
    }

    void OnMouseDown()
    {
        if (cam == null) return;
        Ray r = cam.ScreenPointToRay(Input.mousePosition); // 注意 P 大写
        if (dragPlane.Raycast(r, out float hit))
        {
            offset = transform.position - r.GetPoint(hit);
            dragging = true;
        }
    }

    void OnMouseDrag()
    {
        if (!dragging || cam == null) return;
        Ray r = cam.ScreenPointToRay(Input.mousePosition);
        if (dragPlane.Raycast(r, out float hit))
        {
            Vector3 p = r.GetPoint(hit) + offset;
            // 锁定高度到平台表面上方一点
            transform.position = new Vector3(p.x, 0.55f, p.z);
        }
    }

    void OnMouseUp()
    {
        if (!dragging) return;
        dragging = false;

        // 在当前位置附近找平台（包含触发器）
        Collider[] hits = Physics.OverlapSphere(
            transform.position, 0.25f, ~0, QueryTriggerInteraction.Collide);

        PlatformSnap target = null;
        foreach (var h in hits)
        {
            var ps = h.GetComponent<PlatformSnap>();
            if (ps != null && ps.IsInside(transform.position)) { target = ps; break; }
        }

        if (target == null)
        {
            ReturnToPool();
            LogResult(false);
            return;
        }

        bool isPool = target.platformName == "Pool";
        // Pool 不校验；Left/Right 校验类别
        if (!isPool && clip != null && target.acceptCategory != clip.expected)
        {
            ReturnToPool();
            LogResult(false);
        }
        else
        {
            Vector3 snap = target.GetSnapPosition();
            transform.position = snap;
            LogResult(!isPool); // 落在 Pool 不算“正确”
        }
    }

    void ReturnToPool()
    {
        // 回到素材台区域（带随机避免重叠）
        transform.position = new Vector3(0f, 0.55f, -1.2f) +
                             new Vector3(Random.Range(-0.6f, 0.6f), 0f, Random.Range(-0.3f, 0.3f));
    }

    void LogResult(bool correct)
    {
        Debug.Log($"[Task1] {(correct ? "Correct" : "Wrong")}");
    }
}
