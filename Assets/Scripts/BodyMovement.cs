using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using UnityEngine.SceneManagement;

//The intent of this script is to make the body and any child objects
//look like they're slowly moving up and down.
//Maybe the amount the body moves up and down depends on how far the mouse
//is from it on the screen?

//How about like... a leg starts moving and then that makes it more likely for the other body parts to start moving...
//clicking the buttons like switches various body parts moving?
//If all the limbs are moving the head starts moving and then the creature dies.
public class BodyMovement : MonoBehaviour
{
   private GameObject bodyObject;
   private Transform bodyObjectTransform;

   private float timer;
   private float limbTimer;
   [SerializeField]
   private float amplitude;
   private float[] limbFreq = new float[5]; //from 10 to 100
   private float[] limbAmp = new float[5];
   [SerializeField] 
   private float frequency; //minimum should be 1

   [SerializeField] private Camera cam;
   private Vector3 mouseInput;
   private Vector3 mouseInWorld;

   private int randomNumber;

   [SerializeField] private AudioClip steamSound;
   [SerializeField] private AudioSource steamSource;
   [SerializeField] private GameObject[] limbs;

   [SerializeField] private ParticleSystem phlegmParticle, bloodParticle, yellowBileParticle, blackBileParticle;

   private float headMoveTimer = 0f;
   private float gameEndTimer = 0f;
   
   private bool lArmMoving = false, 
                rArmMoving = false,
                lLegMoving = false,
                rLegMoving = false,
                headMoving = false;


   private bool yellowPressed = false,
                blackPressed = false,
                phlegmPressed = false,
                bloodPressed = false;

   private float limbTimerMax;

   private float particleTimer;
   private void Start()
   {
      //limbFreq = 0f;
      limbTimer = 0f;
      timer = 0f;
      bodyObject = gameObject;
      bodyObjectTransform = bodyObject.transform;
      limbTimerMax = UnityEngine.Random.Range(0, 5f);
      phlegmParticle.Stop();
      bloodParticle.Stop();
      yellowBileParticle.Stop();
      blackBileParticle.Stop();
      limbFreq[4] = 30f;
      limbAmp[4] = 2f;
   }

   private void FixedUpdate()
   {
      //mouseInput = Input.mousePosition;
      //mouseInWorld = cam.ScreenToWorldPoint(mouseInput);
      //print(mouseInWorld);
      timer += Time.deltaTime;

      bodyObjectTransform.position = new Vector3(bodyObjectTransform.position.x,
                                              bodyObjectTransform.position.y + (Mathf.Cos(timer * frequency) * amplitude), 
                                                 bodyObjectTransform.transform.position.z);

      if (limbTimer >= limbTimerMax)
      {
         limbTimer = 0;
         randomNumber = UnityEngine.Random.Range(1, 5);
         MoveRandomLimb(randomNumber);
      }
      else
      {
         limbTimer += Time.deltaTime;  
      }

      //expels smoke
      if (yellowPressed || blackPressed || bloodPressed || phlegmPressed)
      {
         particleTimer += Time.deltaTime;
         if (particleTimer >= 1f)
         {
            particleTimer = 0;
            yellowBileParticle.Stop();
            blackBileParticle.Stop();
            bloodParticle.Stop();
            phlegmParticle.Stop();
            yellowPressed = false;
            blackPressed = false;
            bloodPressed = false;
            phlegmPressed = false;
         }
      }
      
      if (lArmMoving)
      {
         limbs[0].transform.localPosition = new Vector3(limbs[0].transform.localPosition.x,
            limbs[0].transform.localPosition.y + (Mathf.Sin(timer * limbFreq[0]) * amplitude * limbAmp[0]), 
            limbs[0].transform.localPosition.z);
         print("larmmoving");
      }

      if (rArmMoving)
      {
         limbs[1].transform.localPosition = new Vector3(limbs[1].transform.localPosition.x,
            limbs[1].transform.localPosition.y + (Mathf.Sin(timer * limbFreq[1]) * amplitude * limbAmp[1]), 
            limbs[1].transform.localPosition.z);
         print("rarmmoving");
      }

      if (lLegMoving)
      {
         limbs[2].transform.localPosition = new Vector3(limbs[2].transform.localPosition.x,
            limbs[2].transform.localPosition.y + (Mathf.Sin(timer * limbFreq[2]) * amplitude * limbAmp[2]), 
            limbs[2].transform.localPosition.z);
         print("llegmoving");
      }

      if (rLegMoving)
      {
         limbs[3].transform.localPosition = new Vector3(limbs[3].transform.localPosition.x,
            limbs[3].transform.localPosition.y + (Mathf.Sin(timer * limbFreq[3]) * amplitude * limbAmp[3]), 
            limbs[3].transform.localPosition.z);
         print("rlegmoving");
      }

      if (rLegMoving && rArmMoving && lLegMoving && rLegMoving)
      {
         HeadMoveDiscover();
      }
      else
      {
         headMoving = false;
      }
      if (headMoving)
      {
         limbs[4].transform.localPosition = new Vector3(limbs[4].transform.localPosition.x,
            limbs[4].transform.localPosition.y + (Mathf.Sin(timer * limbFreq[4]) * amplitude * limbAmp[4]), 
            limbs[4].transform.localPosition.z);
         print("headmoving");
         if (gameEndTimer >= 2f)
         {
            SceneManager.LoadScene(2);
            gameEndTimer = 0;
         }
         else
         {
            gameEndTimer += Time.deltaTime;
         }

      }

      
   }

