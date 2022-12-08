using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;

    // [SerializeField] private GameObject goalPlayer;

    private int shooterId;
    //private float goalPosX;
    private Transform t;
    //private Vector3 goalPos;
    private Vector3 direction;
    private float speed = 18;
    private bool isFired;
    private float bombLifeTime = 10;
    

    public void Fire()
    {
        isFired = true;
        //t.position = t.parent.position;
        StartCoroutine(KillBomb());
        //t.gameObject.SetActive(true);
    }

    private IEnumerator KillBomb()
    {
        yield return new WaitForSeconds(bombLifeTime);
        t.gameObject.SetActive(false);
        isFired = false;
        t.parent.GetComponent<Grandmother>().AddToCurBombs(-1);
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        
        direction = t.parent.GetComponent<Grandmother>().GetPointerPosition() - t.parent.position;
        direction = direction.normalized;
        // Vector3 curPos = bomb.transform.position;
        /*float enemyX = 
            shooterId == 1 ? Grandmother.Grandmas[1].transform.position.x : Grandmother.Grandmas[0].transform.position.x;
        // print(t.position.x + " To " + enemyX);
        enemyX -= shooterId == 1 ? 2 : -2;
        float fromX = shooterId == 1 ? t.position.x + 2 : t.position.x - 2;
        goalPosX = Random.Range(fromX, enemyX);
        // t.SetParent(null);
        Vector3 enemyPosition = 
            shooterId == 1 ? Grandmother.Grandmas[1].transform.position : Grandmother.Grandmas[0].transform.position;
        goalPos = shooterId == 1 ? enemyPosition + (Vector3.left * 5) : enemyPosition + (Vector3.right * 5);
        */

    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * speed * Time.deltaTime;
        /*Vector3 position = t.position;
        if (shooterId == 2 && position != goalPos)  // grandma 2
        {
            t.position += (goalPos - position) * (3 * Time.deltaTime);
        } else if (shooterId == 1 && position != goalPos)  // grandma 1
        {
            t.position += (goalPos - position) * (3 * Time.deltaTime);
        }*/
    }

    public void SetSpeed(float newSpeed)
    {
        //speed = newSpeed;
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
        isFired = false;
        t.parent.GetComponent<Grandmother>().AddToCurBombs(-1);
    }
}