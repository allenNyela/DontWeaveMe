using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    public SpiderController spider;
    public RectTransform potentialBar;
    public RectTransform actualBgBar;
    public Slider slider;
    private float maxBarWidth = 150f;
    private float potentialBarWidth;
    private float heathWidth = 30;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        maxBarWidth = 150f;
    }

    public void RefreshBarSize(SpiderController spider)
    {
        this.spider = spider;

        float percent = (float)spider.maxStamina / heathWidth;
        potentialBar.sizeDelta = new Vector2(maxBarWidth * percent, potentialBar.sizeDelta.y);
        actualBgBar.sizeDelta = new Vector2(maxBarWidth * percent, actualBgBar.sizeDelta.y);

        potentialBarWidth = potentialBar.sizeDelta.x;
        slider.GetComponent<RectTransform>().sizeDelta = new Vector2(maxBarWidth * percent, slider.GetComponent<RectTransform>().sizeDelta.y);

        //Debug.LogError($"============> percent: {percent} max bard width {maxBarWidth} {slider.GetComponent<RectTransform>().sizeDelta}");
    }

    public void UpdateStaminaRealtime()
    {
        slider.value = (float)(spider.stamina - spider.potentialStaminaUse) / (float)spider.maxStamina;
    }

    public void UpdateStaminaConsume()
    {
        float percent = (float)spider.stamina / (float)spider.maxStamina;
        potentialBar.sizeDelta = new Vector2(potentialBarWidth * percent, potentialBar.sizeDelta.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
