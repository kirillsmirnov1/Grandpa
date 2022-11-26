using System.Collections;
using UnityEngine;
using UnityEngine.Events;

//This script handles the character being killed and respawning
namespace GMTK
{
    public class CharacterHurt : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private Animator myAnim;
        [SerializeField] private AudioSource hurtSFX;
        [SerializeField] public SpriteRenderer spriteRenderer;

        [Header("Settings")]
        [SerializeField] private float flashDuration;

        [Header("Events")] 
        [SerializeField] public UnityEvent onHurt = new UnityEvent();
        
        private bool waiting = false;
        private bool hurting = false;
        
        private Coroutine flashRoutine;
        private Rigidbody2D body;

        void Start()
        {
            body = GetComponent<Rigidbody2D>();
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            {
                //If the player hits layer 7 (saw blade) or 8 (spikes), start the hurt routine
                if (collision.gameObject.layer == 7 || collision.gameObject.layer == 8)
                {
                    if (hurting == false)
                    {
                        //If it's spikes, stop the character's velocity
                        if (collision.gameObject.layer == 8)
                        {
                            body.velocity = Vector2.zero;
                        }

                        hurting = true;
                        HurtRoutine();
                    }
                }
            }
        }

        public void HurtRoutine()
        {
            hurtSFX.Play();

            Stop(0.1f);
            myAnim.SetTrigger("Hurt");
            Flash();
        }

        //These three functions handle the hit stop effect, where the game pauses for a brief moment on death
        public void Stop(float duration)
        {
            Stop(duration, 0.0f);
        }

        public void Stop(float duration, float timeScale)
        {
            if (waiting)
                return;
            Time.timeScale = timeScale;
            StartCoroutine(Wait(duration));
        }

        IEnumerator Wait(float duration)
        {
            waiting = true;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1.0f;
            waiting = false;
        }

        //These two functions handle the flashing white effect when Kit dies
        public void Flash()
        {
            if (flashRoutine != null)
            {
                StopCoroutine(flashRoutine);
            }

            flashRoutine = StartCoroutine(FlashRoutine());
        }

        private IEnumerator FlashRoutine()
        {
            // Show the flash
            spriteRenderer.enabled = true;

            // Pause the execution of this function for "duration" seconds.
            yield return new WaitForSeconds(flashDuration);

            // Hide the flash
            spriteRenderer.enabled = false;

            // Set the routine to null, signaling that it's finished.
            flashRoutine = null;
        }
    }
}