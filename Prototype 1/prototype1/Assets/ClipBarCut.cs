using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))] // ClipBar �����Ǹ� Cube���Դ� BoxCollider
public class ClipBarCut : MonoBehaviour
{
    [Header("References")]
    public Transform marker;          // �����壺Marker
    public Transform leftSeg;         // �����壺LeftSeg
    public Transform rightSeg;        // �����壺RightSeg

    [Header("Interaction")]
    public KeyCode cutKey = KeyCode.C;
    public float planeY = 0.55f;      // ���ӳ���õ�ƽ��߶ȣ������������ڸ߶�һ�£�
    public float markerYOffset = 0.06f;

    [Header("Scale Limits (Length on X)")]
    public float minScaleX = 0.5f;
    public float maxScaleX = 3.0f;

    [Header("Events")]
    public UnityEvent<float> onCut;   // �����и�ʱ�ص� (0..1)

    Camera cam;
    Plane dragPlane;

    // ��ǰ���ȣ�localScale.x����볤
    float Width => transform.localScale.x;
    float Half => transform.localScale.x * 0.5f;

    void Start()
    {
        cam = Camera.main;
        dragPlane = new Plane(Vector3.up, new Vector3(0, planeY, 0));

        // �ݴ���û�ֶ������ã��������Զ���������
        if (!marker) marker = transform.Find("Marker");
        if (!leftSeg) leftSeg = transform.Find("LeftSeg");
        if (!rightSeg) rightSeg = transform.Find("RightSeg");

        ClampMarkerToBar();
        UpdateSegments();
    }

    void Update()
    {
        // 1) �������ų��ȣ�X ����
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.0001f)
        {
            var s = transform.localScale;
            s.x = Mathf.Clamp(s.x * (1f + scroll), minScaleX, maxScaleX);
            transform.localScale = s;
            ClampMarkerToBar();
            UpdateSegments();
        }

        // 2) ����϶� Marker��������� planeY ƽ���ϵ���������ͶӰ���������꣬�����Ƶ� [-Half, Half]
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

        // 3) �����и��� 0..1 �ı�������ͨ�� UnityEvent ֪ͨ
        if (Input.GetKeyDown(cutKey))
        {
            float ratio01 = Mathf.InverseLerp(-Half, Half, marker ? marker.localPosition.x : 0f);
            Debug.Log($"[Task2] Cut ratio = {ratio01:0.000}");
            onCut?.Invoke(ratio01); // �޼���Ҳ���ᱨ��
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

        // ����������Ϊ 0��marker ����� & �Ҳ���
        float leftW = Mathf.Max(marker.localPosition.x + halfW, 0.0001f);
        float rightW = Mathf.Max(w - leftW, 0.0001f);

        // �Ӷεĺ�ȡ���ȿɰ����
        leftSeg.localScale = new Vector3(leftW, 0.04f, 0.10f);
        rightSeg.localScale = new Vector3(rightW, 0.04f, 0.10f);

        leftSeg.localPosition = new Vector3(-halfW + leftW * 0.5f, 0f, 0f);
        rightSeg.localPosition = new Vector3(+halfW - rightW * 0.5f, 0f, 0f);
    }

#if UNITY_EDITOR
    // ѡ��ʱ�������η�Χ�뵱ǰ�е㣬�������
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
