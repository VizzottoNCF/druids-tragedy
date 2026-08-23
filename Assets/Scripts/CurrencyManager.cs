using UnityEngine;
using TMPro;
using System.Collections;
using Unity.VisualScripting;

public class CurrencyManager : MonoBehaviour
{
    public int currencyAmount = 5;
    public int currencyVisual;
    public TMP_Text txt;
    private bool isAnimating = false;

    private void Awake() => ServiceLocator.Register<CurrencyManager>(this);

    private void Start() { StartCoroutine(AnimateCurrencyChange()); }
    public void Add(int x)
    {
        currencyAmount += x;
        if (isAnimating) { StopAllCoroutines(); }
        StartCoroutine(AnimateCurrencyChange());
    }

    public IEnumerator AnimateCurrencyChange()
    {
        isAnimating = true;
        int targetAmount = currencyAmount;
        while (currencyVisual != targetAmount)
        {
            if (currencyVisual < targetAmount)
            {
                currencyVisual++;
            }
            else
            {
                currencyVisual--;
            }
            UpdateText();
            yield return new WaitForSeconds(0.05f);
        }
        isAnimating = false;
        yield return null;
    }

    public void UpdateText()
    {
        txt.text = $"{currencyVisual}";
    }
}
