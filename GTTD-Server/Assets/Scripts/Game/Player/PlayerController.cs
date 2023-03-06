using UnityEngine;
using Vulf.GTTD.Game.Input;

namespace Vulf.GTTD.Game.Player
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour
	{
		#region Properties
		// Components
		public InputSource Input { get; set; }
		public Rigidbody2D Rigidbody { get; private set; }
		public Collider2D Collider { get; private set; }

		// States
		public bool IsAccelerating => Input.MovementAxis != Vector2.zero;

		// Movement
		float AccelerationAmount => 1f / accelerationTime;
		float DecelerationAmount => 1f / decelerationTime;
		#endregion

		#region Settings
		[Header("Movement Settings")]
		[SerializeField, Tooltip("The movement speed of the player in m/s")]
		float maxSpeed;
		[SerializeField, Tooltip("The time (in frames) it takes for the player to reach max speed from 0 m/s")]
		int accelerationTime;
		[SerializeField, Tooltip("The time (in frames) it takes for the player to reach 0 m/s from max speed")]
		int decelerationTime;
		#endregion

		#region Fields
		// Movement
		Vector2 movementDirection;
		float accelerationFactor;
		#endregion

		#region Private Methods
		void Awake()
		{
			// Get components from the player object
			Rigidbody = GetComponent<Rigidbody2D>();
			Collider = GetComponent<Collider2D>();
		}

		void Start()
		{
			Input = new InputSource();
		}

		void FixedUpdate()
		{
			CalculateMovement();
		}

		void CalculateMovement()
		{
			// Update the player's movement direction if they are actively moving
			if (IsAccelerating)
				movementDirection = Input.MovementAxis;

			// Calculate the acceleration amount
			if (IsAccelerating)
				accelerationFactor += AccelerationAmount;
			else
				accelerationFactor -= DecelerationAmount;

			// Clamp the acceleration factor between 0 and 1
			accelerationFactor = Mathf.Clamp01(accelerationFactor);

			// Calculate the movement speed from the player's current acceleration factor
			float movementSpeed = maxSpeed * accelerationFactor;

			// Set the velocity of the rigidbody
			Rigidbody.velocity = movementSpeed * movementDirection;
		}
		#endregion
	}
}