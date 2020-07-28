using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Image fillImage;
    private Coroutine progressCorotine;
    public int minValue;
    public int maxValue = 10;

    public void StartLevel()
    {
        minValue = 0;
        if (progressCorotine != null)
        {
            StopCoroutine(progressCorotine);
        }
        if(this.gameObject.activeSelf)
        {
            progressCorotine = StartCoroutine(FillProgressBar());
        }
    }
    private IEnumerator FillProgressBar()
    {
        while(true)
        {
            yield return null;
            fillImage.fillAmount = CharacterController.GetInstance.transform.position.z / 300.0f;
        }
    }
    public void StopLevelprogress()
    {
        if (progressCorotine != null)
        {
            StopCoroutine(progressCorotine);
        }
    }
}
