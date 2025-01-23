using Godot;

public partial class Detect_Avoid : RayCast3D
{
	[Signal]
    public delegate void ScoreChangedEventHandler();

	bool avoided;
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
			EmitSignal(SignalName.ScoreChanged);

			//+1 to score when ray cast collided with player in prev frame and is not colliding in current frame
			// if (Controller.score % 10 == 0) Controller.nextLevel();
		}

		avoided = IsColliding();
	}
}