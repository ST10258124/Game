using System;
using Godot;

public partial class Globals : Node
{
	const float BASE_ACCELARATION = 12.0f; //metres/s
	const float BASE_SPACING = 5.5f; //metres

	public int score;
	public float spikeSpacing = BASE_SPACING;
	public float accelaration = BASE_ACCELARATION;
	//also refers to the spike, just added "spike" to "Spacing" because "spacing" is a parameter in spawnSpikes()

	public Boolean playing, Game_Over;
	private Boolean allowSpike;
	private String[] playerActions = {"left", "left",
							"right", "right",
							"split", "split",
							"centre", "centre"};
	private String[,] spikesList = {{"Right_Spike.tscn", "Right_Spike.tscn",
						"Left_Spike.tscn", "Left_Spike.tscn",
						"Center_Spike_Big.tscn", "Center_Spike_Small.tscn",
						"Side_Spikes.tscn", "Side_Spikes.tscn"},

									{"4", "4", "4", "4", "8", "6", "2", "2"}}; //length of bounding box on z axis
	private String[,] spikesSpawn = new string[2, 10];

	//thomas had never seen such bullshit >:(
	PackedScene spikeOne;
	PackedScene spikeTwo;
	PackedScene spikeThree;
	PackedScene spikeFour;
	PackedScene spikeFive;
	PackedScene spikeSix;
	PackedScene spikeSeven;
	PackedScene spikeEight;
	PackedScene spikeNine;
	PackedScene spikeTen;

	PackedScene Gate = GD.Load<PackedScene>("res://Gate.tscn");
	PackedScene leftExplode = GD.Load<PackedScene>("res://Explode_Left.tscn");

	RigidBody3D One;
	RigidBody3D Two;
	RigidBody3D Three;
	RigidBody3D Four;
	RigidBody3D Five;
	RigidBody3D Six;
	RigidBody3D Seven;
	RigidBody3D Eight;
	RigidBody3D Nine;
	RigidBody3D Ten;
	//^^those are related. spikeOne is for one and so on. . .^^
	Node3D gate;

	public override void _Ready()
	{
		Gate = GD.Load<PackedScene>("res://Gate.tscn");
		playing = false;
		Game_Over = false;
		SetPhysicsProcess(playing);
	}

	public override void _PhysicsProcess(double delta)
	{
		One.LinearVelocity = new Vector3(0, 0, accelaration);
		Two.LinearVelocity = new Vector3(0, 0, accelaration);
		Three.LinearVelocity = new Vector3(0, 0, accelaration);
		Four.LinearVelocity = new Vector3(0, 0, accelaration);
		Five.LinearVelocity = new Vector3(0, 0, accelaration);
		Six.LinearVelocity = new Vector3(0, 0, accelaration);
		Seven.LinearVelocity = new Vector3(0, 0, accelaration);
		Eight.LinearVelocity = new Vector3(0, 0, accelaration);
		Nine.LinearVelocity = new Vector3(0, 0, accelaration);
		Ten.LinearVelocity = new Vector3(0, 0, accelaration);
	}

	public void nextLevel()
	{
		clearLevel();
		//spacing and accelaration feel fine, maybe buff player sped every 100
		spikeSpacing += BASE_SPACING * 0.035f;
		accelaration += BASE_ACCELARATION * 0.1f;

		spawnSpikes(spikeSpacing);
		/*losing my shit at this point
		shouldn't need a method to call a method i don't think
		rethink structure when get everything working properly*/

		/*changed nextLevel() to now increase spacing and accelaration
		  changed spawnSpikes() to expect an argument for spacing
		  _PhysicsProcess in Start class will now get accelaration from this script
		  
		  maybe nextLevel() is useful now? idek
		*/
	}

