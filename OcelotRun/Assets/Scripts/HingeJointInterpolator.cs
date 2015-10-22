using UnityEngine;
using System.Collections;

[RequireComponent(typeof(HingeJoint2D))]
public class HingeJointInterpolator : MonoBehaviour
{
    public float TargetAngle;
    public float JointSpeed;
    public float MinTorque;
    public float MaxTorque;

    // Target Body is required in case the Object has multiple hinge joints.
    public Transform TargetBody;

    private HingeJoint2D joint;

    // Use this for initialization
    void Start()
    {
        HingeJoint2D[] joints = this.GetComponents<HingeJoint2D>();
        if (joints.Length > 0 && TargetBody == null)
        {
            Debug.LogError("TargetBody is required when there is more than one hinge joint");
        }
        if (joints.Length == 1)
        {
            joint = joints[0];
        }
        else
        {
            foreach (HingeJoint2D j in joints)
            {
                if (j.connectedBody.name == TargetBody.name)
                {
                    joint = j;
                    break;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float angleDiff = TargetAngle - joint.jointAngle;
        float factor = Mathf.Abs(angleDiff) / 180.0f;
        float torque = Mathf.Lerp(MinTorque, MaxTorque, factor);
        JointMotor2D motor = joint.motor;
        motor.motorSpeed = JointSpeed * Mathf.Sign(angleDiff);
        motor.maxMotorTorque = torque;
        joint.motor = motor;
    }

    public void SetTargetAngle(float targetAngle)
    {
        TargetAngle = targetAngle;
    }

    public string TargetBodyName()
    {
        return TargetBody.name;
    }
}
