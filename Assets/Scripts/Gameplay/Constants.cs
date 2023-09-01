using UnityEngine;

public class Constants
{
    private static Vector3 InvertedRot = new Vector3(0, 180, 0);

    public static Quaternion QInvertedRot = Quaternion.Euler(InvertedRot);
    public static Quaternion QRevealRot = Quaternion.identity;

    public const string MatTexStr = "_CardFront";
    public const float FlipTime = 1;

    public static Vector2Int MaxAllowedCards = new Vector2Int(13, 4);
    public const float GridStartPercent = 20;
}
