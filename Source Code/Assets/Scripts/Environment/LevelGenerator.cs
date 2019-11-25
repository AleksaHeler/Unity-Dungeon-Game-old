using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: check to see if bottom right cell is really the farthest from center

public class LevelGenerator : MonoBehaviour
{
	// A grid where true is cell
	bool[,] grid;
	public bool autoGenerate = true;
	
	[Header("Parameters")]
	public int maxWidth;
	public int maxHeight;
	public float chanceToChangeDirection = 0.5f;
	public int iterationSteps = 100;
	public GameObject cellPrefab;
	public int cellWidth = 18;
	public int cellHeight = 18;
	public int corridorLength = 8;
	
	struct RandomWalker {
		public Vector2 dir;
		public Vector2 pos;
	}
	RandomWalker walker;

	private void Awake() {
		if(autoGenerate)
			GenerateLevel();
	}

	void GenerateLevel() {
		// Create empty map
		grid = new bool[maxWidth, maxHeight];

		// Generate a walker with pos at the middle and random dir
		walker = new RandomWalker();
		walker.dir = RandomDirection();
		Vector2 pos = new Vector2(Mathf.RoundToInt(maxWidth / 2.0f), Mathf.RoundToInt(maxHeight / 2.0f));
		walker.pos = pos;

		// Iterate
		for (int i = 0; i < iterationSteps; i++) { 
			// Mark current position as a cell
			grid[(int)walker.pos.x, (int)walker.pos.y] = true;

			// Move and select new direction
			walker.pos += walker.dir;
			if (Random.value < chanceToChangeDirection) walker.dir = RandomDirection();
			walker.dir = RandomDirection();

			// Avoid borders
			walker.pos.x = Mathf.Clamp(walker.pos.x, 1, maxWidth - 2);
			walker.pos.y = Mathf.Clamp(walker.pos.y, 1, maxHeight - 2);
		}

		// Find: leftmost, topmost, bottommost
		int leftmostIndex = maxWidth;
		int bottommostIndex = maxHeight;
		int topmostIndex = 0;
		for (int x = 0; x < maxWidth; x++) {
			for (int y = 0; y < maxHeight; y++) {
				if (grid[x, y]) {
					if (x < leftmostIndex) leftmostIndex = x;
					if (y < bottommostIndex) bottommostIndex = y;
					if (y > topmostIndex) topmostIndex = y;
				}
			}
		}

		// Find: top left, bottom right
		Vector2 topLeftCell = new Vector2(maxWidth, topmostIndex);
		Vector2 bottomRight = new Vector2(0, bottommostIndex);
		for (int x = 0; x < maxWidth; x++) {
			for (int y = 0; y < maxHeight; y++) {
				if (grid[x, y] && y == topmostIndex) {
					if (x < topLeftCell.x) topLeftCell.x = x;
				}
				if (grid[x, y] && y == bottommostIndex) {
					if (x > bottomRight.x) bottomRight.x = x;
				}
			}
		}

		// Instantiate prefabs
		for (int x = 0; x < maxWidth; x++) {
			for (int y = 0; y < maxHeight; y++) {
				if (grid[x, y]) {
					// Find the position and instantiate
					int posX = (x - maxWidth / 2) * (cellWidth + corridorLength);
					int posY = (y - maxHeight / 2) * (cellHeight + corridorLength);
					Vector3 p = new Vector3(posX, posY);
					GameObject thisCell = Instantiate(cellPrefab, p, Quaternion.identity, transform);

					// Find cells position
					int cellX = x - maxWidth / 2 + Mathf.Abs(leftmostIndex - maxWidth);
					int cellY = y - maxHeight / 2 + Mathf.Abs(bottommostIndex - maxHeight);
					Vector2 cellPos = new Vector2(cellX, cellY);

					// If it has a cell to its right or bottom enable these corridors
					bool[] c = new bool[2];
					c[0] = grid[x + 1, y];
					c[1] = grid[x, y - 1];

					// Check neighbouring cells to remove walls
					bool[] w = new bool[4];
					w[0] = !grid[x, y + 1];
					w[1] = !grid[x + 1, y];
					w[2] = !grid[x, y - 1];
					w[3] = !grid[x - 1, y];

					// Set player spawn and exit as topleft and bottomright
					bool playerSpawn = x == topLeftCell.x && y == topLeftCell.y;
					bool exit = x == bottomRight.x && y == bottomRight.y;

					// This acts like a constructor
					thisCell.GetComponent<Cell>().Setup(cellPos, c, w, playerSpawn, exit);
				}
			}
		}
	}

	Vector2 RandomDirection() {
		int choice = Mathf.FloorToInt(Random.value * 3.99f);
		switch (choice) {
			case 0: return Vector2.down;
			case 1: return Vector2.left;
			case 2: return Vector2.up;
			default: return Vector2.right;
		}
	}
}
