
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public float MoveForce = 100f;
    public float jumpForce = 5f;
    Rigidbody2D rb;
    public Transform groundcheck;
    bool grounded = false;

    public GameController gamecontroller;
    private string duplicable = "Duplicable";
    private string goal = "goal";

    // Start is called before the first frame update
    void Start()
    {
        // assing rb this object's rigidbody2D
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //if the circel below the player is overlapping something
        if (Physics2D.OverlapCircle(groundcheck.position, 0.2f, 9))
        {
            // make it known that the player is on the ground
            grounded = true;
        }
        else
        {
            //if not the player is in the air
            grounded = false;
        }
        //get direction of horizontal input (a, d, or, <-, ->)
        float InputX = Input.GetAxis("Horizontal");

        // multiply input direction by moveforce
        float MoveX = InputX * MoveForce;

        //change x velocity to move character
        rb.velocity = (new Vector2(MoveX, rb.velocity.y));

        //check for space key or a key on controller and if the player is on the ground
        if (Input.GetButtonDown("Jump") && grounded)
        {
            rb.velocity = (new Vector2(MoveX, jumpForce));
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(duplicable)) //the player collides with a duplicable object
        {
            //Debug.Log("the player has collided with the enemy");
            gamecontroller.Duplicate(collision.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(goal))
        {
            gamecontroller.Duplicate(collision.gameObject);
            gamecontroller.addPoints(1, collision.gameObject,this.gameObject);
        }
    }
}
