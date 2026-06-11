using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text moneyText;
    [SerializeField] private string prefix = "Para: ";
    [SerializeField] private string suffix = " ₺";

    private void Awake()
    {
        if (moneyText == null) moneyText = GetComponent<TMP_Text>();
        if (moneyText == null) moneyText = GetComponentInChildren<TMP_Text>();
    }

    private void OnEnable()
    {
        GameManager.OnMoneyChanged += Refresh;
        if (GameManager.Instance != null)
            Refresh(GameManager.Instance.Money);
    }

    private void OnDisable() => GameManager.OnMoneyChanged -= Refresh;

    private void Refresh(int amount)
    {
        if (moneyText == null) return;
        moneyText.text = prefix + amount + suffix;
        moneyText.transform.DOKill();
        moneyText.transform.DOPunchScale(Vector3.one * 0.25f, 0.3f, 6, 0.5f);
    }
}
