using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector.CharacterController
{
    public class SecurityCamScript : MonoBehaviour
    {

        public Transform _cameraBody;
        public Transform _cameraStand;
        public Transform _cameraLens;

        [Tooltip("Camera rotates based on positive and negative versions of this value.")]
        public float _rotationAngle = 30;

        [Tooltip("Camera rotation speed.")]
        public float _rotationSpeed = 5;

        private bool _toRight = true;
        private bool _toLeft = false;

        private Quaternion _startRot;

        public float timeToSpotPlayer = 1;

        public Light visionCone;
        public float viewDistance;
        public LayerMask viewMask;

        private float viewAngle;
        private float playerVisibleTimer;


        Quaternion _targetRotation;

        Transform player;

        Color originalVisionConeColor;

        // Use this for initialization
        void Awake()
        {
            _startRot = transform.rotation;

            player = GameObject.FindGameObjectWithTag("Player").transform;
            viewAngle = visionCone.spotAngle;
            originalVisionConeColor = visionCone.color;
        }

      

        // Update is called once per frame
        void Update()
        {

            RotateCam();


            if (CanSeePlayer() && playerVisibleTimer < timeToSpotPlayer)
            {
                playerVisibleTimer += Time.deltaTime;
            }
            // Otherwise, playerVisibleTimer is decresed by Time.deltaTime.
            else if (!CanSeePlayer() && playerVisibleTimer > 0)
            {
                playerVisibleTimer -= Time.deltaTime;
            }

            // Clamps the value of the playerVisibleTimer variable 
            // to be between zero and the timeToSpotPlayer variable.
            playerVisibleTimer = Mathf.Clamp(playerVisibleTimer, 0, timeToSpotPlayer);

            // The enemy's vision cone lerps towards red 
            // if the player is visible to the enemy.
            visionCone.color = Color.Lerp(originalVisionConeColor, Color.red,
                playerVisibleTimer / timeToSpotPlayer);

            // If the playerVisibleTimer reaches the value of the timeToSpotPlayer
            // variable, the event OnEnemyHasSpottedPlayer is called.
            if (playerVisibleTimer >= timeToSpotPlayer)
            {
                //if (OnEnemyHasSpottedPlayer != null)
                //{
                //IsSpotted();

                GameUI._losingStatement = true;
                Debug.Log("SpottedPlayer");
                //}
            }


        }
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(visionCone.transform.position, visionCone.transform.forward * viewDistance);
        }

        bool CanSeePlayer()
        {
            // If the distance between the enemy and the player is less than
            // the view distance that has been set, the angle between them is
            // less than the view angle divided by 2 and the player's collider
            // is recognized by the enemy, the enemy is able to see the player.
            if (Vector3.Distance(visionCone.transform.position, player.position) < viewDistance)
            {
                Vector3 dirToPlayer = (player.position - visionCone.transform.position).normalized;
                float angleBetweenEnemyAndPlayer = Vector3.Angle(visionCone.transform.forward, dirToPlayer);

                if (angleBetweenEnemyAndPlayer < viewAngle / 2)
                {
                    if (!Physics.Linecast(visionCone.transform.position, player.position, viewMask))
                    {
                        Debug.Log("CanSeePlayer");
                        return true;
                    }
                }
            }
            return false;
        }


        private void RotateCam()
        {
            Quaternion cRot = _cameraBody.transform.rotation;
            _targetRotation = _startRot;



            if (_toRight)
            {

                _targetRotation = Quaternion.Euler(-55, _startRot.eulerAngles.y + _rotationAngle, _startRot.eulerAngles.z + _rotationAngle);
                if (cRot == _targetRotation)
                {
                    _toLeft = true;
                    _toRight = false;
                }
            }
            else if (_toLeft)
            {
                _targetRotation = Quaternion.Euler(-55, _startRot.eulerAngles.y - _rotationAngle, _startRot.eulerAngles.z - _rotationAngle);
                if (cRot == _targetRotation)
                {
                    _toLeft = false;
                    _toRight = true;
                }
            }

            cRot = Quaternion.RotateTowards(cRot, _targetRotation, _rotationSpeed * Time.deltaTime);

            _cameraBody.transform.rotation = cRot;
            _cameraStand.transform.rotation = cRot;
            _cameraLens.transform.rotation = cRot;

        }
    }

   


}