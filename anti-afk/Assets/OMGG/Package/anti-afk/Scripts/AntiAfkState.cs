using UnityEngine;

enum AntiAfkState
{
    Idle,               // Not doing anything
    WaitingForInput,    // Waiting for user input
    Coutdown            // Countdown before triggering anti-AFK action
}
