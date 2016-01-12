using BeardedManStudios.Network;
using UnityEngine;

public class ForgeExample_AuthoritativeControllerBody : NetworkedMonoBehavior
{
	public float speed = 5.0f;
	public float horizontal = 0.0f;
	public float vertical = 0.0f;

	public ForgeExample_AuthoritativeControllerFloats inputController = null;

	private void Update()
	{
		horizontal = inputController.horizontal * speed * Time.deltaTime;
		vertical = inputController.vertical * speed * Time.deltaTime;

		transform.position += new Vector3(horizontal, vertical, 0);
	}
}