using UnityEngine;
using System.Collections;

public class JumpEquationTester : MonoBehaviour 
{
	public float maxJumpHeight;
	public float minJumpHeight;
	public float jumpDistance_BeforeApex;
	public float jumpDistance_AfterApex;
	public float playerSpeed;

	private float jumpTimer;
	private bool isJumping;
	private float gravity;
	private float initialVelocity;
	private Vector2 velocity;
	private bool jumpReleasedEarly;

	void Start()
	{
		isJumping = false;
		jumpTimer = 0.0f;
		velocity = Vector2.zero;

		jumpReleasedEarly = false;
	}

	void Update()
	{
		//Movement...
		velocity.x = Input.GetAxisRaw("Horizontal") * playerSpeed;
		//velocity.x = playerSpeed; //For testing purposes.

		if (Input.GetKeyDown(KeyCode.Space) && !isJumping) //Initiating a jump...
		{
			isJumping = true;
			jumpTimer = 0.0f;

			//Set to initial velocity...
			initialVelocity = (2.0f * maxJumpHeight * playerSpeed) / (jumpDistance_BeforeApex);
			velocity.y = initialVelocity;
		}
		if (Input.GetKeyUp (KeyCode.Space) && isJumping && !jumpReleasedEarly && velocity.y > 0) //Releasing jump key during a jump...
		{
			jumpReleasedEarly = true;
			Debug.Log ("Jump Released Early :: " + jumpReleasedEarly);
			if (transform.position.y <= minJumpHeight) //If we haven't reached the minimum jump height...
			{
				//Debug.Log ("Released :: Before Min Apex");
				gravity = -(Mathf.Pow(initialVelocity, 2.0f)) / (2.0f * minJumpHeight);
			}
			else //If we're between min and max jump heights...
			{
				//Debug.Log ("Released :: After Min Apex");
				gravity = -(Mathf.Pow(initialVelocity, 2.0f)) / (2.0f * (transform.position.y - minJumpHeight)); 
				//Transform position here is representative of the current jumped height, not the actual position of the player.
			}
		}
	}

	void FixedUpdate()
	{
		//Time.timeScale = .25f;
		if (isJumping && velocity.y < 0)// && !jumpReleasedEarly) 
		{
			//Post-apex, pre-landing gravity.
			gravity = (-(2.0f * maxJumpHeight) * Mathf.Pow(playerSpeed, 2.0f)) / Mathf.Pow(jumpDistance_AfterApex, 2.0f);
		} 
		else if (!jumpReleasedEarly)
		{
			//Normal gravity.
			gravity = (-(2.0f * maxJumpHeight) * Mathf.Pow(playerSpeed, 2.0f)) / Mathf.Pow(jumpDistance_BeforeApex, 2.0f);
		}

		if (isJumping) //Jump velocity stuff
		{
			jumpTimer += Time.deltaTime;
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

		//Modified velocity verlet method of interpolation (Broken)
		//velocity *= Time.deltaTime;
		//transform.position += ((Vector3)velocity * Time.deltaTime) + new Vector3(0.0f, (0.5f * gravity * Time.deltaTime * Time.deltaTime), 0.0f);
		velocity.y += gravity; ///Apply gravity.
		transform.position += (Vector3)velocity;// * Time.deltaTime;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, minJumpHeight, transform.position.z), 0.125f);
		Gizmos.DrawWireSphere (new Vector3 (transform.position.x, maxJumpHeight, transform.position.z), 0.125f);
	}
}
