using UnityEngine;
using System.Collections;

public class PlayerRotation : MonoBehaviour
{

    public enum RotMode
    {
        __INIT,
        STABLE,
        FLOP,
        BACK_FLIP,
        FRONT_FLIP
    }

    public HingeJoint2D joint;
    public float debugAngle;
    public float stableAngle;
    public float stableSpeed;
    public float minStableTorque;
    public float maxStableTorque;
    public float flipSpeed;
    public float flipTorque;
    public float targetAngle;
    private RotMode CurrentRotMode;

    // This is used to add full rotations to the stable target after full flips.
    private float rotationOffset = 0f;

    // Use this for initialization
    void Start()
    {
        //CurrentRotMode = RotMode.STABLE;
        SetMode(RotMode.STABLE);
    }

    // Update is called once per frame
    void Update()
    {
        debugAngle = joint.jointAngle;

        switch (CurrentRotMode)
        {
            case RotMode.STABLE:
                float angleDiff = targetAngle + rotationOffset - joint.jointAngle;
                float factor = Mathf.Abs(angleDiff) / 45.0f;
                float torque = Mathf.Lerp(minStableTorque, maxStableTorque, factor);
                JointMotor2D motor = joint.motor;
                motor.motorSpeed = stableSpeed * Mathf.Sign(angleDiff);
                motor.maxMotorTorque = torque;
                joint.motor = motor;
                break;
            case RotMode.BACK_FLIP:
               if (joint.jointAngle > targetAngle)
                {
                    rotationOffset += 360f;
                    SetMode(RotMode.STABLE);
                }
                break;
            case RotMode.FRONT_FLIP:
                if (joint.jointAngle < targetAngle)
                {
                    rotationOffset -= 360f;
                    SetMode(RotMode.STABLE);
                }
                break;
        }
    }

    public void SetMode(RotMode newMode)
    {
        if (newMode == CurrentRotMode)
        {
            return;
        }
        switch (newMode)
        {
            case RotMode.STABLE:
                targetAngle = joint.jointAngle - joint.jointAngle % 360.0f + stableAngle;
                break;
            case RotMode.FLOP:
                break;
            case RotMode.FRONT_FLIP:
                {
                    float initialOffset = joint.jointAngle % 360.0f;
                    if (initialOffset< 180.0f)
                    {
                        initialOffset += 360.0f;
                    }
                    targetAngle = joint.jointAngle - initialOffset;

                    JointMotor2D motor = joint.motor;
                    motor.motorSpeed = -flipSpeed;
                    motor.maxMotorTorque = flipTorque;
                    joint.motor = motor;
                    break;
                }
            case RotMode.BACK_FLIP:
                {
                    float initialOffset = joint.jointAngle % 360.0f;
                    if (initialOffset>180.0f)
                    {
                        initialOffset -= 360.0f;
                    }
                    initialOffset = 360.0f - initialOffset;

                    targetAngle = joint.jointAngle + initialOffset - 90f;
                                       
                    JointMotor2D motor = joint.motor;
                    motor.motorSpeed = flipSpeed * 0.75f;
                    motor.maxMotorTorque = flipTorque;
                    joint.motor = motor;
                    break;
                }
        }
        if (newMode == RotMode.FLOP)
        {
            joint.useMotor = false;
        }
        else
        {
            joint.useMotor = true;
        }
        CurrentRotMode = newMode;
    }

    public RotMode GetMode()
    {
        return CurrentRotMode;
    }
}
