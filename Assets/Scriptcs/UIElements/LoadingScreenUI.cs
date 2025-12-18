using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenUI : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;
    [SerializeField] private Slider loadingSlider;
    [SerializeField] private TextMeshProUGUI sliderCounter;
    [SerializeField] private float duration = 1f;
    public void ActiveLoadingScreen()
    {
        gameObject.SetActive(true);
        StartCoroutine(LoadingRoutine());
    }

    private IEnumerator LoadingRoutine()
    {
        float elapsedTime = 0f;

        loadingSlider.value = 0;
        loadingSlider.maxValue = 100;
        sliderCounter.text = $"{loadingSlider.value:0}%";

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / duration;

            loadingSlider.value = Mathf.Lerp(0, 100, progress);
            sliderCounter.text = $"{loadingSlider.value:0}%";
            yield return null;
        }
        loadingSlider.value = 100;


        while (uiManager.LoadingComplete() == false)
            yield return new WaitForSeconds(0.1f);

        gameObject.SetActive(false);
    }
}
