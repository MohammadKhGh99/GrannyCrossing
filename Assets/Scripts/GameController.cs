using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject carsParent;
    [SerializeField] private float fieldLimit = 30;
    [SerializeField] private bool controlCarsPositions;
    [SerializeField] private bool longPress;

    [SerializeField] private float[] carsPositions = { -36.8f, -25.5f, -20.4f, -15.2f, -10.1f, 10.1f, 15.2f, 20.3f, 25.6f, 37f };

    [SerializeField] private string[] carsDirections = { "up", "down", "up", "down", "up", "down", "up", "down", "up", "down" };

    private const int MaxCars = 10;

    private readonly Car[] cars = new Car[MaxCars];
    private float[] carsPos = { -36.8f, -25.5f, -20.4f, -15.2f, -10.1f, 10.1f, 15.2f, 20.3f, 25.6f, 37f };
    
    private int fpsCounter;
    private float fpsTime;

    public static Sprite[] CarsTypes;
    private static bool _hasLoaded;
    public static int NumCarsTypes;
    private const string CarsFolder = "Sprites/cars";

    private GameObject startGameCanvas;
    private GameObject player1WonCanvas;
    private GameObject player2WonCanvas;
    private Grandmother[] grandmothers;
    private bool isGameRunning;

    static void LoadCarsSprites()
    {
        CarsTypes = Resources.LoadAll<Sprite>(CarsFolder);
        NumCarsTypes = CarsTypes.Length;
        _hasLoaded = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameRunning = false;
        if (!_hasLoaded)
            LoadCarsSprites();

        if (controlCarsPositions)
            carsPos = carsPositions;

        // todo - Ask designers if there is a need for this command call!!!
        // Screen.SetResolution(1920, 1080, true);

        for (int i = 0; i < MaxCars; i++)
        {
            var curPos = carsDirections[i].Equals("up")
                ? new Vector2(carsPos[i], -fieldLimit)
                : new Vector2(carsPos[i], fieldLimit);
            Quaternion carDirection = carsDirections[i].Equals("up")
                ? Quaternion.AngleAxis(180, Vector3.right)
                : Quaternion.identity;

            GameObject temp =
                Instantiate(Resources.Load("Car"), curPos, carDirection, carsParent.transform) as GameObject;
            if (temp == null)
            {
                throw new NullReferenceException("Car Prefab Not Found!");
            }

            temp.GetComponent<SpriteRenderer>().sprite = CarsTypes[Random.Range(0, NumCarsTypes)];

            cars[i] = temp.GetComponent<Car>();
            cars[i].SetDirection(carsDirections[i]);
            cars[i].SetDirection(carsDirections[i].Equals("up") ? Vector3.up : Vector3.down);
            cars[i].SetId(i);
        }

        fpsCounter = 0;
        fpsTime = 1;
        GameObject tempGameObject = null;
        grandmothers = new Grandmother[2];
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).name.Equals("StartGame"))
            {
                startGameCanvas = transform.GetChild(i).gameObject;
            }
            
            if (transform.GetChild(i).name.Equals("Player1Won"))
            {
                
                player1WonCanvas = transform.GetChild(i).gameObject;
            }
            if (transform.GetChild(i).name.Equals("Player2Won"))
            {
                player2WonCanvas = transform.GetChild(i).gameObject;
            }

            if (transform.GetChild(i).name.Equals("Players"))
            {
                tempGameObject = transform.GetChild(i).gameObject;
            }
        }

        int n = 0;
        if (tempGameObject != null)
        {
            for (int i = 0; i < tempGameObject.transform.childCount; i++)
            {
                if (tempGameObject.transform.GetChild(i).name.EndsWith("Grandma"))
                {
                    grandmothers[n] = tempGameObject.transform.GetChild(i).GetComponent<Grandmother>();
                    n++;
                }
            }
        }
    }

    public float GetFieldLimit()
    {
        return fieldLimit;
    }

    public bool IsLongPress()
    {
        return longPress;
    }

    private void Update()
    {
        if (Input.anyKeyDown && !isGameRunning)
        {
            startGameCanvas.SetActive(false);
            for (int i = 0; i < grandmothers.Length; i++)
            {
                grandmothers[i].StartGame();
            }

            isGameRunning = true;
        }

        if (grandmothers[0].WhoWon() == 1)
        { 
            player1WonCanvas.SetActive(true);
        }
        if (grandmothers[0].WhoWon() == 2)
        { 
            player2WonCanvas.SetActive(true);
        }
        

        if (Input.GetKey(KeyCode.Escape))
        {
            player1WonCanvas.SetActive(false);
            player2WonCanvas.SetActive(false);
            startGameCanvas.SetActive(true);
            isGameRunning = false;
        }
    }
}