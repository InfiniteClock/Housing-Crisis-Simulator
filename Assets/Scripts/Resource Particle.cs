using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum ResourceType { Happy, Unhappy, Profit, Cost}
public class ResourceParticle : MonoBehaviour
{
    [SerializeField] private Sprite happy;
    [SerializeField] private Sprite unhappy;
    [SerializeField] private Sprite profit;
    [SerializeField] private Sprite cost;
    [SerializeField] public float spawnRadius;

    private ResourceType type;
    private Vector3 destination;
    private Vector3 start;
    private Canvas canvas;
    public void Spawn(ResourceType resourceType, Vector3 dest, Vector3 startLoc, Canvas can)
    {
        Image img = GetComponent<Image>();
        
        type = resourceType;
        destination = dest;
        canvas = can;

        transform.SetParent(canvas.transform);
        // Set the correct image for the resource type
        switch (type)
        {
            case ResourceType.Happy:
                img.sprite = happy;
                break;
            case ResourceType.Unhappy:
                img.sprite = unhappy;
                break;
            case ResourceType.Profit:
                img.sprite = profit;
                break;
            case ResourceType.Cost:
                img.sprite = cost;
                break;
            default:
                img.sprite = null;
                break;
        }

        // Set scale to 0 and position to start with random spawn adjustment
        transform.localScale = Vector3.zero;
        Vector3 randSpawn = UnityEngine.Random.insideUnitCircle * spawnRadius;
        start = startLoc + randSpawn;
        transform.position = start;
        StartCoroutine(Appear());
    }

    // Spawns in the resource and grows it to full size
    private IEnumerator Appear()
    {
        float timer = UnityEngine.Random.Range(-0.2f, 0f);
        float timerMax = 0.4f;
        while (timer < 0f)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        while (timer < timerMax)
        {
            timer += Time.deltaTime;
            transform.localScale += Vector3.one * (timer / timerMax);
            transform.localScale = Vector3.Min(Vector3.one, transform.localScale);
            yield return null;
        }
        transform.localScale = Vector3.one;
        StartCoroutine(Move());
    }
    // Moves the resource from start to destination
    private IEnumerator Move()
    {
        float timer = 0;
        float timerMax = 0.8f;
        while (timer < timerMax)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(start, destination, timer/timerMax);
            yield return null;
        }
        transform.position = destination;
        StartCoroutine(Disappear());
    }
    // Shrinks the resource, then removes it
    private IEnumerator Disappear()
    {
        float timer = 0;
        float timerMax = 0.2f;
        while (timer < timerMax)
        {
            timer += Time.deltaTime;
            transform.localScale -= Vector3.one * (timer / timerMax);
            transform.localScale = Vector3.Max(Vector3.zero, transform.localScale);
            yield return null;
        }
        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }
}
