using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNTrajectory : MonoBehaviour
{
    public float power = 1f;
    Rigidbody2D rb;
    LineRenderer lr;
    Vector2 DragStartPos;
    public SlowMotion slowMotion;
    Vector3 lastVelocity;
    public GameObject explosion;
    float velocity = 1f;
    bool firing = false;
    
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        lr = GetComponent<LineRenderer>();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        }
        if (Input.GetMouseButton(0))
        {
            Vector2 DragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 _velocity = (DragStartPos - DragEndPos) * power;
            
            Vector2[] trajectory = Plot(rb, (Vector2)transform.position,_velocity ,1600);
            lr.positionCount = trajectory.Length;
            Debug.Log(_velocity);
            Vector3[] positions = new Vector3[trajectory.Length];
            for(int i=0; i < positions.Length; i++)
            {
                positions[i] = trajectory[i];
            }
            lr.SetPositions(positions);
            
            slowMotion.DoSlowotion();
            firing = true;
            if(firing == true)
            {
                Camera.main.orthographicSize = Mathf.SmoothDamp(Camera.main.orthographicSize, 20f, ref velocity, 0.05f);
            }

        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 DragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 _velocity = (DragStartPos-DragEndPos) * power;
            rb.velocity = _velocity;
            lr.positionCount = 0;

            firing = false;
            if (firing == false)
            {
                Camera.main.orthographicSize = 13f;
            }






        }
        lastVelocity = rb.velocity;
        
    }
    public Vector2 [] Plot (Rigidbody2D rigidbody, Vector2 pos , Vector2 velocity, int steps)
    {
        Vector2[] results = new Vector2[steps];
        float timestep = Time.fixedDeltaTime / Physics2D.velocityIterations;
        Vector2 gravityAccel = Physics2D.gravity * rigidbody.gravityScale * timestep * timestep;
        float drag = 1f - timestep * rigidbody.drag;
        Vector2 movestep = velocity * timestep;
        for(int i =0; i<steps; i++)
        {
            movestep += gravityAccel;
            movestep *= drag;
            pos += movestep;
            results[i] = pos;

        }
        return results;

        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "target")
        {
            Instantiate(explosion, collision.transform.position, Quaternion.identity);
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "Wall")
        {
            var speed = lastVelocity.magnitude/2;
            var direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);
            rb.velocity = direction * Mathf.Max(speed,0f);
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "target")
        {

            rb.AddForce(transform.up*90, ForceMode2D.Impulse);
            
            
        }

        
        

    }

}
