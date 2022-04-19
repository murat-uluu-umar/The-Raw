using Godot;
using System;

public class Rat : KinematicBody
{

    // Signals
    [Signal]
    public delegate void OnScreen(KinematicBody body); 
    // Constants
    private const String life = "parameters/life/current";
    private const String rest = "parameters/rest/current";
    private const String action_trans = "parameters/action_trans/current";
    private const String action = "parameters/action/active";
    private const String state = "parameters/state/blend_amount";
    private const String deathType = "parameters/death_type/blend_amount";
    private const String block = "parameters/block/blend_amount";

    // Variables
    private AnimationTree animationTree = null;
    private VisibilityNotifier visibilityNotifier = null;
    private KinematicBody target = null;

    public override void _Ready()
    {
        animationTree = GetNode<AnimationTree>("AnimationTree");
        visibilityNotifier = GetNode<VisibilityNotifier>("VisibilityNotifier");
        Connect("OnScreen", GetParent(), "Submit", this);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (visibilityNotifier.IsOnScreen())
        {
            EmitSignal("OnScreen");
        }
    }

    public void moveTo(Vector3 target)
    {
        
    }

}
