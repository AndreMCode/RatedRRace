using UnityEngine;
using System.Collections;
using UnityEditor;
using TMPro;

public class UIMainMenu : MonoBehaviour
{
    // Main Menu user interface elements, graphics, audio, and logic
    // -------------------------------------------------------------

    public AudioSource menuIntro; // plays once, from time 0
    public AudioSource menuLoop;  // looped source; its clip should be the loop section and set to loop = true
    private static readonly WaitForSeconds _waitForSeconds0_5 = new(0.5f);
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject playMenu;
    [SerializeField] GameObject buffsMenu;

    [SerializeField] TextMeshProUGUI bubbleCountTxt;

    [SerializeField] GameObject endlessMenu;
    [SerializeField] GameObject shopButton;
    [SerializeField] GameObject shopMenu;
    [SerializeField] GameObject settingsMenu;
    [SerializeField] GameObject InstructionMenu;
    [SerializeField] GameObject InstructionMenuPG1;
    [SerializeField] GameObject InstructionMenuPG2;
    [SerializeField] GameObject InstructionMenuPG3;
    [SerializeField] GameObject InstructionMenuPG4;
    [SerializeField] GameObject InstructionMenuPG5;
    public int levelAccess;
    public int shopAccess;
    private int instpage;

    void Start()
    {
        instpage = 0;

        HideAllMenus();
        SetMenuAudio();
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
        InstructionMenu.SetActive(false);
        InstructionMenuPG1.SetActive(false);
        InstructionMenuPG2.SetActive(false);
        InstructionMenuPG3.SetActive(false);
        InstructionMenuPG4.SetActive(false);
        InstructionMenuPG5.SetActive(false);
    }

    void SetMenuAudio()
    {
        // Use DSP scheduling for seamless handoff from intro to loop.
        if (menuIntro != null && menuLoop != null && menuIntro.clip != null && menuLoop.clip != null)
        {
            // Ensure loop source is set to loop
            menuLoop.loop = true;

            // Start intro immediately using PlayScheduled
            double dspStart = AudioSettings.dspTime + 0.05; // small lead time
            menuIntro.PlayScheduled(dspStart);

            // Schedule the loop to start exactly when intro ends
            double introDuration = menuIntro.clip.length - menuIntro.time;
            double loopStartDsp = dspStart + introDuration;
            menuLoop.PlayScheduled(loopStartDsp);
        }
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

    // Add Bubble Shield to player loadout
    public void OnClickAddBubble()
    {
        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 1)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountBronze", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 10f * (count + 1);

            if (bubblePrice <= currentMoney)
            {
                currentMoney -= bubblePrice;
                count++;
                bubblePrice = 10f * (count + 1);
                PlayerPrefs.SetInt("BubbleShieldCountBronze", count);
                PlayerPrefs.SetFloat("Money", currentMoney);
            }

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 2)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountSilver", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 15f * (count + 1);

