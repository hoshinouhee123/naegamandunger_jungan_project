using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] ItemSO data;     // Inspector 드래그

    public int GetPoint()
    {
        return data.point; // ItemSO의 point 값 반환
    }
}