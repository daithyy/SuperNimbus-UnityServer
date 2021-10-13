
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : CharacterMotor
{
    public int Id;

    public string Username;

    private bool firstFrame = true;

    private bool[] actions;

    public void Initialize(int id, string userName)
    {
        Id = id;
        Username = userName;

        actions = new bool[1];
    }

    public void ReadInput(Vector3 inputDirection, Quaternion rotation, Vector3 eulerAngles, bool[] actions)
    {
        DesiredMovementDirection = inputDirection;
        
        transform.rotation = rotation;

        transform.GetChild(0).transform.eulerAngles = eulerAngles;

        this.actions = actions;
    }

    private void OnEnable()
    {
        firstFrame = true;
    }

    private void FixedUpdate()
    {
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        CharacterController controller = GetComponent(typeof(CharacterController)) as CharacterController;
        Vector3 velocity = controller.velocity;

        if (firstFrame)
        {
            velocity = Vector3.zero;
            firstFrame = false;
        }
        if (Grounded) velocity = Util.ProjectOntoPlane(velocity, transform.up);

        // Calculate how fast we should be moving
        Vector3 movement = velocity;

        Jumping = false;

        if (Grounded)
        {
            // Apply a force that attempts to reach our target velocity
            Vector3 velocityChange = (DesiredVelocity - velocity);

            if (velocityChange.magnitude > MaxVelocityChange)
            {
                velocityChange = velocityChange.normalized * MaxVelocityChange;
            }
            movement += velocityChange;

            // Jump
            if (CanJump && actions[(int)PlayerAction.Jump]) // TODO: && INPUT = JUMP
            {
                movement += transform.up * Mathf.Sqrt(2 * JumpHeight * Gravity);
                Jumping = true;
            }
        }

        // Apply downwards gravity
        movement += transform.up * -Gravity * Time.deltaTime;

        if (Jumping)
        {
            movement -= transform.up * -Gravity * Time.deltaTime / 2;
        }

        // Apply movement
        CollisionFlags flags = controller.Move(movement * Time.deltaTime);
        Grounded = (flags & CollisionFlags.CollidedBelow) != 0;

        ServerController.PlayerPosition(Id, transform.position);
        ServerController.PlayerRotation(Id, transform.rotation, transform.GetChild(0).transform.eulerAngles);
        ServerController.PlayerAnimation(Id, Jumping, Grounded);
    }
}