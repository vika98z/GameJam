using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var horizontal = Mathf.Clamp(Input.GetAxis("Horizontal") * 3 + transform.position.x, -13f, 13f);
        var vertical = Mathf.Clamp(Input.GetAxis("Vertical") * 3 + transform.position.z, -12f, 13f);
        transform.position = new Vector3(horizontal, 0, vertical);

    }
}
