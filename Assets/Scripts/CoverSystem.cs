using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Invector.CharacterController
{
    public class CoverSystem : MonoBehaviour
    {


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

        public GameObject _helper;

        private void Awake()
        {
            _motor = GetComponent<vThirdPersonMotor>();

            _helper.active = true;
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
            DetectCover();


        }

        /// <summary>
        /// Draw ray facing forward from player and draw a cube when close enough from a cover spot. 
        /// Lerp position and rotation that to an helper cube if cover is possible
        /// Move to incover() function
        /// </summary>
        private void DetectCover()
        {
            //if (!_motor.isBehindCover)
            //{
            //    _helper.SetActive(true);
                
            //}

            RaycastHit hit;
            Vector3 origin = Player.transform.position + offset;
            float distOffset = 0.6f;
            Debug.DrawRay(origin, Player.transform.forward, Color.blue);
            if (Physics.Raycast(origin,Player.transform.forward,out hit, 2f,layer))
            {
                
                Vector3 _cubeOffset = -hit.normal * distOffset;
                //_motor.transform.position = Vector3.Lerp(_motor.transform.position, hit.point,1f*Time.deltaTime);
                if (hit.transform.GetComponent<BoxCollider>()) {
                    _helper.transform.position = PosWithOffset(origin, hit.point);

                    //FixNormal(origin, ref hit, layer.value);

                    _helper.transform.rotation = Quaternion.LookRotation(-hit.normal);
                    Quaternion TargetRot = Quaternion.LookRotation(-_helper.transform.forward);

                    Debug.Log(Quaternion.LookRotation(_helper.transform.forward));
                    if (_motor.isBehindCover) {


                        _motor.transform.position = Vector3.Lerp(_motor.transform.position, new Vector3 ( _helper.transform.position.x,_motor.transform.position.y, _helper.transform.position.z), 0.25f);
                        
                        _motor.transform.rotation =Quaternion.Slerp(_motor.transform.rotation, TargetRot,0.25f);
                        
                        //_helper.SetActive(false);
                        //InCover();
                    }
                }
            }



            
        }


        public static void FixNormal(Vector3 position, ref RaycastHit hit, int layermask)
        {
            RaycastHit rayHit;
            Physics.Raycast(position, hit.point - position, out rayHit, 2 * hit.distance, layermask);
            hit.normal = rayHit.normal;
        }


        /// <summary>
        /// calculate position with static offset for helpercube
        /// </summary>
        /// <param name="origin"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        Vector3 PosWithOffset(Vector3 origin, Vector3 target)
        {
            float OffsetFromWall = 0.1f;
            Vector3 direction = origin - target;
            direction.Normalize();
            Vector3 dirOffset = direction * OffsetFromWall;
            Vector3 retVal = target + dirOffset;
            return retVal;

        }

        /// <summary>
        /// Three raycasts to look for cover, 
        /// look for left and right 
        /// if there is no more cover when in cover to stop and initiate peeking
        /// 
        /// </summary>
        private void InCover()
        {
            
            _inputHorizontal = _motor.GetXForCover;
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

            
            PeekFromCover();
        }


        private void PeekFromCover()
        {
            if (_motor.animator!=null && _motor.isBehindCover) { 
            if (_leftRay && _inputHorizontal < 0.1f)
            {
                    //moveLeft while crouching, keep inputvertical at zero
                   
                    
                }
            else if (_rightRay && _inputHorizontal > 0.1f)
            {
                    
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