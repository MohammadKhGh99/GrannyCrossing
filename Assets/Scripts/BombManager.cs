using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;

    // [SerializeField] private GameObject goalPlayer;

    private int shooterId;
    private float goalPosX;
    private Transform t;
    private Vector3 goalPos;
    
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        // Vector3 curPos = bomb.transform.position;
        float enemyX = 
            shooterId == 1 ? Grandmother.Grandmas[1].transform.position.x : Grandmother.Grandmas[0].transform.position.x;
        // print(t.position.x + " To " + enemyX);
        enemyX -= shooterId == 1 ? 2 : -2;
        float fromX = shooterId == 1 ? t.position.x + 2 : t.position.x - 2;
        goalPosX = Random.Range(fromX, enemyX);
        // t.SetParent(null);
        Vector3 enemyPosition = 
            shooterId == 1 ? Grandmother.Grandmas[1].transform.position : Grandmother.Grandmas[0].transform.position;
        goalPos = shooterId == 1 ? enemyPosition + (Vector3.left * 5) : enemyPosition + (Vector3.right * 5);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = t.position;
        if (shooterId == 2 && position != goalPos)  // grandma 2
        {
            t.position += (goalPos - position) * (3 * Time.deltaTime);
        } else if (shooterId == 1 && position != goalPos)  // grandma 1
        {
            t.position += (goalPos - position) * (3 * Time.deltaTime);
        }
    }

    public int GetShooterId()
    {
        return shooterId;
    }

    public void SetShooterId(int other)
    {
        shooterId = other;
    }
}