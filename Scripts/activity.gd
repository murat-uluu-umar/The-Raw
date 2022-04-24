extends KinematicBody

#signals
signal action_signal(what)

signal death_signal()

#enums
enum ActionState {ATTACK=0, SLASH=1, BLOCK=2, NONE=3}

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
var block = 'parameters/block/active'
var life = 'parameters/life/current'
#variables
var velocity = Vector3(0,0,0)
var enemy = null
var action_state = ActionState.NONE

func _ready():
	gl.player = self
	connect('action_signal' ,get_parent().get_parent(), 'PlayerAction')
	connect('death_signal' ,get_parent().get_parent(), 'GameOver')

func _physics_process(delta):
	if is_alive():
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
	if Input.is_action_just_pressed("block") and is_able():
		do_block()
	elif Input.is_action_just_pressed("attack") and is_able():
		action(0)
	elif Input.is_action_just_pressed("slash") and is_able():
		action(1)

	if Input.is_action_pressed("ui_right") and is_able():
		move(-1)
		rotate_to(false)
		throw_signal(ActionState.NONE)
	elif Input.is_action_pressed("ui_left") and is_able():
		move(1)
		rotate_to(true)
		throw_signal(ActionState.NONE)
	else: 
		velocity.z = 0
		movement_state(1)
	if enemy != null:
		if enemy.global_transform.origin.distance_to(global_transform.origin) < 1.5:
			if enemy.global_transform.origin.z < global_transform.origin.z:
				rotate_to(false)
			else:
				rotate_to(true)
	if Input.is_action_pressed("ui_down") and is_able():
		interpolate(animation_tree, state, animation_tree.get(state), DOWN, 0.05)
		switch_coliders(DOWN)
	elif Input.is_action_just_released("ui_down"): 
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
	tween.stop(object, property)
	tween.interpolate_property(object,property,
	from, to, duration, trans_type,
	ease_type)
	tween.start()

func action(input : int) -> void:
	animation_tree.set(activity, input)
	animation_tree.set(activity_shot, true)
	if input:
		throw_signal(ActionState.ATTACK)
	else: 
		throw_signal(ActionState.SLASH)

func do_block():
	if animation_tree.get(block) == false:
		throw_signal(ActionState.BLOCK)
		animation_tree.set(block, true)
	
	

func is_able():
	if animation_tree.get(activity_shot) or animation_tree.get(block):
		return false
	return true

func rotate_to(leftin : bool):
	if !leftin:
		rotation_degrees.y = lerp(rotation_degrees.y, 180, 0.12)
	else:
		rotation_degrees.y = lerp(rotation_degrees.y, 0, 0.12)

func enemy_submit(rat):
	enemy = rat

func death():
	if is_alive():
		animation_tree.set(life, 1)
		emit_signal('death_signal')

func throw_signal(action_states):
	if (action_states != action_state):
		if (action_state == ActionState.ATTACK):
			yield(get_tree().create_timer(0.6), 'timeout')
		elif (action_state == ActionState.SLASH):
			yield(get_tree().create_timer(15 / 18), 'timeout')
		elif (action_state == ActionState.BLOCK):
			yield(get_tree().create_timer(0.5), 'timeout')
		print('yield')
		emit_signal('action_signal', action_states)
		action_state = action_states

func is_alive():
	return !animation_tree.get(life)
