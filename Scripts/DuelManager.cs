using Godot;
using System.Collections.Generic;
using RatBot;

public class DuelManager : Spatial
{

    [Signal]
    public delegate void EnemySubmit(Rat rat);
    [Signal]
    public delegate void PlayerDeath();

    private const float harmDistance = 1.4f;
    private const float playerHarmDistance = 1f;
    public Rat duelant = null;
    private Node global;
    private KinematicBody player = null;
    private AudioStreamPlayer swordDraw = null;
    private AudioStreamPlayer swordDraw2 = null;
    private AudioStreamPlayer playerDeath = null;
    private int playerAction = 3;
    private int botAction = 3;
    private int whoIsFirst = -1;
    private bool playerWasted = false;

    public override void _Ready()
    {
        global = GetNode<Node>("/root/gl");
        player = (KinematicBody)global.Get("player");
        swordDraw = GetNode<AudioStreamPlayer>("SwordDraw");
        swordDraw2 = GetNode<AudioStreamPlayer>("SwordDraw2");
        playerDeath = GetNode<AudioStreamPlayer>("PlayerDeath");
        Connect("EnemySubmit", player, "enemy_submit");
        Connect("PlayerDeath", player, "death");
    }

    public void Submit(Rat body)
    {
        if (playerWasted)
            body.playerDeath = true;
        if (player != null && duelant == null && playerWasted == false )
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
        whoIsFirst = 0;
        playerAction = actionType;
        if (duelant != null)
        {
            duelant.brain.playerState = actionType;
            if (!IsEntityWasted() && duelant.playerDeath == false)
                if (GD.RandRange(0,2) == 0)
                    swordDraw.Play();
                else
                    swordDraw2.Play();

        }
    }

    public void BotAction(int actionType)
    {
        whoIsFirst = 1;
        botAction = actionType;
    }

    public void Duel()
    {
        if (duelant != null)
        {
            if (IsEntityWasted())
            {
                if (whoIsFirst == 0 && duelant.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) <= playerHarmDistance)
                {
                    if (duelant.IsAlive())
                    {
                        duelant.Death();
                        duelant = null;
                        whoIsFirst = -1;
                    }
                }
                else if (whoIsFirst == 1 && duelant.GlobalTransform.origin.DistanceTo(player.GlobalTransform.origin) <= harmDistance)
                    EmitSignal("PlayerDeath");
            }
        }
    }

    public bool IsEntityWasted()
    {
        if (playerAction == (int)StateMachine.WALK && botAction == (int)StateMachine.WALK)
            return false;
        int actionState = Mathf.Abs(playerAction - botAction);
        if (actionState <= 1)
            return false;
        else
            return true;
    }

    public void GameOver()
    {
        if (duelant != null)
        {
            duelant.player = null;
            duelant.playerDeath = true;
            playerWasted = true;
            duelant.stateMachine = (StateMachine)GD.RandRange(4, 6);
            playerDeath.Play();
        }
    }

}