using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviour : MonoBehaviour
{
    public float moveSpeed;

    private float Move;

    public float jump;

    public bool isJumping;

    private Rigidbody2D rb;
    private Vector2 oldPos;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move = Input.GetAxis("Horizontal");

        oldPos = rb.position;
        rb.MovePosition(new Vector2(rb.position.x + moveSpeed * Move, rb.position.y + -0.05f));
        //rb.position += new Vector2(moveSpeed * Move, -0.05f);
        //rb.rotation = 0;
        //transform.position += new Vector3(moveSpeed * Move, -0.05f, 0);

        if (Input.GetButtonDown("Jump") && isJumping == false)
        {
            rb.AddForce(new Vector2(rb.velocity.x, jump));
            //rb.AddTorque(-18.0f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            //var contactPt = other.GetContact(0);
            //contactPt

            //rb.position = new Vector2(rb.position.x, 0);
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }
}