using Godot;
using System.Collections.Generic;

public class DuelManager : Spatial
{

    private Node global;
    private KinematicBody player = null;
    private Queue<KinematicBody> duelants;

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
}