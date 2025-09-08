using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] // ClipBar 建议是个 Cube，自带 BoxCollider
public class ClipBarCut : MonoBehaviour
{
    [Header("References")]
    public Transform marker;          // 子物体：Marker
    public Transform leftSeg;         // 子物体：LeftSeg
    public Transform rightSeg;        // 子物体：RightSeg

    [Header("Interaction")]
    public KeyCode cutKey = KeyCode.C;
    public float planeY = 0.55f;      // 鼠标映射用的平面高度（与你条形所在高度一致）
    public float markerYOffset = 0.06f;

    [Header("Scale Limits (Length on X)")]
    public float minScaleX = 0.5f;
    public float maxScaleX = 3.0f;

    [Header("Events")]
    public UnityEvent<float> onCut;   // 触发切割时回调 (0..1)

    Camera cam;
    Plane dragPlane;

    // 当前长度（localScale.x）与半长
    float Width => transform.localScale.x;
    float Half => transform.localScale.x * 0.5f;

    void Start()
    {
        cam = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, planeY, 0));

        // 容错：若没手动拖引用，按名称自动找子物体
        if (!marker) marker = transform.Find("Marker");
        if (!leftSeg) leftSeg = transform.Find("LeftSeg");
        if (!rightSeg) rightSeg = transform.Find("RightSeg");

        ClampMarkerToBar();
        UpdateSegments();
    }

    void Update()
    {
        // 1) 滚轮缩放长度（X 方向）
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            var s = transform.localScale;
            s.x = Mathf.Clamp(s.x * (1f + scroll), minScaleX, maxScaleX);
            transform.localScale = s;
            ClampMarkerToBar();
            UpdateSegments();
        }

        // 2) 鼠标拖动 Marker：把鼠标在 planeY 平面上的世界坐标投影到本地坐标，再限制到 [-Half, Half]
        if (Input.GetMouseButton(0) && cam)
        {
            Ray r = cam.ScreenPointToRay(Input.mousePosition);
            if (dragPlane.Raycast(r, out float hit))
            {
                Vector3 world = r.GetPoint(hit);
                Vector3 local = transform.InverseTransformPoint(world);
                local.x = Mathf.Clamp(local.x, -Half, Half);
                local.y = markerYOffset;
                local.z = 0f;

                if (marker) marker.localPosition = local;
                UpdateSegments();
            }
        }

        // 3) 按键切割：输出 0..1 的比例，并通过 UnityEvent 通知
        if (Input.GetKeyDown(cutKey))
        {
            float ratio01 = Mathf.InverseLerp(-Half, Half, marker ? marker.localPosition.x : 0f);
            Debug.Log($"[Task2] Cut ratio = {ratio01:0.000}");
            onCut?.Invoke(ratio01); // 无监听也不会报错
        }
    }

    void ClampMarkerToBar()
    {
        if (!marker) return;
        Vector3 p = marker.localPosition;
        p.x = Mathf.Clamp(p.x, -Half, Half);
        p.y = markerYOffset;
        p.z = 0f;
        marker.localPosition = p;
    }

    void UpdateSegments()
    {
        if (!marker || !leftSeg || !rightSeg) return;

        float w = Width;
        float halfW = w * 0.5f;

        // 以条形中心为 0，marker 左侧宽度 & 右侧宽度
        float leftW = Mathf.Max(marker.localPosition.x + halfW, 0.0001f);
        float rightW = Mathf.Max(w - leftW, 0.0001f);

        // 子段的厚度、深度可按需改
        leftSeg.localScale = new Vector3(leftW, 0.04f, 0.10f);
        rightSeg.localScale = new Vector3(rightW, 0.04f, 0.10f);

        leftSeg.localPosition = new Vector3(-halfW + leftW * 0.5f, 0f, 0f);
        rightSeg.localPosition = new Vector3(+halfW - rightW * 0.5f, 0f, 0f);
    }

#if UNITY_EDITOR
    // 选中时画出条形范围与当前切点，方便对齐
    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.6f);
        Vector3 c = transform.position;
        Vector3 x = transform.right * Half;
        Gizmos.DrawLine(c - x, c + x);
        if (marker)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(marker.position + Vector3.forward * 0.1f, marker.position - Vector3.forward * 0.1f);
        }
    }
#endif
}
