using System;
using Godot;

public partial class playerLeft : CharacterBody3D
{
	[Export]
	public float Speed = 6.0f;
	Globals Controller;
	Timer Reset;
	MeshInstance3D playerLeftMesh;
	MeshInstance3D playerGlitch;
	PackedScene leftExplode = GD.Load<PackedScene>("res://Explode_Left.tscn");
	Node3D explode;
	AudioStreamPlayer3D sfxDeath;
	GpuParticles3D trail;

	String[] explosionSounds = { "Explode1", "Explode2" };
	public override void _Ready()
	{
		Controller = (Globals)GetNode("/root/Globals");
		Reset = GetNode<Timer>("ResetTimer");
		explode = (Node3D)leftExplode.Instantiate();
		playerLeftMesh = GetNode<MeshInstance3D>("MeshInstance3D");
		playerGlitch = GetNode<MeshInstance3D>("GlitchBG");
		sfxDeath = GetNode<AudioStreamPlayer3D>("Death");
		trail = GetNode<GpuParticles3D>("Trail");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Controller.Game_Over)
		{
			Reset.Start();
			SetPhysicsProcess(false);

			if (!explode.IsInsideTree())
			{
				playerGlitch.Visible = true;
				playerLeftMesh.Visible = false;
				trail.Emitting = false;

				sfxDeath.Stream = (AudioStream)ResourceLoader.Load("res://SFX/Glitchy.wav");
				sfxDeath.Play();
			}
		}

		var currentPosition = GlobalTransform.Origin;

		if (!Input.IsActionPressed("MoveLeft") && Input.IsActionPressed("MoveRight"))
		{ //MOVE RIGHT
			GlobalPosition = currentPosition.Lerp(new Vector3(4.5f, 0.5f, 6.5f), (Speed + 0.75f) * (float)delta);
		}

		if (Input.IsActionPressed("MoveLeft") && !Input.IsActionPressed("MoveRight"))
		{ //MOVE LEFT
			GlobalPosition = currentPosition.Lerp(new Vector3(-5.25f, 0.5f, 6.5f), Speed * (float)delta);
		}

		if (Input.IsActionPressed("MoveLeft") && Input.IsActionPressed("MoveRight"))
		{ //SPLIT
			GlobalPosition = currentPosition.Lerp(new Vector3(-5.25f, 0.5f, 6.5f), Speed * (float)delta);
		}

		if (!Input.IsActionPressed("MoveLeft") && !Input.IsActionPressed("MoveRight")) //RE-CENTER
		{
			GlobalPosition = currentPosition.Lerp(new Vector3(-0.3f, 0.5f, 6.5f), (Speed + 1.75f) * (float)delta);
		}

		MoveAndSlide();

		/*
		EXACT PLAYER CO-ORDS
		(-4.625f, 0.5f, 6.5f) - LEFT
		(3.875f, 0.5f, 6.5f) - RIGHT
		(-0.375f, 0.5f, 6.5f) - CENTER
		*/
	}

	void _on_area_3d_area_entered(Area3D area)
	{
		if (area.IsInGroup("Spike"))
		{
			Random rng = new Random();

			Controller.gameOver();
			playerLeftMesh.Visible = false;
			AddChild(explode);

			sfxDeath.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + explosionSounds[rng.Next(2)] + ".wav");
			sfxDeath.Play();

			playerGlitch.Visible = false;
			trail.Emitting = false;

			Reset.Start();
		}
	}

	public void _on_reset_timer_timeout()
	{
		QueueFree();
	}
}