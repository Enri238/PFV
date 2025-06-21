using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{

    #region Variables
    public Transform target;

    private Vector3 offset;
    #endregion

    #region Unity Methods    

    // Start is called before the first frame update
    void Start()
    {
        offset = target.position - transform.position;
	}

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(target.position.x - offset.x, target.position.y - offset.y, target.position.z - offset.z);
    }

    #endregion
}
