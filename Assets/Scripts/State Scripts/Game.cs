public static class Game
{
    public static GameContext Ctx { get; private set; }

    public static void SetContext(GameContext ctx)
    {
        if (Ctx != null && Ctx != ctx)
        {
            UnityEngine.Debug.LogError("GameContext already set. Shawn, you fool, did you load Bootstrap twice?");
            return;
        }

        Ctx = ctx;
    }

    public static bool IsReady => Ctx != null;
}