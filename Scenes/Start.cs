using Godot;

public class Start : Button
{
    public override void _Pressed()
    {
        base._Pressed();
        GetTree().ChangeScene("res://Scenes/Village.tscn");
    }
}
