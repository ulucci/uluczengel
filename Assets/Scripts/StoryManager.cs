using UnityEngine;

public class StoryManager : MonoBehaviour
{
    public static StoryManager Instance { get; private set; }
    public int Phase { get; private set; } = 0;

    [SerializeField] private GameObject dorisObject;
    [SerializeField] private GameObject gilbertObject;

    private int phase1TasksDone;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        dorisObject?.SetActive(false);
        gilbertObject?.SetActive(false);
    }

    public void OnPatronIntroDone()
    {
        Phase = 1;
    }

    public void OnDoloresDelivered()
    {
        dorisObject?.SetActive(true);
        gilbertObject?.SetActive(true);
    }

    public void OnPhase1TaskDone()
    {
        phase1TasksDone++;
        if (phase1TasksDone >= 2)
        {
            Phase = 2;
            dorisObject?.SetActive(true);
            gilbertObject?.SetActive(true);
        }
    }
}