            if (bubblePrice <= currentMoney)
            {
                currentMoney -= bubblePrice;
                count++;
                bubblePrice = 15f * (count + 1);
                PlayerPrefs.SetInt("BubbleShieldCountSilver", count);
                PlayerPrefs.SetFloat("Money", currentMoney);
            }

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 3)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountGold", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 20f * (count + 1);

            if (bubblePrice <= currentMoney)
            {
                currentMoney -= bubblePrice;
                count++;
                bubblePrice = 20f * (count + 1);
                PlayerPrefs.SetInt("BubbleShieldCountGold", count);
                PlayerPrefs.SetFloat("Money", currentMoney);
            }

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 4)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCount", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 25f * (count + 1);

            if (bubblePrice <= currentMoney)
            {
                currentMoney -= bubblePrice;
                count++;
                bubblePrice = 25f * (count + 1);
                PlayerPrefs.SetInt("BubbleShieldCount", count);
                PlayerPrefs.SetFloat("Money", currentMoney);
            }

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }
    }

    // Start run
    public void OnClickBuffPlay()
    {
        buffsMenu.SetActive(false);

        // Used for locking out Shop button until first attempt
        if (shopAccess == 0) PlayerPrefs.SetInt("ShopAccess", 1);

        StartCoroutine(LoadRunMode());
    }

    public void OnClickShop()
    {
        DisplayShopMenu();
    }

    public void OnClickInstruction()
    {
        instpage = 1;
        Debug.Log(instpage);
        DisplayInstructionMenu();
    }

    public void CycleInstruction()
    {
        instpage++;
        DisplayInstructionMenu();
    }

    public void OnClickSettings()
    {
        DisplaySettingsMenu();
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }

    void OnClickBack()
    {
        DisplayMainMenu();
    }


    // Reset game progress
    public void OnClickResetProgress()
    {
        PlayerPrefs.SetFloat("BestBronze", 0f);
        PlayerPrefs.SetFloat("BestSilver", 0f);
        PlayerPrefs.SetFloat("BestGold", 0f);
        PlayerPrefs.SetFloat("BestEndless", 0f);

        PlayerPrefs.SetInt("LevelAccess", 1);
        PlayerPrefs.SetFloat("Money", 0f);
        PlayerPrefs.SetInt("BubbleShieldCountBronze", 0);
        PlayerPrefs.SetInt("BubbleShieldCountSilver", 0);
        PlayerPrefs.SetInt("BubbleShieldCountGold", 0);
        PlayerPrefs.SetInt("BubbleShieldCount", 0);
        PlayerPrefs.SetFloat("BestSilver", 0f);
        PlayerPrefs.SetFloat("BestEndless", 0f);
        PlayerPrefs.SetInt("ShopAccess", 0);
        levelAccess = 1;
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

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 1)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountBronze", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 10f * (count + 1);

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 2)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountSilver", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 15f * (count + 1);

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 3)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCountGold", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 20f * (count + 1);

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }

        if (PlayerPrefs.GetInt("SelectedBracket", 0) == 4)
        {
            int count = PlayerPrefs.GetInt("BubbleShieldCount", 0);
            float currentMoney = PlayerPrefs.GetFloat("Money", 0f);
            float bubblePrice = 25f * (count + 1);

            bubbleCountTxt.text = "Funds:" + currentMoney.ToString("F2") + " - Cost: $" + bubblePrice.ToString() + " - Owned: x" + count.ToString();
        }
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

    void DisplayInstructionMenu()
    {
        HideAllMenus();
        //Cycles through the instruction pages when prompted
        InstructionMenu.SetActive(true);
        switch (instpage)
        {
            case 1:
                InstructionMenuPG1.SetActive(true);
                return;
            case 2:
                InstructionMenuPG2.SetActive(true);
                return;
            case 3:
                InstructionMenuPG3.SetActive(true);
                return;
            case 4:
                InstructionMenuPG4.SetActive(true);
                return;
            case 5:
                InstructionMenuPG5.SetActive(true);
                return;
            default:
                break;
        }
    }

    private IEnumerator FadeOutTracks(float fadeDuration)
    {
        // Fade both intro and loop sources if present
        AudioSource[] targets = new AudioSource[] { menuIntro, menuLoop };
        float[] startVolumes = new float[targets.Length];
        for (int i = 0; i < targets.Length; i++) startVolumes[i] = targets[i] != null ? targets[i].volume : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / fadeDuration;

            for (int i = 0; i < targets.Length; i++)
            {
                var src = targets[i];
                if (src == null) continue;
                src.volume = Mathf.Lerp(startVolumes[i], 0f, t);
            }

            yield return null;
        }

        // Stop sources and reset volumes to 0
        for (int i = 0; i < targets.Length; i++)
        {
            var src = targets[i];
            if (src == null) continue;
            src.Stop();
            src.volume = 0f;
        }
    }

    private IEnumerator LoadRunMode()
    {
        StartCoroutine(FadeOutTracks(0.5f));
        yield return _waitForSeconds0_5;
        UnityEngine.SceneManagement.SceneManager.LoadScene("RunMode");
    }
}
