using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterController2D : MonoBehaviour
{
	#region CONFIGURATION
#pragma warning disable 0649
	[Header("Movement")]
	[SerializeField] float movementSpeed = 10f;
	[Tooltip("Used for acceleration. Bigger is the value, longer the character make to stop or start moving.")]
	[Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
	[Header("Jump")]
	[Tooltip("The horizontal impulsion given to player when he hits jump button holding a direction.")]
	[SerializeField] float initialXJumpForce = 3;
	[Tooltip("The vertical impulsion given to the player when he hits jump button.")]
	[SerializeField] float initialYJumpForce = 15;
	[Tooltip("The force add to velocity.y at each frame if the player keeps jumping.")]
	[SerializeField] float incrementYJumpForce = 2;
	[Tooltip("The value modifying velocity.x each frame when the player is in air. Greater it is greater the air control is.")]
	[SerializeField] float airControl = .7f;
	[Tooltip("Maximum horizontal speed in air, if this value is greater than 'movementSpeed' the player will move faster in air.")]
	[SerializeField] float maxXSpeedInAir = 10f;
	[Tooltip("Total time where player can keep jumping.")]
	[SerializeField] float jumpTimeMax = .2f;
	[Header("Jump polish")]
	[Tooltip("How many frames player can still jump after leaving the ground (Coyote time).")]
	[SerializeField] int nbFramesCoyoteTime = 5;
	[Tooltip("Hom many frames jump input is stocked when the player is not grounded.")]
	[SerializeField] int nbFramesJumpBuffering = 10;
	[Header("Ground Detection")]
	[SerializeField] LayerMask whatIsGround;
	[SerializeField] Transform groundCheck;
	[SerializeField] Vector2 groundCheckSize = new Vector2(1f, .25f);
	[Header("On nerve parameters")]
	[SerializeField] float invicibilityTime = .5f;
	[Range(0.01f, .25f)] [SerializeField] float animationSpeed;
	[SerializeField] SpriteRenderer[] whatIsBlinking;
	[Header("Debug")]
	[SerializeField] bool showCheckerDebug = true;
	[SerializeField] bool showMovementDebug = true;
	[SerializeField] Color groundedColor = Color.cyan;
	[SerializeField] Color jumpColor = Color.yellow;
	[SerializeField] Color fallColor = Color.red;
#pragma warning restore 0649
	#endregion

	[Header("Components")]
	[Header("⚠ DON'T TOUCH BELOW ⚠")]
	public Animator animator;
	public Animator animatorFace;
	public Rigidbody2D rb;
	public ParticleSystem walkFX;
	public GameObject JLFXspot;
	public GameObject JLFX;
	public AudioSource jumpLandingSource;
	InteractionManager interactionManager;

	[Header("Variables")]
	//Movement variables
	public GameObject associatedMovingPlatform;
	public int playerNumber;
	bool isGrounded;
	bool wasGrounded;
	bool facingRight = true;
	float movementInput;
	public float jumpPadForce;
	Vector3 velocity = Vector3.zero;
	//Jump variables
	bool isJumping;
	float jumpTimeCounter;
	//Polish jump variables
	bool jumpBuffering;
	int framesCounterCoyoteTime;
	int framesCounterJumpBuffering;
	//Anim variables
	bool isOnNerve;
	//Pause variables
	float stockGravity;
	//Debug variables
	Color debugColor;

	private void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		interactionManager = GetComponent<InteractionManager>();
	}

	public void AskMovement(InputAction.CallbackContext ctx)
    {
		if (ctx.performed)
			movementInput = ctx.ReadValue<float>();
		else if(ctx.canceled)
			movementInput = 0f;
	}

	public void AskJump(InputAction.CallbackContext ctx)
    {
		if (ctx.started)
			StartJumping();
		else if (ctx.canceled)
			StopJumping();
    }

	public void AskPause()
    {
		UI_Manager.instance.Pause();
    }

	private void FixedUpdate()
	{
		if (GameManager.instance.levelStarted)
		{
			if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused && AnimationNotCurrentlyBlocking())
			{
				if (rb.bodyType != RigidbodyType2D.Dynamic)
					rb.bodyType = RigidbodyType2D.Dynamic;
				GroundDetection();
				CoyoteTimeSystem();
				JumpBufferingSystem();
				Move(movementInput);
				KeepJumping();
				if (showMovementDebug)
					Debug.DrawLine(transform.position, transform.position - new Vector3(0, -.1f, 0), debugColor, 10);
			}
            else if(!AnimationNotCurrentlyBlocking())
            {
				if (rb.bodyType != RigidbodyType2D.Dynamic)
					rb.bodyType = RigidbodyType2D.Dynamic;
				rb.velocity = Vector2.zero;
			}
            else
            {
				if (rb.bodyType != RigidbodyType2D.Static)
					rb.bodyType = RigidbodyType2D.Static;
				if (animator.GetBool("Running"))
					animator.SetBool("Running", false);
            }				
		}
	}

	void GroundDetection()
	{
		wasGrounded = isGrounded;
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapBoxAll(groundCheck.position, groundCheckSize, 0, whatIsGround);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
				if (!wasGrounded)
				{
					walkFX.Play();
					animator.SetBool("Landing",true);
					Instantiate(JLFX, JLFXspot.transform.position, JLFX.transform.rotation);
				}
			}
		}
		if (isGrounded)
			debugColor = groundedColor;
		else
		{
			debugColor = fallColor;
			walkFX.Stop();
			animator.SetBool("Landing", false);
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
		if (GameManager.instance.levelStarted)
		{
			if (!ScoreManager.instance.levelEnded && !GameManager.instance.levelPaused && AnimationNotCurrentlyBlocking())
			{
				if (isGrounded)
				{
					animator.speed = 1;
					animator.SetTrigger("Jump");
					jumpBuffering = false;
					isGrounded = false;
					isJumping = true;
					framesCounterCoyoteTime = 0;
					jumpTimeCounter = 0f;
					if (jumpPadForce != 0)
						SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.SuperJump, jumpLandingSource);
					else
						SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.Jump, jumpLandingSource);
					if (movementInput != 0)
						rb.velocity = new Vector2((facingRight ? 1 : -1) * initialXJumpForce, initialYJumpForce + jumpPadForce);
					else
						rb.velocity = new Vector2(0f, initialYJumpForce + jumpPadForce);
					debugColor = jumpColor;
					Instantiate(JLFX, JLFXspot.transform.position, JLFX.transform.rotation);
				}
				else
				{
					jumpBuffering = true;
					framesCounterJumpBuffering = 0;
				}
			}
		}
	}

	void KeepJumping()
	{
		if (isJumping)
		{
			if (jumpTimeCounter < jumpTimeMax)
			{
				jumpTimeCounter += Time.deltaTime;
				rb.velocity += new Vector2(0f, incrementYJumpForce);
				debugColor = jumpColor;
			}
			else
				StopJumping();
		}
	}

	void StopJumping()
	{
		jumpBuffering = false;
		isJumping = false;
		debugColor = fallColor;
	}

	void Move(float horizontalMove)
	{
		if (isGrounded)
		{
			if (associatedMovingPlatform)
			{
				if (horizontalMove == 0)
				{
					if (transform.parent != associatedMovingPlatform.transform)
						transform.parent = associatedMovingPlatform.transform;
				}
				else
					transform.parent = null;
			}
			else
			{
				if (transform.parent != null)
					transform.parent = null;
			}

			Vector3 targetVelocity = new Vector2(horizontalMove * movementSpeed, rb.velocity.y);
			rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref velocity, movementSmoothing);

			if (horizontalMove > 0 && !facingRight)
				Flip(false);
			else if (horizontalMove < 0 && facingRight)
				Flip(false);

			if(horizontalMove != 0f)
			{
				animator.SetBool("Running", true);
				animator.speed = Mathf.Lerp(.5f, 1f, Mathf.Abs(horizontalMove));
			}
			else
			{
				animator.SetBool("Running", false);
				animator.speed = 1;
			}
		}
		else
		{
			animator.SetBool("Running", false);
			animator.speed = 1;
			if (horizontalMove > 0 && !facingRight)
				Flip(true);
			else if (horizontalMove < 0 && facingRight)
				Flip(true);
			if(horizontalMove != 0)
			{
				rb.velocity += new Vector2((facingRight ? 1 : -1) * airControl, 0f);
				if (Mathf.Abs(rb.velocity.x) > maxXSpeedInAir)
					rb.velocity = new Vector2((facingRight ? 1 : -1) * maxXSpeedInAir, rb.velocity.y);
			}
			else
			{
				if(Mathf.Abs(rb.velocity.x) > airControl)
				{
					rb.velocity -= new Vector2((facingRight ? 1 : -1) * airControl, 0f);
				}
			}
		}
	}

	private void Flip(bool callInAir)
	{
		facingRight = !facingRight;

		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
		if (callInAir)
			rb.velocity = new Vector2(0f, rb.velocity.y);
	}

	private void OnDrawGizmos()
	{
		if (showCheckerDebug)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
		}
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		switch (collision.tag)
		{
			case "MovingPlatform":
				associatedMovingPlatform = collision.gameObject;
				transform.parent = associatedMovingPlatform.transform;
				break;
			case "JumpPad":
				jumpPadForce = collision.gameObject.GetComponent<JumpPad>().jumpPadForce;
				break;
			default:
				break;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "ElectricPlatform":
				animator.SetTrigger("StartShocked");
				animator.SetBool("Shocked", true);
				
				break;
			case "Nerve":
				if (!isOnNerve)
				{
					ScoreManager.instance.LosePoints(GameManager.instance.pointsLossNerveHit);
					collision.gameObject.GetComponent<Animator>().SetTrigger("Hit");
					isOnNerve = true;
					StartCoroutine(OnNerveAnimation());
					SoundsManager.instance.PlaySoundOneShot(SoundsManager.SoundName.NerveHit, interactionManager.interactionSource);
				}
				break;
			default:
				break;
		}
	}

    private void OnTriggerExit2D(Collider2D collision)
	{
		switch (collision.gameObject.tag)
		{
			case "MovingPlatform":
				associatedMovingPlatform = null;
				transform.parent = null;
				break;
			case "JumpPad":
				jumpPadForce = 0;
				break;
			default:
				break;
		}
	}

	IEnumerator OnNerveAnimation()
	{
		float timer = 0f;
		while(timer < invicibilityTime)
		{
			foreach (SpriteRenderer item in whatIsBlinking)
			{
				if (item.color.a == 1)
					item.color = new Color(item.color.r, item.color.g, item.color.b, .25f);
				else
					item.color = new Color(item.color.r, item.color.g, item.color.b, 1);
			}
			yield return new WaitForSeconds(animationSpeed);
			timer += animationSpeed;
		}
		foreach(SpriteRenderer item in whatIsBlinking)
				item.color = new Color(item.color.r, item.color.g, item.color.b, 1);
		isOnNerve = false;
	}

	public bool AnimationNotCurrentlyBlocking()
	{
		return !animator.GetBool("Interacting") && !animator.GetBool("Teleporting") && !animator.GetBool("Shocked") && !animator.GetBool("Holding");
	}
}
