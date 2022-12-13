using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;
using RandomS = System.Random;


public class Grandmother : MonoBehaviour
{
    [SerializeField] private int id;
    [SerializeField] private float movementTime;
    [SerializeField] private float movementDistance;
    [SerializeField] private float fireCollDownTime = 0.8f;
    [SerializeField] private float loadingBombsPercentage;
    [SerializeField] private GameObject bombParent;
    [SerializeField] private AudioSource throwSound;
    [SerializeField] private AudioSource dizzySound;
    [SerializeField] private AudioSource hitByCarSound;
    [SerializeField] private AudioSource confusedSound;

    [SerializeField] private AudioSource hitByBombSound;

    // Types of the bombs
    private const int NumBombsEffects = 1;
    private const int GoBackToStart = 0; // Works Good
    private const int FreezeInPlace = 1; // Works Good
    private const int CrazyPointer = 2; // Works Good
    private const int CrazyDirections = 3; // Works Good

    private Vector3 moveDirection;
    private Transform t;
    private int curBombs;
    private const float RecoveryTime = 2;

    private BombManager[] bombs;
    private const int MaxBombs = 15;
    private Vector3 startPosition;
    private float fireCoolDown = 0.8f;
    private const float FireCoolDownMax = 0.8f;

    private const float RightIslandX = 29.8f, MiddleIslandX = -1.8f, LeftIslandX = -29.9f;
    // private const float MiddleIslandX = -1.8f;
    // private const float LeftIslandX = -29.3f;

    private bool isBeaten;
    private Transform pointer;
    private const float PointerSpeed = 150, PointerSpeedLoseControl = 500;
    private bool isTurnRight;
    private SpriteRenderer spriteRenderer;

    private bool freezeOrNot;
    private Action[] randomDirections;
    private bool isUnderControl = true;
    private const float LoseControlTime = 5;
    private bool won;
    private int winner;
    
    private bool pointerIsUnderControl = true;
    private const float PointerLoseControlTime = 5;
    
    private static GameObject[] _bombsTypes;
    private static bool _hasLoaded;
    private const string BombsFolder = "Bombs";

    public Animator animator;
    private bool isLongPress;

    static void LoadSprites()
    {
        _bombsTypes = Resources.LoadAll<GameObject>(BombsFolder);
        _hasLoaded = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        won = false;
        winner = 0;
        if (!_hasLoaded)
            LoadSprites();
        
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
        isLongPress = t.parent.parent.GetComponentInParent<GameController>().IsLongPress();

        // Initializing the Bombs for each Grandma
        curBombs = 0;
        loadingBombsPercentage = (FireCoolDownMax - fireCoolDown) / FireCoolDownMax;
        bombs = new BombManager[MaxBombs]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
        for (int i = 0; i < MaxBombs; i++)
        {
            GameObject bombToLoad = _bombsTypes[Random.Range(0, _bombsTypes.Length)];
            GameObject temp = Instantiate(bombToLoad, pointer.position, Quaternion.identity, t);
            if (temp == null)
            {
                throw new NullReferenceException("Bomb Prefab Not Found!");
            }


            bombs[i] = temp.GetComponent<BombManager>();
            bombs[i].SetId(Random.Range(0, NumBombsEffects));
            
            // Sets each bomb's shooter, blue grandma ot red one
            bombs[i].SetShooterId(id);
        }
        StartCoroutine(Move());
    }

