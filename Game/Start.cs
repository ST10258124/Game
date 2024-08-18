using Godot;
using System;

public partial class Start : StaticBody3D
{

	PackedScene leftCube = GD.Load<PackedScene>("res://player_Left.tscn");
	PackedScene rightCube = GD.Load<PackedScene>("res://player_Right.tscn");
	Timer Reset;
	Globals Controller;
	WorldEnvironment Sky;
	CharacterBody3D PlayerLeft;
	CharacterBody3D PlayerRight;
	MeshInstance3D BeamOuter;
	MeshInstance3D BeamInner;
	AudioStreamPlayer BGMusic;
	AudioStreamPlayer3D sfxGate, sfxGateSecondary, sfxHumLeft, sfxHumRight, sfxHumMiddle, sfxMetallicLeft, sfxMetallicRight, sfxCrackleLeft, sfxCrackleRight;
	GpuParticles3D sparksGateBlue, sparksGatePink, metalSparksLeft, metalSparksRight;
	AnimationPlayer transition;
	AnimatedSprite3D title;
	Label3D highScore, currentScore;
	bool SkyMoveDone;
	bool splashDone = false;
	Random rng;
	BGM_Shuffler shuffler = new BGM_Shuffler();
	HighScoreManager best = new HighScoreManager();
	StringEncryptor aes = new StringEncryptor(new byte[32], new byte[16]);
	/*some of the above woulda been declared on the same line but they behave incorrectly when the game statrs and somehow
	having it like this prevents the issue. . .idek :')*/

	/* float skyDefaultX = -156.0f;
	 float skyDefaultY = 260.0f;
	 float skyDefaultZ = -40.0f; */
	float skyX = -156.0f;
	float skyY = 260.0f;
	float skyZ = -40.0f;
	Vector3 LeftPos, RightPos;
	int bgmIndex, bgmPrev;

	private String[] Menuloops = { "Menu Variation0", "Menu Variation1", "Menu Variation2", "Menu Variation3" };
	private String[] HumSounds = { "Hum1", "Hum2" };
	private String[] metallicSounds = { "Metallic1", "Metallic2", "Metallic3" };

	public override void _Process(double delta)
	{
		/*
		GetNode<Label>("%FPSCounter").Text = Math.Truncate((1000 / delta) / 1000).ToString(); 
		dont have this run every frame, every second will be fine
		FPS = (1000 / delta) / 1000
		*/

		if (!SkyMoveDone)
		{
			Sky.Environment.SkyRotation = Sky.Environment.SkyRotation.Lerp(new Vector3(-skyX, skyY, -skyZ), 0.001f * (float)delta);

			SkyMoveDone = Sky.Environment.SkyRotation == new Vector3(-skyX, skyY, -skyZ);
		}
		else
		{
			skyX = rng.Next(-360, 360); //negative
			skyY = rng.Next(-360, 360);
			skyZ = rng.Next(-360, 360); //negative

			SkyMoveDone = false;
		}
	}

