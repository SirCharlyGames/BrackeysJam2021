using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class GameController : MonoBehaviour
{
    public float duplicate_cooldown = 1f;
    private float timer = 0;
    public GameObject[] prefabs;
    public GameObject[] SpawnParticles;
    private string crate = "crate";
    private string enemy_string = "enemy";
    private string chip = "chip";
    private string goal = "goal";
    private string duplicable = "Duplicable";
    public int points = 0;
    public int pointsNeeded;
    public GameObject player;
    private bool spawned = false;
    [HideInInspector] public bool duplicate_enabled = true;
    private Vector3 pos;
    [HideInInspector] public List<GameObject> enemy_objects;
    private bool chip_exists = false;
    private bool enemy_exists = false;
    public Text high_score;
    private void Start()
    {
        enemy_objects = new List<GameObject>();
        high_score.text = "High Score: "  + PlayerPrefs.GetInt("HighScore",0).ToString();
    }
    public void Duplicate(GameObject duplicable)
    {
        if (Time.time >= duplicate_cooldown + timer && duplicate_enabled) //can duplicate
        {
            switch (duplicable.name)
            {
                //crate
                case "crate":
                    pos = new Vector3(duplicable.transform.position.x + Random.Range(-1, 1), duplicable.transform.position.y + Random.Range(0, 1), 0); //get a random position near the collision
                    var dup = Instantiate(prefabs[0], pos, Quaternion.identity); //instantiate the dupe
                    Instantiate(SpawnParticles[0], pos, Quaternion.identity);
                    dup.name = crate;
                    break;
                //enemy
                case "enemy":
                    StartCoroutine(enemy_spawner(0.5f));
                    enemy_exists = true;
                    break;
                 //chip
                 case "chip":
                     StartCoroutine(chip_spawner(0.5f));
                     chip_exists = true;
                     break;
             }
             timer = Time.time;            
        }
    }
    public void addPoints(int amountToAdd, GameObject objectTouched,GameObject who_touched)
    {
        if(who_touched == player) points += amountToAdd;
        Instantiate(SpawnParticles[1], objectTouched.transform.position, Quaternion.identity);
        Destroy(objectTouched);
    }
    private void Update()
    {
        GameObject[] dups = GameObject.FindGameObjectsWithTag(duplicable); 
        var count = dups.Length;
        if(count >= 10)
        {
            duplicate_enabled = false; //stop duplicating if max reached
        }
        else
        {
            duplicate_enabled = true;
        }
        if(GameObject.FindGameObjectWithTag(goal) == null && chip_exists == false) //if theres no chip in the scene, spawn one
        {
            StartCoroutine(chip_spawner(0.5f));
            chip_exists = true;
        }
        if (GameObject.FindGameObjectsWithTag(duplicable).Length < 3 && enemy_exists == false) //if theres no enemy in the scene, spawn one
        {
            StartCoroutine(enemy_spawner(0.5f));
            enemy_exists = true;
        }
    }
    IEnumerator chip_spawner(float seconds)
    {
        //spawn another chip after "seconds"
        yield return new WaitForSeconds(seconds);
        spawned = false;
        while (!spawned) //loop until the object spawns away from the character
        {
            pos = new Vector3(Random.Range(-3.5f, 3), Random.Range(0, 3), 0);
            if ((pos - player.transform.position).magnitude < 1.5)
            {
                continue;
            }
            else
            {
                var dup = Instantiate(prefabs[2], pos, Quaternion.identity); //instantiate the dupe
                Instantiate(SpawnParticles[0], pos, Quaternion.identity);
                dup.name = chip;
                spawned = true;
                chip_exists = false;
            }
        }
    } 
    IEnumerator enemy_spawner(float seconds)
    {
        //spawn another enemy after "seconds"
        yield return new WaitForSeconds(seconds);
        spawned = false;
        while (!spawned) //loop until the object spawns away from the character
        {
            pos = new Vector3(Random.Range(-3.5f, 3), Random.Range(0, 3), 0);
            if ((pos - player.transform.position).magnitude < 1.5)
            {
                continue;
            }
            else
            {
                enemy_objects.Add((GameObject)Instantiate(prefabs[1],pos, Quaternion.identity));
                Instantiate(SpawnParticles[0], pos, Quaternion.identity);
                var counter = enemy_objects.Count;
                var item = counter - 1;
                enemy_objects[item].name = enemy_string;
                spawned = true;
                enemy_exists = false;
            }
        }
    }
    public void EndGame()
    {
        if(points > PlayerPrefs.GetInt("HighScore",0))
        {
            //save high score
            PlayerPrefs.SetInt("HighScore", points); 
            high_score.text = points.ToString();
        }
        Restart();
    }
    public void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }
}
