using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
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
    private Image imageStartGame;
    private Image imagePlayer1Won;
    private Image imagePlayer2Won;
    private bool isGameRunning;
    private bool isGameOver;
    

    static void LoadCarsSprites()
    {
        CarsTypes = Resources.LoadAll<Sprite>(CarsFolder);
        NumCarsTypes = CarsTypes.Length;
        _hasLoaded = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        isGameOver = false;
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
        
        startGameCanvas = transform.GetChild(0).gameObject;
        imageStartGame = startGameCanvas.GetComponent<Transform>().GetChild(0).GetComponent<Image>();
        
        player1WonCanvas = transform.GetChild(1).gameObject;
        imagePlayer1Won = player1WonCanvas.GetComponent<Transform>().GetChild(0).GetComponent<Image>();
        
        player2WonCanvas = transform.GetChild(2).gameObject;
        imagePlayer2Won = player2WonCanvas.GetComponent<Transform>().GetChild(0).GetComponent<Image>();
        GameObject players = transform.GetChild(4).gameObject;
        grandmothers = new []{players.transform.GetChild(0).GetComponent<Grandmother>(), players.transform.GetChild(1).GetComponent<Grandmother>()};
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
        if ((Input.GetKey(KeyCode.Space) && isGameOver) || Input.GetKey(KeyCode.Escape))
        {
            StartCoroutine(FadeOut(imagePlayer1Won));
            StartCoroutine(FadeOut(imagePlayer2Won));
    
            //player1WonCanvas.SetActive(false);
            //player2WonCanvas.SetActive(false);
            foreach (var granny in grandmothers)
                granny.StartGame();

            StartCoroutine(FadeIn(imageStartGame));

            isGameRunning = false;
            isGameOver = false;
            return;
        }
        if (Input.anyKeyDown && !isGameRunning)
        {
            //startGameCanvas.SetActive(false);
            StartCoroutine(FadeOut(imageStartGame));
            foreach (var granny in grandmothers)
                granny.StartGame();

            isGameRunning = true;
        }
        
        if (grandmothers[0].WhoWon() == 1)
        {
            print("Win");
            isGameOver = true;
            StartCoroutine(FadeIn(imagePlayer1Won));
        }
        if (grandmothers[1].WhoWon() == 2)
        {
            print("Win");
            isGameOver = true;
            StartCoroutine(FadeIn(imagePlayer2Won));
        }
    }
    
    private IEnumerator FadeOut(Image image)
    {
        Color c = image.color;
        
        for (float i = 0.25f; i >= 0; i -= Time.deltaTime) 
        {
            image.color = new Color(c.r, c.g, c.b, i * 4);
            yield return null;
        }
        image.gameObject.SetActive(false);
    }
    
    private IEnumerator FadeIn(Image image)
    {
        image.gameObject.SetActive(true);
        Color c = image.color;
        for (float i = 0; i <= 0.25f; i += Time.deltaTime) 
        {
            image.color = new Color(c.r, c.g, c.b, i * 4);
            yield return null;
        }
        image.color = new Color(c.r, c.g, c.b, 1);
    }
}