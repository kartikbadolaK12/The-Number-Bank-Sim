using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneChanger : MonoBehaviour
{
    [Header("Optional Fade Canvas (assign CanvasGroup)")]
    public CanvasGroup fadeCanvas;

    [Header("Transition Settings")]
    public float fadeDuration = 1f;        // Time to fade in/out
    public float delayBeforeStart = 0f;    // ⏳ NEW delay before fade starts
    public float delayBeforeLoad = 0f;     // Delay after fade, before scene load

    private bool isTransitioning = false;

    // Call from Button (string overload)
    public void LoadScene(string sceneName)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneRoutine(sceneName));
    }

    // Call by index overload
    public void LoadScene(int sceneIndex)
    {
        if (!isTransitioning)
            StartCoroutine(LoadSceneRoutine(SceneManager.GetSceneByBuildIndex(sceneIndex).name));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        isTransitioning = true;

        // ⏳ 0️⃣ Optional delay BEFORE fade starts
        if (delayBeforeStart > 0)
            yield return new WaitForSeconds(delayBeforeStart);

        // 1️⃣ Fade to black
        if (fadeCanvas)
            yield return StartCoroutine(Fade(0f, 1f, fadeDuration));

        // 2️⃣ Optional delay BEFORE loading
        if (delayBeforeLoad > 0)
            yield return new WaitForSeconds(delayBeforeLoad);

        // 3️⃣ Load Scene
        SceneManager.LoadScene(sceneName);

        yield return null;

        // 4️⃣ Fade back
        if (fadeCanvas)
            StartCoroutine(Fade(1f, 0f, fadeDuration));
    }

    private IEnumerator Fade(float from, float to, float duration)
    {
        fadeCanvas.gameObject.SetActive(true);
        float elapsed = 0f;
        fadeCanvas.alpha = from;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            fadeCanvas.alpha = Mathf.Lerp(from, to, elapsed / duration);
            yield return null;
        }

        fadeCanvas.alpha = to;

        if (to == 0f)
            fadeCanvas.gameObject.SetActive(false);
    }
}
