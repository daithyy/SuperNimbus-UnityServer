using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static Dictionary<int, Spawner> Spawners = new Dictionary<int, Spawner>();

    public int Id;

    public bool HasItem = false;

    private static int nexId = 1;

    private void Start()
    {
        HasItem = false;
        Id = nexId;

        nexId++;

        Spawners.Add(Id, this);

        StartCoroutine(ItemSpawn());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (HasItem && other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            if (player.Pickup())
            {
                ItemCollect(player.Id);
            }
        }
    }

    private IEnumerator ItemSpawn()
    {
        yield return new WaitForSeconds(10f);

        HasItem = true;

        ServerController.ItemSpawn(Id);
    }

    private void ItemCollect(int playerId)
    {
        HasItem = false;

        ServerController.ItemCollect(Id, playerId);

        StartCoroutine(ItemSpawn());
    }
}
