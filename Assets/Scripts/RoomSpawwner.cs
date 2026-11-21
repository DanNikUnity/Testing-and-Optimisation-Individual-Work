using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class RoomSpawner : MonoBehaviour
{
    public GameObject[] roomPrefabs; // Массив префабов комнат
    public GameObject BossRoomPrefab; // Префаб босс-комнаты
    public Transform spawnPoint; // Точка спавна новых комнат
    public float roomLength = 10f; // Длина каждой комнаты
    public int initialRooms = 5; // Начальное количество комнат

    [Header("Generare pseudo-random")]
    [Tooltip("Activat: shuffle bag (fiecare cameră apare de un număr aproape egal de ori în nivel).\n" +
             "Dezactivat: Random simplu, ca în build-ul baseline BW_0.1.0.")]
    public bool useShuffleBag = true;

    [Tooltip("Scrie în Console câte camere de fiecare tip s-au generat (util pentru ES-02).")]
    public bool logRoomStats = true;

    private Queue<GameObject> rooms = new Queue<GameObject>();
    private bool bossRoomSpawned = false; // Флаг, указывающий, что босс-комната уже создана

    // Shuffle bag: o „pungă” cu indicii tuturor camerelor, amestecată.
    // Camerele se extrag pe rând; când punga se golește, se umple și se amestecă din nou.
    private readonly List<int> shuffleBag = new List<int>();
    private int lastRoomIndex = -1;
    private readonly Dictionary<string, int> roomStats = new Dictionary<string, int>();

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

        if (logRoomStats) LogRoomStats();
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
            int roomIndex = useShuffleBag
                ? NextRoomFromBag()
                : Random.Range(0, roomPrefabs.Length); // varianta veche (baseline)

            selectedRoomPrefab = roomPrefabs[roomIndex];
            lastRoomIndex = roomIndex;

            roomStats.TryGetValue(selectedRoomPrefab.name, out int count);
            roomStats[selectedRoomPrefab.name] = count + 1;
        }

        // Определяем, где создать новую комнату
        Vector3 spawnPosition = spawnPoint.position + Vector3.forward * rooms.Count * roomLength;
        GameObject newRoom = Instantiate(selectedRoomPrefab, spawnPosition, Quaternion.identity);
        rooms.Enqueue(newRoom);
    }

    /// <summary>
    /// Extrage următoarea cameră din shuffle bag.
    /// Fiecare cameră apare o dată în fiecare „rundă” de roomPrefabs.Length camere,
    /// deci într-un nivel cu N camere fiecare tip apare de N/5 ori, rotunjit în jos sau în sus
    /// (de ex. 9 camere din 5 tipuri: fiecare apare de 1–2 ori).
    /// </summary>
    int NextRoomFromBag()
    {
        if (shuffleBag.Count == 0) RefillBag();

        int index = shuffleBag[shuffleBag.Count - 1];
        shuffleBag.RemoveAt(shuffleBag.Count - 1);
        return index;
    }

    void RefillBag()
    {
        shuffleBag.Clear();
        for (int i = 0; i < roomPrefabs.Length; i++) shuffleBag.Add(i);

        // Amestecare Fisher–Yates: fiecare ordine are aceeași probabilitate.
        for (int i = shuffleBag.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (shuffleBag[i], shuffleBag[j]) = (shuffleBag[j], shuffleBag[i]);
        }

        // Evită aceeași cameră de două ori la rând la trecerea dintre runde.
        // Camera extrasă prima este ultima din listă.
        int next = shuffleBag.Count - 1;
        if (shuffleBag.Count > 1 && shuffleBag[next] == lastRoomIndex)
        {
            (shuffleBag[next], shuffleBag[0]) = (shuffleBag[0], shuffleBag[next]);
        }
    }

    void LogRoomStats()
    {
        string mode = useShuffleBag ? "Shuffle bag" : "Random";
        string stats = string.Join(", ", roomStats.OrderBy(kv => kv.Key).Select(kv => $"{kv.Key} x{kv.Value}"));
        Debug.Log($"[RoomSpawner] {mode}, {initialRooms - 1} camere + Boss: {stats}");
    }

    void DestroyOldestRoom()
    {
        if (rooms.Count == 0) return; // Нет комнат для удаления

        // Удаляем самую старую комнату
        GameObject oldRoom = rooms.Dequeue();
       // Destroy(oldRoom);
    }
}
