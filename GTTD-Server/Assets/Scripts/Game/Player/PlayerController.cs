using UnityEngine;
using Vulf.GTTD.Game.Input;

namespace Vulf.GTTD.Game.Player
{
	[RequireComponent(typeof(Rigidbody2D))]
	public class PlayerController : MonoBehaviour, IDamageable
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

		// Health
		public int Health { get; private set; } = maxHealth;

		// Weapon
		public int CurrentAmmo { get; private set; } = maxAmmo;
		public bool OnCooldown => cooldownTimer <= fireRate;
		public bool CanFire => !OnCooldown && CurrentAmmo > 0;
		public bool CanReload => !OnCooldown && CurrentAmmo < maxAmmo;
		public bool IsReloading { get; private set; }
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

		[SerializeField] LayerMask bulletMask;

		// Health
		const int maxHealth = 150;

		// Weapon
		const int maxAmmo = 20;
		const int fireRate = 10;
		const int reloadTime = 75;
		const int spreadAngle = 10;
		const int damage = 25;

		int cooldownTimer = fireRate;
		int reloadTimer = 0;

		#endregion

		#region Private Methods
		void Awake()
		{
			// Get components from the player object
			Rigidbody = GetComponent<Rigidbody2D>();
			Collider = GetComponent<Collider2D>();
			Input = new InputSource();
		}

		void Start()
		{
			
		}

		void FixedUpdate()
		{
			CalculateMovement();
			UpdateWeapon();
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

		void UpdateWeapon()
		{
			// Increment the cooldown timer
			cooldownTimer++;

			// Fire on player input
			if (CanFire && Input.FirePressed)
				Fire();

			// Check if the player should reload
			if (CanReload && (CurrentAmmo == 0 || Input.ReloadPressed))
				IsReloading = true;

			// Cancel reload if not allowed
			if (IsReloading && !CanReload)
				IsReloading = false;

			// Increase the reload timer while reloading, otherwise reset the reload timer
			if (IsReloading)
				reloadTimer++;
			else
				reloadTimer = 0;

			// On reload complete, reset the reload timer and add ammo to the weapon
			if (reloadTimer >= reloadTime)
			{
				CurrentAmmo = maxAmmo;
				reloadTimer = 0;
			}
		}

		void Fire()
		{
			CurrentAmmo--;
			cooldownTimer = 0;

			// Calculate fire direction
			float fireAngle = Vector2.SignedAngle(Vector2.right, Input.AimDirection);
			fireAngle += Random.Range(-spreadAngle * 0.5f, spreadAngle * 0.5f);
			Vector2 fireDirection = new Vector2(Mathf.Cos(fireAngle * Mathf.Deg2Rad), Mathf.Sin(fireAngle * Mathf.Deg2Rad));

			Debug.DrawRay(transform.position + ((Vector3)Input.AimDirection * 0.55f), fireDirection * 50, Color.red, 0.1f);

			RaycastHit2D hit = Physics2D.Raycast(transform.position + ((Vector3)Input.AimDirection * 0.55f), fireDirection, 50f, bulletMask);

			if (hit.collider != null)
			{
				hit.collider.gameObject.TryGetComponent<IDamageable>(out IDamageable target);
				target.Damage(damage);
			}
		}
		#endregion

		public void Damage(float amount)
		{
			Health -= Mathf.CeilToInt(amount);
			Health = Mathf.Max(Health, 0);
		}
	}
}