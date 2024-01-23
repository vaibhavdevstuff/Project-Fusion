using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DC.HealthSystem
{
    [HelpURL("https://domino-code.gitbook.io/health-system/")]
    public class Health : NetworkBehaviour
    {
        public enum OnDeathSelf { None, Deactivate, Destroy }

        #region Variables

        [Tooltip("The maximum health value for this object.")]
        [SerializeField] private float maxValue;

        [Tooltip("The minimum health value for this object.")]
        [SerializeField] private float minValue;

        [Tooltip("The current health value for this object.")]
        public float value;

        [Tooltip("Whether this object is invincible.")]
        [SerializeField] private bool invincible;

        [Tooltip("The time in seconds that this object is invincible after being spawned (Can't be less than 0)")]
        [SerializeField][Min(0)] private float timeInvincibleAfterSpawn;

        [Tooltip("Determines the action to take after the object dies.")]
        [SerializeField] private OnDeathSelf onDeathDoSelf;

        [Tooltip("The delay (in seconds) before the After Death action is performed (Can't be less than 0)")]
        [SerializeField][Min(0)] private float afterDeathDelay;

        [Tooltip("Objects to spawn when this object dies.")]
        [SerializeField] private List<GameObject> spawnObjectsOnDeath;

        [Tooltip("Objects to destroy when this object dies.")]
        [SerializeField] private List<GameObject> destroyObjectsOnDeath;

        [Tooltip("If checked, enables audio playback.")]
        [SerializeField] private bool enableAudio;

        [Tooltip("The audio source to use for playback.")]
        [SerializeField] private AudioSource audioSource;

        [Tooltip("Audio clips to play when healing.")]
        [SerializeField] private List<AudioClip> healAudio;

        [Tooltip("Audio clips to play when taking damage.")]
        [SerializeField] private List<AudioClip> damageAudio;

        [Tooltip("Audio clips to play when dying.")]
        [SerializeField] private List<AudioClip> deathAudio;

        [Tooltip("Event that is triggered when this object is healed.")]
        [SerializeField] private UnityEvent<float> OnHealEvent;

        [Tooltip("Event that is triggered when this object takes damage.")]
        [SerializeField] private UnityEvent<float> OnDamageEvent;

        [Tooltip("Event that is triggered when this object dies.")]
        [SerializeField] private UnityEvent<float> OnDeathEvent;

        [Tooltip("Determine weather the object is dead or not")]
        private bool isDead = false;

        //Action Events

        /// <summary>
        /// Event Trigger when Object Get Heal
        /// </summary>
        /// <returns>Heal Amount (float)</returns>
        public Action<float> OnHeal { get; set; }
        /// <summary>
        /// Event Trigger when Object Get Damage
        /// </summary>
        /// <returns>Damage Amount (float)</returns>
        public Action<float> OnDamage { get; set; }
        /// <summary>
        /// Event Trigger when Object Die
        /// </summary>
        /// <returns>Damage Amount (float)</returns>
        public Action<float> OnDeath { get; set; }

        #endregion

        #region Getters And Setters

        public float MaxValue { get => maxValue; }
        public float MinValue { get => minValue; }
        public bool Invincible { get => invincible; set => invincible = value; }
        public float TimeInvincibleAfterSpawn { get => timeInvincibleAfterSpawn; set => timeInvincibleAfterSpawn = value < 0 ? 0 : value; }
        public OnDeathSelf OnDeathDoSelf { get => onDeathDoSelf; set => onDeathDoSelf = value; }
        public float AfterDeathDelay { get => afterDeathDelay; set => afterDeathDelay = value < 0 ? 0 : value; }
        public List<GameObject> SpawnObjectsOnDeath { get => spawnObjectsOnDeath; set => spawnObjectsOnDeath = value; }
        public List<GameObject> DestroyObjectsOnDeath { get => destroyObjectsOnDeath; set => destroyObjectsOnDeath = value; }
        public bool EnableAudio { get => enableAudio; set => enableAudio = value; }
        public AudioSource AudioSource { get => audioSource; set => audioSource = value; }
        public List<AudioClip> HealAudio { get => healAudio; set => healAudio = value; }
        public List<AudioClip> DamageAudio { get => damageAudio; set => damageAudio = value; }
        public List<AudioClip> DeathAudio { get => deathAudio; set => deathAudio = value; }
        public bool IsDead { get => isDead; }


        #endregion

        [Networked]
        public float HealthValue { get; set; }

        public override void Spawned()
        {
            HealthValue = maxValue;
            value = HealthValue;
        }

        public override void FixedUpdateNetwork()
        {
            if(value != HealthValue)
                value = HealthValue;
        }


        #region Unity Methods

        private void Start()
        {
            // Start a coroutine that makes the entity invincible for a short time after it spawns
            StartCoroutine(InvincibleAfterSpawn());
        }

        private void OnValidate()
        {
            //Restrict Health Value in Unity Editor
            {
                //Max

                //Min
                if (minValue > maxValue) minValue = maxValue;

                //Current
                if (value > maxValue) value = maxValue;
                if (value < minValue) value = minValue;
            }

            //Restrict Invincible Time in Unity Editor
            {
                if (timeInvincibleAfterSpawn < 0) timeInvincibleAfterSpawn = 0;
            }

        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Increases the health value by the specified amount.
        /// </summary>
        /// <param name="HealAmount">The amount to increase health value by.</param>
        public void Heal(float HealAmount)
        {
            // Check if the entity is already dead. If so, return without doing anything
            if (isDead) return;

            // Update the value by adding the HealAmount, and clamp it between the minValue and maxValue
            value = Mathf.Clamp(value += HealAmount, minValue, maxValue);

            //Play Heal Audio Clip If Available
            PlayHealAudio();

            // Invoke the OnHealEvent, which will notify any listeners that the entity has been healed
            InvokeOnHealEvent(HealAmount);
        }

        /// <summary>
        /// Decrease the health value by the specified amount.
        /// </summary>
        /// <param name="DamageAmount">The amount to decrease health value by.</param>
        public void Damage(float DamageAmount)
        {
            // Check if the entity is already dead or invincible. If so, return without doing anything
            if (isDead || invincible) return;

            // Update the value by subtracting the DamageAmount, and clamp it between the minValue and maxValue
            value = Mathf.Clamp(value -= DamageAmount, minValue, maxValue);

            // If the entity is not dead, invoke the OnDamageEvent to notify any listeners that it has taken damage
            InvokeOnDamageEvent(DamageAmount);

            // If the value has reached the minValue, the entity is dead
            if (value == minValue)
            {
                // Set the isDead flag to true
                isDead = true;

                // Start a coroutine to self-destruct the entity after a short delay
                StartCoroutine(AfterDeathSelfDestruct());

                // Call the AfterDeathFunction, which will perform any necessary cleanup
                AfterDeathFunction();

                //Play Death Audio Clip If Available
                PlayDeathAudio();

                // Invoke the OnDeathEvent, which will notify any listeners that the entity has died
                InvokeOnDeathEvent(DamageAmount);

                // Return without invoking the OnDamageEvent
                return;
            }


            //Play Damage Audio Clip If Available
            PlayDamageAudio();

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Makes the entity invincible for a short time after it spawns
        /// </summary>
        IEnumerator InvincibleAfterSpawn()
        {
            yield return new WaitForSeconds(timeInvincibleAfterSpawn);

            if (timeInvincibleAfterSpawn > 0 && invincible)
                invincible = false;

        }

        /// <summary>
        /// Coroutine that handles self-destruction or deactivation of the entity after it dies.
        /// </summary>
        private IEnumerator AfterDeathSelfDestruct()
        {
            // If the onDeathDoSelf flag is set to something other than None and there is a delay specified, wait for the delay
            if (onDeathDoSelf != OnDeathSelf.None && afterDeathDelay > 0)
                yield return new WaitForSeconds(AfterDeathDelay);

            // Depending on the value of the onDeathDoSelf flag, either deactivate or destroy the entity
            switch (onDeathDoSelf)
            {
                case OnDeathSelf.Deactivate:
                    gameObject.SetActive(false);
                    break;

                case OnDeathSelf.Destroy:
                    Destroy(gameObject);
                    break;
            }

            // Return null to end the coroutine
            yield return null;
        }

        /// <summary>
        /// Function that handles after death functionalities of objects when the entity dies.
        /// </summary>
        private void AfterDeathFunction()
        {
            // If there are objects to be spawned on death, instantiate them at the entity's position
            if (spawnObjectsOnDeath.Count > 0)
            {
                foreach (GameObject obj in spawnObjectsOnDeath)
                {
                    if (obj != null)
                        Instantiate(obj, gameObject.transform.position, Quaternion.identity);
                }
            }

            // If there are objects to be destroyed on death, destroy them
            if (destroyObjectsOnDeath.Count > 0)
            {
                foreach (GameObject obj in destroyObjectsOnDeath)
                {
                    if (obj != null)
                        Destroy(obj);
                }
            }
        }

        #endregion

        #region Audio Methods

        /// <summary>
        /// Plays a random audio clip from a list of audio clips for the heal event, if enabled.
        /// </summary>
        private void PlayHealAudio()
        {
            // Check if audio is enabled and there are audio clips for heal event
            if (!enableAudio) return;
            if (healAudio.Count <= 0) return;

            // Pick a random audio clip from the list
            int index = UnityEngine.Random.Range(0, healAudio.Count);

            // Check if the selected audio clip is not null
            if (healAudio[index] == null)
            {
                Debug.LogError($"Missing Heal Audio clip at index {index}");
                return;
            }

            // Play the selected audio clip
            PlayAudio(healAudio[index]);
        }

        /// <summary>
        /// Plays a random audio clip from a list of audio clips for the damage event, if enabled.
        /// </summary>
        private void PlayDamageAudio()
        {
            // Check if audio is enabled and there are audio clips for damage event
            if (!enableAudio) return;
            if (damageAudio.Count <= 0) return;

            // Pick a random audio clip from the list
            int index = UnityEngine.Random.Range(0, damageAudio.Count);

            // Check if the selected audio clip is not null
            if (damageAudio[index] == null)
            {
                Debug.LogError($"Missing Damage Audio clip at index {index}");
                return;
            }

            // Play the selected audio clip
            PlayAudio(damageAudio[index]);
        }

        /// <summary>
        /// Plays a random audio clip from a list of audio clips for the death event, if enabled.
        /// </summary>
        private void PlayDeathAudio()
        {
            // Check if audio is enabled and there are audio clips for death event
            if (!enableAudio) return;
            if (deathAudio.Count <= 0) return;

            // Pick a random audio clip from the list
            int index = UnityEngine.Random.Range(0, deathAudio.Count);

            // Check if the selected audio clip is not null
            if (deathAudio[index] == null)
            {
                Debug.LogError($"Missing Death Audio clip at index {index}");
                return;
            }

            // Play the selected audio clip
            PlayAudio(deathAudio[index]);
        }

        /// <summary>
        /// Plays the given audio clip using an AudioSource component on the entity.
        /// If the entity does not have an AudioSource, one will be added.
        /// </summary>
        /// <param name="clip">The audio clip to play.</param>
        private void PlayAudio(AudioClip clip)
        {
            // Check if the entity has an AudioSource component, add one if it does not
            if (!audioSource)
            {
                if (!TryGetComponent(out audioSource))
                    audioSource = gameObject.AddComponent<AudioSource>();
            }

            // Set the audio clip and play it
            audioSource.clip = clip;
            audioSource.Play();
        }


        #endregion

        #region Event Invoking

        /// <summary>
        /// Invoke the OnHealEvent, which will notify any listeners that the entity has been healed.
        /// </summary>
        private void InvokeOnHealEvent(float HealAmount)
        {
            OnHeal?.Invoke(HealAmount);
            OnHealEvent?.Invoke(HealAmount);
        }

        /// <summary>
        /// Invoke the OnDamageEvent, which will notify any listeners that the entity has dealt a damage.
        /// </summary>
        private void InvokeOnDamageEvent(float DamageAmount)
        {
            OnDamage?.Invoke(DamageAmount);
            OnDamageEvent?.Invoke(DamageAmount);
        }

        /// <summary>
        /// Invoke the OnDeathEvent, which will notify any listeners that the entity has been dead.
        /// </summary>
        private void InvokeOnDeathEvent(float DamageAmount)
        {
            OnDeath?.Invoke(DamageAmount);
            OnDeathEvent?.Invoke(DamageAmount);
        }

        #endregion


    }


}
