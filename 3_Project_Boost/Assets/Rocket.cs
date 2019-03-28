using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 1200f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngineSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip levelFinishSound;
    [SerializeField] ParticleSystem starboardEngineParticles;
    [SerializeField] ParticleSystem portEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem levelFinishParticles;

    [SerializeField] int levelID;

    Rigidbody rigidbody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending };
    State state = State.Alive;

    bool isCollidersOn = true;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        RespondToDebugInput();
        if (state == State.Alive)
        {

            RespondToThrustInput();
            RsepondToRotateInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return; //ignores collisions when dead

        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
        
    }

    private void StartDeathSequence()
    {
        if (!isCollidersOn) return;

        state = State.Dying;
        audioSource.Stop();
        portEngineParticles.Stop();
        starboardEngineParticles.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        Invoke("ReloadScene", levelLoadDelay); //parameterize this time
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        portEngineParticles.Stop();
        starboardEngineParticles.Stop();
        audioSource.PlayOneShot(levelFinishSound);
        levelFinishParticles.Play();
        Invoke("LoadNextScene", levelLoadDelay); //parameterize this time
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(levelID);
    }

    private void LoadNextScene()
    {
        levelID++;
        SceneManager.LoadScene(levelID); //todo allow for more than two levels
        
    }

    private void RsepondToRotateInput()
    {
        rigidbody.freezeRotation = true; //freeze physics
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidbody.freezeRotation = false; //resume physics
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            starboardEngineParticles.Stop();
            portEngineParticles.Stop();
        }
    }

    private void RespondToDebugInput()
    {
        if(Input.GetKey(KeyCode.C))
        {
            TogglePlayerColliders();
        }
        else if(Input.GetKey(KeyCode.N))
        {
            SceneManager.LoadScene(1);
        }
        else if (Input.GetKey(KeyCode.P))
        {
            SceneManager.LoadScene(0);
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngineSound);
            portEngineParticles.Play();
            starboardEngineParticles.Play();
        }
    }

    private void TogglePlayerColliders()
    {
        if (isCollidersOn)
        {
            isCollidersOn = false;
        }
        else isCollidersOn = true;
    }
}
