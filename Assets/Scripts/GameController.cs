using System;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject carsParent;

    private const int MaxCars = 8;

    private readonly Car[] cars = new Car[MaxCars];
    private readonly float[] carsPos = {-31.5f, -20.5f, -15.5f, -5, 5, 15.5f, 20.5f, 31.5f};
    private readonly int[] downToUpCars = { 0, 2, 5, 7 };
    private int fpsCounter;
    private float fpsTime;

    // Start is called before the first frame update
    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        for (int i = 0; i < MaxCars; i++)
        {
            var curPos = downToUpCars.Contains(i) ? new Vector3(carsPos[i], -16.5f, 0) : new Vector3(carsPos[i], 16.5f, 0);
            
            GameObject temp = Instantiate(Resources.Load("Car"), curPos, Quaternion.identity, carsParent.transform) as GameObject;
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