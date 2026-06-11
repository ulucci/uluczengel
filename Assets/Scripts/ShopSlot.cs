using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopSlot : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private NPCDialogue npc;
    private ReceptionCounter counter;

    private Image hoverOverlay;
    private TextMeshProUGUI priceLabel;
    private CanvasGroup canvasGroup;
    private bool animating;

    private Color priceColor;

    public void Init(NPCDialogue npcRef, ReceptionCounter counterRef)
    {
        npc     = npcRef;
        counter = counterRef;
        priceColor = npcRef.ItemCost > 0 ? Color.yellow : Color.green;

        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        var bg = gameObject.AddComponent<Image>();
        bg.color = new Color(0.13f, 0.13f, 0.13f, 0.95f);

        // Ürün görseli — AspectRatioFitter ile doğru oran
        var imgGO = MakeChild("Img");
        SetAnchors(imgGO, 0.1f, 0.3f, 0.9f, 0.95f);
        var img = imgGO.AddComponent<Image>();
        img.sprite = npc.ItemSprite;
        img.preserveAspect = true;
        if (npc.ItemSprite == null)
        {
            img.color = new Color(0.4f, 0.4f, 0.4f);
        }
        else
        {
            var arf = imgGO.AddComponent<AspectRatioFitter>();
            arf.aspectMode  = AspectRatioFitter.AspectMode.FitInParent;
            arf.aspectRatio = (float)npc.ItemSprite.rect.width / npc.ItemSprite.rect.height;
        }

        // İsim (alt, her zaman görünür)
        var nameGO = MakeChild("Name");
        SetAnchors(nameGO, 0f, 0f, 1f, 0.3f);
        var nameTmp = nameGO.AddComponent<TextMeshProUGUI>();
        nameTmp.text      = npc.ItemName;
        nameTmp.alignment = TextAlignmentOptions.Center;
        nameTmp.fontSize  = 11;
        nameTmp.color     = Color.white;

        // Hover overlay (karartma)
        var overlayGO = MakeChild("Overlay");
        SetAnchors(overlayGO, 0f, 0f, 1f, 1f);
        hoverOverlay       = overlayGO.AddComponent<Image>();
        hoverOverlay.color = new Color(0f, 0f, 0f, 0f);

        // Fiyat (overlay üstünde, hover'da görünür)
        var priceGO = MakeChild("Price");
        SetAnchors(priceGO, 0f, 0.3f, 1f, 0.7f);
        priceLabel            = priceGO.AddComponent<TextMeshProUGUI>();
        priceLabel.text       = npc.ItemCost > 0 ? npc.ItemCost + " ₺" : "Ücretsiz";
        priceLabel.alignment  = TextAlignmentOptions.Center;
        priceLabel.fontSize   = 15;
        priceLabel.fontStyle  = FontStyles.Bold;
        priceLabel.color      = new Color(priceColor.r, priceColor.g, priceColor.b, 0f);
    }

    // ── Hover ────────────────────────────────────────────────────────────────

    public void OnPointerEnter(PointerEventData _)
    {
        if (animating) return;
        hoverOverlay.DOFade(0.55f, 0.12f);
        priceLabel.DOFade(1f, 0.12f);
    }

    public void OnPointerExit(PointerEventData _)
    {
        if (animating) return;
        hoverOverlay.DOFade(0f, 0.12f);
        priceLabel.DOFade(0f, 0.12f);
    }

    // ── Tıklama ──────────────────────────────────────────────────────────────

    public void OnPointerClick(PointerEventData _)
    {
        if (animating) return;

        bool canAfford = npc.ItemCost == 0 || GameManager.Instance.Money >= npc.ItemCost;
        if (canAfford) PlayBuyAnim();
        else           PlayDenyAnim();
    }

    private void PlayBuyAnim()
    {
        animating = true;
        transform.DOPunchScale(Vector3.one * 0.18f, 0.22f, 8, 0.5f)
            .OnComplete(() =>
                canvasGroup.DOFade(0f, 0.25f)
                    .OnComplete(() => counter.CompletePurchase(npc, gameObject)));
    }

    private void PlayDenyAnim()
    {
        animating = true;
        // Fiyatı kırmızı yak, salla, geri al
        priceLabel.DOFade(1f, 0f);
        hoverOverlay.DOFade(0.55f, 0f);

        DOTween.Sequence()
            .Append(priceLabel.DOColor(Color.red, 0.08f))
            .Append(((RectTransform)transform).DOShakeAnchorPos(0.35f, new Vector2(10f, 0f), 20, 0f))
            .Append(priceLabel.DOColor(priceColor, 0.2f))
            .OnComplete(() =>
            {
                animating = false;
                hoverOverlay.DOFade(0f, 0.12f);
                priceLabel.DOFade(0f, 0.12f);
            });
    }

    // ── Yardımcı ─────────────────────────────────────────────────────────────

    private GameObject MakeChild(string n)
    {
        var go = new GameObject(n, typeof(RectTransform));
        go.transform.SetParent(transform, false);
        return go;
    }

    private static void SetAnchors(GameObject go,
        float xMin, float yMin, float xMax, float yMax)
    {
        var r = go.GetComponent<RectTransform>();
        r.anchorMin = new Vector2(xMin, yMin);
        r.anchorMax = new Vector2(xMax, yMax);
        r.offsetMin = r.offsetMax = Vector2.zero;
    }
}
