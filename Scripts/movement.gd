extends KinematicBody

const UP = 0
const DOWN = 1

onready var animation_tree = get_node('../AnimationTree')
onready var boy : Spatial = get_node('Boy')
var state = 'parameters/state/blend_amount'
var up_state = 'parameters/up_state/blend_amount'
var down_state = 'parameters/down_state/blend_amount'
var turn_around = 'parameters/turn_around/active'
var jump = 'parameters/jump/active'
var velocity = Vector3(0,0,0)
var left = true
func _ready():
	pass

func _physics_process(delta):
	if Input.is_action_pressed("ui_down"):
		animation_tree.set(state, DOWN)
	else: animation_tree.set(state, UP)

	if Input.is_action_pressed("ui_right"):
		move(-1)
		if left:
			rotation.y = deg2rad(180)
			left = false
	elif Input.is_action_pressed("ui_left"):
		move(1)
		if !left:
			rotation.y = 0
			left = true
	else: 
		velocity.z = 0
		movement_state(1)
	if Input.is_action_pressed("ui_up") && is_on_floor():
		velocity.y += gl.jump
		animation_tree.set(jump, true)
	elif !is_on_floor():
		velocity.y -= gl.gravity
	else: 
		velocity.y = 0
	move_and_slide(velocity,Vector3.UP)

func move(dir : int) -> void:
	movement_state(0)
	velocity.z = gl.speed * dir

func movement_state(mstate : int) -> void:
	animation_tree.set(up_state, mstate)
	animation_tree.set(down_state, mstate)
