using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [SerializeField] private bool controlSpeed;
    [SerializeField] private int speed;
    [SerializeField] private bool controlPosition;
    [SerializeField] private float x;
    [SerializeField] private float y;
    [SerializeField] private string upOrDown;
    [SerializeField] private bool withoutCars;
   
    private Transform t;
    private int id;
    
    private Vector3 direction;
    private Vector3 startPosition;
    private float fieldLimit;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        spriteRenderer = t.GetComponent<SpriteRenderer>();
        if (controlPosition)
            t.position = new Vector3(x, y, 0);
        if (!withoutCars)
            speed = !controlSpeed ? id is 0 or 9 ? Random.Range(7, 15) : Random.Range(12, 20) : speed;

        startPosition = t.position;
        fieldLimit = t.parent.GetComponentInParent<GameController>().GetFieldLimit();
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (speed * Time.deltaTime);
        if (t.position.y > fieldLimit || t.position.y < -fieldLimit)
        {
            t.position = startPosition;
            spriteRenderer.sprite = GameController.CarsTypes[Random.Range(0, GameController.NumCarsTypes)];
        }
    }

    public void SetDirection(string other)
    {
        upOrDown = other;
    }
    
    public void SetDirection(Vector3 direct)
    {
        direction = direct;
    }
    
    public void SetId(int newId)
    {
        id = newId;
    }
}