	public void spawnSpikes(float spacing)
	{

		String prev = ""; //holds prev action
		Random rng = new Random();
		int index; //index of current action
		allowSpike = true;

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
		spikeOne = GD.Load<PackedScene>("res://" + spikesSpawn[0, 0]);
		spikeTwo = GD.Load<PackedScene>("res://" + spikesSpawn[0, 1]);
		spikeThree = GD.Load<PackedScene>("res://" + spikesSpawn[0, 2]);
		spikeFour = GD.Load<PackedScene>("res://" + spikesSpawn[0, 3]);
		spikeFive = GD.Load<PackedScene>("res://" + spikesSpawn[0, 4]);
		spikeSix = GD.Load<PackedScene>("res://" + spikesSpawn[0, 5]);
		spikeSeven = GD.Load<PackedScene>("res://" + spikesSpawn[0, 6]);
		spikeEight = GD.Load<PackedScene>("res://" + spikesSpawn[0, 7]);
		spikeNine = GD.Load<PackedScene>("res://" + spikesSpawn[0, 8]);
		spikeTen = GD.Load<PackedScene>("res://" + spikesSpawn[0, 9]);

		One = (RigidBody3D)spikeOne.Instantiate();
		Two = (RigidBody3D)spikeTwo.Instantiate();
		Three = (RigidBody3D)spikeThree.Instantiate();
		Four = (RigidBody3D)spikeFour.Instantiate();
		Five = (RigidBody3D)spikeFive.Instantiate();
		Six = (RigidBody3D)spikeSix.Instantiate();
		Seven = (RigidBody3D)spikeSeven.Instantiate();
		Eight = (RigidBody3D)spikeEight.Instantiate();
		Nine = (RigidBody3D)spikeNine.Instantiate();
		Ten = (RigidBody3D)spikeTen.Instantiate();
		
		gate = (Node3D)Gate.Instantiate();
		
		One.Position = new Vector3(0, 0.5f, -20);
		Two.Position = new Vector3(0, 0.5f, One.Position.Z - int.Parse(spikesSpawn[1, 0]) - spacing);
		Three.Position = new Vector3(0, 0.5f, Two.Position.Z - int.Parse(spikesSpawn[1, 1]) - spacing);
		Four.Position = new Vector3(0, 0.5f, Three.Position.Z - int.Parse(spikesSpawn[1, 2]) - spacing);
		Five.Position = new Vector3(0, 0.5f, Four.Position.Z - int.Parse(spikesSpawn[1, 3]) - spacing);
		Six.Position = new Vector3(0, 0.5f, Five.Position.Z - int.Parse(spikesSpawn[1, 4]) - spacing);
		Seven.Position = new Vector3(0, 0.5f, Six.Position.Z - int.Parse(spikesSpawn[1, 5]) - spacing);
		Eight.Position = new Vector3(0, 0.5f, Seven.Position.Z - int.Parse(spikesSpawn[1, 6]) - spacing);
		Nine.Position = new Vector3(0, 0.5f, Eight.Position.Z - int.Parse(spikesSpawn[1, 7]) - spacing);
		Ten.Position = new Vector3(0, 0.5f, Nine.Position.Z - int.Parse(spikesSpawn[1, 8]) - spacing);

		AddChild(One);
		AddChild(Two);
		AddChild(Three);
		AddChild(Four);
		AddChild(Five);
		AddChild(Six);
		AddChild(Seven);
		AddChild(Eight);
		AddChild(Nine);
		AddChild(Ten);

		Ten.AddChild(gate);
		gate.Position = new Vector3(0, 0, - int.Parse(spikesSpawn[1, 9]) - 8);
	}

	public void clearLevel()
	{
		One.QueueFree();
		Two.QueueFree();
		Three.QueueFree();
		Four.QueueFree();
		Five.QueueFree();
		Six.QueueFree();
		Seven.QueueFree();
		Eight.QueueFree();
		Nine.QueueFree();
		Ten.QueueFree();
	}

	public void gameOver()
	{
		Game_Over = true;
		SetPhysicsProcess(false);
		One.LinearVelocity = new Vector3(0, 0, 0);
		Two.LinearVelocity = new Vector3(0, 0, 0);
		Three.LinearVelocity = new Vector3(0, 0, 0);
		Four.LinearVelocity = new Vector3(0, 0, 0);
		Five.LinearVelocity = new Vector3(0, 0, 0);
		Six.LinearVelocity = new Vector3(0, 0, 0);
		Seven.LinearVelocity = new Vector3(0, 0, 0);
		Eight.LinearVelocity = new Vector3(0, 0, 0);
		Nine.LinearVelocity = new Vector3(0, 0, 0);
		Ten.LinearVelocity = new Vector3(0, 0, 0);
	}

	public void newGame()
	{

		accelaration = BASE_ACCELARATION;
		spikeSpacing = BASE_SPACING;

		spawnSpikes(spikeSpacing);
	}

}
