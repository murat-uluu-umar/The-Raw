using Godot;
using System;

namespace RatBot
{

    public enum StateMachine
    {
        ATTACK,
        SLASH,
        BLOCK,
        WALK,
        IDLE,
        REST,
    };

    public enum ActionState
    {
        ATTACK = 0,
        SLASH = 1,
        BLOCK = 2,
        NONE = 3
    }

    public class Rat : KinematicBody
    {

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
        public const String deathType = "parameters/death_type/blend_amount";
        public const String block = "parameters/block/active";

        // Variables
        private AnimationTree animationTree = null;

        private VisibilityNotifier visibilityNotifier = null;
        private KinematicBody target = null;
        public StateMachine stateMachine = StateMachine.REST;
        public ActionState actionState = ActionState.NONE;
        private Tween tween = null;
        public KinematicBody player { set; get; } = null;
        public Brain brain = null;

        public override void _Ready()
        {
            brain = new Brain(60);
            animationTree = GetNode<AnimationTree>("AnimationTree");
            visibilityNotifier = GetNode<VisibilityNotifier>("VisibilityNotifier");
            tween = GetNode<Tween>("Tween");
            Connect("OnScreen", GetParent(), "Submit");
            Connect("ActionSignal", GetParent(), "BotAction");
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

        public void MakeActionSignal(ActionState actionState)
        {
            if (this.actionState != actionState)
            {
                EmitSignal("ActionSignal", actionState);
                this.actionState = actionState;
            }
        }

    }

}
