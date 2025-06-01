using UnityEngine;

public class Food : MonoBehaviour
{
    public Collider2D gridArea;
    private Snake snake;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        RandomizePosition();
    }

    public void RandomizePosition()
    {
        Bounds bounds = gridArea.bounds;

        // Alan içinde rastgele bir pozisyon seç
        // Değerleri yuvarlayarak hizalanmasını sağla
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        // Yemin yılanın üzerine doğmasını engelle
        while (snake.Occupies(x, y))
        {
            x++;

            // Eğer x sınırı aşıldıysa başa dön ve y'yi artır
            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                // Eğer y sınırı aşıldıysa başa dön
                if (y > bounds.max.y)
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        // Yemi seçilen pozisyona yerleştir
        transform.position = new Vector2(x, y);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        RandomizePosition();
    }
}