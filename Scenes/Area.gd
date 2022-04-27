extends Area

func _on_Area_body_entered(body:KinematicBody):
	if body == gl.player:
		get_tree().change_scene("res://Scenes/Thanks.tscn")
