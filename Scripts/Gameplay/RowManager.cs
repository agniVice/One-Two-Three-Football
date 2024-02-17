using Unity.VisualScripting;
using UnityEngine;

public class RowManager : MonoBehaviour
{
    public static RowManager Instance;
    [SerializeField] private Transform[] _rows;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    public float GetYPositionRow(int row)
    {
        return _rows[row].position.y;
    }
}
