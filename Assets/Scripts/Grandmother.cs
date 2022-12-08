using System;
using System.Collections;
using Unity.VisualScripting;
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
    [SerializeField] private float fireCollDownTime = 0.8f;
    //[SerializeField] private Text livesText;

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
    private const int NumBombs = 15;
    public static readonly GameObject[] Grandmas = new GameObject[2];
    private Vector3 startPosition;
    private Quaternion fireDirection;
    private float fireCoolDown;
    private bool firstShoot;
    private int lastIsland; //0 for initial island, 1 for left island, 2 for middle island, 3 right island
    private float rightIslandX = 27.7f;
    private float middleIslandX = -1.8f;
    private float leftIslandX = -29.3f;
    // private float TOLERANCE = 0.5f;
    private bool isBeaten;
    private Transform pointer;
    private float pointerSpeed = 150;
    private float maxPointerSpeed = 800;
    private float minPointerSpeed = 50;

    


    private SpriteRenderer spriteRenderer;

    private void InitPointerPosition()
    {
        pointer.RotateAround(transform.position, Vector3.forward, Random.value * 360);
    }

    private void PointerMove()
    {
        /*if (moveDirection != Vector3.zero && pointerSpeed <= maxPointerSpeed)
        {
            pointerSpeed += 300 * Time.deltaTime;
            pointerSpeed = pointerSpeed > maxPointerSpeed ? maxPointerSpeed : pointerSpeed;

        }
        else if (moveDirection == Vector3.zero && pointerSpeed >= minPointerSpeed)
        {
            pointerSpeed -= 10 * Time.deltaTime;
            pointerSpeed = pointerSpeed < minPointerSpeed ? minPointerSpeed : pointerSpeed;
        }*/
        pointer.RotateAround(transform.position, Vector3.forward, pointerSpeed * Time.deltaTime);
    }

    public Vector3 GetPointerPosition()
    {
        return pointer.position;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // lastIsland = 0;
        lives = 1;
        // livesText.text = InitialTextLives + lives;
        
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Pointer")
            {
                pointer = transform.GetChild(i);
            }
        }
        
        InitPointerPosition();
        
        
        lives = StartLife;
        //livesText.text = InitialTextLives + lives;
        // powersText.text = InitialTextPowers + NumBombs;
        firstShoot = true;
        curBombs = 0;
        fireCoolDown = 4;
        Grandmas[id - 1] = grandma;
        bombs = new BombManager[NumBombs]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
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
            GameObject temp = Instantiate(Resources.Load("Bomb"), curPos, Quaternion.identity, transform) as GameObject;
            if (temp == null)
            {
                throw new NullReferenceException("Bomb Prefab Not Found!");
            }
            
            bombs[i] = temp.GetComponent<BombManager>();
            bombs[i].SetShooterId(id);
            //bombs[i].SetSpeed(bombsSpeed);
            bombs[i].GetComponent<SpriteRenderer>().color = id == 1 ? Color.blue : Color.red;
            // bombs[NumBombs].SetActive(false);
        }
        // fireDirection = id == 1 ? new Quaternion(0, 90, 0, 1) : new Quaternion(90, 0, 0, 1);
    }

    void Update()
    {
        /*if (lives <= 0)
        {
            
            t.position = startPosition;
            lives = StartLife;
            livesText.text = InitialTextLives + lives;
        }*/
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
        PointerMove();
        fireCoolDown -= Time.deltaTime;
        // print("Bombs: " + curBombs);
        if (!isBeaten && ((id == 1 && Input.GetKeyDown(KeyCode.LeftAlt)) ||
                        (id == 2 && Input.GetKeyDown(KeyCode.RightAlt))) && curBombs < NumBombs && 
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
            bombs[i].Fire();
            curBombs++;
            fireCoolDown = fireCollDownTime;
        }
    }

    public void GoBack()
    {
        Vector3 position = t.position;
        switch (id)
        {
            case 2:
                if (position.x < leftIslandX)
                {
                    t.position = new Vector3(leftIslandX, position.y, position.z);
                }else if (position.x < middleIslandX)
                {
                    t.position = new Vector3(middleIslandX, position.y, position.z);

                }else if (position.x < rightIslandX)
                {
                    t.position = new Vector3(rightIslandX, position.y, position.z);
                }
                else
                {
                    t.position = startPosition;
                }
                break;
            case 1:
                if (position.x > rightIslandX)
                {
                    t.position = new Vector3(rightIslandX, position.y, position.z);
                }else if (position.x > middleIslandX)
                {
                    t.position = new Vector3(middleIslandX, position.y, position.z);

                }else if (position.x > leftIslandX)
                {
                    t.position = new Vector3(leftIslandX, position.y, position.z);

                }
                else
                {
                    t.position = startPosition;
                }
                break;
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

    public void AddToCurBombs(int other)
    {
        curBombs += other;
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
            //lives -= 2;
            //livesText.text = InitialTextLives + lives;
            t.position = startPosition;
            InitPointerPosition();
            carHit = true;
            isBeaten = true;
        }

        if (collision.collider.name.EndsWith("Wall") && carHit)
        {
            t.position = startPosition;
        }

        
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.collider.name.StartsWith("Car"))
        {
            carHit = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // if (col.gameObject.name.StartsWith("Island"))
        // {
        //     canFire = true;
        // }

        BombManager curBomb = col.gameObject.GetComponent<BombManager>();
        if (col.gameObject.name.StartsWith("Bomb") && curBomb.GetShooterId() != id)
        {
            //lives -= 1;
            //livesText.text = InitialTextLives + lives;
            //col.gameObject.SetActive(false);
            // col.transform.position = t.position;
            // col.transform.SetParent(t);
            isBeaten = true;
            curBomb.ActivateBomb(gameObject.GetComponent<Grandmother>());
            //int enemyId = id == 1 ? 2 : 1;
            //Grandmother enemy = Grandmas[enemyId - 1].GetComponent<Grandmother>(); 
            //enemy.AddToCurBombs(enemy.GetCurBombs() - 1);
        }
    }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.gameObject.name.StartsWith("Island"))
    //     {
    //         canFire = false;
    //     }
    // }

    
    private IEnumerator Recovery()
    {
        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(recoveryTime);
        isBeaten = false;
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1);

    }

    private IEnumerator FadeInOut()
    {
        Color c = spriteRenderer.color;
        
        // for (float i = 0.25f; i >= 0; i -= Time.deltaTime)
        while (isBeaten)
        {
            for (float i = 0.25f; i >= 0; i -= Time.deltaTime)
            {
                spriteRenderer.color = new Color(c.r, c.g, c.b, i * 4);
                yield return null;
            }
            for (float i = 0; i <= 0.25f; i += Time.deltaTime)
            {
                spriteRenderer.color = new Color(c.r, c.g, c.b, i * 4);
                yield return null;
            }
        }
    }

}