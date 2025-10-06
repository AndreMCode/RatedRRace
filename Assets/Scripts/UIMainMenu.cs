using UnityEngine;
using System.Collections;
using UnityEditor;

public class UIMainMenu : MonoBehaviour
{
    public AudioSource menuBGM;
    private static WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject buffsMenu;
    [SerializeField] GameObject shopButton;
    // [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject endlessMenu;
    public int levelAccess;
    public int shopAccess;

    void Start()
    {
        if (menuBGM != null)
        {
            menuBGM.Play();
        }

        DisplayMainMenu();
    }

    void Update()
    {

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
        StartCoroutine(FadeOutTracks(0.4f));
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

            DisplayBuffsMenu();
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
            
            DisplayBuffsMenu();
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
            
            DisplayBuffsMenu();
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
            
            DisplayBuffsMenu();
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

    public void OnClickBuffPlay()
    {
        buffsMenu.SetActive(false);

        StartCoroutine(LoadRunMode());
    }

    public void OnClickAddBubble()
    {
        Debug.Log("Added a Bubble Shield to the player!");
    }

    public void OnClickBuffPlayBack()
    {
        DisplayPlayMenu();
    }

    public void OnClickSettings()
    {
        DisplaySettingsMenu();
    }

    public void OnClickResetProgress()
    {
        // Also reset high scores?
        PlayerPrefs.SetInt("LevelAccess", 3); // !! RESET TO 1 BEFORE BUILD !!
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
        buffsMenu.SetActive(false);
        endlessMenu.SetActive(false);
        settingsMenu.SetActive(false);

        if (shopAccess == 1) shopButton.SetActive(true);
    }

    void DisplayPlayMenu()
    {
        playMenu.SetActive(true);

        buffsMenu.SetActive(false);
        mainMenu.SetActive(false);
        if (levelAccess > 3)
        {
            endlessMenu.SetActive(true);
        }
        shopButton.SetActive(false);
        settingsMenu.SetActive(false);
    }

    void DisplayBuffsMenu()
    {
        buffsMenu.SetActive(true);

        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        settingsMenu.SetActive(false);
        shopButton.SetActive(false);
        endlessMenu.SetActive(false);
    }

    void DisplaySettingsMenu()
    {
        settingsMenu.SetActive(true);

        mainMenu.SetActive(false);
        playMenu.SetActive(false);
        buffsMenu.SetActive(false);
        shopButton.SetActive(false);
        endlessMenu.SetActive(false);
    }
}
