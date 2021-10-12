
using System;
using UnityEngine;

public class Player: MonoBehaviour
{
    public int Id;

    public string Username;

    private readonly float moveSpeed = 5f / Constants.TicksPerSecond;

    private bool[] actions;

    public void Initialize(int id, string userName)
    {
        Id = id;
        Username = userName;

        actions = new bool[4];
    }

    public void FixedUpdate()
    {
        Vector2 direction = Vector2.zero;

        if (actions[0])
        {
            direction.y += 1;
        }

        if (actions[1])
        {
            direction.y -= 1;
        }

        if (actions[2])
        {
            direction.x -= 1;
        }

        if (actions[3])
        {
            direction.x += 1;
        }

        Move(direction);
    }

    public void SetInput(bool[] actions, Quaternion rotation, Vector3 eulerAngles)
    {
        this.actions = actions;
        transform.rotation = rotation;
        transform.GetChild(0).transform.eulerAngles = eulerAngles;
    }

    private void Move(Vector2 direction)
    {
        Vector3 moveDirection = transform.right * direction.x + transform.forward * direction.y;
        transform.position += moveDirection * moveSpeed;

        SendController.PlayerPosition(this);
        SendController.PlayerRotation(this);
    }
}
