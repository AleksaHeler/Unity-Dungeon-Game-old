using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Tilemaps;
using UnityEngine;


// TODO: add enemies and one boss
// When player enters the cell close all walls until the boss is defeated
// TODO: Set up the exit

public class Cell : MonoBehaviour {
	Vector2 pos;
	int cellWidth = 18;
	int cellHeight = 18;

	public GameObject playerPrefab;

	[Header("Cell parts")]
	public GameObject wallTop;
	public GameObject wallRight;
	public GameObject wallBottom;
	public GameObject wallLeft;
	public GameObject corridorRight;
	public GameObject corridorBottom;

	[Header("Items")]
	public CellItem[] items;
	private bool[,] posTaken = new bool[18, 18];

	[Header("Minimap")]
	public GameObject cellSprite;
	public GameObject chestSprite;

	private void Start() {
		// Generate sprite for minimap
		Instantiate(cellSprite, transform.position, Quaternion.identity, transform);

		// Loop trough all items
		foreach (CellItem item in items) {

			// Select how many copies, then for each copy check the chance (some just have chance=1)
			int n = Mathf.FloorToInt(Random.Range(item.minNum, item.maxNum));
			lastRandomPos = RandomPosition(4);

			if (n > 100) n = 100;

			// Make n copies
			for (int i = 0; i < n; i++) {
				if (Random.value < item.chance) {
					// Every now and then we will start with new random position, just for kicks
					if(Random.value < 0.1f) lastRandomPos = RandomPosition(4);

					// Get random position (relative to this object, not global)
					Vector3 pos = PseudoRandomPosition(5, item.groupingDistance) + item.offset;
					int x = (int)pos.x + cellWidth / 2;
					int y = (int)pos.y + cellHeight / 2;

					// If there is nothing on this position, instantiate it and mark position as taken
					if (!posTaken[x, y]) {
						posTaken[x, y] = true;
						Instantiate(item.item, transform.position + pos, Quaternion.identity, transform);
					}
				}
			}
		}
	}

	// Set all the values as they should be, basically a constructor
	public void Setup(Vector2 _pos, bool[] c, bool[] w, bool playerSpawn, bool exit) {
		pos = _pos;

		corridorRight.SetActive(c[0]);
		corridorBottom.SetActive(c[1]);

		wallTop.SetActive(w[0]);
		wallRight.SetActive(w[1]);
		wallBottom.SetActive(w[2]);
		wallLeft.SetActive(w[3]);

		if(playerSpawn)
			Instantiate(playerPrefab, transform.position + new Vector3(0, 0, -1), Quaternion.identity);

		if(exit)
			GetComponent<Tilemap>().color = Color.red;
	}


	Vector3 lastRandomPos;  // Get random position, but bias it towards last random position
	private Vector3 PseudoRandomPosition(int padding, float groupingDistance) {
		Vector3 randomPos;

		int i = 0;
		do {  // Search for random position, and get one that is not that far away
			int rx = Mathf.FloorToInt(Random.value * (cellWidth - padding)) - (cellWidth - padding) / 2;
			int ry = Mathf.FloorToInt(Random.value * (cellHeight - padding)) - (cellWidth - padding) / 2;
			randomPos = new Vector3(rx, ry, -0.1f);
			i++;
		} while (Vector3.Distance(lastRandomPos, randomPos) > Random.value * groupingDistance && i < 100);

		lastRandomPos = randomPos;
		return randomPos;
	}

	// Get completely position
	private Vector3 RandomPosition(int padding) {
		int rx = Mathf.FloorToInt(Random.value * (cellWidth - padding)) - (cellWidth - padding) / 2;
		int ry = Mathf.FloorToInt(Random.value * (cellHeight - padding)) - (cellWidth - padding) / 2;
		return new Vector3(rx, ry, -0.1f);
	}
}

