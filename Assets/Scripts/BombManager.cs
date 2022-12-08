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
        Transform parent = t.parent;
        parentGrandma = parent.GetComponent<Grandmother>();
        direction = parentGrandma.GetPointerPosition() - parent.position;
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
        grandmother.GetComponent<Grandmother>().GoBack();
        StartCoroutine(grandmother.GetComponent<Grandmother>().Recovery());
        t.gameObject.SetActive(false);
        isFired = false;
        t.parent.GetComponent<Grandmother>().AddToCurBombs(-1);
    }
}