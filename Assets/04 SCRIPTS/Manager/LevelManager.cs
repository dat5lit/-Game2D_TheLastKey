using UnityEngine;

public class LevelManager : MonoBehaviour
{
    private const int MaxLevel = 5;

    public static int UnlockedLevel
    {
        get
        {
            PlayerPrefs.SetInt("UnlockedLevel", 1);
            return PlayerPrefs.GetInt("UnlockedLevel", 1);
        }
    }

    public static void UnlockNextLevel(int currentLevel)
    {
        int nextLevel = currentLevel + 1;

        // Không mở khóa vượt quá level cuối cùng
        if (nextLevel <= MaxLevel && nextLevel > UnlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", nextLevel);
            PlayerPrefs.Save();
        }
    }
}