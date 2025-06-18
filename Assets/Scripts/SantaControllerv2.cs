using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class SantaControllerv2 : MonoBehaviour
{
	[Header("Setup")]
	[SerializeField]
	private float _smoothing = 0.06f; // Smoothing factor for transitions
	[SerializeField]
	private float _runningSmoothing = 0.08f; // Smoothing factor for running transitions
	[SerializeField]
	private float _moveSpeed = 5f;
	[SerializeField]
	private float _jumpForce = 5f;
	[SerializeField]
	private float _jumpCooldown = 0.5f;

	private Animator _animator;
	private Rigidbody _rb;

	// Target values for x and y parameters
	private float _targetX = 0f;
	private float _targetY = 0f;

	// Current value
	private float _currentSpeed = 0f;

	private bool _canJump, _firstTimeJumping, _jump;

	private float _lastLandTime = -Mathf.Infinity;

	void Awake()
	{
		_animator = GetComponent<Animator>();
		_rb = GetComponent<Rigidbody>();
		_canJump = true;
		_firstTimeJumping = true;
		_jump = false;
	}

	void Update()
	{
		bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

		float currentSmoothing = isRunning ? _runningSmoothing : _smoothing;

		// Handle animations
		HandleMovementAnimations(isRunning, currentSmoothing);

		// Check if jump requested
		CheckJump();

		// Handle movement
		HandleMovement(isRunning);
	}

	void FixedUpdate()
	{
		// Handle jumping
		HandleJumping();
	}

	private void HandleMovementAnimations(bool isRunning, float currentSmoothing)
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

		// Smoothly transition current value towards target value
		float targetSpeed = Mathf.Max(Mathf.Abs(_targetX), Mathf.Abs(_targetY));
		_currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, currentSmoothing);
		
		if (_currentSpeed < 0.001f)
			_currentSpeed = 0f;

		// Update animator parameters
		_animator.SetFloat("Speed", _currentSpeed);
	}

	private void HandleMovement(bool isRunning)
	{
		Vector3 moveDir = new Vector3(_targetX, 0f, _targetY);
		
		if (moveDir.magnitude < 0.01f) return;

		float speedMultiplier = isRunning ? 1f : 0.5f;
		Vector3 velocity = _moveSpeed * speedMultiplier * moveDir.normalized;

		transform.Translate(velocity * Time.deltaTime, Space.World);

		// Smooth rotation
		Quaternion targetRotation = Quaternion.LookRotation(moveDir.normalized, Vector3.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
	}

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

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground"))
		{
			_canJump = true;
			_animator.SetBool("Grounded", true);
			if (!_firstTimeJumping)
				_lastLandTime = Time.time;
			else
				_firstTimeJumping = false;
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Ground") && _canJump)
		{
			_canJump = false;
			_animator.SetTrigger("Fall");
			_animator.SetBool("Grounded", false);
		}
	}
}
