using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;

    // [SerializeField] private GameObject goalPlayer;

    private int shooterId;
    private Transform t;
    private Vector3 goalPos;
    private Quaternion bombRotation;
    private Vector3 initialScale;
    private Vector3 flyingScale;
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        Vector3 enemyPosition = 
            shooterId == 1 ? Grandmother.Grandmas[1].transform.position : Grandmother.Grandmas[0].transform.position;
        goalPos = shooterId == 1 ? enemyPosition + (Vector3.left * 5) : enemyPosition + (Vector3.right * 5);
        initialScale = t.localScale;
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

        // todo scaling for throwing effect
        // if (t.position.x - goalPos.x <= 0.5f * goalPos.x)
        //     t.localScale += new Vector3(0.05f, 0.05f, 0);
        // else if (t.position.x +)
        //     t.localScale -= new Vector3(0.05f, 0.05f, 0);
    }

    public int GetShooterId()
    {
        return shooterId;
    }

    public void SetShooterId(int other)
    {
        shooterId = other;
    }

    public void ActivateBomb(Grandmother grandmother)
    {
        grandmother.GoBack();
        t.gameObject.SetActive(false);
    }
}