    void Update()
    {
        if (!isBeaten)
        {
            SetMoveDirection();
            PointerMove();
        }
        
        fireCoolDown -= Time.deltaTime;
        fireCoolDown = fireCoolDown < 0 ? 0 : fireCoolDown;
        float temp = (FireCoolDownMax - fireCoolDown) / FireCoolDownMax;
        loadingBombsPercentage = isBeaten ? 0 : temp >= 1 ? 1 : temp;

        bool blueFired = id == 1 && Input.GetKeyDown(KeyCode.G);
        bool redFired = id == 2 && Input.GetKeyDown(KeyCode.L);
        if ((blueFired || redFired) && curBombs < MaxBombs && loadingBombsPercentage >= 1.0f)
            Fire();
        
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
            //t.Rotate(Vector3.up, 180);
            //PointerTurn();
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isTurnRight = true;
        }
    }

    private void MoveLeft()
    {
        moveDirection = Vector3.left;
        if (isTurnRight)
        {
            //t.Rotate(Vector3.up, 180);
            //PointerTurn();
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isTurnRight = false;
        }
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

    private IEnumerator DelayForShootingAnimator(int i)
    {
        animator.SetBool("isShooting", true);
        //print("Before Shooting: " + animator.GetBool("isShooting"));
        bombs[i].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        
        animator.SetBool("isShooting", false);
        //print("After Shooting: " + animator.GetBool("isShooting"));
    }
    
    private void Fire()
    {
        throwSound.Play();
        int i = Random.Range(0, MaxBombs);
        while (bombs[i].gameObject.activeInHierarchy)
            i = Random.Range(0, MaxBombs);

        bombs[i].transform.position = GetPointerPosition();
        bombs[i].SetDirection((GetPointerPosition() - t.position).normalized);
        StartCoroutine(DelayForShootingAnimator(i));
        curBombs++;
        fireCoolDown = fireCollDownTime;
        // throwSound.Stop();
    }

    public GameObject GetBombParent()
    {
        return bombParent;
    }
    

    private void ReturnLastIsland(int carId)
    {
        Vector3 position = t.position;
        switch (id)
        {
            case 2:
                t.position = carId switch
                {
                    < 1 => new Vector3(LeftIslandX, position.y, position.z),
                    < 5 => new Vector3(MiddleIslandX, position.y, position.z),
                    < 9 => new Vector3(RightIslandX, position.y, position.z),
                    _ => startPosition
                };
                if (isTurnRight)
                {
                    //t.Rotate(Vector3.up, 180);
                    //PointerTurn();
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                    isTurnRight = false;
                }

                break;
            case 1:
                t.position = carId switch
                {
                    > 8 => new Vector3(RightIslandX, position.y, position.z),
                    > 4 => new Vector3(MiddleIslandX, position.y, position.z),
                    > 0 => new Vector3(LeftIslandX, position.y, position.z),
                    _ => startPosition
                };

                if (!isTurnRight)
                {
                    //t.Rotate(Vector3.up, 180);
                    //PointerTurn();
                    spriteRenderer.flipX = !spriteRenderer.flipX;
                    isTurnRight = true;
                }

                break;
        }
    }

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
        // Debug.Log("move");
        moveDirection = Vector3.zero;
        StartCoroutine(Move());
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
            /*case FreezeInPlace:
                dizzySound.Play();
                animator.SetBool("Freeze", true);
                StartCoroutine(FreezeMovement());
                animator.SetBool("Freeze", false);
                // dizzySound.Stop();
                break;
            case CrazyPointer:
                animator.SetBool("FastArrow", true);
                PointerLoseControl();
                animator.SetBool("FastArrow", false);
                break;*/
            case GoBackToStart:
                //dizzySound.Play();
                hitByBombSound.Play();
                //animator.SetBool("Dead", true);
                t.position = startPosition;
                switch (id)
                {
                    case 1 when !isTurnRight:
                        //t.Rotate(Vector3.up, 180);
                        //PointerTurn();
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                        isTurnRight = true;
                        break;
                    case 2 when isTurnRight:
                        //t.Rotate(Vector3.up, 180);
                        //PointerTurn();
                        spriteRenderer.flipX = !spriteRenderer.flipX;
                        isTurnRight = false;
                        break;
                }
                
                InitPointerPosition();
                pointer.gameObject.SetActive(false);
                isBeaten = true;
                moveDirection = Vector3.zero;
                StartCoroutine(Recovery());
                animator.SetBool("Dead", false);
                
                // animator.SetBool("LastIsland", true);
                // GoBack();
                // StartCoroutine(Recovery());
                // animator.SetBool("LastIsland", false);
                break;
            /*case CrazyDirections:
                confusedSound.Play();
                animator.SetBool("Confused", true);
                LoseControl();
                animator.SetBool("Confused", false);
                break;*/
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name.StartsWith("Car"))
        {
            hitByCarSound.Play();
            // return to last island
            ReturnLastIsland(collision.gameObject.GetComponent<Car>().GetId());
            //GoBack();
            pointer.gameObject.SetActive(false);
            isBeaten = true;
            moveDirection = Vector3.zero;
            StartCoroutine(Recovery());
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        BombManager curBomb = col.gameObject.GetComponent<BombManager>();
        if (col.gameObject.CompareTag("Bomb") && curBomb.GetShooterId() != id)
            HitByBomb(curBomb.GetId());

        string objectName = col.gameObject.name;
        if (!won && objectName.EndsWith("flag HD"))
        {
            if (objectName.StartsWith("purple") && id == 1)
            {
                won = true;
                winner = id;
            }else if (objectName.StartsWith("yellow") && id == 2)
            {
                won = true;
                winner = id;
            }
        }
    }

    public void StartGame()
    {
        t.position = startPosition;
        if (isTurnRight && id == 2 || !isTurnRight && id == 1)
        {
            //t.Rotate(Vector3.up, 180);
            //PointerTurn();
            spriteRenderer.flipX = !spriteRenderer.flipX;
            isTurnRight = !isTurnRight;
        }
        for (int i = 0; i < MaxBombs; i++)
        {
            bombs[i].gameObject.SetActive(false);
        }
        fireCoolDown = 0.8f;
        isBeaten = false;
        isUnderControl = true;
        pointerIsUnderControl = true;
        won = false;
        winner = 0;
        freezeOrNot = false;
    }
    
    // return the id winner, 0 if game not over yet
    public int WhoWon()
    {
        return winner;
    }
    
}