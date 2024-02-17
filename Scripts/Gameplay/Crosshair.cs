using UnityEngine;

public class Crosshair : MonoBehaviour
{
    public static Crosshair Instance;

    public Ball CurrentBall;
    public int CurrentColumn = 2;
    public int TargetColumn = 2;

    [SerializeField] private Transform _crosshair;

    private int _lastCol = 2;
    private float[] _possiblePositions = { -1.73f, -0.86f, 0f, 0.86f, 1.73f };

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
          return;

        UpdateTargetPosition();
        UpdateCurrentPosition();
        UpdateCrosshairPosition();
    }
    private void UpdateTargetPosition()
    {
        if (CurrentBall == null)
            return;
        float x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        float closestPosition = _possiblePositions[0];
        float closestDistance = Mathf.Abs(x - _possiblePositions[0]);
        TargetColumn = 0;

        for (int i = 1; i < _possiblePositions.Length; i++)
        {
            float distance = Mathf.Abs(x - _possiblePositions[i]);

            if (distance < closestDistance)
            {
                closestPosition = _possiblePositions[i];
                TargetColumn = i;
                closestDistance = distance;
            }
        }
    }
    private void UpdateCurrentPosition()
    {
        if (CurrentBall == null)
            return;
        if (BallManager.Instance.CanIChangeColumn(TargetColumn, CurrentBall.Row))
            CurrentColumn = TargetColumn;

        CurrentBall.Col = CurrentColumn;
    }
    private void UpdateCrosshairPosition()
    {
        _crosshair.transform.position = new Vector2(_possiblePositions[CurrentColumn], _crosshair.transform.position.y);
    }
    public void SetColumnNumber(int number)
    { 
        CurrentColumn = number;
        TargetColumn = number;

        UpdateCurrentPosition();
        UpdateCrosshairPosition();
    }
    public Vector2 GetCrosshairPosition()
    { 
        return _crosshair.transform.position;
    }
    public float GetXPosition(int col)
    {
        return _possiblePositions[col];
    }
}
