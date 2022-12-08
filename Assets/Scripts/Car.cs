using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Car : MonoBehaviour
{
    [SerializeField] private int speed;
    [SerializeField] private bool controlPosition;
    [SerializeField] private float x;
    [SerializeField] private float y;
    private Transform t;
    private int id;
    
    private Vector3 direction;
    private Vector3 startPosition;
    private float fieldLimit;

    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        if (controlPosition)
            t.position = new Vector3(x, y, 0);
        
        speed = speed == 0 ? id is 0 or 9 ? Random.Range(7, 15) : Random.Range(12, 20) : speed;
        SetStartPosition(t.position);
        fieldLimit = t.parent.GetComponentInParent<GameController>().GetFieldLimit();
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (speed * Time.deltaTime);
        if (t.position.y > fieldLimit || t.position.y < -fieldLimit)
            Reused();
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
    
}