using UnityEngine;

public static class FrameRateManager
{

    [RuntimeInitializeOnLoadMethod]
    static void SetFrameRate()
    {
        Application.targetFrameRate = 60;
    }
}
