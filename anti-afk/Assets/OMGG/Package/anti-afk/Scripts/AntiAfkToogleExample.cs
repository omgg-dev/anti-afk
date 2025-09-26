using UnityEngine;

public class AntiAfkToogleExample : MonoBehaviour
{
    public int playerId = 0;
    public AntiAfk AntiAfk;

    public void ToggleAfk()
    {
        AntiAfk.ToogleAfk(playerId);
    }
}
