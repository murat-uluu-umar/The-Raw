using Godot;
using System.Collections.Generic;

public class DuelManager : Spatial
{
    Queue<KinematicBody> duelants;

    public override void _Ready()
    {
        duelants = new Queue<KinematicBody>();

    }
    
    public void Submit(KinematicBody body)
    {

    }
}