using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SantaController : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField] private float _smoothing = 0.06f; // Smoothing factor for transitions
	[SerializeField] private float _runningSmoothing = 0.08f; // Smoothing factor for running transitions
	[SerializeField] private float _defaultMoveSpeed = 5f;
	[SerializeField] private float _defaultJumpForce = 5f;
	[SerializeField] private float _jumpCooldown = 0.25f;
	[SerializeField] private float _groundCheckDistance = 0.3f;
	[SerializeField] private bool _acceleratedIceJump;
	[SerializeField] private float _restartHeight;

	private Animator _animator;
	private Rigidbody _rb;

	// Target values for x and y parameters
	private float _targetX = 0f;
	private float _targetY = 0f;

	// Current value
	private float _currentSpeed = 0f;

	private bool _canJump = true;
	private bool _jump = false;
	private bool _boostJump = false;
	private bool _onIce = false;

	private float _lastLandTime = -Mathf.Infinity;
	private float _moveSpeed, _jumpForce;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();

		_moveSpeed = _defaultMoveSpeed;
		_jumpForce = _defaultJumpForce;
	}

	void Update()
	{
		// Check if the player is grounded
		CheckGrounded();

		// Check if jump requested
		CheckJump();

		// Check if fall for restart
		CheckRestart();
	}

	void FixedUpdate()
	{
		bool isRunning = IsRunning();
		Vector3 moveDirection = new Vector3(_targetX, 0f, _targetY);

		// Handle animations
		UpdateMovementTargets(isRunning);
		UpdateAnimations(isRunning);

		// Handle jumping
		HandleJumping();

		// Boost jump
		CheckBoostJump();

		// Handle movement
		if (!_onIce)
			HandleMovement(isRunning, moveDirection);
		else
			ApplyIceForces(isRunning, moveDirection);
		
		// Apply smooth rotation towards movement direction
		ApplySmoothRotation(moveDirection);
	}

	// ---------------------------------------------
	// Animaciones y movimiento
	// ---------------------------------------------

	private bool IsRunning()
	{
		return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
	}

	private void UpdateMovementTargets(bool isRunning)
	{
		// Reset target values
		_targetX = 0f;
		_targetY = 0f;

		// WASD input handling
		if (Input.GetKey(KeyCode.W))
			_targetY = isRunning ? 1f : 0.5f;
		else if (Input.GetKey(KeyCode.S))
			_targetY = isRunning ? -1f : -0.5f;

		if (Input.GetKey(KeyCode.A))
			_targetX = isRunning ? -1f : -0.5f;
		else if (Input.GetKey(KeyCode.D))
			_targetX = isRunning ? 1f : 0.5f;
	}

	private void UpdateAnimations(bool isRunning)
	{
		float currentSmoothing = isRunning ? _runningSmoothing : _smoothing;

		// Smoothly transition current value towards target value
		float targetSpeed = Mathf.Max(Mathf.Abs(_targetX), Mathf.Abs(_targetY));
		_currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, currentSmoothing);

		if (_currentSpeed < 0.001f)
			_currentSpeed = 0f;

		// Update animator parameters
		_animator.SetFloat("Speed", _currentSpeed);
	}

	private void HandleMovement(bool isRunning, Vector3 moveDirection)
	{
		if (moveDirection.magnitude < 0.01f) return;

		float speedMultiplier = isRunning ? 1f : 0.5f;
		Vector3 velocity = _moveSpeed * speedMultiplier * moveDirection.normalized;

		_rb.MovePosition(_rb.position + velocity * Time.fixedDeltaTime);
	}

	private void ApplySmoothRotation(Vector3 moveDirection)
	{
		if (moveDirection.magnitude < 0.01f) return;

		// Smooth rotation
		Quaternion targetRotation = Quaternion.LookRotation(moveDirection.normalized, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
	}
	
	private void ApplyIceForces(bool isRunning, Vector3 moveDirection)
	{
		float forceMultiplier = isRunning ? 1f : 0.5f;
		float forceAmount = forceMultiplier * 10f * Time.fixedDeltaTime;

		if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
			_rb.AddForce(forceAmount * moveDirection, ForceMode.VelocityChange);
	}

	// ---------------------------------------------
	// Salto
	// ---------------------------------------------

	private void CheckJump()
	{
		if (_canJump && Input.GetKeyDown(KeyCode.Space) && Time.time - _lastLandTime >= _jumpCooldown)
			_jump = true;
	}

	private void HandleJumping()
	{
		if (_jump)
		{
			_canJump = false;
			_rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
			_animator.SetTrigger("Jump");
			_animator.SetBool("Grounded", false);
			_jump = false;
		}
	}

	// ---------------------------------------------
	// Ground check
	// ---------------------------------------------

	private void CheckGrounded()
	{
		Vector3 origin = transform.position + 0.1f * Vector3.up;
		float offset = 0.2f;
		bool isGroundedNow = IsGroundedRaycasts(origin, offset);

		if (isGroundedNow && !_canJump)
			OnLand();
		else if (!isGroundedNow && _canJump)
			InTheAir();
	}

	private bool IsGroundedRaycasts(Vector3 origin, float offset)
	{
		return Physics.Raycast(origin, Vector3.down, _groundCheckDistance) ||
			   Physics.Raycast(origin + offset * Vector3.forward, Vector3.down, _groundCheckDistance) ||
			   Physics.Raycast(origin + offset * Vector3.right, Vector3.down, _groundCheckDistance) ||
			   Physics.Raycast(origin + offset * Vector3.back, Vector3.down, _groundCheckDistance) ||
			   Physics.Raycast(origin + offset * Vector3.left, Vector3.down, _groundCheckDistance);
	}

	private void OnLand()
	{
		_canJump = true;
		_animator.SetBool("Grounded", true);
		_lastLandTime = Time.time;
	}

	private void InTheAir()
	{
		_canJump = false;
		_animator.SetBool("Grounded", false);
	}

	// ---------------------------------------------
	// Físicas del suelo
	// ---------------------------------------------

	public void AlterPhysics(Floor.FloorType floor)
	{
		switch (floor)
		{
			case Floor.FloorType.Slippery:
				_onIce = true;
				break;

			case Floor.FloorType.Sticky:
				_jumpCooldown = 0.5f;
				_moveSpeed = _defaultMoveSpeed * 0.6f;
				_jumpForce = _defaultJumpForce * 0.6f;
				break;

			case Floor.FloorType.Trampoline:
				_boostJump = true;
				break;

			// Reset values
			default:
				ResetPhysics();
				break;
		}
	}

	private void ResetPhysics()
	{
		_jumpCooldown = 0.25f;
		_moveSpeed = _defaultMoveSpeed;
		_jumpForce = _defaultJumpForce;
		_boostJump = false;
		_onIce = false;

		// Parar aceleración en saltos sobre hielo
		if (!_acceleratedIceJump)
		{
			_rb.velocity = new Vector3(0f, _rb.velocity.y, 0f);
			_rb.angularVelocity = Vector3.zero;
		}
	}

	private void CheckBoostJump()
	{
		if (_boostJump)
		{
			_rb.AddForce(_jumpForce * 2.5f * Vector3.up, ForceMode.Impulse);
			_boostJump = false;
		}
	}

	// ---------------------------------------------
	// Reinicio de nivel
	// ---------------------------------------------

	public void CheckRestart()
	{
		if (transform.position.y < _restartHeight)
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
