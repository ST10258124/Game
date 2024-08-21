using System;
using System.Collections.Generic;
using Godot;

public partial class Globals : Node
{
	Random rng = new Random();
	public List<RigidBody3D> spikes = new List<RigidBody3D>();

	const float BASE_ACCELARATION = 12.0f; //metres/s
	const float BASE_SPACING = 5.5f; //metres

	public int score;
	public float spikeSpacing = BASE_SPACING;
	public float accelaration = BASE_ACCELARATION;
	//also refers to the spike, just added "spike" to "Spacing" because "spacing" is a parameter in spawnSpikes()

	public bool playing, Game_Over;
	private bool allowSpike;
	private static string[] playerActions = {"left", "left",
							"right", "right",
							"split", "split",
							"centre", "centre"};
	private static string[,] spikesList = {{"Right_Spike.tscn", "Right_Spike.tscn",
						"Left_Spike.tscn", "Left_Spike.tscn",
						"Center_Spike_Big.tscn", "Center_Spike_Small.tscn",
						"Side_Spikes.tscn", "Side_Spikes.tscn"},

									{"4", "4", "4", "4", "8", "6", "2", "2"}}; //length of bounding box on z axis
	private string[,] spikesSpawn = new string[2, 10];

	PackedScene Gate = GD.Load<PackedScene>("res://Gate.tscn");
	Node3D gate;

	public override void _Ready()
	{
		playing = false;
		Game_Over = false;
		SetPhysicsProcess(playing);
	}

	public override void _PhysicsProcess(double delta)
	{
		foreach (RigidBody3D Spike in spikes)
		{
			Spike.LinearVelocity = new Vector3(0, 0, accelaration);
		}
	}

	public void nextLevel()
	{
		clearLevel();
		//spacing and accelaration feel fine, maybe buff player sped every 100
		spikeSpacing += BASE_SPACING * 0.035f;
		accelaration += BASE_ACCELARATION * 0.1f;

		spawnSpikes(spikeSpacing);
	}

	public void spawnSpikes(float spacing)
	{

		string prev = ""; //holds prev action
		
		int index; //index of current action
		allowSpike = true;

		//the thingy below is mainly to ensure that the player doesn't have to do the same, expected, action more than twice in a row
		for (int i = 0; i < 10; i++)
		{
			do
			{
				index = rng.Next(8);
				if (!allowSpike) allowSpike = !prev.Equals(playerActions[index]);
			} while (!allowSpike);

			allowSpike = !prev.Equals(playerActions[index]);
			spikesSpawn[0, i] = spikesList[0, index];
			spikesSpawn[1, i] = spikesList[1, index];
			prev = playerActions[index];
		}
        
		//Load the spike scenes to the packed scene by getting file name from spikeSpawn array

		for (int i = 0; i < 10; i++)
		{
			PackedScene tempSpike = GD.Load<PackedScene>("res://" + spikesSpawn[0, i]);
			spikes[i] = (RigidBody3D)tempSpike.Instantiate();
			AddChild(spikes[i]);
		}
		
		gate = (Node3D)Gate.Instantiate();
		
		spikes[0].Position = new Vector3(0, 0.5f, -20);
		for (int i = 1; i < 10; i++)
		{
			spikes[i].Position = new Vector3(0, 0.5f, spikes[i - 1].Position.Z - int.Parse(spikesSpawn[1, i - 1]) - spacing);
		}

		spikes[9].AddChild(gate); //add the gate as a child of the last spike in the level
		gate.Position = new Vector3(0, 0, - int.Parse(spikesSpawn[1, 9]) - 8);
	}

	public void clearLevel()
	{
		foreach (RigidBody3D Spike in spikes)
		{
			Spike.QueueFree();
		}
	}

	public void gameOver()
	{
		Game_Over = true;
		SetPhysicsProcess(false);

		foreach (RigidBody3D Spike in spikes)
		{
			Spike.LinearVelocity = new Vector3(0, 0, 0);
		}
	}

	public void newGame()
	{
		clearLevel();
		accelaration = BASE_ACCELARATION;
		spikeSpacing = BASE_SPACING;

		spawnSpikes(spikeSpacing);
	}
}