using Godot;
using System.Collections.Generic;

public class DuelManager : Spatial
{

    private Node Global;
    private KinematicBody Player = null;
    private Queue<KinematicBody> duelants;

    public override void _Ready()
    {
        Global = GetNode<Node>("/root/gl");
        Player = (KinematicBody) Global.Get("player");
        duelants = new Queue<KinematicBody>();
    }

    public void Submit(Rat body)
    {
        duelants.Enqueue(body);
        if (Player != null)
            body.Player = Player;
        GD.Print(duelants.Count);
    }
}