using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Grandmother : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float movementTime;
    [SerializeField] private float movementDistance;
    [SerializeField] private GameObject grandma;
    [SerializeField] private GameObject parent;

    // [SerializeField] private Text livesText;

    // [SerializeField] private Text powersText;
    // [SerializeField] private GameObject redParent;

    private Vector3 moveDirection;
    private Transform t;
    private bool carHit;
    private bool canFire;
    private int curBombs;
    private int lives;
    private const string InitialTextLives = "Lives:";
    private const string InitialTextPowers = "Powers Left:";
    
    private const int StartLife = 1;
    private const float recoveryTime = 2;

    private BombManager[] bombs;
    private const int NumBombs = 6;
    public static readonly GameObject[] Grandmas = new GameObject[2];
    private Vector3 startPosition;
    private Quaternion fireDirection;
    private float fireCoolDown;
    private bool firstShoot;
    private int lastIsland; //0 for initial island, 1 for left island, 2 for middle island, 3 right island
    private float rightIslandX = 27.7f;
    private float middleIslandX = -1.8f;
    private float leftIslandX = -29.3f;
    private float TOLERANCE = 0.5f;
    private bool isBeaten;

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        lastIsland = 0;
        lives = 1;
        // livesText.text = InitialTextLives + lives;
        // powersText.text = InitialTextPowers + NumBombs;
        firstShoot = true;
        curBombs = 0;
        fireCoolDown = 4;
        Grandmas[id - 1] = grandma;
        bombs = new BombManager[6]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
        carHit = false;
        canFire = false;
        moveDirection = Vector3.zero;
        t = GetComponent<Transform>();
        startPosition = t.position;
        isBeaten = false;
        
        
        for (int i = 0; i < NumBombs; i++)
        {
            Vector3 curPos = id == 1 ? Vector3.right + t.position : Vector3.left + t.position;
            // Quaternion curRotate = id == 1 ? new Quaternion(0, 0, -90, 1) : new Quaternion(0, 0, 90, 1);
            GameObject temp = Instantiate(Resources.Load("Bomb"), curPos, Quaternion.identity, parent.transform) as GameObject;
            if (temp == null)
            {
                throw new NullReferenceException("Bomb Prefab Not Found!");
            }
            
            bombs[i] = temp.GetComponent<BombManager>();
            bombs[i].SetShooterId(id);
            bombs[i].GetComponent<SpriteRenderer>().color = id == 1 ? Color.blue : Color.red;
            // bombs[NumBombs].SetActive(false);
        }
        // fireDirection = id == 1 ? new Quaternion(0, 90, 0, 1) : new Quaternion(90, 0, 0, 1);
    }

    void Update()
    {
        print(t.position);
        if (lives <= 0)
        {
            t.position = startPosition;
            lives = 1;
            // livesText.text = InitialTextLives + lives;
        }
        // powersText.text = InitialTextPowers + (NumBombs - curBombs);
        SetMoveDirection();
        if (!isBeaten)
        {
            StartCoroutine(Move());
        }
        else
        {
            StartCoroutine(Recovery());
        }
        
        if (Math.Abs(t.position.x - leftIslandX) < TOLERANCE)
        {
            lastIsland = 1;
        }else if (Math.Abs(t.position.x - middleIslandX) < TOLERANCE)
        {
            lastIsland = 2;
        }else if (Math.Abs(t.position.x - rightIslandX) < TOLERANCE)
        {
            lastIsland = 3;
        }
        fireCoolDown -= Time.deltaTime;
        // print("Bombs: " + curBombs);
        if (!isBeaten && ((id == 1 && Input.GetKeyDown(KeyCode.LeftControl)) ||
                        (id == 2 && Input.GetKeyDown(KeyCode.RightControl))) && curBombs < NumBombs && 
                        (fireCoolDown <= 0 || firstShoot))
        {
            // print("What Now: " + curBombs);
            firstShoot = false;
            int i = Random.Range(0, NumBombs);
            while (bombs[i].gameObject.activeInHierarchy)
            {
                i = Random.Range(0, NumBombs);
            }
            bombs[i].transform.position = t.position;
            bombs[i].gameObject.SetActive(true);
            curBombs++;
            fireCoolDown = 3;
        }
    }

    public int GetId()
    {
        return id;
    }

    public int GetCurBombs()
    {
        return curBombs;
    }

    public void SetCurBombs(int other)
    {
        curBombs = other;
    }

    private void SetMoveDirection()
    {
        switch (id)
        {
            case 1:
                if (Input.GetKeyDown(KeyCode.W))
                {
                    moveDirection = Vector3.up;
                }

                if (Input.GetKeyDown(KeyCode.S))
                {
                    moveDirection = Vector3.down;
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    moveDirection = Vector3.left;
                }

                if (Input.GetKeyDown(KeyCode.D))
                {
                    moveDirection = Vector3.right;
                }

                break;
            case 2:
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    moveDirection = Vector3.up;
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    moveDirection = Vector3.down;
                }

                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    moveDirection = Vector3.left;
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    moveDirection = Vector3.right;
                }

                break;
        }
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(movementTime);
        t.position += moveDirection * movementDistance;
        moveDirection = Vector3.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.StartsWith("Car"))
        {
            carHit = true;
        }

        if (collision.collider.name.EndsWith("Wall") && carHit)
        {
            t.position = startPosition;
        }

        // if (collision.gameObject.name.StartsWith("Car"))
        // {
        //     Debug.Log("Car trigger");
        //     switch (id)
        //     {
        //         case 1:
        //             t.position += Vector3.left;
        //             break;
        //         case 2:
        //             t.position += Vector3.right;
        //             break;
        //         
        //     }
        // }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.name.StartsWith("Car"))
        {
            carHit = false;
            lives--;
            // livesText.text = InitialTextLives + lives;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.name.StartsWith("Island"))
        {
            canFire = true;
        }

        BombManager curBomb = col.gameObject.GetComponent<BombManager>();
        if (col.gameObject.name.StartsWith("Bomb") && curBomb.GetShooterId() != id)
        {
            float curX = t.position.x;
            if (lastIsland == 0)
            {
                t.position = startPosition;
            }else if (lastIsland == 1)
            {
                t.position = new Vector3(leftIslandX, t.position.y, 0);
            }else if (lastIsland == 2)
            {
                t.position = new Vector3(middleIslandX, t.position.y, 0);
            }else if (lastIsland == 3)
            {
                t.position = new Vector3(rightIslandX, t.position.y, 0);
            }
            lives--;
            // livesText.text = InitialTextLives + lives;
            col.gameObject.SetActive(false);
            isBeaten = true;
            curBomb.ActivateBomb(Grandmas[id-1].GetComponent<Grandmother>());
            int enemyId = id == 1 ? 2 : 1;
            Grandmother enemy = Grandmas[enemyId - 1].GetComponent<Grandmother>(); 
            enemy.SetCurBombs(enemy.GetCurBombs() - 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.name.StartsWith("Island"))
        {
            canFire = false;
        }
    }

    public void Beaten()
    {
        //isBeaten = true;
    }
    private IEnumerator Recovery()
    {
        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(recoveryTime);
        isBeaten = false;
    }

    private IEnumerator FadeInOut()
    {
        Color c = spriteRenderer.color;
        
        for (float i = 0.25f; i >= 0; i -= Time.deltaTime)
        {
            c = new Color(c.r, c.g, c.b, i * 4);
            yield return null;
        }
        for (float i = 0; i <= 0.25f; i += Time.deltaTime)
        {
            c = new Color(c.r, c.g, c.b, i * 4);
            yield return null;
        }
    }

}