	public override void _Ready()
	{
		rng = new Random();
		Controller = (Globals)GetNode("/root/Globals");
		Reset = GetNode<Timer>("ResetTimer");
		Sky = GetNode<WorldEnvironment>("WorldEnvironment");
		SkyMoveDone = true;

		BeamOuter = GetNode<MeshInstance3D>("Beam/Outer");
		BeamOuter.Visible = false;
		BeamInner = GetNode<MeshInstance3D>("Beam/Inner");
		BeamInner.Visible = false;

		sparksGateBlue = GetNode<GpuParticles3D>("Beam/SparksGateBlue");
		sparksGatePink = GetNode<GpuParticles3D>("Beam/SparksGatePink");
		metalSparksLeft = GetNode<GpuParticles3D>("MetalSparksLeft");
		metalSparksRight = GetNode<GpuParticles3D>("MetalSparksRight");

		shuffler.Shuffle(Menuloops);
		bgmIndex = 0;
		BGMusic = GetNode<AudioStreamPlayer>("BGM");

		title = GetNode<AnimatedSprite3D>("2DStuff/AnimatedTitle");
		title.Play("default");

		sfxGate = GetNode<AudioStreamPlayer3D>("Gate");
		sfxGateSecondary = GetNode<AudioStreamPlayer3D>("GateSecondary");
		sfxHumLeft = GetNode<AudioStreamPlayer3D>("HumLeft");
		sfxHumRight = GetNode<AudioStreamPlayer3D>("HumRight");
		sfxHumMiddle = GetNode<AudioStreamPlayer3D>("Beam/HumMiddle");
		sfxMetallicLeft = GetNode<AudioStreamPlayer3D>("WallLeft/MetallicLeft");
		sfxMetallicRight = GetNode<AudioStreamPlayer3D>("WallRight/MetallicRight");
		sfxCrackleLeft = GetNode<AudioStreamPlayer3D>("WallLeft/CrackleLeft");
		sfxCrackleRight = GetNode<AudioStreamPlayer3D>("WallRight/CrackleRight");

		highScore = GetNode<Label3D>("2DStuff/AnimatedTitle/HighScore");
		currentScore = GetNode<Label3D>("2DStuff/Score");

		best.ReplaceHighScore(0);//security measure. . . also prevents a negative score if that magically happens. unless ofc the user gone and tried to be smart
		highScore.Text = "High Score: " + aes.DecryptString(best.ReadHighScoreFile()[0]);

		

		SetPhysicsProcess(false);
		/*I THINK IT BE LIKE:
		  NAME_OF_CLASS VAR_NAME
		  VAR_NAME = (NAME_OF_CLASS)GETNODE("/ROOT/NAME_GIVEN_IN_AUTOLOAD");*/
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Controller.Game_Over)
		{
			Reset.Start();
			best.ReplaceHighScore(Controller.score);

			BeamOuter.Visible = false;
			BeamInner.Visible = false;

			sfxMetallicLeft.Stop();
			sfxMetallicRight.Stop();
			sfxCrackleLeft.Stop();
			sfxCrackleRight.Stop();

			metalSparksLeft.Emitting = false;
			metalSparksRight.Emitting = false;

			SetPhysicsProcess(false);
		}

		currentScore.Text = Controller.score.ToString();
		/*shitty way to go about doing this
		woulda worked fine with a signal but some wanker changed the syntax
		and didn't update it in the documentation.
		dunno if that only affects c# or GD script too but am not changing the language
		of a whole ass script for a signal :/

		works perfectly like this but look into a more resource efficient way of doing this*/

		LeftPos = PlayerLeft.Position;
		RightPos = PlayerRight.Position;

		//======================================SCALING OF THE "BEAM" BELOW=============================================

		BeamOuter.GlobalPosition = new Vector3((RightPos.X + LeftPos.X) / 2, 0.5f, LeftPos.Z);
		BeamInner.GlobalPosition = new Vector3((RightPos.X + LeftPos.X) / 2, 0.5f, LeftPos.Z);

		/*X = calculated midpoint of cubes (player)
		  Y = 0.5 because that never changes
		  Z = Z position of any cube because of that slight offset at start of game*/

