using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    public BallType CurrentType;

    public bool Situated = false;
    public int Row;
    public int Col;

    public int TargetRaw;
    public int TargetCol;

    [SerializeField] private Sprite[] _sprites;
    [SerializeField] private GameObject[] _particlePrefabs;

    private Rigidbody2D _rigidBody;
    private Collider2D _collider;
    private SpriteRenderer _spriteRenderer;

    private Vector2 _startScale;

    private bool _isReleased;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _startScale = transform.localScale;
    }
    private void FixedUpdate()
    {
        if (!_isReleased)
            return;

        _rigidBody.velocity = Vector2.up * (100 + PlayerScore.Instance.Score) * Time.fixedDeltaTime;

        transform.position = new Vector2(Crosshair.Instance.GetCrosshairPosition().x, transform.position.y);
    }
    public void Initialize()
    {
        CurrentType = (BallType)Random.Range(0, 6);

        _spriteRenderer.sprite = _sprites[(int)CurrentType];
    }
    private void SpawnParticle()
    {
        var particle = Instantiate(_particlePrefabs[(int)CurrentType]).GetComponent<ParticleSystem>();

        particle.transform.position = new Vector2(transform.position.x, transform.position.y + 0.2f);
        particle.Play();

        Destroy(particle.gameObject, 2f);
    }
    public void Release()
    {
        _rigidBody.constraints = RigidbodyConstraints2D.None;
        _isReleased = true;
        _collider.isTrigger = false;
    }
    private void Situate(Vector2 position)
    {
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        _isReleased = false;
        Situated = true;
        BallManager.Instance.ReleaseNextBall();
        transform.DOMove(position, 0.3f).SetLink(gameObject).SetEase(Ease.OutBack);
        BallManager.Instance.AddBall(this, Row, Col);
        BallManager.Instance.CheckForMatches();
    }
    public void MoveToCurrentDigit(Vector2 position)
    {
        _spriteRenderer.DOFade(1f, 0.3f).SetLink(gameObject).SetDelay(0.4f);

        transform.DOScale(_startScale, 0.3f).SetLink(gameObject).SetEase(Ease.OutBack).SetDelay(0.4f);
        transform.DOMove(position, 0.6f).SetLink(gameObject);

        BallManager.Instance.CurrentBall = this;
        BallManager.Instance.SpawnNextBall();
    }
    public void MoveToNewPosition()
    {
        transform.DOMove(new Vector2(Crosshair.Instance.GetXPosition(Col), RowManager.Instance.GetYPositionRow(Row)), 0.3f).SetLink(gameObject).SetEase(Ease.OutBack);
    }
    public void OnBallDestroy()
    {
        _collider.isTrigger = true;
        tag = "Untagged";
        Destroy(_rigidBody);
        transform.DOScale(0, 0.4f).SetLink(gameObject).SetEase(Ease.OutBack);
        Destroy(gameObject, 0.6f);
    }
    public void OnNumberSpawn(bool nextNumber = false)
    {
        if (nextNumber)
        {
            _spriteRenderer.DOFade(0.4f, 0.3f).SetLink(gameObject);
            transform.DOScale(0.25f, 0.3f).SetLink(gameObject).SetEase(Ease.OutBack);
        }
        _rigidBody.freezeRotation = true;
        _rigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        _collider.isTrigger = true;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ball"))
        {
            var ball = collision.gameObject.GetComponent<Ball>();

            if (ball.Situated && _isReleased)
            {
                Situate(new Vector2(Crosshair.Instance.GetCrosshairPosition().x, RowManager.Instance.GetYPositionRow(Row)));
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Row"))
        { 
            var row = collision.GetComponent<Row>();

            Row = row.Number;
            if (Row == 7 && !Situated)
            {
                Situate(new Vector2(Crosshair.Instance.GetCrosshairPosition().x, RowManager.Instance.GetYPositionRow(Row)));
                return;
            }
            /*if (BallManager.Instance.IsBallHigher(Row, Col))
            {
                Situate(new Vector2(Crosshair.Instance.GetCrosshairPosition().x, RowManager.Instance.GetYPositionRow(Row)));
            }*/
        }
    }
}
