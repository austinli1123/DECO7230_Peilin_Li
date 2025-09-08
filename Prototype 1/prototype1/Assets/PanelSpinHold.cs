using UnityEngine;

// �ҵ�����������壨Quad���ϣ�������������ס����һ����ͣ���ߣ����ָֻ���
[RequireComponent(typeof(Renderer))]
[RequireComponent(typeof(Collider))]  // Quad ��Ҫ�� Collider ������Ӧ OnMouseDown/Up
public class PanelSpinHold : MonoBehaviour
{
    [Header("Spin")]
    public float speedDegPerSec = 60f;     // ��ת�ٶȣ���/�룩
    public Vector3 axis = new Vector3(0, 1, 0); // ��ת�ᣬĬ���� Y ת

    [Header("Visual (optional)")]
    public Color playingColor = Color.white;
    public Color pausedColor = new Color(0.85f, 0.85f, 0.85f);

    static int holdCount = 0;              // ��ǰ������ס�������������>0 ��ȫ����ͣ��
    bool isHolding = false;                // ������Ƿ�������ס
    Renderer rend;

    static bool Paused => holdCount > 0;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        // ȷ���пɵ���� Collider����û�ӣ��Զ���һ�� BoxCollider��
        if (!TryGetComponent<Collider>(out _)) gameObject.AddComponent<BoxCollider>();
    }

    void Update()
    {
        // �ͷ��������Ҳ�ָܻ�����ֹ����ס��ͣ����
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

    void OnDisable()        // ���屻����/����ʱ�����ȫ����ͣ��ס
    {
        if (isHolding)
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }
    }

    void OnApplicationFocus(bool focus)
    {
        if (!focus && isHolding)   // �г�����ʱҲ�ָ�
        {
            isHolding = false;
            holdCount = Mathf.Max(0, holdCount - 1);
        }
    }
}
