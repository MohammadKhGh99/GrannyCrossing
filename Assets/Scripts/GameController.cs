using System;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject carsParent;

    private const int MaxCars = 10;

    private readonly Car[] cars = new Car[MaxCars];
    private readonly float[] carsPos = {-35.5f, -25f, -19.8f, -14.2f, -8.7f, 6.1f, 11.6f, 16.9f, 22.1f, 35.5f};
    private readonly int[] downToUpCars = { 0, 2, 4, 6,8};
    private int fpsCounter;
    private float fpsTime;
    [SerializeField] private float fieldLimit = 30;

    // Start is called before the first frame update
    void Start()
    {
        carsParent = gameObject;
        Quaternion carDirection;
        Screen.SetResolution(1920, 1080, true);
        for (int i = 0; i < MaxCars; i++)
        {
            var curPos = downToUpCars.Contains(i) ? new Vector3(carsPos[i], -fieldLimit, 0) : new Vector3(carsPos[i], fieldLimit, 0);
            carDirection = downToUpCars.Contains(i) ? Quaternion.AngleAxis(180, Vector3.right) : Quaternion.identity;
            GameObject temp = Instantiate(Resources.Load("Car"), curPos, carDirection, transform) as GameObject;
            print(temp.transform.position);
            if (temp == null)
            {
                throw new NullReferenceException("Car Prefab Not Found!");
            }
            cars[i] = temp.GetComponent<Car>();
            cars[i].SetDirection(downToUpCars.Contains(i) ? Vector3.up : Vector3.down);
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
    void Update()
    {
        fpsCounter++;
        fpsTime -= Time.deltaTime;
        
        // Showing fps value
        if (fpsTime <= 0)
        {
            Debug.Log("FPS is: " + fpsCounter);
            fpsCounter = 0;
            fpsTime = 1;
        }
    }
}