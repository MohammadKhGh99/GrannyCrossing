// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class BombManagerHelper : MonoBehaviour
// {
//     [SerializeField] private GameObject bomb;
//
//     // [SerializeField] private GameObject goalPlayer;
//
//     private float goalPosZ;
//     
//     // Start is called before the first frame update
//     void Start()
//     {
//         // Vector3 curPos = bomb.transform.position;
//         // goalPos = new Vector3(curPos.x, 0, Random.Range(curPos.z, goalPlayer.transform.position.z));
//         float enemyZ;
//         if (bomb.transform.parent == Grandmother.Grandmas[0].transform)
//         {
//             enemyZ = Grandmother.Grandmas[1].transform.position.z;
//         }
//         else
//         {
//             enemyZ = Grandmother.Grandmas[0].transform.position.z;
//         }
//         goalPosZ = Random.Range(bomb.transform.position.z, enemyZ);
//         bomb.transform.SetParent(null);
//     }
//
//     // Update is called once per frame
//     void Update()
//     {
//         if (bomb.transform.position.z >= goalPosZ)
//         {
//             bomb.transform.position += Vector3.back * (10 * Time.deltaTime);
//         }
//         // else
//         // {
//         //     bomb.transform.SetParent(null);
//         // }
//     }
// }
