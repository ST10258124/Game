using Godot;
using System;

public partial class Detect_Avoid : RayCast3D
{
	bool avoided;
	Globals Controller;
	RigidBody3D spike;
	public override void _Ready()
	{
		avoided = false;
		Controller = (Globals)GetNode("/root/Globals");
		spike = GetParent() as RigidBody3D;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (avoided && !IsColliding()){
			Controller.score++;
			// if (Controller.score % 10 == 0) Controller.nextLevel();
		}

		if (spike.IsQueuedForDeletion()){
			QueueFree();
		}

		avoided = IsColliding();
	}
}