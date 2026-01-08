using System.Collections.Generic;
using UnityEngine;

public class SilkBar : MonoBehaviour
{
    public Transform barPotential;
    public Transform barFill;
    public Transform divider;
    public Transform barBackground;

    private Material divLineMat;

    public int StaminaPerLine = 10; //10 stamina for one line;

    private float _value;
    public float value
    {
        get => _value;
        set
        {
            _value = value;
            RefreshBar();
        }
    }

    private float barBackgroundWidth = 2f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        barBackgroundWidth = barBackground.transform.localScale.x;

        SpriteRenderer mr = divider.GetComponent<SpriteRenderer>();
        divLineMat = mr.material;
    }

    public void SetTickCount(int count)
    {
        if (divLineMat != null)
        {
            divLineMat.SetFloat("_TickCount", count);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<SpriteRenderer> GetSpriteRenderers()
    {
        var ret = new List<SpriteRenderer>();
        ret.Add(barBackground.GetComponent<SpriteRenderer>());
        ret.Add(barPotential.GetComponent<SpriteRenderer>());
        ret.Add(barFill.GetComponent<SpriteRenderer>());
        ret.Add(divider.GetComponent<SpriteRenderer>());
        return ret;
    }

    public void RefreshSilkBar(SpiderController spider)
    {
        value = (float)spider.stamina / (float)spider.maxStamina;

        //how many lines
        int lines = Mathf.CeilToInt((float)spider.maxStamina / (float)StaminaPerLine);
        SetTickCount(lines);

        RefreshPotentialBar(spider);
    }

    private void RefreshPotentialBar(SpiderController spider)
    {
        float percent = (float)(spider.stamina - spider.potentialStaminaUse) / (float)spider.maxStamina;
        var fillscale = barPotential.transform.localScale;
        fillscale.x = barBackgroundWidth * percent;

        barFill.localScale = fillscale;
    }

    public void RefreshBar()
    {
        var fillscale = barFill.transform.localScale;
        fillscale.x = barBackgroundWidth * value;

        barPotential.localScale = fillscale;
    }
}
