using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //  This speed is used by the camera and the GroundGenerator to look like it moves.
    //  The player is always close to 0 in x.
    public float Speed;

    private Rigidbody2D TorsoRB;
    private GroundTrigger GroundTrigger;
    private InputManager InputManager;
    private SliderJoint2D GroundMotor;

    private float TotalJumpPhase1Time = 0.05f;
    private float JumpPhase1Timer = 0.0f;
    private float TotalJumpPhase2Time = 0.4f;
    private float JumpPhase2Timer = 0.0f;

    public bool JumpPhase1 = false;
    public bool JumpPhase2 = false;

    private enum STATE
    {
        RUN,
        JUMP,
        FALL,
        SWING,
        FRONT_FLIP,
        BACK_FLIP,
        HIT_BY_CAR,
        DEAD
    }
    private STATE CurrentState;

    // Use this for initialization
    void Start()
    {
        Transform torsoObj = transform.Find("Torso");
        TorsoRB = torsoObj.GetComponent<Rigidbody2D>();

        Transform groundCollider = transform.Find("GroundCollider");
        GroundMotor = groundCollider.GetComponent<SliderJoint2D>();

        GroundTrigger = GetComponentInChildren<GroundTrigger>();
        InputManager = GetComponent<InputManager>();

        ChangeState(STATE.RUN);
    }

    // Update is called once per frame
    void Update()
    {
        AutoCenter();
        switch (CurrentState)
        {
            case STATE.RUN:
                {
                    if (InputManager.Pressed(Action.JUMP) && GroundTrigger.IsOnGround)
                    {
                        ChangeState(STATE.JUMP);
                    }
                    break;
                }
            case STATE.JUMP:
                {
                    JumpPhase1Timer -= Time.deltaTime;
                    JumpPhase2Timer -= Time.deltaTime;

                    if (JumpPhase1Timer > 0f)
                    {
                        JumpPhase1 = true;
                    }
                    else if (JumpPhase2Timer > 0f && InputManager.Held(Action.JUMP))
                    {
                        JumpPhase1 = false;
                        JumpPhase2 = true;
                    }
                    else
                    {
                        JumpPhase1 = false;
                        JumpPhase2 = false;
                        ChangeState(STATE.FALL);
                    }
                    break;
                }
            case STATE.FALL:
                {
                    if (GroundTrigger.IsOnGround)
                    {
                        ChangeState(STATE.RUN);
                    }
                    break;
                }
            case STATE.SWING:
                {
                    break;
                }
            case STATE.FRONT_FLIP:
                {
                    break;
                }
            case STATE.BACK_FLIP:
                {
                    break;
                }
            case STATE.HIT_BY_CAR:
                {
                    break;
                }
            case STATE.DEAD:
                {
                    break;
                }
        }
    }

    void ChangeState(STATE newState)
    {
        if (newState == CurrentState)
        {
            return;
        }
        Debug.Log("Change State" + newState);
        switch (newState)
        {
            case STATE.RUN:
                {
                    break;
                }
            case STATE.JUMP:
                {
                    JumpPhase1Timer = TotalJumpPhase1Time;
                    JumpPhase2Timer = TotalJumpPhase2Time;
                    break;
                }
            case STATE.FALL:
                {
                    break;
                }
            case STATE.SWING:
                {
                    break;
                }
            case STATE.FRONT_FLIP:
                {
                    break;
                }
            case STATE.BACK_FLIP:
                {
                    break;
                }
            case STATE.HIT_BY_CAR:
                {
                    break;
                }
            case STATE.DEAD:
                {
                    break;
                }
        }
        CurrentState = newState;
    }

    void FixedUpdate()
    {
        if (JumpPhase1)
        {
            TorsoRB.velocity = new Vector2(TorsoRB.velocity.x, 5f);
        }
        else if (JumpPhase2)
        {
            TorsoRB.AddForce(new Vector2(0f, TorsoRB.mass*2f));
        }
    }

    // Apply force to keep the playing in the middle of the screen.
    void AutoCenter()
    {
        float force = Mathf.Lerp(0f, 300f, Mathf.Abs(TorsoRB.transform.position.x)) * Mathf.Sign(TorsoRB.transform.position.x);
        TorsoRB.AddForce(new Vector2(-force, 0f));
    }

    public float GetSpeed()
    {
        return Speed;
    }
}
