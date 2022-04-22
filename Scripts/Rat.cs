using Godot;
using System;

public class Rat : KinematicBody
{

    // Enums
    enum StateMachine {
        WALK,
        IDLE,
        REST,
        BLOCK,
        SLASH,
        ATTACK
    };
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
    private StateMachine stateMachine = StateMachine.REST;
    private Tween tween = null;
    public KinematicBody player {set; get;} = null;
    public Brain brain = null;

    public override void _Ready()
    {
        brain = new Brain();
        animationTree = GetNode<AnimationTree>("AnimationTree");
        visibilityNotifier = GetNode<VisibilityNotifier>("VisibilityNotifier");
        tween = GetNode<Tween>("Tween");
        Connect("OnScreen", GetParent(), "Submit");
    }

    public override void _PhysicsProcess(float delta)
    {
        SignalHandling();
        StateSwitching();
        brain.Mind(this, player);
    }

    public void SignalHandling()
    {
        if (visibilityNotifier.IsOnScreen() && player == null)
        {
            EmitSignal("OnScreen", this);
        }
    }

    public void StateSwitching()
    {
        switch (stateMachine)
        {
            case StateMachine.WALK:
                animationTree.Set(rest,1);
                tween.InterpolateProperty(animationTree,state,animationTree.Get(state), 0, 0.1f, Tween.TransitionType.Linear,Tween.EaseType.InOut);
                tween.Start();
                break;
            case StateMachine.IDLE:
                animationTree.Set(rest,1);
                tween.InterpolateProperty(animationTree,state,animationTree.Get(state), 1, 0.1f, Tween.TransitionType.Linear,Tween.EaseType.InOut);
                tween.Start();
                break;
            case StateMachine.REST:
                animationTree.Set(rest,0);
                break; 
            case StateMachine.BLOCK:
                tween.InterpolateProperty(animationTree,block,animationTree.Get(block), 1, 0.1f, Tween.TransitionType.Linear,Tween.EaseType.InOut);
                tween.Start();
                tween.InterpolateProperty(animationTree,block,animationTree.Get(block), 0, 0.1f, Tween.TransitionType.Linear,Tween.EaseType.InOut);
                tween.Start();
                break;
            case StateMachine.SLASH:
                animationTree.Set(action_trans,1);
                animationTree.Set(action, true);
                break;
            case StateMachine.ATTACK:
                animationTree.Set(action_trans,0);
                animationTree.Set(action, true);
                break;
        }
    }

    public class Brain 
    {
        const float distanceAmount = 1.147f;
        private Vector3 velocity = new Vector3(0,0,2);
        public Brain(){}

        public void Mind(KinematicBody rat, KinematicBody player)
        {
            if (player != null)
            {
                Orientation(rat, player);
                Move(rat,player);
            }
        }

        private void Orientation(KinematicBody rat, KinematicBody player)
        {
            if (player.GlobalTransform.origin.z > rat.GlobalTransform.origin.z)
            {
                rat.RotationDegrees = new Vector3(0,Mathf.Lerp(rat.RotationDegrees.y, 0, 0.12f),0);
            } else
            {
                rat.RotationDegrees = new Vector3(0,Mathf.Lerp(rat.RotationDegrees.y, 180, 0.12f),0);
            }
        }

        private void Move(KinematicBody rat, KinematicBody player)
        {
            if (rat.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) > distanceAmount)
            {
                if (!rat.IsOnFloor()) velocity.y -= 1;
                else velocity.y = 0;
                rat.MoveAndSlide(velocity.Rotated(Vector3.Up, rat.Rotation.y), Vector3.Up);
                Rat ratState = (Rat) rat;
                ratState.stateMachine = StateMachine.WALK;
            }else
            {
                Rat ratState = (Rat) rat;
                ratState.stateMachine = StateMachine.IDLE;
            }
        }

    }

}

