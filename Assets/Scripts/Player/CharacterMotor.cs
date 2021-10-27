using UnityEngine;

public abstract class CharacterMotor : MonoBehaviour
{
    public float MaxForwardSpeed = 4f;

    public float MaxBackwardsSpeed = 4f;

    public float MaxSidewaysSpeed = 4f;

    public float MaxVelocityChange = 0.2f;

    public float Gravity = 10.0f;

    public bool CanJump = true;

    public float JumpHeight = 1.0f;

    private bool grounded = false;

    public bool Grounded
    {
        get { return grounded; }
        protected set { grounded = value; }
    }

    private bool jumping = false;

    public bool Jumping
    {
        get { return jumping; }
        protected set { jumping = value; }
    }

    private Vector3 desiredMovementDirection;

    public Vector3 DesiredMovementDirection
    {
        get { return desiredMovementDirection; }
        set
        {
            desiredMovementDirection = value;
            if (desiredMovementDirection.magnitude > 1) desiredMovementDirection = desiredMovementDirection.normalized;
        }
    }

    public Vector3 DesiredVelocity
    {
        get
        {
            if (desiredMovementDirection == Vector3.zero) return Vector3.zero;
            else
            {
                float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? MaxForwardSpeed : MaxBackwardsSpeed) / MaxSidewaysSpeed;
                Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
                float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * MaxSidewaysSpeed;
                Vector3 velocity = desiredMovementDirection * length;
                return transform.rotation * velocity;
            }
        }
    }
}