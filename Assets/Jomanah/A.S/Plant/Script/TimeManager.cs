using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance;

    public UnityEvent OnDayPass = new UnityEvent();

    [Header("Time Settings")]
    [SerializeField] bool useAutoTime = true;   
    [SerializeField] float timeBetweenDays = 5f;
    private float dayTimer;

    [Header("UI")]
    [SerializeField] Button nextDayButton;      

    public int currentDay = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        
        useAutoTime = true;
        dayTimer = timeBetweenDays;

        
        if (nextDayButton != null)
        {
            nextDayButton.onClick.RemoveAllListeners();
        }
    }

    private void Update()
    {
        if (!useAutoTime) return;

        dayTimer -= Time.deltaTime;
        if (dayTimer <= 0f)
        {
            PassDay();
            dayTimer = timeBetweenDays;
        }
    }

    public void PassDay()
    {
        currentDay++;
        Debug.Log("Day Passed: " + currentDay);
        OnDayPass.Invoke();
    }
}
