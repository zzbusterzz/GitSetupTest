using UnityEngine;

public class Constants
{
    private static Vector3 InvertedRot = new Vector3(0, 180, 0);

    public static Quaternion QInvertedRot = Quaternion.Euler(InvertedRot);
    public static Quaternion QRevealRot = Quaternion.identity;

    public const string Key = "LevelSave";

    public const float FlipTime = 1;
    public const float GridStartPercent = 20;
}