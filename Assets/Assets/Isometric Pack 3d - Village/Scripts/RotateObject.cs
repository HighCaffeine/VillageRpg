using UnityEngine;
using System.Collections;

public class RotateObject : MonoBehaviour 
{
	[SerializeField]
	private Vector3 Axis = Vector3.up;
	[SerializeField]
	private float Speed = 1f;

	void Update () 
	{
		transform.Rotate(Axis * Time.deltaTime * Speed, Space.World);
	}
}
