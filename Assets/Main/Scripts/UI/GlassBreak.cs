using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassBreak : MonoBehaviour {

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.tag == "Bullet")
			Destroy(gameObject);
	}
}
