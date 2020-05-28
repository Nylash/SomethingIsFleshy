using UnityEngine;
using System.Collections;

public class CharacterController2D : MonoBehaviour
{
    #region CONFIGURATION
#pragma warning disable 0649
    [Header("Movement")]
	[SerializeField] float movementSpeed = 10f;
	[Tooltip("Used for acceleration. Bigger is the value, longer the character make to stop or start moving.")]
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
	[Header("Jump")]
	[Tooltip("The first impulsion given to the player when he hits jump button.")]
	[SerializeField] float initialJumpForce = 20f;
	[Tooltip("Total time where player can keep jumping.")]
	[SerializeField] float jumpTimeMax = .35f;
	[Tooltip("Percentage of jump force used at the end on the jump's duration. A lerp is done during the jump, reducing the jump force to jumpForce to jumpForce * jumpSlowDownRatio. Lower is this value, stronger is the slow.")]
	[Range(0, 1)] [SerializeField] float jumpSlowDownRatio =.5f;
	[Tooltip("Time during which the player 'levitate' after reaching jump's apex.")]
	[SerializeField] float levitatingTimeAtApex = .15f;
	[Tooltip("Percentage at jump's apex at which player loose all velocity from jump and start falling.")]
	[Range(0, 1)] [SerializeField] float percentageGravityHandleFall = .5f;
	[Tooltip("Gravity ratio multiplier for levitate effect. 1 = initial gravity, 0 = no gravity.")]
	[Range(0, 1)] [SerializeField] float gravityLevitateRatio = .33f;
	[Tooltip("Horizontal speed during jump, should be equal or inferior at movementSpeed. Otherwise, player will move faster by jumping.")]
	[SerializeField] float jumpControlMovementSpeed = 10f;
	[Tooltip("Speed during air control (when not jumping), should be equal or inferior at jumpControlMovementSpeed.")]
	[SerializeField] float fallControlMovementSpeed = 7.5f;
	[Header("Jump polish")]
	[Tooltip("How many frames player can still jump after leaving the ground (Coyote time).")]
	[SerializeField] int nbFramesCoyoteTime = 5;
	[Tooltip("Hom many frames jump input is stocked when the player is not grounded.")]
	[SerializeField] int nbFramesJumpBuffering = 5;
	[Header("Ground Detection")]
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] Transform groundCheck;
	[SerializeField] float groundedRadius = .2f;
	[Header("Debug")]
	[SerializeField] bool showMovementDebug;
	[SerializeField] Color movementColor = Color.cyan;
	[SerializeField] Color jumpColor = Color.yellow;
	[SerializeField] Color apexJumpColor = Color.green;
#pragma warning restore 0649
    #endregion
	
    [Header("Components")]
	[Header("DON'T TOUCH BELOW")]
	Rigidbody2D rb;
	ActionsMap actionsMap;

	[Header("Variables")]
	//Movement variable
	bool isGrounded;
	bool wasGrounded;
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
	//Polish jump variable
	bool jumpBuffering;
	int framesCounterCoyoteTime;
	int framesCounterJumpBuffering;
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
		GroundDetection();
		CoyoteTimeSystem();
		JumpBufferingSystem();
		Move(movementInput);
		KeepJumping();
		if(showMovementDebug)
			Debug.DrawLine(transform.position, transform.position - new Vector3(0, -.1f, 0), debugColor, 10);
	}

	void GroundDetection()
	{
		wasGrounded = isGrounded;
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
	}

	void CoyoteTimeSystem()
	{
		if (!isGrounded && wasGrounded)
		{

			framesCounterCoyoteTime++;
			if (framesCounterCoyoteTime <= nbFramesCoyoteTime)
				isGrounded = true;
			else
				framesCounterCoyoteTime = 0;
		}
	}

	void JumpBufferingSystem()
	{
		if (isGrounded && jumpBuffering)
		{
			StartJumping();
		}
		if (jumpBuffering)
		{
			framesCounterJumpBuffering++;
			if (framesCounterJumpBuffering > nbFramesJumpBuffering)
				jumpBuffering = false;
		}
	}

    void StartJumping()
	{
		if (isGrounded)
		{
			jumpBuffering = false;
			isGrounded = false;
			isJumping = true;
			framesCounterCoyoteTime = 0;
			jumpTimeCounter = 0f;
			rb.velocity = new Vector2(rb.velocity.x,initialJumpForce);
		}
		else
		{
			jumpBuffering = true;
			framesCounterJumpBuffering = 0;
		}
	}

	void KeepJumping()
	{
		if (isJumping)
		{
			if (jumpTimeCounter < jumpTimeMax)
			{
				jumpTimeCounter += Time.deltaTime;
				rb.velocity = Vector2.Lerp(new Vector2(rb.velocity.x, initialJumpForce), new Vector2(rb.velocity.x, initialJumpForce * jumpSlowDownRatio), jumpTimeCounter / jumpTimeMax);
				debugColor = jumpColor;
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
	}

	void StopJumping()
	{
		jumpBuffering = false;
		isJumping = false;
		isLevitating = false;
		rb.gravityScale = initialGravity;
		StopCoroutine(LevitateAtApexJump());
		debugColor = movementColor;
	}

	IEnumerator LevitateAtApexJump()
	{
		isJumping = false;
		isLevitating = true;
		levitatingTimeCounter = 0f;
		velocityAtApex = rb.velocity.y;
		rb.gravityScale = initialGravity * gravityLevitateRatio;
		debugColor = apexJumpColor;
		yield return new WaitForSeconds(levitatingTimeAtApex);
		rb.gravityScale = initialGravity;
		isLevitating = false;
		debugColor = movementColor;
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
