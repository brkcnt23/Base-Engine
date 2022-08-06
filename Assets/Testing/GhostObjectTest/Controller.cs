using System.Collections;
using System.Collections.Generic;
using Base;
using UnityEngine;

public class Controller : MonoBehaviour {
    public Material GhostMaterial;
    private bool _holdingObject;
    private bool _pressed;

    private GameObject holdObject;

    // Update is called once per frame
    void Update()
    {
        return;
        if (Input.GetMouseButtonDown(0)) {
            _pressed = true;
            if(_holdingObject) return;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit)) {
                holdObject = hit.collider.gameObject.CreateGhostObject(GhostMaterial, this.transform);
                
            }
        }

        if (_pressed) {
            if (holdObject == null) {
                return;
            }
            
            //Drag the object around with mouse
            Vector3 movePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            movePos.y = 0;
            holdObject.transform.position = movePos;
        }

        if (Input.GetMouseButtonUp(0)) {
            _pressed = false;
            holdObject = null;
        }
    }
}
