using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [SerializeField] private int speed;
    private Transform t;
    private int id;
    // private int speed;
    private Vector3 direction;
    private Vector3 startPosition;
    private static readonly int[] HighWayCars = { 1, 2, 5, 6 };

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        speed = speed == 0 ? HighWayCars.Contains(id) ? Random.Range(12, 20) : Random.Range(7, 15) : speed;
        SetStartPosition(t.position);
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (speed * Time.deltaTime);
    }

    public void SetDirection(Vector3 direct)
    {
        direction = direct;
    }
    
    public void SetId(int newId)
    {
        id = newId;
    }

    public void SetStartPosition(Vector3 position)
    {
        startPosition = position;
    }

    public void Reused()
    {
        t.position = startPosition;
    }
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        // Debug.Log(col.collider.name);
        if (col.collider.name.EndsWith("Wall"))
        {
            Reused();
        }
    }
}