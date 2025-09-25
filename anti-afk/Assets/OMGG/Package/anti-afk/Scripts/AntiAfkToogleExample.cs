using UnityEngine;

public class AntiAfkToogleExample : MonoBehaviour
{
    public int playerId = 0;
    public AntiAfk antiAfk;

    public void ToggleAfk()
    {
        antiAfk.ToogleAfk(playerId);
    }
}
