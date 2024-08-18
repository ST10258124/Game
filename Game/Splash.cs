using Godot;
using System;

public partial class Splash : VideoStreamPlayer
{

	public override void _Ready(){
		
	}
	void _on_finished(){
		var sceneTree = GetTree();

		Node currentScene = sceneTree.CurrentScene;
		currentScene.QueueFree();
		//^^Above 2 lines are like that because just "QueueFree() deletes the node only, this way whole scene get deleted"

		var main = GD.Load<PackedScene>("res://playspace.tscn");

		Node instance = main.Instantiate();
		//this is the prolly the last script i'm writing and am starting to feel the imposter syndrome

		sceneTree.Root.AddChild(instance);
		sceneTree.CurrentScene = instance;

		/*this works perfectly and as expected and hoped for and all that good stuff
		i simultaneously understand and do not understand what i've done

		i've learned alot but i know nothing
		*/
		
	}
}
