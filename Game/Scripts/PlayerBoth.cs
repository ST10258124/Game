using Godot;

public partial class PlayerBoth : Node3D
{
    MeshInstance3D BeamOuter, BeamInner;
    Vector3 LeftPos, RightPos;
    CharacterBody3D PlayerLeft, PlayerRight;
    Globals Controller;

    public override void _Ready()
    {
        BeamOuter = GetNode<MeshInstance3D>("Beam/Outer");
        BeamInner = GetNode<MeshInstance3D>("Beam/Inner");

        PlayerLeft = GetNode<CharacterBody3D>("PlayerChar");
        PlayerRight = GetNode<CharacterBody3D>("PlayerChar2");

        Controller = (Globals)GetNode("/root/Globals");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Controller.Game_Over)
        {
            BeamOuter.Visible = false;
            BeamInner.Visible = false;
        }

        //======================================"BEAM" SCALING AND POSITIONING BELOW=============================================
        LeftPos = PlayerLeft.GlobalPosition;
		RightPos = PlayerRight.GlobalPosition;	

        /*X = calculated midpoint of cubes (player)
		  Y = 0.5 because that never changes
		  Z = Z position of any cube because of that slight offset at start of game*/
		BeamOuter.GlobalPosition = new Vector3((RightPos.X + LeftPos.X) / 2, 0.5f, LeftPos.Z);
		BeamInner.GlobalPosition = new Vector3((RightPos.X + LeftPos.X) / 2, 0.5f, LeftPos.Z);

		BeamOuter.Scale = new Vector3((RightPos.X - LeftPos.X) / 0.6f,
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f),
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f)); //length formula thingy here

		BeamInner.Scale = new Vector3((RightPos.X - LeftPos.X) / 0.3f,
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f),
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f)); //length formula thingy here
    }
}