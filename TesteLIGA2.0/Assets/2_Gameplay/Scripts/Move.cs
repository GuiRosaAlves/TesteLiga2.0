using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{


	public new Rigidbody2D rigidbody;


	public float vel;
	public float jumpForce;

	private void FixedUpdate()
	{
		float x = 0;

		if (Input.GetKey(KeyCode.A))
			x = -1;
		else if (Input.GetKey(KeyCode.D))
			x = 1;
		
		rigidbody.velocity = new Vector2(x * vel, rigidbody.velocity.y);
		
		if(Input.GetKeyDown(KeyCode.Space))
			rigidbody.AddForce(new Vector2(0f, jumpForce));

	}
}
