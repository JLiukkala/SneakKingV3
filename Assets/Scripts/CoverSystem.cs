using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.CharacterController
{
    public class CoverSystem : MonoBehaviour
    {


        public Transform Player;
        public float _rayHeight = 0.3f;
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
        private void Awake()
        {
            _motor = GetComponent<vThirdPersonMotor>();
        }

        private void Update()
        {
            _inputHorizontal = _motor.GetXForCover;
        }

        // Update is called once per frame
        void LateUpdate()
        {
           
            offset = new Vector3(0, _rayHeight, 0);
            sideOffsetLeft = offset - Player.transform.right*_rayWidth;
            sideOffsetRight = offset + Player.transform.right * _rayWidth;
            
            GetCover();
            
        }


        /// <summary>
        /// Three raycasts to look for cover, 
        /// look for left and right 
        /// if there is no more cover when in cover to stop and initiate peeking
        /// 
        /// </summary>
        private void GetCover()
        {

            _inputHorizontal = _motor.GetXForCover;
           ///* Debug.Log(_motor.GetXForCover)*/;
            Debug.DrawRay(Player.transform.position+offset, Player.transform.forward, Color.red);

            Debug.DrawRay(Player.transform.position+sideOffsetLeft, -Player.transform.forward, Color.green);

            Debug.DrawRay(Player.transform.position+ sideOffsetRight, -Player.transform.forward, Color.yellow);

            if (Physics.Raycast(Player.transform.position + sideOffsetRight, -Player.transform.forward, 1f))
            {
                _rightRay = true;
            } else { _rightRay = false; }

            if (Physics.Raycast(Player.transform.position + sideOffsetLeft, -Player.transform.forward,1f))
            {
                _leftRay = true;
            }
            else { _leftRay = false; }
            if (Physics.Raycast(Player.transform.position + offset, Player.transform.forward, 1f))
            {
                _allowCover = true;
            } else
            {
                _allowCover = false;
            }

            //Debug.Log(input.x);
            PeekFromCover();
        }


        private void PeekFromCover()
        {
            if (_motor.animator!=null && _motor.isBehindCover) { 
            if (_leftRay && _inputHorizontal < 0.1f)
            {
                    //moveLeft while crouching, keep inputvertical at zero
                    _motor.animator.SetFloat("InputHorizontal", _inputHorizontal);
                    
                }
            else if (_rightRay && _inputHorizontal > 0.1f)
            {
                    _motor.animator.SetFloat("InputHorizontal", _inputHorizontal);
                    //moveRight while crouching, keep inputvertical at zero
                }
            else
            {
                if (!_leftRay && _inputHorizontal < 0.1f)
                {
                    PeekDir = false;
                    
                } else if (!_rightRay && _inputHorizontal > 0.1f)
                {
                    PeekDir = true;
                   
                }
                _motor.animator.SetBool("PeekDir", PeekDir);
            }
            }


        }
    }

}