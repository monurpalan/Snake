using System.Collections.Generic;
using UnityEngine;

public class Snake : MonoBehaviour
{
    [Header("Snake Settings")]
    [SerializeField] private Transform segmentPrefab;
    [SerializeField] private int initialSize = 4;
    [SerializeField] private float speed = 20f;
    [SerializeField] private float speedMultiplier = 1.1f;
    [SerializeField] private bool moveThroughWalls = false;

    private readonly List<Transform> segments = new List<Transform>();
    private Vector2Int direction = Vector2Int.right;
    private Vector2Int input = Vector2Int.right;
    private float nextMoveTime = 0f;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        // Yılanın hareket etme zamanı gelmediyse çık
        if (Time.time < nextMoveTime)
            return;

        UpdateDirection();
        Move();
        nextMoveTime = Time.time + (1f / (speed * speedMultiplier)); // Sonraki hareket zamanını ayarla
    }

    private void HandleInput()
    {
        // Yılan yatay gidiyorsa sadece yukarı ve aşağıya dönebilir
        if (direction.x != 0)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                input = Vector2Int.up;
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                input = Vector2Int.down;
        }
        // Yılan dikey gidiyorsa sadece sağa ve sola dönebilir
        else if (direction.y != 0)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                input = Vector2Int.right;
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                input = Vector2Int.left;
        }
    }

    private void UpdateDirection()
    {
        // Girdinin tersi yöne gitmesini engelle
        if (input != Vector2Int.zero && input != -direction)
            direction = input;
    }

    private void Move()
    {
        // Her segmenti bir öncekinin pozisyonuna taşı
        for (int i = segments.Count - 1; i > 0; i--)
            segments[i].position = segments[i - 1].position;

        // Yılanın başını yeni yöne hareket ettir
        Vector2 newPos = (Vector2)transform.position + (Vector2)direction;
        transform.position = new Vector3(Mathf.Round(newPos.x), Mathf.Round(newPos.y), 0f);
    }

    public void Grow()
    {
        // Yeni segment oluştur ve kuyruğun sonuna ekle
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        input = Vector2Int.right;
        transform.position = Vector3.zero;

        // Baş hariç tüm segmentleri sil
        for (int i = 1; i < segments.Count; i++)
            Destroy(segments[i].gameObject);

        segments.Clear();
        segments.Add(transform);

        // Başlangıç uzunluğuna ulaşana kadar segment ekle
        for (int i = 1; i < initialSize; i++)
            Grow();
    }

    public bool Occupies(int x, int y)
    {
        // Yılanın herhangi bir segmenti verilen koordinattaysa true döndür
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y)
            {
                return true;
            }
        }
        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Food"))
        {
            Grow();
        }
        else if (other.CompareTag("Obstacle"))
        {
            ResetState();
        }
        // Yılan duvara çarparsa, duvardan geçme açıksa geç, değilse yeniden başlat
        else if (other.CompareTag("Wall"))
        {
            if (moveThroughWalls)
                Traverse(other.transform);
            else
                ResetState();
        }
    }

    private void Traverse(Transform wall)
    {
        // Yılan duvardan geçerse karşı tarafa ışınla
        Vector3 newPosition = transform.position;
        if (direction.x != 0)
            newPosition.x = -wall.position.x + direction.x;
        else if (direction.y != 0)
            newPosition.y = -wall.position.y + direction.y;
        transform.position = new Vector3(Mathf.Round(newPosition.x), Mathf.Round(newPosition.y), 0f);
    }
}