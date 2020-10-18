using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    [SerializeField]
    private Image _loadingCircle1;

    [SerializeField]
    private Image _loadingCircle2;

    [SerializeField]
    private Text _loadingText;
    public void UpdateMainLoadingCircle(float progress)
    {
        _loadingCircle1.fillAmount = progress;
        _loadingText.text = ((int)(progress * 100)).ToString() + "%";
    } 

    public void UpdateSecondaryCircle(float progress)
    {
        _loadingCircle2.fillAmount = progress;
    }
}
