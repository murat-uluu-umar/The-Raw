using Godot;

public class Quit : Button
{
    public override void _Pressed()
    {
        base._Pressed();
        GetTree().Quit();
    }
}
