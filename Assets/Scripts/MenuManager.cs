using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject startMenuPanel;
    public GameObject customizePanel;

    public Slider redSlider;
    public Slider greenSlider;
    public Slider blueSlider;

    public Renderer previewRenderer;
    public Material playerMaterial;
    public GameObject previewBall;

    private void Start()
    {
        bool isRestarting = PlayerPrefs.GetInt("IsRestarting", 0) == 1;

        if (!isRestarting)
        {
            Time.timeScale = 0f; // Pause the game only on first load
            startMenuPanel.SetActive(true); // Show menu only on first load
        }
        else
        {
            Time.timeScale = 1f;
            startMenuPanel.SetActive(false); // Skip menu
            PlayerPrefs.SetInt("IsRestarting", 0); // Clear the flag
        }

        // Load saved color
        float r = PlayerPrefs.GetFloat("ColorR", 1f);
        float g = PlayerPrefs.GetFloat("ColorG", 1f);
        float b = PlayerPrefs.GetFloat("ColorB", 1f);
        Color currentColor = new Color(r, g, b);
        redSlider.value = currentColor.r;
        greenSlider.value = currentColor.g;
        blueSlider.value = currentColor.b;

        UpdateColor();
        previewBall.SetActive(false);
    }



    private void Update()
    {
        UpdateColor();


        if (previewBall.activeSelf)
        {
            previewBall.transform.Rotate(Vector3.up, 30f * Time.deltaTime);
        }
    }

    public void PlayGame()
    {
        // Save selected color
        PlayerPrefs.SetFloat("ColorR", redSlider.value);
        PlayerPrefs.SetFloat("ColorG", greenSlider.value);
        PlayerPrefs.SetFloat("ColorB", blueSlider.value);
        startMenuPanel.SetActive(false);

        Time.timeScale = 1f; // Resume time when starting the game
    }

    public void OpenCustomize()
    {
        startMenuPanel.SetActive(false);
        customizePanel.SetActive(true);
        previewBall.SetActive(true);
    }

    public void BackToMenu()
    {
        customizePanel.SetActive(false);
        startMenuPanel.SetActive(true);
        previewBall.SetActive(false);
    }

    public void UpdateColor()
    {
        Color newColor = new Color(redSlider.value, greenSlider.value, blueSlider.value);
        previewRenderer.material.color = newColor;
        playerMaterial.color = newColor;
    }

    public void Restart()
    {
        PlayerPrefs.SetInt("IsRestarting", 1); // Mark that we're restarting
        Time.timeScale = 1f; // Ensure time is normal before reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
