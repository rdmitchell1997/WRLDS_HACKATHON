using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour {

    public Slider happiness_slider;
    public Slider hunger_slider;
    public Slider cleanliness_slider;


    public Stats stats;

    public Image fill_happiness;
    public Image fill_hunger;
    public Image fill_cleanliness;


    // Use this for initialization
    void Start () {
        stats = GameObject.FindObjectOfType<Stats>();
	}
	
	// Update is called once per frame
	void Update () {
        //Setting the sliders values
        happiness_slider.value = stats.Happiness;
        hunger_slider.value = stats.Hunger;
        cleanliness_slider.value = stats.Cleanliness;

        //Changes colour of the Happiness slider
        if (stats.Happiness > 75 && stats.Happiness < 100)
        {
            fill_happiness.color = Color.green;
        }
        else if (stats.Happiness > 25 && stats.Happiness < 74)
        {
            fill_happiness.color = Color.yellow;
        }
        else if (stats.Happiness > 0 && stats.Happiness < 24)
        {
            fill_happiness.color = Color.red;
        }

        //Changes colour of the Hunger slider
        if (stats.Hunger > 75 && stats.Hunger < 100)
        {
            fill_hunger.color = Color.green;
        }
        else if (stats.Hunger > 25 && stats.Hunger < 74)
        {
            fill_hunger.color = Color.yellow;
        }
        else if (stats.Hunger > 0 && stats.Hunger < 24)
        {
            fill_hunger.color = Color.red;
        }

        //Changes colour of the Cleanliness slider
        if (stats.Cleanliness > 75 && stats.Cleanliness < 100)
        {
            fill_cleanliness.color = Color.green;
        }
        else if (stats.Cleanliness > 25 && stats.Cleanliness < 74)
        {
            fill_cleanliness.color = Color.yellow;
        }
        else if (stats.Cleanliness > 0 && stats.Cleanliness < 24)
        {
            fill_cleanliness.color = Color.red;
        }

    }
}
