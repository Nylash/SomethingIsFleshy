using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
#pragma warning disable 0649
	[Header("Movement")]
	[Header("CONFIGURATION")]
	[SerializeField] float movementSpeed = 10f;
	[Tooltip("Used for acceleration. Bigger is the value, longer the character make to stop or start moving.")]
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
	[Header("Jump")]
	[Tooltip("The first impulsion given to the player when he hits jump button.")]
	[SerializeField] float initialJumpForce = 20f;
	[Tooltip("Total time where player can keep jumping.")]
	[SerializeField] float jumpTimeMax = .4f;
	[Tooltip("Time during which the player 'levitate' after reaching jump's apex.")]
	[SerializeField] float levitatingTimeAtApex = .15f;
	[Tooltip("Percentage at jump's apex at which player loose all velocity from jump and start falling.")]
	[Range(0, 1)] [SerializeField] float percentageGravityHandleFall = .5f;
	[Tooltip("Gravity divider for levitate effect. Lower is the value quicker the player fall.")]
	[Range(1, 10)] [SerializeField] float gravityLevitateRatio = 3f;
	[Tooltip("Horizontal speed during jump, should be equal or inferior at movementSpeed.")]
	[SerializeField] float jumpControlMovementSpeed = 10f;
	[Tooltip("Speed during air control (when not jumping), should be equal or inferior at jumpControlMovementSpeed.")]
	[SerializeField] float fallControlMovementSpeed = 5f;
	[Header("Ground Detection")]
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] Transform groundCheck;
	[SerializeField] float groundedRadius = .2f;
#pragma warning restore 0649

	[Header("COMPONENTS")]
	Rigidbody2D rb;
	ActionsMap actionsMap;

	[Header("VARIABLES")]
	//Movement variable
	bool isGrounded;
	bool facingRight = true;
	float movementInput;
	Vector3 velocity = Vector3.zero;
	//Jump variable
	bool isJumping;
	bool isLevitating;
	float jumpTimeCounter;
	float levitatingTimeCounter;
	float initialGravity;
	float velocityAtApex;
	//Debug variable
	Color debugColor = Color.cyan;

	private void OnEnable() => actionsMap.Gameplay.Enable();
	private void OnDisable() => actionsMap.Gameplay.Disable();

	private void Awake()
	{
		rb = GetComponent<Rigidbody2D>();
		initialGravity = rb.gravityScale;

		actionsMap = new ActionsMap();

		actionsMap.Gameplay.Movement.performed += ctx => movementInput = ctx.ReadValue<float>();
		actionsMap.Gameplay.Movement.canceled += ctx => movementInput = 0f;
		actionsMap.Gameplay.Jump.started += ctx => StartJumping();
		actionsMap.Gameplay.Jump.canceled += ctx => StopJumping();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = isGrounded;
		isGrounded = false;

		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, groundedRadius, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
				if (!wasGrounded)
				{
					//Landing
				}
			}
		}

		Move(movementInput);

		KeepJumping();
	}

	void StartJumping()
	{
		if (isGrounded)
		{
			isGrounded = false;
			isJumping = true;
			jumpTimeCounter = 0f;
			rb.velocity = new Vector2(rb.velocity.x,initialJumpForce);
		}
	}

	void KeepJumping()
	{
		if (isJumping)
		{
			if (jumpTimeCounter < jumpTimeMax)
			{
				jumpTimeCounter += Time.deltaTime;
				rb.velocity = Vector2.Lerp(new Vector2(rb.velocity.x, initialJumpForce), new Vector2(rb.velocity.x, initialJumpForce/2), jumpTimeCounter / jumpTimeMax);
			}
			else
				StartCoroutine(LevitateAtApexJump());
		}
		else if (isLevitating)
		{
			levitatingTimeCounter += Time.deltaTime;
			rb.velocity = Vector2.Lerp(new Vector2(rb.velocity.x, velocityAtApex), new Vector2(rb.velocity.x,  0f), levitatingTimeCounter / (levitatingTimeAtApex * percentageGravityHandleFall));
			if (rb.velocity.y == 0)
				isLevitating = false;
		}

		Debug.DrawLine(transform.position, transform.position - new Vector3(0, -.1f, 0), debugColor, 10);
	}

	void StopJumping()
	{
		isJumping = false;
		isLevitating = false;
		rb.gravityScale = initialGravity;
		StopCoroutine(LevitateAtApexJump());
		debugColor = Color.cyan;
	}

	IEnumerator LevitateAtApexJump()
	{
		isJumping = false;
		isLevitating = true;
		levitatingTimeCounter = 0f;
		velocityAtApex = rb.velocity.y;
		rb.gravityScale = initialGravity / gravityLevitateRatio;
		debugColor = Color.green;
		yield return new WaitForSeconds(levitatingTimeAtApex);
		rb.gravityScale = initialGravity;
		isLevitating = false;
		debugColor = Color.cyan;
	}

	public void Move(float horizontalMove)
	{
		if (isGrounded)
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * movementSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip();
			else if (horizontalMove < 0 && facingRight)
				Flip();
		}
		else if (isJumping)
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * jumpControlMovementSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip();
			else if (horizontalMove < 0 && facingRight)
				Flip();
		}
		else
		{
			Vector3 targetVelocity = new Vector2(horizontalMove * fallControlMovementSpeed, rb.velocity.y);
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
