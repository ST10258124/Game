using Godot;
using System;

public partial class Detect_Avoid : RayCast3D
{
	Boolean avoided;
	Globals Controller;
	public override void _Ready()
	{
		avoided = false;
		Controller = (Globals)GetNode("/root/Globals");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (avoided && !IsColliding()){
			Controller.score++;
			// if (Controller.score % 10 == 0) Controller.nextLevel();
		}

		avoided = IsColliding();
	}
}
