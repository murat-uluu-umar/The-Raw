extends KinematicBody

#constants
const UP = 0
const DOWN = 1

#nodes
onready var animation_tree = get_node('../AnimationTree')
onready var stand_colider : CollisionShape = get_node('CollisionShape')
onready var crouch_colider : CollisionShape = get_node('CollisionShapeCrouch')
onready var boy : Spatial = get_node('Boy')
onready var tween : Tween = get_node('../Tween')

#animation tree parameters
var state = 'parameters/state/blend_amount'
var up_state = 'parameters/up_state/blend_amount'
var down_state = 'parameters/down_state/blend_amount'
var turn_around = 'parameters/turn_around/active'
var jump = 'parameters/jump/active'
var activity = 'parameters/activity/current'
var activity_shot = 'parameters/activity_shot/active'
var block = 'parameters/block/blend_amount'
#variables
var velocity = Vector3(0,0,0)
var left = true

func _ready():
	pass

func _physics_process(delta):
	input_handler()
	move_and_slide(velocity,Vector3.UP)

func move(dir : int) -> void:
	movement_state(0)
	velocity.z = gl.speed * dir

func movement_state(mstate : int) -> void:
	interpolate(animation_tree, up_state, animation_tree.get(up_state), mstate, 0.05)
	interpolate(animation_tree, down_state, animation_tree.get(down_state), mstate, 0.05)

func switch_coliders(state_colider : int) -> void:
	if state_colider == UP:
		stand_colider.disabled = false
		crouch_colider.disabled = true
	else: 
		stand_colider.disabled = true
		crouch_colider.disabled = false

func input_handler():
	if Input.is_action_pressed("block"):
		do_block()
	elif Input.is_action_just_released("block"):
		interpolate(animation_tree, block, animation_tree.get(block), 0, 0.05)
	elif Input.is_action_just_pressed("attack"):
		action(0)
	elif Input.is_action_just_pressed("slash"):
		action(1)
	if Input.is_action_pressed("ui_right") and is_able():
		move(-1)
		if left:
			interpolate(self, 'rotation_degrees', rotation_degrees, Vector3(0,180,0), 0.2, Tween.TRANS_CIRC)
			left = false
	elif Input.is_action_pressed("ui_left") and is_able():
		move(1)
		if !left:
			interpolate(self, 'rotation_degrees', rotation_degrees, Vector3(0,0,0), 0.2, Tween.TRANS_CIRC)
			left = true
	else: 
		velocity.z = 0
		movement_state(1)
	if Input.is_action_pressed("ui_down") and is_able():
		interpolate(animation_tree, state, animation_tree.get(state), DOWN, 0.05)
		switch_coliders(DOWN)
	elif Input.is_action_just_released("ui_down") and is_able(): 
		interpolate(animation_tree, state, animation_tree.get(state), UP, 0.05)
		switch_coliders(UP)
	if Input.is_action_pressed("ui_up") and is_on_floor() and is_able():
		velocity.y += gl.jump
		animation_tree.set(jump, true)
	elif !is_on_floor():
		velocity.y -= gl.gravity
	else: 
		velocity.y = 0

func interpolate(object : Object, property : String, from, to, duration : float = 0.2, trans_type : int = Tween.TRANS_LINEAR, ease_type : int = Tween.EASE_IN_OUT):
	tween.interpolate_property(object,property,
	from, to, duration, trans_type,
	ease_type)
	tween.start()

func action(input : int) -> void:
	animation_tree.set(activity, input)
	animation_tree.set(activity_shot, true)

func do_block():
	interpolate(animation_tree, block, animation_tree.get(block), 1, 0.05)

func is_able():
	if animation_tree.get(activity_shot) or animation_tree.get(block) != 0:
		return false
	return true