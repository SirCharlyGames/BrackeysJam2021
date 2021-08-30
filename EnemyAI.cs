using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyAI : MonoBehaviour
{
    [Header("Pathfinding")]
    public float activateDistance = 50f;
    public float pathUpdateSeconds = 0.5f;
    [HideInInspector] private GameObject target;

    [Header("Physics")]
    public float speed = 200f;
    public float nextWaypointDistance = 3f;
    public float jumpNodeHeightRequirement = 0.8f;
    public float jumpModifier = 0.3f;
    public float jumpCheckOffset = 0.1f;

    [Header("Custom Behavior")]
    public bool followEnabled = true;
    public bool jumpEnabled = true;
    public bool directionLookEnabled = true;
    private Path path;
    private int currentWaypoint = 0;
    public bool isGrounded = false;
    private Seeker seeker;
    private Rigidbody2D rb;

    private GameObject gamecontroller;
    private GameController gamecontroller_script;
    private string goal = "goal";
    private void Awake()
    {
        gamecontroller = GameObject.Find("gamecontroller");
        gamecontroller_script = gamecontroller.GetComponent<GameController>();
    }
    private void Start()
    {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSeconds);
        StartCoroutine(destroy(3));
       
    }
    private void Update()
    { 
        if(target == null)
        {
            //find a new chip to follow
            target = GameObject.FindGameObjectWithTag(goal);
        }
    }
    private void FixedUpdate()
    {
        if(TargetInDistance() && followEnabled)
        {
            PathFollow();
        }
    }
    private void UpdatePath()
    {
        if (followEnabled && TargetInDistance() && seeker.IsDone())
        {
            seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
        }
    }
    private void PathFollow()
    {
        if(path == null)
        {
            return;
        }

        //Reached end of path
        if(currentWaypoint >= path.vectorPath.Count)
        {
            return;
        }

        //see if colliding with anything
        isGrounded = Physics2D.Raycast(transform.position, -Vector3.up, GetComponent<Collider2D>().bounds.extents.y + jumpCheckOffset);

        // direction calculation
        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        //jump
        if(jumpEnabled && isGrounded)
        {
            if(direction.y > jumpNodeHeightRequirement)
            {
                rb.AddForce(Vector2.up * speed * jumpModifier);
            }
        }
        //movement
        rb.AddForce(force);
        //next waypoint
        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if(distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }

        // direction graphics handling
        if(directionLookEnabled)
        {
            if(rb.velocity.x > 0.05f)
            {
                transform.localScale = new Vector3(-1f * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if(rb.velocity.x < -0.05f)
            {
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }
    }
    private bool TargetInDistance()
    {
        if(target != null)
        {
            return Vector2.Distance(transform.position, target.transform.position) < activateDistance;
        }
        else
        {
            return false;
        }
        
    }
    private void OnPathComplete(Path p)
    {
        if(!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //when enemy grabs a chip
        if(collision.gameObject.CompareTag(goal))
        {
            gamecontroller_script.Duplicate(collision.gameObject);
            gamecontroller_script.addPoints(1, collision.gameObject, this.gameObject);
        }
    }
    IEnumerator destroy(int seconds) //destroy enemies each 5 seconds
    {
        yield return new WaitForSeconds(seconds);
        Destroy(this.gameObject);
    }
    
}
