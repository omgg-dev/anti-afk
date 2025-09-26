using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AntiAfk : MonoBehaviour
{
    #region Settings

    [Header("Anti-AFK Settings")]
    [Tooltip("Time (in seconds) before the anti-AFK is triggered.")]
    [SerializeField] private float _WaitForInputSecs = 30;

    [Tooltip("Duration (in seconds) of the coutdown before the a turn is skipped.")]
    [SerializeField] private float _CountdownSecs = 15;

    [Tooltip("Toggle if a player can be kicked or not.")]
    [SerializeField] private bool _IsStrict = true;

    [Tooltip("Max number of AFK tolerated before a kick.")]
    [SerializeField] private int _MaxAfkTurns = 3;

    #endregion

    #region Properties

    private float _Timer = 0;
    private float _Countdown = 0;
    private bool _IsCountingDown = false;

    private bool _Toggle = false; // Each time _Toggle is turned to true, the anti-AFK start checking for AFK one single time

    private Dictionary<int, int> _PlayerAfkStatus = new Dictionary<int, int>();
    private int _CurrentPlayerId = 0;

    private Coroutine _TimerCoroutine;
    private Coroutine _CountdownCoroutine;

    #endregion

    #region Events

    public UnityEvent<int, float> OnCountdownStarted;
    public UnityEvent<int, float> OnCountdownTick;

    public UnityEvent<int> OnCountdownEnded;
    public UnityEvent<int> OnPlayerKicked;
    public UnityEvent<int> OnPlayerTurnSkipped;
    public UnityEvent<int> OnPlayerWakeUp;

    #endregion

    /// <summary>
    /// Initializes the AntiAfk component by starting the AFK checking process.
    /// </summary>
    void Update()
    {
        if (!_Toggle)
            return;

        if (Input.anyKeyDown) // Check if we get an input then stop both timer and coutdown
        {
            // Debug.Log("[AntiAfk] Input detected, stopping timers");
            ResetCheckingForAfk();
        }
        else if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0) // Check if the player move the mouse
        {
            // Debug.Log("[AntiAfk] Mouse movement detected, stopping timers");
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
        if (_IsStrict && !_PlayerAfkStatus.ContainsKey(playerId))
        {
            _PlayerAfkStatus[playerId] = _MaxAfkTurns;
        }

        _Toggle = true;
        _CurrentPlayerId = playerId;

        Debug.Log($"[AntiAfk] Toggling AFK for player {playerId}");

        ResetCheckingForAfk();
    }

    #region Private Methods
    private void StartCheckingForAfk()
    {
        if (_Toggle == false)
            return;

        _Timer = 0;
        _IsCountingDown = false;
        _TimerCoroutine = StartCoroutine(UpdateTimerCoroutine());
    }

    private void ResetCheckingForAfk()
    {
        if (_TimerCoroutine != null)
            StopCoroutine(_TimerCoroutine);
        if (_CountdownCoroutine != null)
            StopCoroutine(_CountdownCoroutine);

        OnPlayerWakeUp.Invoke(_CurrentPlayerId);
        StartCheckingForAfk();
    }

    private void StartCountdown()
    {
        _IsCountingDown = true;
        _Countdown = _CountdownSecs;
        OnCountdownStarted.Invoke(_CurrentPlayerId, _Countdown);

        _CountdownCoroutine = StartCoroutine(UpdateCountdownCoroutine());
    }

    private void HandleAfk()
    {
        // Debug.Log("[AntiAfk] Handling AFK");

        if (_IsStrict) // Try to kick the player
        {
            if (!_PlayerAfkStatus.ContainsKey(_CurrentPlayerId))
                return;

            _PlayerAfkStatus[_CurrentPlayerId]--;

            if (_PlayerAfkStatus[_CurrentPlayerId] > 0)
            {
                Debug.Log($"Player {_CurrentPlayerId} is AFK. {_PlayerAfkStatus[_CurrentPlayerId]} turns left before kick.");
            }
            else
            {
                Debug.Log($"Player {_CurrentPlayerId} is kicked for being AFK too many times.");
                // Kick the player
                // Implement your kick logic here
                // Or create an event to notify listeners that the player need to be kicked

                OnPlayerKicked.Invoke(_CurrentPlayerId);
            }
        }
        else // Just skip the turn of the player
        {
            Debug.Log($"Player {_CurrentPlayerId} turn is skipped for being AFK.");
            // Skip the player's turn
            // Implement your skip turn logic here
            // Or create an event to notify listeners that the player need to have his turn skipped

            OnPlayerTurnSkipped.Invoke(_CurrentPlayerId);
        }

        _Toggle = false;
    }
    #endregion

    #region Coroutines
    
    private IEnumerator UpdateTimerCoroutine()
    {
        yield return new WaitForSeconds(1f);

        while (_Timer < _WaitForInputSecs && !_IsCountingDown)
        {
            _Timer++;
            Debug.Log($"[AntiAfk] Timer: {_Timer}");
            yield return new WaitForSeconds(1f);
        }

        Debug.Log("[AntiAfk] Timer reached the limit");
        StartCountdown();
    }
    
    private IEnumerator UpdateCountdownCoroutine()
    {
        yield return new WaitForSeconds(1f);

        while (_Countdown > 0)
        {
            _Countdown--;
            OnCountdownTick.Invoke(_CurrentPlayerId, _Countdown);
            yield return new WaitForSeconds(1f);
        }

        OnCountdownEnded.Invoke(_CurrentPlayerId);
        HandleAfk();
    }
    
    #endregion
}
