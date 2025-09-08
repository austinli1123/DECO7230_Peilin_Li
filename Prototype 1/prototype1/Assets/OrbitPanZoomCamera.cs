using UnityEngine;

// 运行时相机控制：右键+鼠标看向，WASD/QE 移动，Shift 加速；不使用滚轮
public class SceneStyleFlyCam : MonoBehaviour
{
    [Header("Look")]
    public float mouseSensitivity = 4f;         // 视角灵敏度（越大越快）
    public float minPitch = -89f, maxPitch = 89f;

    [Header("Move")]
    public float moveSpeed = 3f;                // 基础速度（米/秒）
    public float sprintMultiplier = 3f;         // 按住 Shift 的加速倍率

    float yaw, pitch;
    bool looking = false;                       // 右键按住时为 true

    void Start()
    {
        var e = transform.rotation.eulerAngles;
        yaw = e.y; pitch = e.x;
    }

    void Update()
    {
        // 右键按下/抬起：锁定/释放鼠标
        if (Input.GetMouseButtonDown(1)) { looking = true; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        if (Input.GetMouseButtonUp(1)) { looking = false; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }

        // 只在右键按住时响应视角与移动（和 Scene 视图一致）
        if (looking)
        {
            // 视角
            float mx = Input.GetAxis("Mouse X");
            float my = Input.GetAxis("Mouse Y");
            yaw += mx * mouseSensitivity;
            pitch -= my * mouseSensitivity;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);

            // 移动
            Vector3 dir = Vector3.zero;
            if (Input.GetKey(KeyCode.W)) dir += Vector3.forward;
            if (Input.GetKey(KeyCode.S)) dir += Vector3.back;
            if (Input.GetKey(KeyCode.A)) dir += Vector3.left;
            if (Input.GetKey(KeyCode.D)) dir += Vector3.right;
            if (Input.GetKey(KeyCode.E)) dir += Vector3.up;      // 上
            if (Input.GetKey(KeyCode.Q)) dir += Vector3.down;    // 下

            if (dir.sqrMagnitude > 1f) dir.Normalize();
            float speed = moveSpeed * (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) ? sprintMultiplier : 1f);
            Vector3 world = transform.TransformDirection(new Vector3(dir.x, 0f, dir.z)) + Vector3.up * dir.y;
            transform.position += world * speed * Time.deltaTime;
        }
    }

    void OnDisable()
    {
        // 确保禁用时释放鼠标
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        looking = false;
    }
}
