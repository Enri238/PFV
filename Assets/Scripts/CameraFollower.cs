using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    #region Variables
    public Transform target;

    private float offset;
    #endregion

    #region Unity Methods    

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position.z - transform.position.z;
	}

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, target.position.z - offset);
    }

    #endregion
}
