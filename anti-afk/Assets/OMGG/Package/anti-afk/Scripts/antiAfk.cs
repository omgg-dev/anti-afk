using TMPro;
using UnityEngine;
using Sirenix.OdinInspector;

public class AntiAfk : MonoBehaviour
{
    #region Settings

    [Title("Anti-AFK Settings")]
    [Tooltip("Time (in seconds) before the anti-AFK is triggered.")]
    [MinValue(5), MaxValue(600)]
    [SerializeField] private float _WaitForInputSecs = 30;

    [Tooltip("Duration (in seconds) of the coutdown before the a turn is skipped.")]
    [MinValue(5), MaxValue(120)]
    [SerializeField] private float _CountdownSecs = 15;

    [Tooltip("Toggle if a player can be kicked or not.")]
    [SerializeField] private bool _IsStrict = true;

    [Tooltip("Max number of AFK tolerated before a kick.")]
    [MinValue(1), MaxValue(10)]
    [ShowIf(nameof(_IsStrict))]
    [SerializeField] private int _MaxAfkTurns = 3;

    #endregion

    #region UI

    [Title("UI References")]
    [Required("A TextMeshPro need to be assigned, otherwise the anti-AFK cannot display the countdown text.")]
    [SerializeField] private TMP_Text _CountdownText;

    [Required("A Parent Gameobject need to be assigned, otherwise the coutdown will always be displayed or hidden.")]
    [SerializeField] private GameObject _ParentContainer;

    #endregion

    #region Properties

    private float _Timer = 0;
    private float _Countdown = 0;

    [OnValueChanged("StartCheckingForAfk")]
    public bool _Toggle = false;

    private bool _IsCountingDown = false;

    #endregion

    void Update()
    {
        // Check if we get an input then stop both timer and coutdown
        if (Input.anyKeyDown)
        {
            Debug.Log("[AntiAfk] Input detected, stopping timers");
            StopCheckingForAfk();
        }
    }

    private void StartCheckingForAfk()
    {
        if (_Toggle == false)
            return;

        _Timer = 0;
        _IsCountingDown = false;
        _ParentContainer.gameObject.SetActive(false);
        InvokeRepeating(nameof(UpdateTimer), 1f, 1f);
    }

    private void UpdateTimer()
    {
        // Count every secconds 
        _Timer += 1;
        Debug.Log($"[AntiAfk] Timer: {_Timer}");
        // If the timer reach the limit
        if (_Timer >= _WaitForInputSecs && !_IsCountingDown)
        {
            Debug.Log("[AntiAfk] Timer reached the limit");
            CancelInvoke(nameof(UpdateTimer));
            StartCountdown();
        }
    }

    private void StopCheckingForAfk()
    {
        CancelInvoke(nameof(UpdateTimer));
        CancelInvoke(nameof(UpdateCountdown));
        StartCheckingForAfk();
    }

    private void StartCountdown()
    {
        _IsCountingDown = true;

        _Countdown = _CountdownSecs;
        _ParentContainer.gameObject.SetActive(true);
        _CountdownText.text = $"You will be kicked in {_Countdown} seconds";
        InvokeRepeating(nameof(UpdateCountdown), 1f, 1f);
    }

    private void UpdateCountdown()
    {
        _Countdown--;
        _CountdownText.text = $"You will be kicked in {_Countdown} seconds";
        if (_Countdown <= 0)
        {
            CancelInvoke(nameof(UpdateCountdown));
            _ParentContainer.gameObject.SetActive(false);
            HandleAfk();
        }
    }

    private void HandleAfk()
    {
        Debug.Log("[AntiAfk] Handling AFK");

        if (_IsStrict)
        {
            Debug.Log("Kick the player");
        }

        _Toggle = false;
    }
}
