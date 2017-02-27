using UnityEngine;
using System.Collections;

public class JumpEquationTester : MonoBehaviour 
{
	public float maxJumpHeight;
	public float minJumpHeight;
	public float jumpDistance_BeforeApex;
	public float jumpDistance_AfterApex;
	public float playerSpeed;

	private bool isJumping;
	private bool jumpReleasedEarly;

	private float gravity;

	private Vector2 velocity;
	private float initialVelocity;

	private float initialPositionY;

	public bool canDoubleJump;
	public float doubleJumpHeight;
	public float doubleJumpDistance_BeforeApex;
	public float doubleJumpDistance_AfterApex;
	private bool hasDoubleJumped;

	void Start()
	{
		isJumping = false;
		velocity = Vector2.zero;

		jumpReleasedEarly = false;
	}

	void Update() //State and input stuff...
	{
		//Movement...
		velocity.x = Input.GetAxisRaw("Horizontal") * playerSpeed;

		if (Input.GetKeyDown(KeyCode.Space)) //Initiating a jump...
		{
			if (!isJumping) 
			{
				isJumping = true;
				initialPositionY = transform.position.y;

				//Set velocity to initial velocity...
				initialVelocity = (2.0f * maxJumpHeight * playerSpeed) / jumpDistance_BeforeApex;
				velocity.y = initialVelocity;
			} 
			else if (canDoubleJump && !hasDoubleJumped) 
			{
				hasDoubleJumped = true;
				jumpReleasedEarly = false; //Reset this so that early release gravity isn't applied.

				//Set a new initial velocity and apply it.
				initialVelocity = (2.0f * doubleJumpHeight * playerSpeed) / doubleJumpDistance_BeforeApex;
				velocity.y = initialVelocity;
			}
		}
		if (Input.GetKeyUp (KeyCode.Space) && isJumping && velocity.y > 0) // && !jumpReleasedEarly && velocity.y > 0) //Releasing jump key during a jump...
		{
			jumpReleasedEarly = true;
		}
	}

	void FixedUpdate() //Physics stuff...
	{
		AssignGravity ();

		//Keep the jumper from falling off-screen...
		if (transform.position.y + velocity.y <= -2.0f)
		{
			transform.position = new Vector3 (transform.position.x, -2.0f, transform.position.z);
			if(velocity.y < 0)
			{
				velocity.y = 0.0f;

				//Reset jumping state stuff when we hit the "ground".
				isJumping = false;
				jumpReleasedEarly = false;
				hasDoubleJumped = false;
			}
		}

		//Apply velocity...
		velocity.y += gravity; ///Apply gravity.
		transform.position += (Vector3)velocity;// * Time.deltaTime;
	}

	//Assigns a value to gravity based on the state of the jumper.
	void AssignGravity()
	{
		if (isJumping) 
		{
			if (!hasDoubleJumped) 
			{
				if (!jumpReleasedEarly)
				{
					if (velocity.y > 0)
					{
						//Gravity for the first half of a full jump.
						gravity = (-(2.0f * (maxJumpHeight)) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (jumpDistance_BeforeApex, 2.0f);
					}
					else if (velocity.y < 0)
					{
						//Gravity for the second half of a full jump.
						gravity = (-(2.0f * (maxJumpHeight)) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (jumpDistance_AfterApex, 2.0f);
					}
				} 
				else
				{
					if (velocity.y > 0) //Gravity to determine the apex of an early release jump is in here.
					{
						if (transform.position.y <= minJumpHeight + initialPositionY) //Below minimum jump height... 
						{ 
							//This gravity will cause the apex to be at the minimum height.
							gravity = -(Mathf.Pow (initialVelocity, 2.0f)) / (2.0f * minJumpHeight);
						} 
						else //Between min and max jump heights...
						{
							//This gravity will cause the apex to be at roughly the current height.
							gravity = -(Mathf.Pow (initialVelocity, 2.0f)) / (2.0f * (transform.position.y - initialPositionY)); 
						}
					}
				}
			}
			else 
			{
				if (velocity.y > 0)
				{
					//Gravity for the rise of a double jump.
					gravity = (-(2.0f * doubleJumpHeight) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (doubleJumpDistance_BeforeApex, 2.0f);
				}
				else if (velocity.y < 0)
				{
					//Gravity for the fall of a double jump.
					gravity = (-(2.0f * doubleJumpHeight) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (doubleJumpDistance_AfterApex, 2.0f);
				}
			}
		} 
		else
		{
			gravity = (-(2.0f * maxJumpHeight) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (jumpDistance_BeforeApex, 2.0f); //Falling gravity.
		}
	}

	//For debugging and testing...
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, minJumpHeight + initialPositionY, transform.position.z), 0.125f);
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, maxJumpHeight + initialPositionY, transform.position.z), 0.125f);
	}
}