   private void HeadMoveDiscover()
   {
      if (headMoveTimer >= 2f)
      {
         headMoving = true;
         headMoveTimer = 0f;
      }
      else
      {
         headMoveTimer += Time.deltaTime;
      }
   }
   private void MoveRandomLimb(int limbValue)
   {
      limbFreq[limbValue - 1] = UnityEngine.Random.Range(10f, 30f);
      limbAmp[limbValue - 1] = UnityEngine.Random.Range(2f, 10f);
      switch (limbValue)
      {
         case 1 :
            lArmMoving = true;
            break;
         case 2 :
            rArmMoving = true;
            break;
         case 3 :
            lLegMoving = true;
            break;
         case 4 :
            rLegMoving = true;
            break;
         case 5 :
            headMoving = true;
            break;
      }
   }
   
   public void AppeaseLimb(int input)
   {
      switch (input)
      {
         case 0 :
            FixPhlegm();
            break;
         case 1 :
            FixBlackBile();
            break;
         case 2 :
            FixYellowBile();
            break;
         case 3 :
            FixBlood();
            break;
      }  
   }
   private void FixYellowBile()
   {
      print("yellow bile");
      yellowPressed = true;
      yellowBileParticle.Play();
      lArmMoving = !lArmMoving;
      rArmMoving = !rArmMoving;
      steamSource.PlayOneShot(steamSound);
   }

   private void FixBlackBile()
   {
    print("black bile");
    blackPressed = true;
    blackBileParticle.Play();
       lLegMoving = !lLegMoving;
       rLegMoving = !rLegMoving;
       steamSource.PlayOneShot(steamSound);

   }

   private void FixPhlegm()
   {
      print("phlegm");
      phlegmPressed = true;
      phlegmParticle.Play();
      rArmMoving = !rArmMoving;
      rLegMoving = !rLegMoving;
      lArmMoving = !lArmMoving;
      steamSource.PlayOneShot(steamSound);

   }

   private void FixBlood()
   {
      print("blood");
      bloodPressed = true;
      bloodParticle.Play();
      lLegMoving = !lLegMoving;
      lArmMoving = !lArmMoving;
      rLegMoving = !rLegMoving;
      steamSource.PlayOneShot(steamSound);

   }
}
