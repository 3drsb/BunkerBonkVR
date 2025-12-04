using UnityEngine;

public class ElevatorMonsterSpawn : MonoBehaviour
{
    public Transform spawnPoint;
    public GameObject monsterPrefab;
    public Animator elevatorAnimator;

    public void SpawnMonster()
    {
        GameObject monster = Instantiate(monsterPrefab, spawnPoint.position, spawnPoint.rotation);
        elevatorAnimator.SetTrigger("openDoors");
        Debug.Log("Monster spawned from elevator!");
    }
}
