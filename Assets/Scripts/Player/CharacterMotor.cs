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

    public Vector3 ForwardVector = Vector3.forward;

    protected Quaternion AlignCorrection;

    private bool m_Grounded = false;

    public bool Grounded
    {
        get { return m_Grounded; }
        protected set { m_Grounded = value; }
    }

    private bool m_Jumping = false;

    public bool Jumping
    {
        get { return m_Jumping; }
        protected set { m_Jumping = value; }
    }

    private Vector3 m_desiredMovementDirection;

    void Start()
    {
        AlignCorrection = new Quaternion();
        AlignCorrection.SetLookRotation(ForwardVector, Vector3.up);
        AlignCorrection = Quaternion.Inverse(AlignCorrection);
    }

    public Vector3 DesiredMovementDirection
    {
        get { return m_desiredMovementDirection; }
        set
        {
            m_desiredMovementDirection = value;
            if (m_desiredMovementDirection.magnitude > 1) m_desiredMovementDirection = m_desiredMovementDirection.normalized;
        }
    }

    public Vector3 DesiredVelocity
    {
        get
        {
            if (m_desiredMovementDirection == Vector3.zero) return Vector3.zero;
            else
            {
                float zAxisEllipseMultiplier = (m_desiredMovementDirection.z > 0 ? MaxForwardSpeed : MaxBackwardsSpeed) / MaxSidewaysSpeed;
                Vector3 temp = new Vector3(m_desiredMovementDirection.x, 0, m_desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
                float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * MaxSidewaysSpeed;
                Vector3 velocity = m_desiredMovementDirection * length;
                return transform.rotation * velocity;
            }
        }
    }
}