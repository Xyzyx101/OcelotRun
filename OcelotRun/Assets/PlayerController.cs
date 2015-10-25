using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    //  This speed is used by the camera and the GroundGenerator to look like it moves.
    //  The player is always close to 0 in x.
    public float Speed;
    public GroundGenerator GroundGenerator;

    private Rigidbody2D TorsoRB;
    private GroundTrigger GroundTrigger;
    private InputManager InputManager;
    private SliderJoint2D GroundMotor;
    private Animator Animator;
    private PlayerRotation PlayerRotation;

    private float TotalJumpPhase1Time = 0.1f;
    private float JumpPhase1Timer = 0.0f;
    private float TotalJumpPhase2Time = 0.5f;
    private float JumpPhase2Timer = 0.0f;

    private float TotalFallTransitionTime = 0.8f;
    private float FallTransitionTimer = 0f;

    private bool JumpPhase1 = false;
    private bool JumpPhase2 = false;

    private float TotalDeathTime = 2.0f;
    private float DeathTimer = 0.0f;

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
        PlayerRotation = GetComponentInChildren<PlayerRotation>();

        Animator = GetComponentInChildren<Animator>();

        ChangeState(STATE.RUN);
    }

    // Update is called once per frame
    void Update()
    {
        if(TorsoRB.transform.position.y < GroundGenerator.GetLastHeight() - 5.0f)
        {
            ChangeState(STATE.DEAD);
        }
        switch (CurrentState)
        {
            case STATE.RUN:
                {
                    if (GroundTrigger.IsOnGround)
                    {
                        FallTransitionTimer = TotalFallTransitionTime;
                    }
                    else
                    {
                        FallTransitionTimer -= Time.deltaTime;
                        if (FallTransitionTimer<0f)
                        {
                            ChangeState(STATE.FALL);
                        }
                    }

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
                    DeathTimer -= Time.deltaTime;
                    if (DeathTimer<0f)
                    {
                        Application.LoadLevel("GameScene");
                    }
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
                    PlayerRotation.targetAngle = -25;
                    ///FootStep();
                    Animator.SetTrigger("Run");
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
                    PlayerRotation.targetAngle = 15f;
                    Animator.SetTrigger("Fall");
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
                    DeathTimer = TotalDeathTime;
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
            TorsoRB.AddForce(new Vector2(0f, TorsoRB.mass * 5f));
        }
    }

    // This is a correction factor that is applied every time the player takes a step.
    // It is intended to makes it look like the biomechanics of running actually work.
    // It is called by an event in the run animation. 
    void FootStep()
    {
        //float correctionFactor = TorsoRB.transform.position.x * 0.125f;
        //float xVel = Mathf.Lerp(0f, 10f, Mathf.Abs(correctionFactor)) * -Mathf.Sign(correctionFactor);
        //TorsoRB.velocity = new Vector2(xVel, 0f);
    }

    public float GetSpeed()
    {
        return Speed;
    }
}
