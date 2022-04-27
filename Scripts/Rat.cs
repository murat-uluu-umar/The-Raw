using Godot;
using System;

namespace RatBot
{

    public enum StateMachine
    {
        SLASH,
        ATTACK,
        BLOCK,
        WALK,
        IDLE,
        REST,
    };

    public enum ActionState
    {
        SLASH = 0,
        ATTACK = 1,
        BLOCK = 2,
        NONE = 3
    }

    public class Rat : KinematicBody
    {

        [Export]
        public float skill = 60;

        // Signals
        [Signal]
        public delegate void OnScreen(KinematicBody body);
        [Signal]
        public delegate void ActionSignal(int actionType);
        // Constants
        public const String life = "parameters/life/current";
        public const String rest = "parameters/rest/current";
        public const String action_trans = "parameters/action_trans/current";
        public const String action = "parameters/action/active";
        public const String state = "parameters/state/blend_amount";
        public const String deathType = "parameters/death_type/current";
        public const String block = "parameters/block/active";
        // Variables
        private AnimationTree animationTree = null;
        private AudioStreamPlayer deathSound = null;
        private AudioStreamPlayer Sound = null;
        private AudioStreamPlayer SecondSound = null;
        private VisibilityNotifier visibilityNotifier = null;
        public StateMachine stateMachine = (StateMachine)GD.RandRange(4, 6);
        public ActionState actionState = ActionState.NONE;
        private Tween tween = null;
        public KinematicBody player { set; get; } = null;
        public bool playerDeath = false;
        public Brain brain = null;

        public override void _Ready()
        {
            brain = new Brain(skill);
            animationTree = GetNode<AnimationTree>("AnimationTree");
            deathSound = GetNode<AudioStreamPlayer>("DeathSound");
            Sound = GetNode<AudioStreamPlayer>("Sound");
            SecondSound = GetNode<AudioStreamPlayer>("SecondSound");
            visibilityNotifier = GetNode<VisibilityNotifier>("VisibilityNotifier");
            tween = GetNode<Tween>("Tween");
            Connect("OnScreen", GetParent(), "Submit");
            Connect("ActionSignal", GetParent(), "BotAction");
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsAlive())
            {
                SignalHandling();
                StateSwitching();
                brain.Mind(this, player);
            }
        }

        public void SignalHandling()
        {
            if (visibilityNotifier.IsOnScreen() && player == null && playerDeath == false)
            {
                EmitSignal("OnScreen", this);
                Sound.Play();
                SecondSound.Play();
            }
        }

        public void StateSwitching()
        {
            switch (stateMachine)
            {
                case StateMachine.WALK:
                    animationTree.Set(rest, 1);
                    tween.InterpolateProperty(animationTree, state, animationTree.Get(state), 0, 0.1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                    tween.Start();
                    MakeActionSignal(ActionState.NONE);
                    break;
                case StateMachine.IDLE:
                    animationTree.Set(rest, 1);
                    tween.InterpolateProperty(animationTree, state, animationTree.Get(state), 1, 0.1f, Tween.TransitionType.Linear, Tween.EaseType.InOut);
                    tween.Start();
                    MakeActionSignal(ActionState.NONE);
                    break;
                case StateMachine.REST:
                    animationTree.Set(rest, 0);
                    MakeActionSignal(ActionState.NONE);
                    break;
                case StateMachine.BLOCK:
                    if (!(Boolean)GetAnimationTree().Get(Rat.action) || !(Boolean)GetAnimationTree().Get(Rat.block))
                        animationTree.Set(block, true);
                    MakeActionSignal(ActionState.BLOCK);
                    break;
                case StateMachine.SLASH:
                    animationTree.Set(action_trans, 1);
                    if (!(Boolean)GetAnimationTree().Get(Rat.action) || !(Boolean)GetAnimationTree().Get(Rat.block))
                        animationTree.Set(action, true);
                    MakeActionSignal(ActionState.SLASH);
                    break;
                case StateMachine.ATTACK:
                    animationTree.Set(action_trans, 0);
                    if (!(Boolean)GetAnimationTree().Get(Rat.action) || !(Boolean)GetAnimationTree().Get(Rat.block))
                        animationTree.Set(action, true);
                    MakeActionSignal(ActionState.ATTACK);
                    break;
            }
        }

        public AnimationTree GetAnimationTree()
        {
            return this.animationTree;
        }

        public void SetAnimationTree(AnimationTree animationTree)
        {
            this.animationTree = animationTree;
        }

        public async void MakeActionSignal(ActionState actionState)
        {
            if (playerDeath == false)
                if (this.actionState != actionState)
                {
                    GD.Print(playerDeath);
                    if (actionState == ActionState.ATTACK)
                    {
                        SecondSound.Play();
                        GD.Print("ATTACK");
                        await ToSignal(GetTree().CreateTimer(0.7f), "timeout");
                    }
                    else if (actionState == ActionState.SLASH)
                    {
                        SecondSound.Play();
                        GD.Print("SLASH");
                        await ToSignal(GetTree().CreateTimer(0.9f), "timeout");
                    }
                    else if (actionState == ActionState.BLOCK)
                    {
                        await ToSignal(GetTree().CreateTimer(0.3f), "timeout");
                        SecondSound.Play();
                        GD.Print("BLOCK");
                    }
                    EmitSignal("ActionSignal", actionState);
                    this.actionState = actionState;
                }
        }

        public void Death()
        {
            deathSound.Play();
            animationTree.Set(deathType, GD.RandRange(0, 2));
            animationTree.Set(life, 1);
        }

        public bool IsAlive()
        {
            return (int)animationTree.Get(life) != 1;
        }

    }

}
