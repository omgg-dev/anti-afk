using TMPro;
using UnityEngine;

/// <summary>
/// This is an example script showing how to use the AntiAfk component to display a countdown UI.
/// In this example, we assume that the AntiAfk component is already present in the scene.
/// Also the UI is very basic and should be adapted to your needs.
/// The AntiAfk UI is client side only, so each player will see their own countdown when they are AFK..
/// </summary>
public class CountdownUIExample : MonoBehaviour
{

    #region UI

    [Header("UI References")]
    [SerializeField] private TMP_Text _CountdownText;

    [SerializeField] private GameObject _ParentContainer;

    #endregion

    #region Properties
    
    private AntiAfk _AntiAfk;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get the AntiAfk component from the scene
        _AntiAfk = FindFirstObjectByType<AntiAfk>();

        if (_AntiAfk == null)
        {
            Debug.LogError("No AntiAfk component found in the scene. Please add one to use the CountdownUIExample.");
            return;
        }

        // Register to the AntiAfk events to update the UI
        _AntiAfk.OnCountdownStarted.AddListener(HandleCountdownStarted);
        _AntiAfk.OnCountdownTick.AddListener(HandleCountdownTick);
        _AntiAfk.OnCountdownEnded.AddListener(HandleCountdownEnded);
        _AntiAfk.OnPlayerWakeUp.AddListener(HandleCountdownInterrupted);
    }

    private void OnDestroy()
    {
        if (_AntiAfk != null)
        {
            // Unregister from the AntiAfk events to avoid memory leaks
            _AntiAfk.OnCountdownStarted.RemoveListener(HandleCountdownStarted);
            _AntiAfk.OnCountdownTick.RemoveListener(HandleCountdownTick);
            _AntiAfk.OnCountdownEnded.RemoveListener(HandleCountdownEnded);
            _AntiAfk.OnPlayerWakeUp.RemoveListener(HandleCountdownInterrupted);
        }
    }

    private void HandleCountdownStarted(int playerId, float countdownDuration)
    {
        if (_ParentContainer != null)
            _ParentContainer.SetActive(true);
        if (_CountdownText != null)
            _CountdownText.text = $"Player {playerId} is AFK! Countdown: {countdownDuration:F0}s";
    }

    private void HandleCountdownTick(int playerId, float secondsLeft)
    {
        if (_CountdownText != null)
            _CountdownText.text = $"Player {playerId} is AFK! Countdown: {secondsLeft:F0}s";
    }

    private void HandleCountdownEnded(int playerId)
    {
        if (_ParentContainer != null)
            _ParentContainer.SetActive(false);
        if (_CountdownText != null)
            _CountdownText.text = string.Empty;
    }

    private void HandleCountdownInterrupted(int playerId)
    {
        // int playerId is not used here, but could be used to display a message, or for networking purposes

        HandleCountdownEnded(playerId);
    }
}
