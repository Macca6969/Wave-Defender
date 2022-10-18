using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamagePopUp : MonoBehaviour
{
    public float destroyTime;
    public Vector3 offset = new Vector3(0, 5, 0);
    public Vector3 startPosition;
    public Vector3 random;
    private GameManager gameManager;
    private Transform target;

    private void Start()
    {
        random = new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f));
        Debug.Log(random);
        startPosition = (transform.position + offset) + random;
        transform.position = startPosition;
        Destroy(gameObject, destroyTime);

        GameObject findPlayer = GameObject.FindGameObjectWithTag("LocalPlayer");
        Vector3 lookDirection = (findPlayer.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(lookDirection.x, 0, lookDirection.z) * -1);
        transform.rotation = lookRotation;
    }
}

