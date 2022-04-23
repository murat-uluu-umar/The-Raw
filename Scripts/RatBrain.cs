using Godot;
using System;
using RatBot;

public class Brain
{
    const float distanceAmount = 1f;
    private float distanceFault = 0;
    private float skill = 0;
    private Vector3 velocity = new Vector3(0, 0, 2);
    public int playerState { set; get; } = 3;
    public Brain(float skill)
    {
        this.skill = skill;
        UpdateDistanceFault();
    }

    public void Mind(KinematicBody rat, KinematicBody player)
    {
        if (player != null)
        {
            Orientation(rat, player);
            Move(rat, player);
            Action(rat, player);
        }
    }

    private void Orientation(KinematicBody rat, KinematicBody player)
    {
        if (player.GlobalTransform.origin.z > rat.GlobalTransform.origin.z)
        {
            rat.RotationDegrees = new Vector3(0, Mathf.Lerp(rat.RotationDegrees.y, 0, 0.12f), 0);
        }
        else
        {
            rat.RotationDegrees = new Vector3(0, Mathf.Lerp(rat.RotationDegrees.y, 180, 0.12f), 0);
        }
    }

    private void Move(KinematicBody rat, KinematicBody player)
    {
        if (rat.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) > distanceAmount + distanceFault)
        {
            if (!rat.IsOnFloor()) velocity.y -= 1;
            else velocity.y = 0;
            rat.MoveAndSlide(velocity.Rotated(Vector3.Up, rat.Rotation.y), Vector3.Up);
            ChangeStateMachine(rat, StateMachine.WALK);
        }
        else
        {
            Rat ratState = (Rat)rat;
            if (ratState.stateMachine == StateMachine.WALK)
                UpdateDistanceFault();
            ChangeStateMachine(rat, StateMachine.IDLE);
        }
    }

    private void Action(KinematicBody rat, KinematicBody player)
    {
        if (rat.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) < distanceAmount + distanceFault)
        {
            ChangeStateMachine(rat, StateMachine.ATTACK);
            if (playerState < 2)
            {
                ChangeStateMachine(rat, (StateMachine)playerState++);
            }
            else if (playerState == 2)
            {
                ChangeStateMachine(rat, (StateMachine)playerState - 2);
            }
            else if (playerState == 3)
            {
                Rat ratState = (Rat)rat;
                if (!(Boolean)ratState.GetAnimationTree().Get(Rat.action))
                {
                    GD.Randomize();
                    ratState.stateMachine = (StateMachine)GD.RandRange(0, 2);
                    GD.Print(ratState.stateMachine);
                }
            }
        }
    }

    private void UpdateDistanceFault()
    {
        GD.Randomize();
        float range = distanceAmount - (distanceAmount / 100) * skill;
        distanceFault = (float)GD.RandRange(-range / 2, range);
    }

    private void ChangeStateMachine(KinematicBody rat, StateMachine stateMachine)
    {
        Rat ratState = (Rat)rat;
        ratState.stateMachine = stateMachine;
    }

}