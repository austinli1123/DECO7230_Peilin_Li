using UnityEngine;

// ����ʱ������ƣ��Ҽ�+��꿴��WASD/QE �ƶ���Shift ���٣���ʹ�ù���
public class SceneStyleFlyCam : MonoBehaviour
{
    [Header("Look")]
    public float mouseSensitivity = 4f;         // �ӽ������ȣ�Խ��Խ�죩
    public float minPitch = -89f, maxPitch = 89f;

    [Header("Move")]
    public float moveSpeed = 3f;                // �����ٶȣ���/�룩
    public float sprintMultiplier = 3f;         // ��ס Shift �ļ��ٱ���

    float yaw, pitch;
    bool looking = false;                       // �Ҽ���סʱΪ true

    void Start()
    {
        var e = transform.rotation.eulerAngles;
        yaw = e.y; pitch = e.x;
    }

    void Update()
    {
        // �Ҽ�����/̧������/�ͷ����
        if (Input.GetMouseButtonDown(1)) { looking = true; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        if (Input.GetMouseButtonUp(1)) { looking = false; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

        // ֻ���Ҽ���סʱ��Ӧ�ӽ����ƶ����� Scene ��ͼһ�£�
        if (looking)
        {
            // �ӽ�
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * mouseSensitivity;
            pitch -= my * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            // �ƶ�
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
            if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
            if (Input.GetKey(KeyCode.E)) dir += Vector3.up;      // ��
            if (Input.GetKey(KeyCode.Q)) dir += Vector3.down;    // ��

            if (dir.sqrMagnitude > 1f) dir.Normalize();
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? sprintMultiplier : 1f);
            Vector3 world = transform.TransformDirection(new Vector3(dir.x, 0f, dir.z)) + Vector3.up * dir.y;
            transform.position += world * speed * Time.deltaTime;
        }
    }

    void OnDisable()
    {
        // ȷ������ʱ�ͷ����
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        looking = false;
    }
}
