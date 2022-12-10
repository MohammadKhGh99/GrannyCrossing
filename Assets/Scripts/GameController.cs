using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject carsParent;
    [SerializeField] private float fieldLimit = 30;
    [SerializeField] private bool controlCarsPositions;
    [SerializeField] private float[] carsPositions;

    private const int MaxCars = 10;
    // public static float FieldLimit = fieldLimit == 0 ? 30 : FieldLimit;

    private readonly Car[] cars = new Car[MaxCars];
    private float[] carsPos = {-35.5f, -25f, -19.8f, -14.2f, -8.7f, 6.1f, 11.6f, 16.9f, 22.1f, 35.5f};
    private readonly int[] downToUpCars = { 0, 2, 4, 6, 8};
    private readonly string[] carsToLoad = { "Car", "Car 1", "Car 2", "Car 3" };
    private int fpsCounter;
    private float fpsTime;
    

    // Start is called before the first frame update
    void Start()
    {
        if (controlCarsPositions)
            carsPos = carsPositions;
        
        // todo - Ask designers if there is a need for this command call!!!
        // Screen.SetResolution(1920, 1080, true);
        
        for (int i = 0; i < MaxCars; i++)
        {
            var curPos = i % 2 == 0 ? new Vector2(carsPos[i], -fieldLimit) : new Vector2(carsPos[i], fieldLimit);
            Quaternion carDirection = i % 2 == 0 ? Quaternion.AngleAxis(180, Vector3.right) : Quaternion.identity;
            string carToLoad = carsToLoad[Random.Range(0, 4)];
            GameObject temp = Instantiate(Resources.Load(carToLoad), curPos, carDirection, carsParent.transform) as GameObject;
            
            if (temp == null)
            {
                throw new NullReferenceException("Car Prefab Not Found!");
            }
            
            cars[i] = temp.GetComponent<Car>();
            cars[i].SetDirection(i % 2 == 0 ? Vector3.up : Vector3.down);
            cars[i].SetId(i);
        }
        fpsCounter = 0;
        fpsTime = 1;
    }

    public float GetFieldLimit()
    {
        return fieldLimit;
    }

    // Update is called once per frame
    // void Update()
    // {
    // }
}