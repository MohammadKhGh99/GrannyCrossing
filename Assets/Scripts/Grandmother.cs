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
    [SerializeField] private float fireCollDownTime = 0.8f;
    [SerializeField] private float loadingBombsPercentage;

    private Vector3 moveDirection;
    private Transform t;
    private int curBombs;
    private const float recoveryTime = 2;

    private BombManager[] bombs;
    private const int NumBombs = 15;
    private Vector3 startPosition;
    private float fireCoolDown = 0.8f;
    private float fireCoolDownMax = 0.8f;

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
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name == "Pointer")
            {
                pointer = transform.GetChild(i);
            }
        }
        InitPointerPosition();
        curBombs = 0;
        loadingBombsPercentage = fireCoolDownMax - fireCoolDown / fireCoolDownMax;
        bombs = new BombManager[NumBombs]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
        moveDirection = Vector3.zero;
        t = GetComponent<Transform>();
        startPosition = t.position;
        isBeaten = false;
        
        
        for (int i = 0; i < NumBombs; i++)
        {
            GameObject temp = Instantiate(Resources.Load("Bomb"), pointer.position, Quaternion.identity, transform) as GameObject;
            if (temp == null)
            {
                throw new NullReferenceException("Bomb Prefab Not Found!");
            }
            bombs[i] = temp.GetComponent<BombManager>();
            bombs[i].SetShooterId(id);
            bombs[i].GetComponent<SpriteRenderer>().color = id == 1 ? Color.blue : Color.red;
        }
    }

    void Update()
    {
        SetMoveDirection();
        if (!isBeaten)
        {
            StartCoroutine(Move());
            PointerMove();
        }
        else
        {
            t.position = startPosition; //I don't know why but without it there is a weird bug...
                                        //you can go one more step after going back when hit by a car or a bomb
        }
        fireCoolDown -= Time.deltaTime;
        fireCoolDown = fireCoolDown < 0 ? 0 : fireCoolDown;
        loadingBombsPercentage = isBeaten ? 0 : (fireCoolDownMax - fireCoolDown) / fireCoolDownMax;

        if (((id == 1 && Input.GetKeyDown(KeyCode.LeftAlt)) ||
                        (id == 2 && Input.GetKeyDown(KeyCode.RightAlt))) && curBombs < NumBombs && 
                        loadingBombsPercentage == 1)
        {
            Fire();
        }
    }

    private void Fire()
    {
        int i = Random.Range(0, NumBombs);
        while (bombs[i].gameObject.activeInHierarchy)
        {
            i = Random.Range(0, NumBombs);
        }
        bombs[i].transform.position = pointer.position;
        bombs[i].gameObject.SetActive(true);
        bombs[i].Fire();
        curBombs++;
        fireCoolDown = fireCollDownTime;
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
            t.position = startPosition;
            InitPointerPosition();
            pointer.gameObject.SetActive(false);
            isBeaten = true;
            StartCoroutine(Recovery());
        }
    }
    

    private void OnTriggerEnter2D(Collider2D col)
    {
        BombManager curBomb = col.gameObject.GetComponent<BombManager>();
        if (col.gameObject.name.StartsWith("Bomb") && curBomb.GetShooterId() != id)
        { 
            isBeaten = true;
            curBomb.ActivateBomb(gameObject);
            pointer.gameObject.SetActive(false);
            StartCoroutine(Recovery());
        }
    }

    public IEnumerator Recovery()
    {
        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(recoveryTime);
        isBeaten = false;
        pointer.gameObject.SetActive(true);
        InitPointerPosition();
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1);

    }

    private IEnumerator FadeInOut()
    {
        Color c = spriteRenderer.color;
        for (int n = 0; n <= recoveryTime * 2; n++)
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