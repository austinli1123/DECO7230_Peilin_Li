using UnityEngine;

public class ClipItem : MonoBehaviour
{
    public enum Category { A, B }
    public Category expected = Category.A; // A组→左平台，B组→右平台
}
