﻿using UnityEngine;
using UnityEngine.UI;
using Unity.AI.Navigation;
using System.Collections;
using static MazeGlobals;
using System.Threading.Tasks;

//<summary>
//Game object, that creates maze and instantiates it in scene
//</summary>
public class MazeSpawner : MonoBehaviour {
	public enum MazeGenerationAlgorithm{
		PureRecursive,
		RecursiveTree,
		RandomTree,
		OldestTree,
		RecursiveDivision,
	}

	public MazeGenerationAlgorithm Algorithm = MazeGenerationAlgorithm.PureRecursive;
	public bool FullRandom = false;
	public int RandomSeed = 12345;
	public GameObject Floor = null;
	public GameObject Wall = null;
	public GameObject Pillar = null;
	public NavMeshSurface NavMeshSurface;
	public int Rows = 5;
	public int Columns = 5;
	public float CellWidth = 4;
	public float CellHeight = 4;
	public bool AddGaps = true;
	public Player spawnedPlayer;
	public GameObject GoalPrefab = null;
	public GameObject[] SpikePrefabs = null;
	public GameObject PatrolEnemyPrefab = null;
	public GameObject[] Powerups = null;

	private BasicMazeGenerator mMazeGenerator = null;
	// private Player spawnedPlayer;

	private int goalNum;
	public int MinNumGoals = 40;
	public int TotalNumSpikes = 20;
	public int TotalNumPatrols = 20;
	public int TotalNumPowerups = 20;

