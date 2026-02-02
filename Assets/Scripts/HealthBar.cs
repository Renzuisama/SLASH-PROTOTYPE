using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBar : MonoBehaviour
{

	public Slider slider;
	public Gradient gradient;
	public Image fill;
	public TextMeshProUGUI healthText;

	public void SetMaxHealth(int health)
	{
		slider.maxValue = health;
		slider.value = health;

		fill.color = gradient.Evaluate(1f);

		if (healthText != null)
		{
			healthText.text = $"{health} / {health}";
		}
	}

    public void SetHealth(int health)
	{
		slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);

		if (healthText != null)
		{
			healthText.text = $"{health} / {slider.maxValue}";
		}
	}

}