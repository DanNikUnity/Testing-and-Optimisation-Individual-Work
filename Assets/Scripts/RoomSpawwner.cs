using UnityEngine;
using System.Collections.Generic;

public class RoomSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Массив префабов комнат
    public GameObject BossRoomPrefab; // Префаб босс-комнаты
    public Transform spawnPoint; // Точка спавна новых комнат
    public float roomLength = 10f; // Длина каждой комнаты
    public int initialRooms = 5; // Начальное количество комнат

    private Queue<GameObject> rooms = new Queue<GameObject>();
    private bool bossRoomSpawned = false; // Флаг, указывающий, что босс-комната уже создана

    void Start()
    {
        initialRooms = PlayerPrefs.GetInt("InitialRooms");
        // Изначально создаем несколько комнат
        for (int i = 0; i < initialRooms - 1; i++)
        {
            SpawnRoom(false);
        }
        // Спавним последнюю комнату как босс-комнату
        SpawnRoom(true);
    }

    void Update()
    {

    }

    void SpawnRoom(bool isBossRoom)
    {
        if (bossRoomSpawned) return; // Прекращаем спавн, если босс-комната уже создана

        GameObject selectedRoomPrefab;

        if (isBossRoom)
        {
            // Спавним босс-комнату
            selectedRoomPrefab = BossRoomPrefab;
            bossRoomSpawned = true;
        }
        else
        {
            // Выбираем случайный префаб комнаты
            int randomIndex = Random.Range(0, roomPrefabs.Length);
            selectedRoomPrefab = roomPrefabs[randomIndex];
        }

        // Определяем, где создать новую комнату
        Vector3 spawnPosition = spawnPoint.position + Vector3.forward * rooms.Count * roomLength;
        GameObject newRoom = Instantiate(selectedRoomPrefab, spawnPosition, Quaternion.identity);
        rooms.Enqueue(newRoom);
    }

    void DestroyOldestRoom()
    {
        if (rooms.Count == 0) return; // Нет комнат для удаления

        // Удаляем самую старую комнату
        GameObject oldRoom = rooms.Dequeue();
       // Destroy(oldRoom);
    }
}
