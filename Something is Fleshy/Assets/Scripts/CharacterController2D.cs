using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
#pragma warning disable 0649
	[Header("CONFIGURATION")]
	[SerializeField] private float movementSpeed = 10f;
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
	[SerializeField] private bool airControl = false;
	[SerializeField] private float airControlSpeed = 5f;
	[SerializeField] private LayerMask whatIsGround;
	[SerializeField] private Transform groundCheck;
	[SerializeField] private float groundedRadius = .2f;
#pragma warning restore 0649

	[Header("COMPONENTS")]
	public Rigidbody2D rb;
	ActionsMap actionsMap;

	[Header("VARIABLES")]
	public bool grounded;
	bool facingRight = true;
	Vector3 velocity = Vector3.zero;
	float movementInput;

	private void OnEnable() => actionsMap.Gameplay.Enable();
	private void OnDisable() => actionsMap.Gameplay.Disable();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();

		actionsMap = new ActionsMap();

		actionsMap.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<float>();
		actionsMap.Gameplay.Movement.canceled += ctx => movementInput = 0f;
	}

	private void FixedUpdate()
	{
		bool wasGrounded = grounded;
		grounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				grounded = true;
				if (!wasGrounded)
				{
					//Landing
				}
			}
		}
		if (!grounded)
		{
			//inAir
		}
		Move(movementInput);
	}

	public void Move(float horizontalMove)
	{
		if (grounded)
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * movementSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip();
			else if (horizontalMove < 0 && facingRight)
				Flip();
		}
		else if (airControl)
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * airControlSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip();
			else if (horizontalMove < 0 && facingRight)
				Flip();
		}
	}

	private void Flip()
	{
		facingRight = !facingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(groundCheck.position, groundedRadius);
	}
}
