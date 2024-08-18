using Godot;
using System;

public partial class Explode : Node3D
{
	Random rng = new Random();

	public override void _Ready()
	{
		SetRigidBodyPropertyRecursive(this);
	}

	private void SetRigidBodyPropertyRecursive(Node3D currentNode)
    {
        // Check if the current node is a RigidBody3D
        if (currentNode is RigidBody3D rigidBody)
        {
            rigidBody.Mass = 0.01f;
            rigidBody.LinearVelocity = new Vector3(0, rng.Next(2), -rng.Next(8));
            rigidBody.Inertia = new Vector3(0.001f, 0.001f, 0.001f);
            rigidBody.ApplyTorqueImpulse(new Vector3(-rng.Next(10) + 1, -rng.Next(10) + 1, -rng.Next(10) + 1));
        }

        // Recursively call this method for all children of the current node
        foreach (Node3D child in currentNode.GetChildren())
        {
            SetRigidBodyPropertyRecursive(child);
        }
    }
}
