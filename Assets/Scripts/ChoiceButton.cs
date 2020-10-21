using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceButton : MonoBehaviour
{
    [SerializeField]
    private Text _choiceText;

    private Button _button;

    public Action<int> OnChoiceSelected;

    public int choiceIndex;

    // Start is called before the first frame update
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        OnChoiceSelected?.Invoke(choiceIndex);
    }

    public void SetButtonText(string text)
    {
        _choiceText.text = text;
    }
}
