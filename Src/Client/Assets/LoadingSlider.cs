using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSlider : MonoBehaviour
{
    public Slider loadingSlider;
    public Text loadingText;
    public  float loadTime=3f;
    private float timer;
    public GameObject loadingPanle;
    public GameObject rigiestPanle;
    public GameObject tipsPanle;
	// Use this for initialization
	void Start ()
    {
        loadingPanle.gameObject.SetActive(true);
        rigiestPanle.gameObject.SetActive(false);
        tipsPanle.gameObject.SetActive(true);
        //loadingSlider = gameObject.GetComponent<Slider>();
	}
	
	// Update is called once per frame
	void Update ()
    {

        Loading();
        Finishedloading();
    }
    void Loading()
    {
        timer += Time.deltaTime;
        if (timer>=loadTime)
        {
            timer = loadTime;
        }
        double percentage = timer / loadTime;
        loadingSlider.value =(float) percentage;
        loadingText.text = "已加载" + percentage.ToString("P") ;
    }
    void Finishedloading()
    {
        if (loadingSlider.value==1)
        {
            loadingSlider.gameObject.SetActive(false);
            rigiestPanle.gameObject.SetActive(true);
        }
    }
}
