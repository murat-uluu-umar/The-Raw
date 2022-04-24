using Godot;
using System.Collections.Generic;
using RatBot;

public class DuelManager : Spatial
{

    [Signal]
    public delegate void EnemySubmit(Rat rat);

    private const float harmDistance = 2f;
    public Rat duelant = null;
    private Node global;
    private KinematicBody player = null;
    private int playerAction = 3;
    private int botAction = 3;

    public override void _Ready()
    {
        global = GetNode<Node>("/root/gl");
        player = (KinematicBody)global.Get("player");
        Connect("EnemySubmit", player, "enemy_submit");
    }

    public void Submit(Rat body)
    {
        if (player != null)
        {
            body.player = player;
            EmitSignal("EnemySubmit", body);
            duelant = body;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        Duel();
    }

    public void PlayerAction(int actionType)
    {
        playerAction = actionType;
        if (duelant != null)
        {
            duelant.brain.playerState = actionType;
        }
    }

    public void BotAction(int actionType)
    {
        botAction = actionType;
    }

    public void Duel()
    {
        if (duelant != null)
        {
            if (duelant.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) <= harmDistance)
            {
                int actionState = Mathf.Abs(playerAction - botAction);
                if (actionState <= 1)
                {
                    // GD.Print("REFLECTED!");
                }
                else if (actionState > 1)
                {
                    // GD.Print("HIT!");    
                }
            }
        }
    }

}