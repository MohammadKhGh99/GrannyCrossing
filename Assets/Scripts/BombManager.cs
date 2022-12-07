using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;

    // [SerializeField] private GameObject goalPlayer;

    private int shooterId;
    private float goalPosX;
    private Transform t;
    
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
    }

    // Update is called once per frame
    void Update()
    {
        if (shooterId == 2 && t.position.x >= goalPosX)  // grandma 2
        {
            t.position += Vector3.left * (20 * Time.deltaTime);
        } else if (shooterId == 1 && t.position.x <= goalPosX)  // grandma 1
        {
            t.position += Vector3.right * (20 * Time.deltaTime);
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