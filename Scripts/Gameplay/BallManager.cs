using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class BallManager : MonoBehaviour
{
    public static BallManager Instance;

    public Ball[,] Balls = new Ball[8, 5];

    public Ball NextBall;
    public Ball CurrentBall;

    [SerializeField] private GameObject _ballPrefab;
    [SerializeField] private Transform _positionNextNumber;
    [SerializeField] private Transform _positionCurrentNumber;

    private List<Ball> matchedBalls = new List<Ball>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }
    private void Start()
    {
        SpawnNextBall();
        ReleaseNextBall();
    }
    public void SpawnNextBall()
    {
        Debug.Log("spawned");
        NextBall = Instantiate(_ballPrefab, _positionNextNumber.position, Quaternion.identity).GetComponent<Ball>();
        NextBall.Initialize();
        AudioVibrationManager.Instance.PlaySound(AudioVibrationManager.Instance.PopUp, Random.Range(0.85f, 1.1f));
        NextBall.OnNumberSpawn(true);

        if (CurrentBall == null)
        {
            NextBall.MoveToCurrentDigit(_positionCurrentNumber.position);
        }
    }
    public void ReleaseNextBall()
    {
        if (GameState.Instance.CurrentState != GameState.State.InGame)
          return;

        CurrentBall.Release();

        Crosshair.Instance.CurrentBall = CurrentBall;
        Crosshair.Instance.SetColumnNumber(2);

        NextBall.MoveToCurrentDigit(_positionCurrentNumber.position);
    }
    private void DeleteBall(Ball ball, int row, int col)
    {
        Balls[row, col] = null;
    }
    public void AddBall(Ball ball, int row, int col)
    {
        if (row == 0 && col == 2 && Balls[row, col] != null)
        {
            GameState.Instance.FinishGame();
            Destroy(ball.gameObject);
            return;
        }
        else
            Balls[row, col] = ball;
    }
    public void CheckForMatches()
    {
        for (int row = 0; row < Balls.GetLength(0); row++)
        {
            for (int col = 0; col < Balls.GetLength(1); col++)
            {
                if (Balls[row, col] != null)
                {
                    BallType currentType = Balls[row, col].CurrentType; // “екущий тип шарика

                    // ѕроверка горизонтальных троек, четверок и п€терок
                    for (int length = 3; length <= 5; length++)
                    {
                        if (col <= Balls.GetLength(1) - length)
                        {
                            bool match = true;
                            for (int i = 0; i < length; i++)
                            {
                                if (Balls[row, col + i] == null || Balls[row, col + i].CurrentType != currentType)
                                {
                                    match = false;
                                    break;
                                }
                            }

                            if (match)
                            {
                                //Debug.Log($"{length} элементов в горизонтали найдены в строке {row} начина€ с колонки {col}");
                                for (int i = 0; i < length; i++)
                                {
                                    matchedBalls.Add(Balls[row, col + i]);
                                }
                            }
                        }
                    }

                    // ѕроверка вертикальных троек, четверок и п€терок
                    for (int length = 3; length <= 5; length++)
                    {
                        if (row <= Balls.GetLength(0) - length)
                        {
                            bool match = true;
                            for (int i = 0; i < length; i++)
                            {
                                if (Balls[row + i, col] == null || Balls[row + i, col].CurrentType != currentType)
                                {
                                    match = false;
                                    break;
                                }
                            }

                            if (match)
                            {
                                //Debug.Log($"{length} элементов в вертикали найдены в строке {row} и колонке {col}");
                                for (int i = 0; i < length; i++)
                                {
                                    matchedBalls.Add(Balls[row + i, col]);
                                }
                            }
                        }
                    }
                }
            }
        }

        if (matchedBalls.Count >= 3)
        {
            PlayerScore.Instance.AddScore(matchedBalls.Count);
            foreach (Ball ball in matchedBalls)
            {
                DeleteBall(ball, ball.Row, ball.Col);
                ball.OnBallDestroy();
            }
            matchedBalls.Clear();

            // —двиг м€чей сверху вниз
            ShiftBallsUp();
        }
    }

    private void ShiftBallsUp()
    {
        for (int row = Balls.GetLength(0) - 1; row >= 0; row--)
        {
            for (int col = 0; col < Balls.GetLength(1); col++)
            {
                if (Balls[row, col] == null && row != 0)
                {
                    if (Balls[row - 1, col] != null)
                    {
                        AddBall(Balls[row - 1, col], row, col);
                        Balls[row - 1, col] = null;
                        Balls[row, col].Row = row;
                        Balls[row, col].MoveToNewPosition();
                    }
                }
            }
        }
        CheckForMatches();
    }
    public bool CanIChangeColumn(int targetCol, int row)
    {
        if (Balls[row, targetCol] == null)
            return true;
        else
            return false;
    }
    public bool IsBallHigher(int row, int col)
    {
        if (row + 1 >=  Balls.GetLength(0))
        {
            return true;
        }
        if (Balls[row+1, col] != null)
            return true;
        else
            return false;
    }
}
