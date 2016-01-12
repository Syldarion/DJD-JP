using BeardedManStudios.Network;
using UnityEngine;

public class ForgeExample_AuthoritativeControllerFloats : NetworkedMonoBehavior
{
	[NetSync]
	public float horizontal = 0.0f;

	[NetSync]
	public float vertical = 0.0f;

	private void Update()
	{
		if (!IsOwner)
			return;

		horizontal = Input.GetAxis("Horizontal");
		vertical = Input.GetAxis("Vertical");
	}
}