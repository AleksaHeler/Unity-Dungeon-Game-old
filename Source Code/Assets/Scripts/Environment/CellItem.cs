using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CellItem {

	public GameObject item;
	public string name;
	public int minNum;
	public int maxNum;
	public float chance;
	public float groupingDistance;
	public Vector3 offset;
}