		BeamOuter.Scale = new Vector3((RightPos.X - LeftPos.X) / 0.6f,
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f),
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f)); //length formula thingy here

		BeamInner.Scale = new Vector3((RightPos.X - LeftPos.X) / 0.3f,
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f),
								-(1.0f / 17.0f) * (RightPos.X - LeftPos.X) + (71.0f / 68.0f)); //length formula thingy here

		//================================PLAYER MOVEMENT SOUND PLAYING LOGIC THINGY BELOW===================================

		if (Input.IsActionJustPressed("MoveLeft") || (Input.IsActionPressed("MoveLeft") && Input.IsActionJustReleased("MoveRight")))
		{
			sfxHumLeft.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + HumSounds[rng.Next(2)] + ".wav");
			sfxHumLeft.Play();
		}

		if (Input.IsActionJustPressed("MoveRight") || (Input.IsActionPressed("MoveRight") && Input.IsActionJustReleased("MoveLeft")))
		{
			sfxHumRight.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + HumSounds[rng.Next(2)] + ".wav");
			sfxHumRight.Play();
		}

		if ((!Input.IsActionPressed("MoveLeft") && Input.IsActionJustReleased("MoveRight")) || (!Input.IsActionPressed("MoveRight") && Input.IsActionJustReleased("MoveLeft")))
		{
			sfxHumMiddle.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + HumSounds[rng.Next(2)] + ".wav");
			sfxHumMiddle.Play();
		}
	}

	public override void _UnhandledInput(InputEvent @event) //prolly a better way to do the "left click to start" bit
	{
		if (@event is InputEventMouseButton mouseButton)
		{
			if (!mouseButton.Pressed && !Controller.playing && splashDone)
			{
				PlayerLeft = (CharacterBody3D)leftCube.Instantiate();
				PlayerLeft.Position = new Vector3(-2.0f, 0.5f, 7.5f);
				//Node3d myNode3D = (Node3D)myObjectToInstantiate.Instantiate()
				PlayerRight = (CharacterBody3D)rightCube.Instantiate();
				PlayerRight.Position = new Vector3(2.0f, 0.5f, 7.5f);

				AddChild(PlayerLeft);
				AddChild(PlayerRight);
				//^^SPAWN PLAYER^^

				BGMusic.Stop();//MAKE THIS LIKE A FADE OUT KIND OF THING
				sfxGateSecondary.Play();
				sfxHumMiddle.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + HumSounds[rng.Next(2)] + ".wav");
				sfxHumMiddle.Play();

				BeamOuter.Visible = true;
				BeamInner.Visible = true;

				Controller.score = 0;
				Controller.playing = true;
				Controller.SetPhysicsProcess(Controller.playing);
				Controller.newGame();

				currentScore.Visible = true;

				sparksGateBlue.Emitting = true;
				sparksGatePink.Emitting = true;

				title.Visible = false;

				SetPhysicsProcess(true);
			}
		}
	}

	public void _on_reset_timer_timeout()
	{
		Controller.clearLevel();
		Controller.playing = false;
		Controller.Game_Over = false;
		title.Visible = true;

		currentScore.Visible = false;
		
		highScore.Text = "High Score: " + aes.DecryptString(best.ReadHighScoreFile()[0]);
		
		BGMusic.Stream = (AudioStream)ResourceLoader.Load("res://Music/" + Menuloops[bgmIndex] + ".ogg");
		if (bgmIndex == 3){
			bgmIndex = -1; //becasue incrementer is after this it will be 0
			shuffler.Shuffle(Menuloops);
		}

		bgmIndex++;
		_on_bgm_finished();
	}

	public void _on_bgm_finished()
	{
		BGMusic.Play();
	}

	public void Level_Cleared(Area3D area)
	{
		if (area.IsInGroup("Gate"))
		{
			Controller.nextLevel();
			sfxGate.Play();
			sfxGateSecondary.Play();

			sparksGateBlue.Emitting = true;
			sparksGatePink.Emitting = true;
		}
	}
	//=====================RNG AND START PLAYING METALLIC SOUND===============================
	public void LeftMetallic(Area3D area)
	{
		if (area.IsInGroup("Player"))
		{
			sfxMetallicLeft.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + metallicSounds[rng.Next(3)] + ".ogg");
			LeftMetallicLoop();
			metalSparksLeft.Emitting = true;
			sfxCrackleLeft.Play();
		}
	}

	public void RightMetallic(Area3D area)
	{
		if (area.IsInGroup("Player"))
		{
			sfxMetallicRight.Stream = (AudioStream)ResourceLoader.Load("res://SFX/" + metallicSounds[rng.Next(3)] + ".ogg");
			RightMetallicLoop();
			metalSparksRight.Emitting = true;
			sfxCrackleRight.Play();
		}
	}
	//=======================STOP PLAYING METALLIC SOUND==============================
	public void LeftMetallicStop(Area3D area)
	{
		sfxMetallicLeft.Stop();
		metalSparksLeft.Emitting = false;
		sfxCrackleLeft.Stop();
	}

	public void RightMetallicStop(Area3D area)
	{
		sfxMetallicRight.Stop();
		metalSparksRight.Emitting = false;
		sfxCrackleRight.Stop();
	}

	//==========================LOOP THE METALLIC SOUND============================
	public void LeftMetallicLoop()
	{
		sfxMetallicLeft.Play();
	}

	public void RightMetallicLoop()
	{
		sfxMetallicRight.Play();
	}

	public void LeftCrackleLoop()
	{
		sfxCrackleLeft.Play();
	}

	public void RightCrackleLoop()
	{
		sfxCrackleRight.Play();
	}
//============================================================
	void _on_animation_player_animation_finished(string anim){
		splashDone = true;
	}
}