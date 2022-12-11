using System;
using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;

    private int shooterId;
    private Transform t;
    private Vector3 direction;
    private const float Speed = 18;
    // private bool isFired;
    private float bombLifeTime = 4;
    private Grandmother parentGrandma;
    private int id;

    // Start is calledbefore the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        Transform parent = t.parent;
        parentGrandma = parent.GetComponent<Grandmother>();
        direction = parentGrandma.GetPointerPosition() - parent.position;
        direction = direction.normalized;
        t.parent = t.parent.parent;
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (Speed * Time.deltaTime);
        t.Rotate(Vector3.forward, 1.0f);
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int other)
    {
        id = other;
    }
    
    public void Fire()
    {
        t.gameObject.SetActive(true);
        t.position = parentGrandma.GetPointerPosition();
        direction = parentGrandma.GetPointerPosition() - parentGrandma.transform.position;
        direction = direction.normalized;
        StartCoroutine(KillBomb());
    }

    private IEnumerator KillBomb()
    {
         yield return new WaitForSeconds(bombLifeTime);
         t.gameObject.SetActive(false);
         parentGrandma.AddToCurBombs(-1);
     }

    public int GetShooterId()
    {
        return shooterId;
    }

    public void SetShooterId(int other)
    {
        shooterId = other;
    }

    // public void ActivateBomb(GameObject grandmother)
    // {
    //     // isFired = false;
    //     // t.parent.GetComponent<Grandmother>().AddToCurBombs(-1);
    //     grandmother.GetComponent<Grandmother>().GoBack();
    //     t.gameObject.SetActive(false);
    // }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name.Equals("Grass"))
        {
            parentGrandma.AddToCurBombs(-1);
            t.gameObject.SetActive(false);
        }
    }
}