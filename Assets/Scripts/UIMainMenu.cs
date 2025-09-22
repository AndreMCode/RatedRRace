using UnityEngine;

public class UIMainMenu : MonoBehaviour
{
    private int levelAccess;
    private int selectedBracket;

    void Start()
    {
        levelAccess = PlayerPrefs.GetInt("LevelAccess", 1);
    }

    void Update()
    {

    }

    void OnBracketSelect(int bracketNumber)
    {
        if (bracketNumber <= levelAccess)
        {
            selectedBracket = bracketNumber;
        }
    }
}
