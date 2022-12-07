// using System;
// using System.Collections;
// using System.Collections.Generic;
// using Unity.VisualScripting;
// using UnityEngine;
//
// public class GrandmotherHelper : MonoBehaviour
// {
//     [SerializeField] private int id;
//     [SerializeField] private float movementTime = 0.25f;
//     [SerializeField] private float movementDistance = 0.08f;
//     [SerializeField] private GameObject grandma;
//
//     private Vector3 moveDirection;
//
//     private Transform t;
//     private bool carHit;
//
//     private GameObject[] bombs;
//     private int numBombs;
//     public static GameObject[] Grandmas = new GameObject[2];
//
//     private void SetMoveDirection()
//     {
//         switch (id)
//         {
//             case 1:
//                 if (Input.GetKeyDown(KeyCode.W))
//                 {
//                     moveDirection = Vector3.right;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.S))
//                 {
//                     moveDirection = Vector3.left;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.A))
//                 {
//                     moveDirection = Vector3.forward;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.D))
//                 {
//                     moveDirection = Vector3.back;
//                 }
//
//                 break;
//             case 2:
//                 if (Input.GetKeyDown(KeyCode.UpArrow))
//                 {
//                     // moveDirection = Vector3.up;
//                     //todo FOR 3D
//                     moveDirection = Vector3.right;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.DownArrow))
//                 {
//                     // moveDirection = Vector3.down;
//                     //todo FOR 3D -
//                     moveDirection = Vector3.left;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.LeftArrow))
//                 {
//                     // moveDirection = Vector3.left;
//                     //todo FOR 3D -
//                     moveDirection = Vector3.forward;
//                 }
//
//                 if (Input.GetKeyDown(KeyCode.RightArrow))
//                 {
//                     // moveDirection = Vector3.right;
//                     //todo FOR 3D -
//                     moveDirection = Vector3.back;
//                 }
//
//                 break;
//         }
//     }
//
//     IEnumerator Move()
//     {
//         yield return new WaitForSeconds(movementTime);
//         t.position += moveDirection * movementDistance;
//         moveDirection = Vector3.zero;
//     }
//
//     // Start is called before the first frame update
//     void Start()
//     {
//         Grandmas[id - 1] = grandma;
//         bombs = new GameObject[6]; // Jewelry, shoe, teeth, medicine, phone, radio, todo etc
//         carHit = false;
//         moveDirection = Vector3.zero;
//         t = GetComponent<Transform>();
//         numBombs = 0;
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         SetMoveDirection();
//         StartCoroutine(Move());
//         if (id == 2)
//         {
//             if (Input.GetKeyDown(KeyCode.Space) && numBombs < 6)
//             {
//                 // Vector3 cur = t.position;
//                 bombs[numBombs] =
//                     Instantiate(Resources.Load("Bomb"), grandma.transform.position, Quaternion.identity, t) as
//                         GameObject;
//                 // bombs[numBombs].transform.SetParent(t);
//                 // bombs[numBombs].transform.position = new Vector3(cur.x, 0, 0);
//                 numBombs++;
//             }
//         }
//     }
//
//     // private void OnTriggerEnter(Collider other)
//     // {
//     //     Debug.Log(other.gameObject.name);
//     // }
//
//     private void OnCollisionEnter(Collision collision)
//     {
//         Debug.Log(collision.gameObject.name);
//         if (collision.collider.name.StartsWith("Car"))
//         {
//             carHit = true;
//         }
//
//         if (collision.collider.name.StartsWith("Bottom") && carHit)
//         {
//             if (carHit && transform.position.x <= -5)
//             {
//                 transform.position = id == 1 ? new Vector3(0, 0, -4.5f) : new Vector3(0, 0, 4.5f);
//             }
//         }
//         // if (collision.gameObject.name.StartsWith("Car"))
//         // {
//         //     Debug.Log("Car trigger");
//         //     switch (id)
//         //     {
//         //         case 1:
//         //             t.position += Vector3.left;
//         //             break;
//         //         case 2:
//         //             t.position += Vector3.right;
//         //             break;
//         //         
//         //     }
//         // }
//     }
//
//     private void OnCollisionExit(Collision other)
//     {
//         carHit = false;
//     }
// }