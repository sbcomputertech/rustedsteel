namespace RustedSteelMod;

public static class Utils
{
    public static bool NextBool(this Random rng)
    {
        return rng.Next(0, 2) > 0;
    }
}