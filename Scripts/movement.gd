extends KinematicBody

var velocity = Vector3(0,0,0)

func _ready():
	pass

func _physics_process(delta):
	if Input.is_action_pressed("ui_right"):
		velocity.x = gl.speed
	elif Input.is_action_pressed("ui_left"):
		velocity.x = -gl.speed
	else: velocity.x = 0
	if Input.is_action_pressed("ui_up") && is_on_floor():
		velocity.y += gl.jump
	elif !is_on_floor():
		velocity.y -= gl.gravity
	else: 
		velocity.y = 0
	move_and_slide(velocity,Vector3.UP)
