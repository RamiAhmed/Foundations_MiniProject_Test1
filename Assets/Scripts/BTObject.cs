using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BTObject : MonoBehaviour {

	public bool bDoneRunning = false;

	public abstract void StartObject();

	public abstract void RemoveSelf();

}
