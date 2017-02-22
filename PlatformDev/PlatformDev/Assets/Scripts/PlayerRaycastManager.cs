using UnityEngine;
using System.Collections;

/// <summary>
/// This class handles raycasting from the pllayer object, and supplies the player controller
/// with data about collisions.
/// 
/// ISSUE: Skin width needs to be big-ish, otherwise L/R raycasts are getting hits from the ground.
/// ISSUE: Not really this class' problem, but the debug rays are being drawn at an old origin point.
/// </summary>
public class PlayerRaycastManager : MonoBehaviour 
{
	public int numHorizRaycasts;
	public int numVertRaycasts;
	public float skinWidth; //An inset for the raycast origins

	private float objWidth, objHeight; //The width and height of the player object.

	public struct CollisionInfo
	{
		public bool topCollision, bottomCollision;
		public bool leftCollision, rightColision;

		public bool topCollision_Prev, bottomCollision_Prev;
		public bool leftCollision_Prev, rightCollision_Prev;

		public void reset()
		{
			topCollision_Prev = topCollision;
			bottomCollision_Prev = bottomCollision;
			leftCollision_Prev = leftCollision;
			rightCollision_Prev = rightColision;

			topCollision = false;
			bottomCollision = false;
			leftCollision = false;
			rightColision = false;
		}
	}
	public CollisionInfo collisionInfo;

	void Start()
	{
		collisionInfo = new CollisionInfo ();

		objWidth = GetComponent<Renderer> ().bounds.size.x;
		objHeight = GetComponent<Renderer> ().bounds.size.y;
	}

	//Requires velocity in order to determine the length of each raycast.
	//Returns the nearest hit (horiz, vert).
	public Vector2 PerformUpdate(Vector2 velocity)
	{
		Vector2 nearestHits = Vector2.zero;

		collisionInfo.reset ();
		nearestHits.x = CheckHorizRaycasts (velocity.x);
		nearestHits.y = CheckVertRaycasts (velocity.y);

		return nearestHits;
	}

	//For some reason, this won't raycast at all if numHorizRaycasts == 1. Not a huge issue, but annoying.
	private float CheckHorizRaycasts(float raycastLength) //Returns the nearest hit point, assuming there is a hit.
	{
		float nearestHit = raycastLength;

		//Left...
		if (raycastLength < 0) 
		{
			for (int i = 0; i < numHorizRaycasts; i++)
			{
				RaycastHit2D hit = new RaycastHit2D ();

				Vector2 origin = (Vector2)transform.position;
				origin.x -= (objWidth / 2) - skinWidth;
				origin.y += (((float)i / ((float)numHorizRaycasts - 1.0f)) * (objHeight - 2.0f * skinWidth)) - (0.5f * objHeight) + skinWidth;

				Vector2 direction = Vector2.left;

				hit = Physics2D.Raycast (origin, direction, raycastLength);

				if (hit.transform != null) 
				{
					if (nearestHit >= hit.point.x) 
					{
						nearestHit = Mathf.Abs(hit.point.x - origin.x);
					}
					collisionInfo.leftCollision = true;
					Debug.DrawRay (origin, direction * raycastLength, Color.yellow);
				} 
				else
				{
					Debug.DrawRay (origin, direction * raycastLength, Color.red);
				}
				//Debug.Log (i + ": " + origin.x + ", " + origin.y );
			}
		} 
		else if (raycastLength > 0)
		{
			//Right...
			for (int i = 0; i < numHorizRaycasts; i++) 
			{
				RaycastHit2D hit = new RaycastHit2D ();

				Vector2 origin = (Vector2)transform.position;
				origin.x += (objWidth / 2) - skinWidth;
				origin.y += (((float)i / ((float)numHorizRaycasts - 1.0f)) * (objHeight - 2.0f * skinWidth)) - (0.5f * objHeight) + skinWidth;

				Vector2 direction = Vector2.right;

				hit = Physics2D.Raycast (origin, direction, raycastLength);

				if (hit.transform != null) 
				{
					if (nearestHit <= hit.point.x) 
					{
						nearestHit = hit.point.x - origin.x;
					}
					collisionInfo.rightColision = true;
					Debug.DrawRay (origin, direction * raycastLength, Color.yellow);
				} 
				else
				{
					Debug.DrawRay (origin, direction * raycastLength, Color.red);
				}
			}
		}

		return nearestHit;
	}

	private float CheckVertRaycasts(float raycastLength)
	{
		float nearestHit = raycastLength - transform.position.y;

		if (raycastLength > 0) 
		{
			//Top...
			for (int i = 0; i < numVertRaycasts; i++) 
			{
				RaycastHit2D hit = new RaycastHit2D ();

				Vector2 origin = transform.position;
				origin.x += (((float)i / ((float)numVertRaycasts - 1.0f)) * (objWidth - 2.0f * skinWidth)) - (0.5f * objWidth) + skinWidth;
				origin.y += (objHeight / 2) - skinWidth;

				Vector2 direction = Vector2.up;

				hit = Physics2D.Raycast (origin, direction, raycastLength);

				if (hit.transform != null) 
				{
					if (nearestHit <= hit.point.y) 
					{
						nearestHit = hit.point.y - origin.y;
					}
					collisionInfo.topCollision = true;
					Debug.DrawRay (origin, direction * raycastLength, Color.yellow);
				}
				else 
				{
					Debug.DrawRay (origin, direction * raycastLength, Color.red);
				}
			}
		} 
		else if (raycastLength < 0) 
		{
			//Bottom...
			for (int i = 0; i < numVertRaycasts; i++) 
			{
				RaycastHit2D hit = new RaycastHit2D ();

				Vector2 origin = transform.position;
				origin.x += (((float)i / ((float)numVertRaycasts - 1.0f)) * (objWidth - 2.0f * skinWidth)) - (0.5f * objWidth) + skinWidth;
				origin.y -= (objHeight / 2) + skinWidth;
				//Debug.DrawLine (new Vector2(origin.x - 0.05f, origin.y), new Vector2(origin.x + 0.05f, origin.y), Color.blue);
				//Debug.DrawLine (origin, new Vector2 (origin.x, origin.y + Vector2.down.y * .05f), Color.blue);

				Vector2 direction = Vector2.down;

				hit = Physics2D.Raycast (origin, direction, -raycastLength);

				if (hit.transform != null)
				{
					if (nearestHit >= hit.point.y - origin.y) 
					{
						nearestHit = Mathf.Abs(hit.point.y - origin.y);
					}
					collisionInfo.bottomCollision = true;
					Debug.DrawRay (origin, direction * -raycastLength, Color.yellow);
				}
				else
				{
					Debug.DrawRay (origin, direction * -raycastLength, Color.red);
				}
			}
		}

		return nearestHit;
	}
}
