using System.Collections;
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
    private bool isStartGame;
    
    private Vector3 direction;
    private Vector3 startPosition;
    private float fieldLimit;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        isStartGame = true;
        t = GetComponent<Transform>();
        spriteRenderer = t.GetComponent<SpriteRenderer>();
        if (controlPosition)
            t.position = new Vector3(x, y, 0);
        if (!withoutCars)
            speed = !controlSpeed ? Random.Range(7, 15) : speed;

        startPosition = t.position;
        fieldLimit = t.parent.GetComponentInParent<GameController>().GetFieldLimit();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartGame)
        {
            isStartGame = false;
            speed = !controlSpeed ? id is 0 or 9 ? Random.Range(7, 12) : Random.Range(12, 18) : speed;
        }
        t.position += direction * (speed * Time.deltaTime);
        if (t.position.y > fieldLimit || t.position.y < -fieldLimit)
        {
            t.position = startPosition;
            speed = !controlSpeed ? id is 0 or 9 ? Random.Range(7, 12) : Random.Range(12, 18) : speed;
            spriteRenderer.sprite = GameController.CarsTypes[Random.Range(0, GameController.NumCarsTypes)];
        }
    }

    public void EndGame()
    {
        t.position = startPosition;
        gameObject.SetActive(false);
    }

    public void StartGame()
    {
        t.position = startPosition;
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

    public int GetId()
    {
        return id;
    }

    
}