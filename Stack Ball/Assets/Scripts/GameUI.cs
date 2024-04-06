using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("InGame")]
    public Image levelSlider;
    public Image currentLevelImg;
    public Image nextlevelImg;

    private Material ballMat;

    void Awake()
    {
        ballMat = FindObjectOfType<Ball>().transform.GetChild(0).GetComponent<MeshRenderer>().material;

        levelSlider.transform.parent.GetComponent<Image>().color = ballMat.color + Color.gray;
        levelSlider.color = ballMat.color;
        currentLevelImg.color = ballMat.color;
        nextlevelImg.color = ballMat.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void levelSliderFill(float fillAmount)
    {
        levelSlider.fillAmount = fillAmount;
    }
}
