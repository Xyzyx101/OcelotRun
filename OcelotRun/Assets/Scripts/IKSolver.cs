using UnityEngine;
using System;
using System.Collections;


// This IK solver only solves the special case of 2 bones in 2D.
public class IKSolver : MonoBehaviour
{
    public GameObject ParentObject;
    public Transform BodyA;
    public Transform BodyB;
    public Transform EndPoint;

    public bool SolvePosAngle2; // Solve for positive angle 2 instead of negative angle 2

    //private HingeJoint2D JointB;
    //private HingeJoint2D JointA;

    private float LengthA;
    private float LengthB;

    private float TargetAngleA;
    private float TargetAngleB;

    private HingeJointInterpolator jointInterpolatorA;
    private HingeJointInterpolator jointInterpolatorB;

    // Use this for initialization
    void Start()
    {
        //HingeJoint2D[] joints = ParentObject.GetComponents<HingeJoint2D>();
        //foreach (HingeJoint2D j in joints)
        //{
        //    if (j.connectedBody.name == BodyA.name)
        //    {
        //        JointA = j;
        //        break;
        //    }
        //}
        
        //joints = BodyA.gameObject.GetComponents<HingeJoint2D>();
        //foreach (HingeJoint2D j in joints)
        //{
        //    if (j.connectedBody.name == BodyB.name)
        //    {
        //        JointB = j;
        //        break;
        //    }
        //}

        HingeJointInterpolator[] interpolators = ParentObject.GetComponents<HingeJointInterpolator>();
        foreach (HingeJointInterpolator interp in interpolators)
        {
            if (interp.TargetBodyName() == BodyA.name)
            {
                jointInterpolatorA = interp;
            }
        }
         interpolators = BodyA.GetComponents<HingeJointInterpolator>();
        foreach (HingeJointInterpolator interp in interpolators)
        {
            if (interp.TargetBodyName() == BodyB.name)
            {
                jointInterpolatorB = interp;
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
    
    void Update()
    {
        Vector3 ikTargetPoint = this.transform.position;
        Vector3 boneChainStart = BodyA.transform.position;
        ikTargetPoint = ParentObject.transform.InverseTransformPoint(ikTargetPoint);
        boneChainStart = ParentObject.transform.InverseTransformPoint(boneChainStart);
        Vector3 relativeTarget = ikTargetPoint - boneChainStart;
        CalcIK(
            LengthA,
            LengthB,
            -relativeTarget.y,
            relativeTarget.x,
            out TargetAngleA,
            out TargetAngleB
        );
        jointInterpolatorA.SetTargetAngle(TargetAngleA * Mathf.Rad2Deg);
        jointInterpolatorB.SetTargetAngle(TargetAngleB * Mathf.Rad2Deg);
    }

    public bool CalcIK(
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

            angle2 = (float)Math.Acos(cosAngle2);
             
            if (SolvePosAngle2)
            {
                if (angle2 < 0.0f)
                {
                    angle2 = -angle2;
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
