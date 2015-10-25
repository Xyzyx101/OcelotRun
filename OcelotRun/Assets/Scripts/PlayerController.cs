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

    // Vine Grab
    private VineTrigger VineTrigger;
    private Rigidbody2D RightArmRB, LeftArmRB;
    private Vector2 HandOffset;
    private VineSection SwingVine;

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

    private bool SwingForward;

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
        VineTrigger = GetComponentInChildren<VineTrigger>();

        Animator = GetComponentInChildren<Animator>();

        RightArmRB = transform.Find("Torso/UpperArm_R/LowerArm_R").GetComponent<Rigidbody2D>();
        LeftArmRB = transform.Find("Torso/UpperArm_L/LowerArm_L").GetComponent<Rigidbody2D>();
        HandOffset = transform.Find("Torso/UpperArm_R/LowerArm_R/ArmEndPoint_R").localPosition;

        ChangeState(STATE.FRONT_FLIP);
    }

    // Update is called once per frame
    void Update()
    {
        if (TorsoRB.transform.position.y < GroundGenerator.GetLastHeight() - 5.0f)
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
                        if (FallTransitionTimer < 0f)
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
                        if (TorsoRB.velocity.y < -1.0f)
                        {
                            ChangeState(STATE.FALL);
                        }
                    }

                    if (VineTrigger.IsTouchingVine && InputManager.Held(Action.JUMP))
                    {
                        ChangeState(STATE.SWING);
                    }

                    break;
                }
            case STATE.FALL:
                {
                    if (VineTrigger.IsTouchingVine && InputManager.Held(Action.JUMP))
                    {
                        ChangeState(STATE.SWING);
                    }

                    if (GroundTrigger.IsOnGround)
                    {
                        ChangeState(STATE.RUN);
                    }
                    break;
                }
            case STATE.SWING:
                {
                    if (!InputManager.Held(Action.JUMP))
                    {
                        Speed = 5f;
                        SwingVine.Release();
                        PlayerRotation.SetMode(PlayerRotation.RotMode.STABLE);
                        ChangeState(STATE.FALL);
                    }
                    else if (SwingForward && TorsoRB.velocity.x < -1f)
                    {
                        TorsoRB.AddForce(new Vector2(-TorsoRB.mass * 5f, 0f), ForceMode2D.Impulse);
                        SwingForward = false;
                        Animator.SetTrigger("SwingBackward");
                    }
                    else if (!SwingForward && TorsoRB.velocity.x > 1f)
                    {
                        TorsoRB.AddForce(new Vector2(TorsoRB.mass * 5f, 0f), ForceMode2D.Impulse);
                        SwingForward = true;
                        Animator.SetTrigger("SwingForward");
                    }
                    break;
                }
            case STATE.FRONT_FLIP:
                {
                    if (PlayerRotation.GetMode() == PlayerRotation.RotMode.STABLE)
                    {
                        ChangeState(STATE.FALL);
                    }
                    break;
                }
            case STATE.BACK_FLIP:
                {
                    if (PlayerRotation.GetMode() == PlayerRotation.RotMode.STABLE)
                    {
                        ChangeState(STATE.FALL);
                    }
                    break;
                }
            case STATE.HIT_BY_CAR:
                {
                    break;
                }
            case STATE.DEAD:
                {
                    DeathTimer -= Time.deltaTime;
                    if (DeathTimer < 0f)
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
                    Animator.SetTrigger("Run");
                    break;
                }
            case STATE.JUMP:
                {
                    Animator.SetTrigger("Jump");
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
                    SwingForward = true;
                    Speed = 0;
                    JumpPhase1 = false;
                    JumpPhase2 = false;
                    SwingVine = VineTrigger.GetVine();
                    PlayerRotation.SetMode(PlayerRotation.RotMode.FLOP);
                    SwingVine.Grab(TorsoRB, LeftArmRB, RightArmRB, HandOffset);
                    TorsoRB.AddForce(new Vector2(TorsoRB.mass * 7f, 0f), ForceMode2D.Impulse);
                    Animator.SetTrigger("SwingForward");
                    break;
                }
            case STATE.FRONT_FLIP:
                {
                    Animator.SetTrigger("FrontFlip");
                    PlayerRotation.SetMode(PlayerRotation.RotMode.FRONT_FLIP);
                    break;
                }
            case STATE.BACK_FLIP:
                {
                    Animator.SetTrigger("BackFlip");
                    PlayerRotation.SetMode(PlayerRotation.RotMode.BACK_FLIP);
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

    public float GetSpeed()
    {
        return Speed;
    }
}
