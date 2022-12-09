using System.Collections;
using UnityEngine;

public class BombManager : MonoBehaviour
{
    [SerializeField] private GameObject bomb;


    private int shooterId;
    private Transform t;
    private Vector3 direction;
    private float speed = 18;
    private bool isFired;
    private float bombLifeTime = 10;
    private Grandmother parentGrandma;
    

    public void Fire()
    {
        isFired = true;
        StartCoroutine(KillBomb());
    }

    private IEnumerator KillBomb()
    {
        yield return new WaitForSeconds(bombLifeTime);
        t.gameObject.SetActive(false);
        isFired = false;
        parentGrandma.AddToCurBombs(-1);
    }
    
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<Transform>();
        direction = t.parent.GetComponent<Grandmother>().GetPointerPosition() - t.parent.position;
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        t.position += direction * (speed * Time.deltaTime);
    }

    public int GetShooterId()
    {
        return shooterId;
    }

    public void SetShooterId(int other)
    {
        shooterId = other;
    }

    public void ActivateBomb(GameObject grandmother)
    {
        t.gameObject.SetActive(false);
        isFired = false;
        t.parent.GetComponent<Grandmother>().AddToCurBombs(-1);
        grandmother.GetComponent<Grandmother>().GoBack();
    }
}