using UnityEngine;
using System.Collections;

public class UIMainMenu : MonoBehaviour
{
    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject shopButton;
    // [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject endlessMenu;
    [SerializeField] GameObject shopMenu;
    public int levelAccess;
    public int shopAccess;

    void Start()
    {
        DisplayMainMenu();
    }

    void Update()
    {

    }

    private IEnumerator LoadRunMode()
    {
        yield return _waitForSeconds0_5;
        UnityEngine.SceneManagement.SceneManager.LoadScene("RunMode");
    }

    public void OnClickPlay()
    {
        DisplayPlayMenu();
    }

    public void OnClickBronze()
    {
        if (levelAccess >= 1)
        {
            if (shopAccess == 0) PlayerPrefs.SetInt("ShopAccess", 1);

            Debug.Log("Bracket accessible");
            PlayerPrefs.SetInt("SelectedBracket", 1);
            playMenu.SetActive(false);

            // Store other powerup quantities in playerprefs to be applied in the run

            StartCoroutine(LoadRunMode());
        }
        else
        {
            Debug.Log("Access denied");
        }
    }

    public void OnClickSilver()
    {
        if (levelAccess >= 2)
        {
            Debug.Log("Bracket accessible");
            PlayerPrefs.SetInt("SelectedBracket", 2);
            playMenu.SetActive(false);

            // Store other powerup quantities in playerprefs to be applied in the run

            StartCoroutine(LoadRunMode());
        }
        else
        {
            Debug.Log("Access denied");
        }
    }

    public void OnClickGold()
    {
        if (levelAccess >= 3)
        {
            Debug.Log("Bracket accessible");
            PlayerPrefs.SetInt("SelectedBracket", 3);
            playMenu.SetActive(false);

            // Store other powerup quantities in playerprefs to be applied in the run

            StartCoroutine(LoadRunMode());
        }
        else
        {
            Debug.Log("Access denied");
        }
    }

    public void OnClickEndless()
    {
        if (levelAccess >= 4)
        {
            Debug.Log("Bracket accessible");
            PlayerPrefs.SetInt("SelectedBracket", 4);
            playMenu.SetActive(false);

            // Store other powerup quantities in playerprefs to be applied in the run

            StartCoroutine(LoadRunMode());
        }
        else
        {
            Debug.Log("Access denied");
        }
    }

    public void OnClickPlayBack()
    {
        DisplayMainMenu();
    }

    public void OnClickSettings()
    {
        DisplaySettingsMenu();
    }

    public void OnClickShop()
    {
        DisplayShopMenu();
    }

    public void OnClickResetProgress()
    {
        // Also reset high scores?
        PlayerPrefs.SetInt("LevelAccess", 1);
        PlayerPrefs.SetInt("ShopAccess", 0);
        levelAccess = 1;
        DisplayMainMenu();
    }

    public void OnClickSettingsBack()
    {
        DisplayMainMenu();
    }

    void DisplayMainMenu()
    {
        levelAccess = PlayerPrefs.GetInt("LevelAccess", 1);
        shopAccess = PlayerPrefs.GetInt("ShopAccess", 0);

        mainMenu.SetActive(true);

        playMenu.SetActive(false);
        endlessMenu.SetActive(false);
        settingsMenu.SetActive(false);
        shopMenu.SetActive(false);

        if (shopAccess == 1) shopButton.SetActive(true);
    }

    void DisplayPlayMenu()
    {
        playMenu.SetActive(true);

        mainMenu.SetActive(false);
        if (levelAccess > 3)
        {
            endlessMenu.SetActive(true);
        }
        shopButton.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void DisplaySettingsMenu()
    {
        settingsMenu.SetActive(true);

        mainMenu.SetActive(false);
        shopButton.SetActive(false);
        playMenu.SetActive(false);
        endlessMenu.SetActive(false);
    }

    void DisplayShopMenu()
    {
        shopMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
}
