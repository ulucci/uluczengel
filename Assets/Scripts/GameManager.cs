using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event System.Action<int> OnMoneyChanged;

    public int Money { get; private set; } = 100;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddMoney(int delta)
    {
        Money += delta;
        OnMoneyChanged?.Invoke(Money);
    }
}