	private async Task<BasicMazeGenerator> CreateMazeRepresentation()
	{
		Debug.Log("Beginning maze generation");
		BasicMazeGenerator maze = await Task.Run<BasicMazeGenerator>(() => 
		{
			switch (Algorithm)
			{
			case MazeGenerationAlgorithm.PureRecursive:
				mMazeGenerator = new RecursiveMazeGenerator (Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveTree:
				mMazeGenerator = new RecursiveTreeMazeGenerator (Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RandomTree:
				mMazeGenerator = new RandomTreeMazeGenerator (Rows, Columns);
				break;
			case MazeGenerationAlgorithm.OldestTree:
				mMazeGenerator = new OldestTreeMazeGenerator (Rows, Columns);
				break;
			case MazeGenerationAlgorithm.RecursiveDivision:
				mMazeGenerator = new DivisionMazeGenerator (Rows, Columns);
				break;
			}

			mMazeGenerator.GenerateMaze();

			return mMazeGenerator;
		});
		Debug.Log("Maze generation ended");

		return maze;
	}

	private async Task CreatePhysicalUnityMaze(BasicMazeGenerator mMazeGenerator)
	{
		// Physically create maze now that we have code representation
		for (int row = 0; row < Rows; row++) {
			for(int column = 0; column < Columns; column++){
				float x = column*(CellWidth+(AddGaps?.2f:0));
				float z = row*(CellHeight+(AddGaps?.2f:0));
				MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
				GameObject tmp;
				tmp = Instantiate(Floor,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
				tmp.transform.parent = transform;
				if (row == 0 && column == 0)
				{
					spawnedPlayer.gameObject.SetActive(true);
					spawnedPlayer.gameObject.transform.position = new Vector3(0, 0.1f, 0);
				}
				if(cell.WallRight){
					tmp = Instantiate(Wall,new Vector3(x+CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
					tmp.transform.localScale += new Vector3((CellHeight - Floor.transform.localScale.x) / Floor.transform.localScale.x, 0, 0);
					tmp.transform.parent = transform;
				}
				if(cell.WallFront){
					tmp = Instantiate(Wall,new Vector3(x,0,z+CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
					tmp.transform.localScale += new Vector3((CellWidth - Floor.transform.localScale.z) / Floor.transform.localScale.z, 0, 0);
					tmp.transform.parent = transform;
				}
				if(cell.WallLeft){
					tmp = Instantiate(Wall,new Vector3(x-CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
					tmp.transform.localScale += new Vector3((CellHeight - Floor.transform.localScale.x) / Floor.transform.localScale.x, 0, 0);
					tmp.transform.parent = transform;
				}
				if(cell.WallBack){
					tmp = Instantiate(Wall,new Vector3(x,0,z-CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
					tmp.transform.localScale += new Vector3((CellWidth - Floor.transform.localScale.z) / Floor.transform.localScale.z, 0, 0);
					tmp.transform.parent = transform;
				}
				if(cell.IsGoal && GoalPrefab != null && row != 0 && column != 0){
					tmp = Instantiate(GoalPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
					tmp.transform.parent = transform;
					goalNum++;
				}
			}
		}
		if(Pillar != null){
			for (int row = 0; row < Rows+1; row++) {
				for (int column = 0; column < Columns+1; column++) {
					float x = column*(CellWidth+(AddGaps?.2f:0));
					float z = row*(CellHeight+(AddGaps?.2f:0));
					GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
					tmp.transform.parent = transform;
				}
			}
		}

		await Task.Delay(100);

		// Check number of coins available - if not enough then randomly add them throughout
		// If they should be at dead end, check if MazeCell has 3 walls around
		// If not that many dead ends, then spawn randomly
		if (goalNum < MinNumGoals)
		{
			for (int i = 0; i < MinNumGoals - goalNum; i++)
			{
				int randomRow = Random.Range(0, Rows);
				int randomColumn = Random.Range(0, Columns);
				if (mMazeGenerator.GetMazeCell(randomRow, randomColumn).IsGoal == false)
				{
					float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
					float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));
					GameObject tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
					tmp.transform.parent = transform;
					mMazeGenerator.GetMazeCell(randomRow, randomColumn).IsGoal = true;
				}
			}
		}

		await Task.Delay(100);

		// Bake NavMeshes on floors for enemy AI
		NavMeshSurface.BuildNavMesh();

		// Spawn enemies random
		for (int i = 0; i < TotalNumPatrols; i++)
		{
			int randomRow = Random.Range(0, Rows);
			int randomColumn = Random.Range(0, Columns);
			MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
			if (mazeCell.HasSpike == false && mazeCell.IsGoal == false && mazeCell.HasEnemy == false)
			{
				float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
				float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

				GameObject tmp = Instantiate(PatrolEnemyPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
				tmp.transform.parent = transform;
				tmp.GetComponent<PatrolEnemy>().player = spawnedPlayer;
				mazeCell.HasEnemy = true;
			}
		}

		await Task.Delay(100);

		// Add spikes randomly
		for (int i = 0; i < TotalNumSpikes; i++)
		{
			int randomRow = Random.Range(0, Rows);
			int randomColumn = Random.Range(0, Columns);
			MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
			if (mazeCell.HasSpike == false && mazeCell.IsGoal == false)
			{
				float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
				float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

				if (mazeCell.WallFront && mazeCell.WallBack)
				{
					GameObject tmp = Instantiate(SpikePrefabs[Random.Range(0, SpikePrefabs.Length)], new Vector3(x, 0.6f, z), Quaternion.Euler(0, 0, 0)) as GameObject;
					tmp.transform.parent = transform;
					mazeCell.HasSpike = true;
				} else if (mazeCell.WallLeft && mazeCell.WallRight)
				{
					GameObject tmp = Instantiate(SpikePrefabs[Random.Range(0, SpikePrefabs.Length)], new Vector3(x, 0.6f, z), Quaternion.Euler(0, 90, 0)) as GameObject;
					tmp.transform.parent = transform;
					mazeCell.HasSpike = true;
				}
			}
		}

		// Spawn random powerups
		for (int i = 0; i < TotalNumPowerups; i++)
		{
			int randomRow = Random.Range(0, Rows);
			int randomColumn = Random.Range(0, Columns);
			MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
			if (mazeCell.HasSpike == false && mazeCell.IsGoal == false && mazeCell.HasEnemy == false && mazeCell.HasPowerup == false)
			{
				float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
				float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

				GameObject tmp = Instantiate(Powerups[Random.Range(0, Powerups.Length)], new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
				tmp.transform.SetParent(transform, false);
				mazeCell.HasPowerup = true;
			}
		}
	}

	public async Task CreateMaze(MazeRunnerDifficulty difficulty)
	{
		// Manage settings depending on difficulty selected
		switch (difficulty)
		{
			case MazeRunnerDifficulty.Easy:
				Rows = 20;
				Columns = 20;
				MinNumGoals = 20;
				TotalNumSpikes = 20;
				TotalNumPatrols = 10;
				TotalNumPowerups = 15;
				break;
			case MazeRunnerDifficulty.Medium:
				Rows = 25;
				Columns = 25;
				MinNumGoals = 30;
				TotalNumSpikes = 50;
				TotalNumPatrols = 20;
				TotalNumPowerups = 15;
				break;
			case MazeRunnerDifficulty.Hard:
				Rows = 25;
				Columns = 25;
				MinNumGoals = 40;
				TotalNumSpikes = 80;
				TotalNumPatrols = 30;
				TotalNumPowerups = 15;
				break;
		}

		BasicMazeGenerator maze = await CreateMazeRepresentation();

		await CreatePhysicalUnityMaze(maze);

		// // Set up maze generator (represented by code purely)
		// if (!FullRandom) {
		// 	// Random.seed = RandomSeed;
		// 	Random.InitState(RandomSeed);
		// }
		// switch (Algorithm) {
		// case MazeGenerationAlgorithm.PureRecursive:
		// 	mMazeGenerator = new RecursiveMazeGenerator (Rows, Columns);
		// 	break;
		// case MazeGenerationAlgorithm.RecursiveTree:
		// 	mMazeGenerator = new RecursiveTreeMazeGenerator (Rows, Columns);
		// 	break;
		// case MazeGenerationAlgorithm.RandomTree:
		// 	mMazeGenerator = new RandomTreeMazeGenerator (Rows, Columns);
		// 	break;
		// case MazeGenerationAlgorithm.OldestTree:
		// 	mMazeGenerator = new OldestTreeMazeGenerator (Rows, Columns);
		// 	break;
		// case MazeGenerationAlgorithm.RecursiveDivision:
		// 	mMazeGenerator = new DivisionMazeGenerator (Rows, Columns);
		// 	break;
		// }

		// mMazeGenerator.GenerateMaze();

		// // Physically create maze now that we have code representation
		// for (int row = 0; row < Rows; row++) {
		// 	for(int column = 0; column < Columns; column++){
		// 		float x = column*(CellWidth+(AddGaps?.2f:0));
		// 		float z = row*(CellHeight+(AddGaps?.2f:0));
		// 		MazeCell cell = mMazeGenerator.GetMazeCell(row,column);
		// 		GameObject tmp;
		// 		tmp = Instantiate(Floor,new Vector3(x,0,z), Quaternion.Euler(0,0,0)) as GameObject;
		// 		tmp.transform.parent = transform;
		// 		if(cell.WallRight){
		// 			tmp = Instantiate(Wall,new Vector3(x+CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,90,0)) as GameObject;// right
		// 			tmp.transform.localScale += new Vector3((CellHeight - Floor.transform.localScale.x) / Floor.transform.localScale.x, 0, 0);
		// 			tmp.transform.parent = transform;
		// 		}
		// 		if(cell.WallFront){
		// 			tmp = Instantiate(Wall,new Vector3(x,0,z+CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,0,0)) as GameObject;// front
        //             tmp.transform.localScale += new Vector3((CellWidth - Floor.transform.localScale.z) / Floor.transform.localScale.z, 0, 0);
        //             tmp.transform.parent = transform;
		// 		}
		// 		if(cell.WallLeft){
		// 			tmp = Instantiate(Wall,new Vector3(x-CellWidth/2,0,z)+Wall.transform.position,Quaternion.Euler(0,270,0)) as GameObject;// left
        //             tmp.transform.localScale += new Vector3((CellHeight - Floor.transform.localScale.x) / Floor.transform.localScale.x, 0, 0);
        //             tmp.transform.parent = transform;
		// 		}
		// 		if(cell.WallBack){
		// 			tmp = Instantiate(Wall,new Vector3(x,0,z-CellHeight/2)+Wall.transform.position,Quaternion.Euler(0,180,0)) as GameObject;// back
        //             tmp.transform.localScale += new Vector3((CellWidth - Floor.transform.localScale.z) / Floor.transform.localScale.z, 0, 0);
        //             tmp.transform.parent = transform;
		// 		}
		// 		if(cell.IsGoal && GoalPrefab != null){
		// 			tmp = Instantiate(GoalPrefab,new Vector3(x,1,z), Quaternion.Euler(0,0,0)) as GameObject;
		// 			tmp.transform.parent = transform;
		// 			goalNum++;
		// 		}
		// 	}
		// }
		// if(Pillar != null){
		// 	for (int row = 0; row < Rows+1; row++) {
		// 		for (int column = 0; column < Columns+1; column++) {
		// 			float x = column*(CellWidth+(AddGaps?.2f:0));
		// 			float z = row*(CellHeight+(AddGaps?.2f:0));
		// 			GameObject tmp = Instantiate(Pillar,new Vector3(x-CellWidth/2,0,z-CellHeight/2),Quaternion.identity) as GameObject;
		// 			tmp.transform.parent = transform;
		// 		}
		// 	}
		// }

		// // Custom code added below

		// // Check number of coins available - if not enough then randomly add them throughout
		// // If they should be at dead end, check if MazeCell has 3 walls around
		// // If not that many dead ends, then spawn randomly
		// if (goalNum < MinNumGoals)
        // {
		// 	for (int i = 0; i < MinNumGoals - goalNum; i++)
        //     {
		// 		int randomRow = Random.Range(0, Rows);
		// 		int randomColumn = Random.Range(0, Columns);
		// 		if (mMazeGenerator.GetMazeCell(randomRow, randomColumn).IsGoal == false)
        //         {
		// 			float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
		// 			float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));
		// 			GameObject tmp = Instantiate(GoalPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
		// 			tmp.transform.parent = transform;
		// 			mMazeGenerator.GetMazeCell(randomRow, randomColumn).IsGoal = true;
		// 		}
        //     }
		// }

		// // Bake NavMeshes on floors for enemy AI
		// NavMeshSurface.BuildNavMesh();

		// // Add spikes randomly
		// for (int i = 0; i < TotalNumSpikes; i++)
		// {
		// 	int randomRow = Random.Range(0, Rows);
		// 	int randomColumn = Random.Range(0, Columns);
		// 	MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
		// 	if (mazeCell.HasSpike == false && mazeCell.IsGoal == false)
		// 	{
		// 		float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
		// 		float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

		// 		if (mazeCell.WallFront && mazeCell.WallBack)
		// 		{
		// 			GameObject tmp = Instantiate(SpikePrefabs[Random.Range(0, SpikePrefabs.Length)], new Vector3(x, 0.6f, z), Quaternion.Euler(0, 0, 0)) as GameObject;
		// 			tmp.transform.parent = transform;
		// 			mazeCell.HasSpike = true;
		// 		} else if (mazeCell.WallLeft && mazeCell.WallRight)
		// 		{
		// 			GameObject tmp = Instantiate(SpikePrefabs[Random.Range(0, SpikePrefabs.Length)], new Vector3(x, 0.6f, z), Quaternion.Euler(0, 90, 0)) as GameObject;
		// 			tmp.transform.parent = transform;
		// 			mazeCell.HasSpike = true;
		// 		}
		// 	}
		// }

		// // Spawn enemies random
		// for (int i = 0; i < TotalNumPatrols; i++)
		// {
		// 	int randomRow = Random.Range(0, Rows);
		// 	int randomColumn = Random.Range(0, Columns);
		// 	MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
		// 	if (mazeCell.HasSpike == false && mazeCell.IsGoal == false && mazeCell.HasEnemy == false)
		// 	{
		// 		float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
		// 		float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

		// 		GameObject tmp = Instantiate(PatrolEnemyPrefab, new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
		// 		tmp.transform.parent = transform;
		// 		mazeCell.HasEnemy = true;
		// 	}
		// }

		// // Spawn random powerups
		// for (int i = 0; i < TotalNumPowerups; i++)
		// {
		// 	int randomRow = Random.Range(0, Rows);
		// 	int randomColumn = Random.Range(0, Columns);
		// 	MazeCell mazeCell = mMazeGenerator.GetMazeCell(randomRow, randomColumn);
		// 	if (mazeCell.HasSpike == false && mazeCell.IsGoal == false && mazeCell.HasEnemy == false && mazeCell.HasPowerup == false)
		// 	{
		// 		float x = randomColumn * (CellWidth + (AddGaps ? .2f : 0));
		// 		float z = randomRow * (CellHeight + (AddGaps ? .2f : 0));

		// 		GameObject tmp = Instantiate(Powerups[Random.Range(0, Powerups.Length)], new Vector3(x, 1, z), Quaternion.Euler(0, 0, 0)) as GameObject;
		// 		tmp.transform.SetParent(transform, false);
		// 		mazeCell.HasPowerup = true;
		// 	}
		// }
	}
}
