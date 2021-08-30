
using UnityEngine;

public class earthquake : MonoBehaviour
{
    public float QuakeForce = 10f;
    public float QuakeRate = 1;
    public TraumaInducer traumaScript;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("k"))
        {
            InvokeRepeating(nameof(Quake), 0f, QuakeRate);
            InvokeRepeating(nameof(disable), 1f, QuakeRate);
        }
    }
    void Quake()
    {
        traumaScript.enabled = true;
        Rigidbody2D[] physicsBois = FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D boi in physicsBois)
        {
            Debug.Log(boi.gameObject.name);
            boi.AddForce(new Vector2(Mathf.PerlinNoise(Random.Range(-100, 100), Random.Range(-100, 100)) * QuakeForce * Random.Range(-2, 2), Mathf.PerlinNoise(Random.Range(-100, 100), Random.Range(-100, 100)) * QuakeForce * Random.Range(-2, 2)));
        }
    }
    private void disable()
    {
        traumaScript.enabled = false;
    }
}
