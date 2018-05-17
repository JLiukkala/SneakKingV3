using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Invector
{
    public class KeyInteraction : MonoBehaviour
    {
        #region Variables
        public GameObject keyInteraction;

        [HideInInspector]
        public GameObject keyObject;

        GameObject player;

        Invector.CharacterController.vThirdPersonController cc;

        // The number of keys that the player has.
        // The player should only have one at a time.
        [HideInInspector]
        public int numberOfKeys = 0;
        #endregion

        void Start()
        {
            keyObject = GameObject.Find("Key");

            player = GameObject.FindGameObjectWithTag("Player");
            cc = player.GetComponent<Invector.CharacterController.vThirdPersonController>();
        }

        void OnTriggerEnter(Collider other)
        {
            // If the player goes inside the collider, the key is picked up, appearing
            // in the top-left corner of the screen. The number of keys is then set
            // to one and the key object and trigger collider are destroyed.
            keyInteraction.SetActive(true);
            numberOfKeys++;

            // Plays the animation for picking up (an object).
            cc.animator.SetTrigger("Pickup");

            Destroy(keyObject);
            Destroy(gameObject.GetComponent<BoxCollider>());
        }
    }
}
