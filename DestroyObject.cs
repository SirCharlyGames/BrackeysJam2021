using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyObject : MonoBehaviour
{
    public float time = 1;

    // Start is called before the first frame update
    void Start()
    {
        //run "Kill" after time seconds
        Invoke("Kill", time);
    }

    void Kill()
    {
        //destroy the attached gameobject
        Destroy(gameObject);
    }
}
