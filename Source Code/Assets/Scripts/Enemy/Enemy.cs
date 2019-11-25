using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	public void Damage(int damage) {
		Debug.Log("Enemy hit. Damage: " + damage);
	}
}
