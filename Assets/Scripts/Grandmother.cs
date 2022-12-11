using System;
using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using RandomS = System.Random;


public class Grandmother : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float movementTime;
    [SerializeField] private float movementDistance;
    [SerializeField] private float fireCollDownTime = 0.8f;
    [SerializeField] private float loadingBombsPercentage;
    [SerializeField] private int maxBombsTypes = 6;

    // [SerializeField] private int maxCarsTypes = 4;
    // [SerializeField] private string[] 

    // Types of the bombs
    private const int NumBombsTypes = 4;
    private const int FreezeInPlace = 0; // Works Good
    private const int CrazyPointer = 1; // Works Good
    private const int GoBackToIsland = 2; // Works Good
    private const int CrazyDirections = 3;

    private Vector3 moveDirection;
    private Transform t;
    private int curBombs;
    private const float RecoveryTime = 2;

    private BombManager[] bombs;
    private const int MaxBombs = 15;
    private Vector3 startPosition;
    private float fireCoolDown = 0.8f;
    private const float FireCoolDownMax = 0.8f;

    private const float RightIslandX = 27.7f;
    private const float MiddleIslandX = -1.8f;

    private const float LeftIslandX = -29.3f;

    private bool isBeaten;
    private Transform pointer;
    private const float PointerSpeed = 150;
    private float maxPointerSpeed = 800;
    private float minPointerSpeed = 50;
    private bool isTurnRight;
    private SpriteRenderer spriteRenderer;

    private bool freezeOrNot;
    private Action[] randomDirections;
    private bool isUnderControl = true;
    private const float LoseControlTime = 5;
    private bool pointerIsUnderControl = true;
    private const float PointerLoseControlTime = 5;
    private const float PointerSpeedLoseControl = 500;
    private float moveCoolDown = 0.5f;
    
    // private readonly string[] bombsTypes = { "Bomb", "Bomb 1", "Bomb 2", "Bomb 3", "Bomb 4", "Bomb 5" };

    private static Sprite[] bombsTypes;
    // private static Sprite[] carsTypes;
    private static bool hasLoaded = false;
    
    static void LoadSprites()
    {
        bombsTypes = Resources.LoadAll<Sprite>("Sprites/bombs");
        // carsTypes = Resources.LoadAll<Sprite>("Sprites/cars");

        hasLoaded = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (!hasLoaded)
        {
            LoadSprites();
        }
        
        randomDirections = new Action[] { MoveUp, MoveRight, MoveDown, MoveLeft };
        freezeOrNot = false;

        // Taking Components from this GameObject
        t = GetComponent<Transform>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // The Grandma is looking right or left (in the beginning)
        isTurnRight = id == 1;

        // Initializing the Pointer of the shooting direction
        pointer = t.GetChild(0);
        InitPointerPosition();

        moveDirection = Vector3.zero;
        startPosition = t.position;
        isBeaten = false;

        // Initializing the Bombs for each Grandma
        curBombs = 0;
        loadingBombsPercentage = (FireCoolDownMax - fireCoolDown) / FireCoolDownMax;
        bombs = new BombManager[MaxBombs]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
        for (int i = 0; i < MaxBombs; i++)
        {
            GameObject temp =
                Instantiate(Resources.Load("Bomb"), pointer.position, Quaternion.identity, t) as GameObject;
            if (temp == null)
            {
                throw new NullReferenceException("Bomb Prefab Not Found!");
            }

            Destroy(temp.GetComponent<PolygonCollider2D>());
            temp.AddComponent<PolygonCollider2D>();
            temp.GetComponent<SpriteRenderer>().sprite = bombsTypes[Random.Range(0, maxBombsTypes)]; 

            bombs[i] = temp.GetComponent<BombManager>();
            
            bombs[i].SetId(Random.Range(0, NumBombsTypes));
            // Sets each bomb's shooter, blue grandma ot red one
            bombs[i].SetShooterId(id);
            // bombs[i].GetComponent<SpriteRenderer>().color = id == 1 ? Color.blue : Color.red;
        }

        moveCoolDown = 0;
    }

    void Update()
    {
        if (freezeOrNot)
            return;
        
        if (!isBeaten)
        {
            if (isUnderControl)
                SetMoveDirection();
            else
                MoveMixDirections();
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
        float temp = (FireCoolDownMax - fireCoolDown) / FireCoolDownMax;
        loadingBombsPercentage = isBeaten ? 0 : temp >= 1 ? 1 : temp;

        bool blueFired = id == 1 && Input.GetKeyDown(KeyCode.LeftAlt);
        bool redFired = id == 2 && Input.GetKeyDown(KeyCode.RightAlt);
        if ((blueFired || redFired) && curBombs < MaxBombs && loadingBombsPercentage >= 1.0f)
            Fire();
    }

    // private void MyMove()
    // {
    //     moveCoolDown -= Time.deltaTime;
    //     if (moveCoolDown <= 0)
    //     {
    //         moveCoolDown = 0.5f;
    //         t.position += moveDirection * movementDistance;
    //         moveDirection = Vector3.zero;
    //     }
    // }
    
    private void LoseControl()
    {
        MixDirections();
        isUnderControl = false;
        StartCoroutine(Recontrol());
    }

    private IEnumerator Recontrol()
    {
        yield return new WaitForSeconds(LoseControlTime);
        isUnderControl = true;
    }

    private void MixDirections()
    {
        RandomS random = new RandomS();
        int n = 4;
        while (n > 1)
        {
            int k = random.Next(n--);
            (randomDirections[n], randomDirections[k]) = (randomDirections[k], randomDirections[n]);
        }
    }

    private void MoveUp()
    {
        moveDirection = Vector3.up;
    }

    private void MoveDown()
    {
        moveDirection = Vector3.down;
    }

    private void MoveRight()
    {
        moveDirection = Vector3.right;
        if (!isTurnRight)
        {
            t.Rotate(Vector3.up, 180);
            isTurnRight = true;
        }
    }

    private void MoveLeft()
    {
        moveDirection = Vector3.left;
        if (isTurnRight)
        {
            t.Rotate(Vector3.up, 180);
            isTurnRight = false;
        }
    }

    private void MoveMixDirections()
    {
        if ((id == 1 && Input.GetKey(KeyCode.W)) || (id == 2 && Input.GetKey(KeyCode.UpArrow)))
            randomDirections[0]();
        if ((id == 1 && Input.GetKey(KeyCode.S)) || (id == 2 && Input.GetKey(KeyCode.DownArrow)))
            randomDirections[2]();
        if ((id == 1 && Input.GetKey(KeyCode.D)) || (id == 2 && Input.GetKey(KeyCode.RightArrow)))
            randomDirections[1]();
        if ((id == 1 && Input.GetKey(KeyCode.A)) || (id == 2 && Input.GetKey(KeyCode.LeftArrow)))
            randomDirections[3]();
    }

    private void InitPointerPosition()
    {
        pointer.RotateAround(t.position, Vector3.forward, Random.value * 360);
    }

    private void PointerLoseControl()
    {
        pointerIsUnderControl = false;
        StartCoroutine(PointerRecontrol());
    }

    private IEnumerator PointerRecontrol()
    {
        yield return new WaitForSeconds(PointerLoseControlTime);
        pointerIsUnderControl = true;
    }

    private void PointerMove()
    {
        if (pointerIsUnderControl)
            pointer.RotateAround(t.position, Vector3.forward, PointerSpeed * Time.deltaTime);
        else
            pointer.RotateAround(t.position, Vector3.forward, PointerSpeedLoseControl * Time.deltaTime);
    }

    public Vector3 GetPointerPosition()
    {
        return pointer.position;
    }

    private void Fire()
    {
        int i = Random.Range(0, MaxBombs);
        while (bombs[i].gameObject.activeInHierarchy)
            i = Random.Range(0, MaxBombs);

        //bombs[i].transform.position = pointer.position;
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
                t.position = position.x switch
                {
                    < LeftIslandX => new Vector3(LeftIslandX, position.y, position.z),
                    < MiddleIslandX => new Vector3(MiddleIslandX, position.y, position.z),
                    < RightIslandX => new Vector3(RightIslandX, position.y, position.z),
                    _ => startPosition
                };
                if (isTurnRight)
                {
                    t.Rotate(Vector3.up, 180);
                    isTurnRight = false;
                }

                break;
            case 1:
                t.position = position.x switch
                {
                    > RightIslandX => new Vector3(RightIslandX, position.y, position.z),
                    > MiddleIslandX => new Vector3(MiddleIslandX, position.y, position.z),
                    > LeftIslandX => new Vector3(LeftIslandX, position.y, position.z),
                    _ => startPosition
                };

                if (!isTurnRight)
                {
                    t.Rotate(Vector3.up, 180);
                    isTurnRight = true;
                }

                break;
        }
    }

    // public int GetId()
    // {
    //     return id;
    // }
    //
    // public int GetCurBombs()
    // {
    //     return curBombs;
    // }

    public void AddToCurBombs(int other)
    {
        curBombs += other;
    }

    private void SetMoveDirection()
    {
        if ((id == 1 && Input.GetKeyDown(KeyCode.W)) || (id == 2 && Input.GetKeyDown(KeyCode.UpArrow)))
            MoveUp();
        if ((id == 1 && Input.GetKeyDown(KeyCode.S)) || (id == 2 && Input.GetKeyDown(KeyCode.DownArrow)))
            MoveDown();
        if ((id == 1 && Input.GetKeyDown(KeyCode.D)) || (id == 2 && Input.GetKeyDown(KeyCode.RightArrow)))
            MoveRight();
        if ((id == 1 && Input.GetKeyDown(KeyCode.A)) || (id == 2 && Input.GetKeyDown(KeyCode.LeftArrow)))
            MoveLeft();
    }

    private IEnumerator Move()
    {
        yield return new WaitForSeconds(movementTime);
        t.position += moveDirection * movementDistance;
        moveDirection = Vector3.zero;
    }

    private IEnumerator FreezeMovement()
    {
        freezeOrNot = true;
        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(4);
        RecoverFading();
        freezeOrNot = false;
    }

    private void HitByBomb(int bombId)
    {
        switch (bombId)
        {
            case FreezeInPlace:
                StartCoroutine(FreezeMovement());
                break;
            case GoBackToIsland:
                GoBack();
                StartCoroutine(Recovery());
                break;
            case CrazyPointer:
                PointerLoseControl();
                break;
            case CrazyDirections:
                print("Losing Control");
                LoseControl();
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.StartsWith("Car"))
        {
            t.position = startPosition;
            switch (id)
            {
                case 1 when !isTurnRight:
                    t.Rotate(Vector3.up, 180);
                    isTurnRight = true;
                    break;
                case 2 when isTurnRight:
                    t.Rotate(Vector3.up, 180);
                    isTurnRight = false;
                    break;
            }

            // switch (id)
            // {
            //     case 1:
            //         if (!isTurnRight)
            //         {
            //             t.Rotate(Vector3.up, 180);
            //             isTurnRight = true;
            //         }
            //         break;
            //     case 2:
            //         if (isTurnRight)
            //         {
            //             t.Rotate(Vector3.up, 180);
            //             isTurnRight = false;
            //         }
            //         break;
            // }

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
            print("Bomb ID: " + curBomb.GetId());
            HitByBomb(curBomb.GetId());
        }
    }

    private void RecoverFading()
    {
        Color c = spriteRenderer.color;
        spriteRenderer.color = new Color(c.r, c.g, c.b, 1);
    }

    private IEnumerator Recovery()
    {
        StartCoroutine(FadeInOut());
        yield return new WaitForSeconds(RecoveryTime);
        isBeaten = false;
        pointer.gameObject.SetActive(true);
        InitPointerPosition();
        RecoverFading();
    }

    private IEnumerator FadeInOut()
    {
        Color c = spriteRenderer.color;
        for (int n = 0; n <= RecoveryTime * 2; n++)
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