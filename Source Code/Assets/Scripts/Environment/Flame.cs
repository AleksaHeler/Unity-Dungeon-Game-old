using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.LWRP;
using UnityEngine;

public class Flame : MonoBehaviour
{
	float baseIntensity;
	float random;
	public float frequency;
	public float flickerAmount;
	public new Light2D light;

	void Start()
    {
		light = GetComponent<Light2D>();
		baseIntensity = light.intensity;
		random = Random.Range(0, 100);
	}
	
    void Update() {
		light.intensity = baseIntensity + Mathf.PerlinNoise(random, Time.time * frequency) * flickerAmount;
	}
}
