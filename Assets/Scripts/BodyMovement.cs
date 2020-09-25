using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The intent of this script is to make the body and any child objects
//look like they're slowly moving up and down.
//Maybe the amount the body moves up and down depends on how far the mouse
//is from it on the screen?
public class BodyMovement : MonoBehaviour
{
   private GameObject bodyObject;
   private Transform bodyObjectTransform;

   private float timer;
   [SerializeField]
   private float amplitude;
   private void Start()
   {
      timer = 0f;
      bodyObject = gameObject;
      bodyObjectTransform = bodyObject.transform;
   }

   private void Update()
   {
      timer += Time.deltaTime;

      bodyObjectTransform.position = new Vector3(bodyObjectTransform.position.x,
                                              bodyObjectTransform.position.y + (Mathf.Cos(timer) * amplitude), 
                                                 bodyObjectTransform.transform.position.z);
   }
}
