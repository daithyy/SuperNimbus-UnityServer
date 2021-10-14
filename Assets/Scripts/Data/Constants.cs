
using UnityEngine;

public class Constants
{
    public const int Port = 26950;
    public const int TicksPerSecond = 60;
    public const int MillisecondsPerTick = 1000 / TicksPerSecond;
    public const int MaxPlayers = 4;
    public static readonly Vector3 SpawnPosition = new Vector3(-31.2f, -3.8f, 13.2f);
    public const int MaxItemCount = 20;
    public const int BufferConstant = 4096;
}

public enum PlayerAction
{
    Jump,
}