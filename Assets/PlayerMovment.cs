using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class NewBehaviour : MonoBehaviour
{
    public float moveSpeed;

    public float jumpSpeed;
    public float maxSpeed;
    public bool debugState;
    private float currentMove;
    private float colliderWidth,colliderHeight;

    Vector2 oldPos;


    private Rigidbody2D rb;

    private enum PLAYER_STATE
    {
        PS_IDLE = 0,
        PS_RUNNING,
        PS_JUMP,
        PS_INAIR,
        PS_JUMPLAND,
        PS_DOUBLEJUMPING
    }

    private enum FACING_DIR
    {
        FD_UNKNOWN = 0,
        FD_RIGHT = 1,
        FD_LEFT = -1,
    }
    private FACING_DIR FacingDir;

    private PLAYER_STATE currentState;
    private PLAYER_STATE prevState;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        var m_Collider = GetComponent<BoxCollider2D>();
        colliderWidth = m_Collider.bounds.size.x/2;
        colliderHeight = m_Collider.bounds.size.y;
    }

    void Update()
    {
        CheckInput();

        CheckInAir();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateFacingDir();

        // Update our state.
        UpdateState(Time.deltaTime);
    }

    void UpdateState(float uSec)
    {
        switch(currentState)
        {
            case PLAYER_STATE.PS_IDLE:

                //rb.AddRelativeForce(new Vector2(moveSpeed * currentMove, 0), ForceMode2D.Force);
            break;

            case PLAYER_STATE.PS_RUNNING:
            {
                rb.AddRelativeForce(new Vector2(moveSpeed * currentMove, rb.velocity.y), ForceMode2D.Force);
                //rb.AddRelativeForce(new Vector2(moveSpeed * currentMove, 0), ForceMode2D.Force);
            } break;

            case PLAYER_STATE.PS_JUMP:
            {
                rb.AddRelativeForce(new Vector2(rb.velocity.x, jumpSpeed), ForceMode2D.Impulse);
                //rb.AddRelativeForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
                SetState(PLAYER_STATE.PS_INAIR);

            } break;

            case PLAYER_STATE.PS_INAIR:
            case PLAYER_STATE.PS_JUMPLAND:
            {
                rb.AddRelativeForce(new Vector2(moveSpeed * (currentMove/2), rb.velocity.y), ForceMode2D.Force);
            } break;

            case PLAYER_STATE.PS_DOUBLEJUMPING:
            {

            } break;
        }

        // Speed Limit X
        //
        if( rb.velocity.magnitude > maxSpeed )
        {
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);
        }
    }

    void UpdateFacingDir()
    {
        if( rb.velocity.normalized.x > 0 )
            FacingDir = FACING_DIR.FD_RIGHT;
        else
        if( rb.velocity.normalized.x < 0 )
            FacingDir = FACING_DIR.FD_LEFT;
    }

    void CheckInput()
    {
        currentMove = Input.GetAxisRaw("Horizontal");
        if(currentMove == 0.0f)
        {
            SetState(PLAYER_STATE.PS_IDLE);
        }
        else
        {
            SetState(PLAYER_STATE.PS_RUNNING);
        }

        if(Input.GetButtonDown("Jump"))
        {
            SetState(PLAYER_STATE.PS_JUMP);
        }
    }

    void CheckInAir()
    {
        Vector2 offsetPos = new Vector2(transform.position.x + ((int)FacingDir * -colliderWidth), transform.position.y );
        RaycastHit2D hit = Physics2D.Raycast(offsetPos, -Vector3.up, 20.0f);
        if (hit)
        {
            if( hit.distance > colliderHeight )
            {
                Debug.DrawLine(offsetPos, hit.point, Color.cyan);
                SetState(PLAYER_STATE.PS_INAIR);
            }
            else
            {
                if( currentState == PLAYER_STATE.PS_INAIR )
                {
                    SetState(PLAYER_STATE.PS_JUMPLAND);
                }
            }
        }
    }

    bool CanJump()
    {
        return( (currentState != PLAYER_STATE.PS_JUMP) && (currentState != PLAYER_STATE.PS_INAIR));
    }
    bool IsJumping()
    {
        return( (currentState == PLAYER_STATE.PS_JUMP) || (currentState == PLAYER_STATE.PS_INAIR));
    }

    void SetState(PLAYER_STATE _newState)
    {
        prevState = currentState;

        switch(_newState)
        {
            case PLAYER_STATE.PS_IDLE:
            {
                // If we're jumping then we can't be idle.
                if( IsJumping() )
                {
                    _newState = prevState;
                }
                break;
            }

            case PLAYER_STATE.PS_RUNNING:
            {
                // If we're jumping then we can run.
                if( IsJumping() )
                {
                    _newState = prevState;
                }
                break;
            }

            case PLAYER_STATE.PS_JUMP:
            {
                // If we're already jumping then no more jumps
                if( !CanJump() )
                {
                    // No jumping if you're in the air.
                     _newState = currentState;
                }
                break;
            }
            case PLAYER_STATE.PS_JUMPLAND:
            {
                // Force us back to Idle.
                _newState = PLAYER_STATE.PS_IDLE;
                break;
            }
            case PLAYER_STATE.PS_DOUBLEJUMPING:
            break;
        }

        currentState = _newState;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
         if (other.gameObject.CompareTag("Ground"))
        {

        }
    }

    private void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Ground"))
        {

        }
    }

    void OnGUI()
    {
        if(debugState)
        {
            GUI.Label(new Rect(0,0,200,100), "Move Dir: " + currentMove.ToString("F2"));
            GUI.Label(new Rect(0,12,200,100), "State: " + currentState.ToString());
            GUI.Label(new Rect(0,24,200,100), "Face Dir: " + FacingDir.ToString());
            GUI.Label(new Rect(0,36,200,100), "Move Speed: " + rb.velocity.x.ToString("F2"));
        }
    }
}