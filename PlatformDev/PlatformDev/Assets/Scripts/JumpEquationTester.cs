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

		if (Input.GetKeyDown(KeyCode.Space) && !isJumping) //Initiating a jump...
		{
			isJumping = true;

			//Set velocity to initial velocity...
			initialVelocity = (2.0f * maxJumpHeight * playerSpeed) / (jumpDistance_BeforeApex);
			velocity.y = initialVelocity;
		}
		if (Input.GetKeyUp (KeyCode.Space) && isJumping && velocity.y > 0) // && !jumpReleasedEarly && velocity.y > 0) //Releasing jump key during a jump...
		{
			jumpReleasedEarly = true;
		}
	}

	void FixedUpdate() //Physics stuff...
	{
		if (isJumping && velocity.y < 0) //Use the gravity for max jump height. (Default)
		{
			//Post-apex, pre-landing gravity.
			gravity = (-(2.0f * maxJumpHeight) * Mathf.Pow (playerSpeed, 2.0f)) / Mathf.Pow (jumpDistance_AfterApex, 2.0f);
		} 
		else if (isJumping && jumpReleasedEarly && velocity.y > 0) //Use gravity based on when the jump button was released. (On early release)
		{
			if (transform.position.y <= minJumpHeight) //Below minimum jump height...
			{
				gravity = -(Mathf.Pow(initialVelocity, 2.0f)) / (2.0f * minJumpHeight);
			}
			else //Between min and max jump heights...
			{
				//Transform position here is representative of the current jumped height, not the actual position of the player.
				gravity = -(Mathf.Pow(initialVelocity, 2.0f)) / (2.0f * (transform.position.y - minJumpHeight)); 
			}
		}
		else if (!jumpReleasedEarly) //Use normal gravity. (Not associated with jumping)
		{
			//Normal gravity.
			gravity = (-(2.0f * maxJumpHeight) * Mathf.Pow(playerSpeed, 2.0f)) / Mathf.Pow(jumpDistance_BeforeApex, 2.0f);
		}

		//Keep the jumper from falling off-screen...
		if (transform.position.y + velocity.y <= 0)
		{
			transform.position = new Vector3 (transform.position.x, 0.0f, transform.position.z);
			if(velocity.y < 0)
			{
				velocity.y = 0.0f;
				isJumping = false; //Reset jumping state when we hit the "ground".
				jumpReleasedEarly = false;
			}
		}

		//Apply velocity...
		velocity.y += gravity; ///Apply gravity.
		transform.position += (Vector3)velocity;// * Time.deltaTime;
	}

	//For debugging and testing...
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, minJumpHeight, transform.position.z), 0.125f);
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, maxJumpHeight, transform.position.z), 0.125f);
	}
}
