using Godot;
using System.Collections.Generic;
using RatBot;

public class DuelManager : Spatial
{

    private const float harmDistance = 2f;

    private Node global;
    private KinematicBody player = null;
    private Queue<KinematicBody> duelants;
    private int playerAction = 3;
    private int botAction = 3;

    public override void _Ready()
    {
        global = GetNode<Node>("/root/gl");
        player = (KinematicBody) global.Get("player");
        duelants = new Queue<KinematicBody>();
    }

    public void Submit(Rat body)
    {
        duelants.Enqueue(body);
        if (player != null)
            body.player = player;
        GD.Print(duelants.Count);
    }

    public override void _PhysicsProcess(float delta)
    {
        Duel();
    }

    public void PlayerAction(int actionType)
    {
        playerAction = actionType;
        if (duelants.Count != 0)
        {
            Rat bot = (Rat) duelants.Peek();
            bot.brain.playerState = actionType;
        }
    }

    public void BotAction(int actionType)
    {
        botAction = actionType;
    }

    public void Duel()
    {
        if (duelants.Count != 0)
        {
            Rat bot = (Rat) duelants.Peek();
            if (bot.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) <= harmDistance)
            {
                int actionState = Mathf.Abs(playerAction - botAction);
                if (actionState <= 1)
                {
                    GD.Print("REFLECTED!");
                } 
                else if (actionState > 1)
                {
                    GD.Print("HIT!");    
                }
            }
        }
    }

}