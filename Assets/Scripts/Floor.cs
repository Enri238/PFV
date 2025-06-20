using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floor : MonoBehaviour
{

	#region Variables
	public enum FloorType
	{
		Normal,
		Slippery,
		Sticky,
		Trampoline,
		Disappearing,
	}

	public FloorType floorType;
	public float destroyTime;
    #endregion

    #region Unity Methods    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			SantaController santaController = collision.gameObject.GetComponent<SantaController>();
			SantaControllerv2 santaControllerv2 = collision.gameObject.GetComponent<SantaControllerv2>();
			
			switch (floorType)
            {
				case FloorType.Slippery:
					if (santaController)
						santaController.AlterPhysics(FloorType.Slippery);
					else if (santaControllerv2)
						santaControllerv2.AlterPhysics(FloorType.Slippery);
					break;

				case FloorType.Sticky:
					if (santaController)
						santaController.AlterPhysics(FloorType.Sticky);
					else if (santaControllerv2)
						santaControllerv2.AlterPhysics(FloorType.Sticky);
					break;

				case FloorType.Trampoline:
					if (santaController)
						santaController.AlterPhysics(FloorType.Trampoline);
					else if (santaControllerv2)
						santaControllerv2.AlterPhysics(FloorType.Trampoline);
					break;

				case FloorType.Disappearing:
					Destroy(gameObject, destroyTime);
					break;

				default:
					if (santaController)
						santaController.AlterPhysics(FloorType.Normal);
					else if (santaControllerv2)
						santaControllerv2.AlterPhysics(FloorType.Normal);
					break;
			}
		}
	}

	private void OnCollisionExit(Collision collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			SantaController santaController = collision.gameObject.GetComponent<SantaController>();
			SantaControllerv2 santaControllerv2 = collision.gameObject.GetComponent<SantaControllerv2>();
			
			if (santaController)
				santaController.AlterPhysics(FloorType.Normal);
			else if (santaControllerv2)
				santaControllerv2.AlterPhysics(FloorType.Normal);
		}
	}

	#endregion
}
