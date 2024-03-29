﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{

    [SerializeField] float inertia;
    [SerializeField] float scrollSpeed;
    [SerializeField] float multitouchSpeed = 0.1f;
    [SerializeField] LayerMask layerMaskForBaseCollider;
    [SerializeField] LayerMask layerMaskForLimits;

    [SerializeField] Collider movementBase;
    [SerializeField] Camera currentCamera;
    bool lockMovement = false;

    public void Init(Camera cam, Collider baseMovementPlane = null)
    {
        if (cam != null)
            currentCamera = cam;
        currentCamera = cam;
        if (baseMovementPlane != null)
            movementBase = baseMovementPlane;
    }

    public void SetLock(bool isLocked)
    {
        lockMovement = isLocked;
    }

    void LateUpdate()
    {
        MoveUpdate();
    }


    Vector3 currentMovementVector;
    Vector3 prevScreenPointOfRaycast;
    Vector3 inertiaVector;
    RaycastHit hit;
    RaycastHit hit2;
    Collider[] limitHits;
    bool movePerformed = false;

    void MoveUpdate()
    {
        if (GameManager.Instance.fingerIsOverUI)
            return;
        if (!lockMovement) // movement screen space
            if (Input.GetMouseButton(0) && Input.touchCount < 2)
            {
                if (Physics.Raycast(currentCamera.ScreenPointToRay(Input.mousePosition), out hit, 3000, layerMaskForBaseCollider.value))
                {
                    Physics.Raycast(currentCamera.ScreenPointToRay(prevScreenPointOfRaycast), out hit2, 3000, layerMaskForBaseCollider.value);
                    currentMovementVector = hit.point - hit2.point;
                    prevScreenPointOfRaycast = Input.mousePosition;



                    if (!movePerformed)
                    {
                        movePerformed = true;
                    }
                    else
                    {
                        limitHits = Physics.OverlapSphere(currentCamera.transform.position - currentMovementVector, 0.5f, layerMaskForLimits.value);
                        if (limitHits.Length == 0)
                        {
                            currentCamera.transform.position = currentCamera.transform.position - currentMovementVector ;
                        }
                    }
                }
            }
            else
            {
                if (movePerformed)
                    inertiaVector = currentMovementVector;
                movePerformed = false;
                prevScreenPointOfRaycast = Vector3.zero;
                transform.position -= inertiaVector * Time.deltaTime * inertia;
                inertiaVector = Vector3.Lerp(inertiaVector, Vector3.zero, Time.timeScale);
            }


        // zoom zoom
        float scaleValue = 0;
        float cameraZoomMagnitude = 0;
        scaleValue = Input.mouseScrollDelta.y * scrollSpeed;

        if (Input.touchCount == 2) // multitouch
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            cameraZoomMagnitude = -(prevTouchDeltaMag - touchDeltaMag)*multitouchSpeed;
        }
        else
            cameraZoomMagnitude = 0;

        scaleValue += cameraZoomMagnitude;

        limitHits = Physics.OverlapSphere(currentCamera.transform.position + currentCamera.transform.forward * scaleValue, 0.5f, layerMaskForLimits.value);
        if (limitHits.Length == 0)
        {
            currentCamera.transform.position += currentCamera.transform.forward * scaleValue;
        }
        

    }







}
