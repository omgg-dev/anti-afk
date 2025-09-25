using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    private bool _IsCountingDown = false;

    [OnValueChanged("StartCheckingForAfk")]
    private bool _Toggle = false; // Each time _Toggle is turned to true, the anti-AFK start checking for AFK one single time

    private Dictionary<int, int> playerAfkStatus = new Dictionary<int, int>();
    private int _CurrentPlayerId = 0;

    #endregion

    /// <summary>
    /// Initializes the AntiAfk component by starting the AFK checking process.
    /// </summary>
    void Update()
    {
        if (!_Toggle)
            return;

        // Check if we get an input then stop both timer and coutdown
        if (Input.anyKeyDown)
        {
            Debug.Log("[AntiAfk] Input detected, stopping timers");
            ResetCheckingForAfk();
        }
    }

    /// <summary>
    /// Toggles the AFK (Away From Keyboard) status for the specified player.
    /// </summary>
    /// <remarks>If strict mode is enabled and the player's AFK status is not already tracked, the player is
    /// added to the AFK status list with the maximum allowed AFK turns. The current player ID is updated to the
    /// specified player and reset to check the status of the player that is currently playing.</remarks>
    /// <param name="playerId">The unique identifier of the player whose AFK status is being checked.</param>
    public void ToogleAfk(int playerId)
    {
        if (_IsStrict && !playerAfkStatus.ContainsKey(playerId))
        {
            playerAfkStatus[playerId] = _MaxAfkTurns;
        }

        _Toggle = true;
        _CurrentPlayerId = playerId;

        ResetCheckingForAfk();
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

    private void ResetCheckingForAfk()
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
            if (!playerAfkStatus.ContainsKey(_CurrentPlayerId))
                return;

            playerAfkStatus[_CurrentPlayerId]--;

            if (playerAfkStatus[_CurrentPlayerId] > 0)
            {
                Debug.Log($"Player {_CurrentPlayerId} is AFK. {playerAfkStatus[_CurrentPlayerId]} turns left before kick.");
            }
            else
            {
                Debug.Log($"Player {_CurrentPlayerId} is kicked for being AFK too many times.");
                // Kick the player
                // Implement your kick logic here
                // Or create an event to notify listeners that the player need to be kicked
            }
        }
        else
        {
            Debug.Log($"Player {_CurrentPlayerId} turn is skipped for being AFK.");
            // Skip the player's turn
            // Implement your skip turn logic here
            // Or create an event to notify listeners that the player need to have his turn skipped
        }

        _Toggle = false;
    }
}
