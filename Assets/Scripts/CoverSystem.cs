﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.CharacterController
{
    public class CoverSystem : MonoBehaviour
    {

        public float _peekOffset = 0.55f;
        public Transform Player;
        public float _rayHeight = 0.5f;
        public float _rayWidth = 0.5f;
        private Vector3 offset;
        private Vector3 sideOffsetLeft;
        private Vector3 sideOffsetRight;
        private bool _leftRay;
        private bool _rightRay;
        private bool _allowCover = false;
        private float _inputHorizontal;
        private bool PeekDir;
        private vThirdPersonMotor _motor;
        public LayerMask layer;

        private Vector3 PosToMove;
        private Quaternion RotToMove;

        public bool _coverPos = false;


        private float _pMov;

        private bool movedToCrouch = false;

        public GameObject _helper;

        private void Awake()
        {
            _motor = GetComponent<vThirdPersonMotor>();

            _helper.SetActive(true);
        }


        private void Update()
        {
            
        }

        // Update is called once per frame
        void LateUpdate()
        {
           
            offset = new Vector3(0, _rayHeight, 0);
            sideOffsetLeft = offset - Player.transform.right*_rayWidth;
            sideOffsetRight = offset + Player.transform.right * _rayWidth;
            if ( !_motor.isBehindCover) { DetectCover(); }


            if (_motor.isBehindCover) { InCover(); }

        }

        /// <summary>
        /// Draw ray facing forward from player and draw a cube when close enough from a cover spot. 
        /// Lerp position and rotation that to an helper cube if cover is possible
        /// Move to incover() function
        /// </summary>
        private void DetectCover()
        {
            
            RaycastHit hit;
            Vector3 origin = Player.transform.position + offset;
            movedToCrouch = false;
            float distOffset = 0.5f;
            //Debug.DrawRay(origin, Player.transform.forward, Color.blue);
            _coverPos = Physics.Raycast(origin, Player.transform.forward, out hit, 2f, layer);
            if (_coverPos )
            {
                
                Vector3 _cubeOffset = -hit.normal * distOffset;
                if (hit.transform.GetComponent<BoxCollider>()) {

                    _helper.transform.position = PosWithOffset(origin, hit.point);
                    
                    //FixNormal(origin, ref hit, layer.value);

                    _helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
                    Quaternion TargetRot = Quaternion.LookRotation(-_helper.transform.forward);
                    Vector3 TargetPos = new Vector3(_helper.transform.position.x, _motor.transform.position.y, _helper.transform.position.z);


                    PosToMove = TargetPos;
                    RotToMove = TargetRot;

                    //_motor.transform.position = Vector3.Lerp(_motor.transform.position,TargetPos , 4f * Time.deltaTime);
                    //_motor.transform.rotation = Quaternion.Slerp(_motor.transform.rotation, Quaternion.LookRotation(hit.normal), 4f *Time.deltaTime);

                    
                    
                }
            }

            
        }




        /// <summary>
        /// calculate position with static offset for helpercube
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            float OffsetFromWall = 0.4f;
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 dirOffset = direction * OffsetFromWall;
            Vector3 retVal = target + dirOffset;
            return retVal;

        }

        IEnumerator MoveAndRotToCover()
        {
            _motor.transform.position = Vector3.Lerp(_motor.transform.position, PosToMove, 4f * Time.deltaTime);
            _motor.transform.rotation = Quaternion.Slerp(_motor.transform.rotation, RotToMove, 4f * Time.deltaTime);
            yield return new WaitForSeconds(1.25f);
            movedToCrouch = true;
        }


        

        /// <summary>
        /// Three raycasts to look for cover, 
        /// look for left and right 
        /// if there is no more cover when in cover to stop and initiate peeking
        /// 
        /// </summary>
        private void InCover()
        {
            if (!movedToCrouch)
            {
                StartCoroutine("MoveAndRotToCover");
            }
            

            Debug.DrawRay(Player.transform.position+offset, Player.transform.forward, Color.red);

            Debug.DrawRay(Player.transform.position+sideOffsetLeft, -Player.transform.forward, Color.green);

            Debug.DrawRay(Player.transform.position+ sideOffsetRight, -Player.transform.forward, Color.yellow);

            if (Physics.Raycast(Player.transform.position + sideOffsetRight, -Player.transform.forward, 1f))
            {
                _leftRay = true;
            } else
            {
                _leftRay = false;
                if (_motor.input.x < 0)
                {
                    _pMov = _motor.input.x;
                    _motor.input.x = 0;
                }
                else
                {
                    _pMov = 0;
                }
            }

            if (Physics.Raycast(Player.transform.position + sideOffsetLeft, -Player.transform.forward,1f))
            {
                _rightRay = true;
                
            }
            else
            {
                _rightRay = false;
                if (_motor.input.x > 0)
                {
                    _pMov = _motor.input.x;
                    _motor.input.x = 0;
                    
                } else
                {
                    _pMov = 0;
                }
            }
            PeekFromCover();
            
        }


        private void PeekFromCover()
        {
            if (_motor.animator!=null && _motor.isBehindCover) {
            if (!_leftRay && _pMov < -0.5f)
            {
                    _motor.isPeeking = true;
                    PeekDir = true;
                    vThirdPersonCamera.instance.rightOffset=Mathf.Lerp( vThirdPersonCamera.instance.rightOffset,  -_peekOffset,10*Time.deltaTime);
                  
            }
            else if (!_rightRay && _pMov > 0.5f)
            {
                    _motor.isPeeking = true;
                    PeekDir = false;
                    vThirdPersonCamera.instance.rightOffset=Mathf.Lerp(vThirdPersonCamera.instance.rightOffset, _peekOffset, 10 * Time.deltaTime);

            } else
            {
                    _motor.isPeeking = false;
                    vThirdPersonCamera.instance.rightOffset=Mathf.Lerp(vThirdPersonCamera.instance.rightOffset, 0, 10 * Time.deltaTime);
            }
            _motor.animator.SetBool("PeekDir", PeekDir);
            }
           

        }
    }

}