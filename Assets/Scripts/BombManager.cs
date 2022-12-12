using System;
using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    private int shooterId;
    private Transform t;
    private Vector3 direction;
    private const float Speed = 20;
    private Grandmother parentGrandma;
    private int id;
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        Transform parent = t.parent;
        parentGrandma = parent.GetComponent<Grandmother>();
        direction = parentGrandma.GetPointerPosition() - parent.position;
        direction = direction.normalized;
        t.SetParent(parentGrandma.GetBombParent().transform);
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (Speed * Time.deltaTime);
        t.Rotate(Vector3.forward, 1.0f);
    }

    public void SetDirection(Vector3 other)
    {
        direction = other;
    }

    public int GetId()
    {
        return id;
    }

    public void SetId(int other)
    {
        id = other;
    }

    public int GetShooterId()
    {
        return shooterId;
    }

    public void SetShooterId(int other)
    {
        shooterId = other;
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name.Equals("Grass"))
        {
            parentGrandma.AddToCurBombs(-1);
            t.gameObject.SetActive(false);
        }
        if ((shooterId == 1 && col.gameObject.name.Equals("RightGrandma")) ||
            (shooterId == 2 && col.gameObject.name.Equals("LeftGrandma")))
        {
            parentGrandma.AddToCurBombs(-1);
            t.gameObject.SetActive(false);
        }
    }
}