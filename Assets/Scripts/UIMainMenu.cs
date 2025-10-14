using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIMainMenu : MonoBehaviour
{
    // Main Menu user interface elements, graphics, audio, and logic
    // -------------------------------------------------------------

    public AudioSource menuBGM;
    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject buffsMenu;
    [SerializeField] GameObject endlessMenu;
    [SerializeField] GameObject shopButton;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject settingsMenu;
    public int levelAccess;
    public int shopAccess;

    void Start()
    {
        HideAllMenus();

        if (menuBGM != null)
        {
            menuBGM.Play();
        }

        DisplayMainMenu();
    }

    void HideAllMenus()
    {
        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        buffsMenu.SetActive(false);
        endlessMenu.SetActive(false);
        shopButton.SetActive(false);
        shopMenu.SetActive(false);
        settingsMenu.SetActive(false);
    }

    public void OnClickPlay()
    {
        DisplayPlayMenu();
    }

    public void OnClickBronze()
    {
        if (levelAccess >= 1)
        {
            PlayerPrefs.SetInt("SelectedBracket", 1);

            DisplayBuffsMenu();
        }
    }

    public void OnClickSilver()
    {
        if (levelAccess >= 2)
        {
            PlayerPrefs.SetInt("SelectedBracket", 2);

            DisplayBuffsMenu();
        }
    }

    public void OnClickGold()
    {
        if (levelAccess >= 3)
        {
            PlayerPrefs.SetInt("SelectedBracket", 3);

            DisplayBuffsMenu();
        }
    }

    public void OnClickEndless()
    {
        if (levelAccess >= 4)
        {
            PlayerPrefs.SetInt("SelectedBracket", 4);

            DisplayBuffsMenu();
        }
    }

    public void OnClickPlayBack()
    {
        DisplayMainMenu();
    }

    // Add Bubble Shield to player loadout
    public void OnClickAddBubble()
    {
        Debug.Log("Added a Bubble Shield to the player!");
    }

    // Start run
    public void OnClickBuffPlay()
    {
        buffsMenu.SetActive(false);

        // Used for locking out Shop button until first attempt
        if (shopAccess == 0) PlayerPrefs.SetInt("ShopAccess", 1);

        StartCoroutine(LoadRunMode());
    }

    public void OnClickBuffPlayBack()
    {
        DisplayPlayMenu();
    }

    public void OnClickShop()
    {
        DisplayShopMenu();
    }

    public void OnClickShopBack()
    {
        DisplayMainMenu();
    }

    public void OnClickSettings()
    {
        DisplaySettingsMenu();
    }

    // Reset game progress
    public void OnClickResetProgress()
    {
        // Also reset high scores?
        PlayerPrefs.SetInt("LevelAccess", 4); // !! RESET TO 1 BEFORE BUILD !!
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
        HideAllMenus();
        
        levelAccess = PlayerPrefs.GetInt("LevelAccess", 1);
        shopAccess = PlayerPrefs.GetInt("ShopAccess", 0);

        mainMenu.SetActive(true);
        if (shopAccess == 1) shopButton.SetActive(true);
    }

    void DisplayPlayMenu()
    {
        HideAllMenus();

        playMenu.SetActive(true);
        if (levelAccess > 3) endlessMenu.SetActive(true);
    }

    void DisplayBuffsMenu()
    {
        HideAllMenus();

        buffsMenu.SetActive(true);
    }

    void DisplayShopMenu()
    {
        HideAllMenus();

        shopMenu.SetActive(true);
    }

    void DisplaySettingsMenu()
    {
        HideAllMenus();

        settingsMenu.SetActive(true);
    }

    private IEnumerator FadeOutTracks(float fadeDuration)
    {
        if (menuBGM != null)
        {
            float startVolume = menuBGM.volume;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float newVolume = Mathf.Lerp(startVolume, 0, elapsedTime / fadeDuration);

                menuBGM.volume = newVolume;

                yield return null;
            }

            menuBGM.Stop();
        }
    }

    private IEnumerator LoadRunMode()
    {
        StartCoroutine(FadeOutTracks(0.5f));
        yield return _waitForSeconds0_5;
        UnityEngine.SceneManagement.SceneManager.LoadScene("RunMode");
    }
}
