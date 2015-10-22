using UnityEngine;
using System;
using System.Collections;


// This IK solver only solves the special case of 2 bones in 2D.
public class IKSolver : MonoBehaviour
{
    public GameObject ParentObject;
    public Rigidbody2D BodyA;
    public Rigidbody2D BodyB;
    public GameObject EndPoint;
    public Vector3 AnchorOffset;

    public HingeJoint2D JointB;
    public HingeJoint2D JointA;

    public float LengthA;
    public float LengthB;

    public float TargetAngleA;
    public float TargetAngleB;

    // Use this for initialization
    void Start()
    {
        HingeJoint2D[] joints = ParentObject.GetComponents<HingeJoint2D>();
        foreach (HingeJoint2D j in joints)
        {
            if (j.connectedBody.name == BodyA.name)
            {
                JointA = j;
                break;
            }
        }

        joints = BodyA.gameObject.GetComponents<HingeJoint2D>();
        foreach (HingeJoint2D j in joints)
        {
            if (j.connectedBody.name == BodyB.name)
            {
                JointB = j;
                break;
            }
        }

        Vector3 pointA = BodyA.transform.position;
        Vector3 pointB = BodyB.transform.position;
        Vector3 pointC = EndPoint.transform.position;

        LengthA = Vector3.Magnitude(pointB - pointA);
        LengthB = Vector3.Magnitude(pointC - pointB);
        Debug.Assert(LengthA >= 0);
        Debug.Assert(LengthB >= 0);
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 ikTargetPoint = this.transform.position;
        Vector3 boneChainStart = BodyA.transform.position;
        ikTargetPoint = ParentObject.transform.InverseTransformPoint(ikTargetPoint);
        boneChainStart = ParentObject.transform.InverseTransformPoint(boneChainStart);
        Vector3 relativeTarget = ikTargetPoint - boneChainStart;
        Debug.Log(relativeTarget);
        bool validSolution = CalcIK(
            false,
            LengthA,
            LengthB,
            -relativeTarget.y,
            relativeTarget.x,
            out TargetAngleA,
            out TargetAngleB
        );
        BodyA.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, TargetAngleA * Mathf.Rad2Deg);
        BodyB.transform.localRotation = Quaternion.Euler(0.0f, 0.0f, TargetAngleB * Mathf.Rad2Deg);
        Debug.Log("AngleA:" + TargetAngleA + "   AngleB:" + TargetAngleB);
    }

    public bool CalcIK(
        bool solvePosAngle2, // Solve for positive angle 2 instead of negative angle 2
        double length1,
        double length2,
        double targetX,
        double targetY,
        out float angle1Param,
        out float angle2Param
        )
    {
        const double epsilon = 0.0001;
        bool foundValidSolution = true;
        double targetDistSqr = (targetX * targetX + targetY * targetY);

        double sinAngle2;
        double cosAngle2;

        float angle1;
        float angle2;

        double cosAngle2_denom = 2 * length1 * length2;
        if (cosAngle2_denom > epsilon)
        {
            cosAngle2 = (targetDistSqr - length1 * length1 - length2 * length2) / (cosAngle2_denom);
            if ((cosAngle2 < -1.0) || (cosAngle2 > 1.0))
            {
                foundValidSolution = false;
            }

            // clamp our value into range so we can calculate the best
            // solution when there are no valid ones
            cosAngle2 = Math.Max(-1.0, Math.Min(1.0, cosAngle2));

            float foo = (float)Math.Acos(cosAngle2);
            angle2 = foo;

            if (solvePosAngle2)
            {
                if (angle2 < 0.0f)
                {
                    angle2 = -angle2;
                }
                if (foo < 0.0f)
                {
                    int DELETEME = 42;
                }
            }
            else
            {
                if (angle2 > 0.0f)
                {
                    angle2 = -angle2;
                }
            }

            sinAngle2 = Math.Sin(angle2);
        }
        else
        {
            // At least one of the bones had a zero length. This means our
            // solvable domain is a circle around the origin with a radius
            // equal to the sum of our bone lengths.
            double totalLenSqr = (length1 + length2) * (length1 + length2);
            if (targetDistSqr < (totalLenSqr - epsilon)
                || targetDistSqr > (totalLenSqr + epsilon))
            {
                foundValidSolution = false;
            }

            // Only the value of angle1 matters at this point. We can just
            // set angle2 to zero. 
            angle2 = 0.0f;
            cosAngle2 = 1.0;
            sinAngle2 = 0.0;
        }

        // Compute the angle1 based on the sine and cosine of angle2
        double triAdjacent = length1 + length2 * cosAngle2;
        double triOpposite = length2 * sinAngle2;

        double tanY = targetY * triAdjacent - targetX * triOpposite;
        double tanX = targetX * triAdjacent + targetY * triOpposite;

        angle1 = (float)Math.Atan2(tanY, tanX);

        angle1Param = angle1;
        angle2Param = angle2;
        return foundValidSolution;
    }
}
