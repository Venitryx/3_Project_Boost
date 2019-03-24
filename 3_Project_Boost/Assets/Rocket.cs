using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 100f;
    Rigidbody rigidbody;
    AudioSource audioSource;

    enum State { Alive, Dying, Transcending};
    State state = State.Alive;

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
        Thrust();
        Rotate();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) return; //ignores collisions when dead
        switch(collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                Invoke("LoadNextScene", 1f); //parameterize this time
                break;
            default:
                state = State.Dying;
                Invoke("LoadLevelOne", 1f); //parameterize this time
                break;
        }
        
    }

    private void LoadLevelOne()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(1); //todo allow for more than two levels
    }

    private void Rotate()
    {
        rigidbody.freezeRotation = true; //freeze physics
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A) && state == State.Alive)
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D) && state == State.Alive)
        {
            transform.Rotate(Vector3.back * rotationThisFrame);
        }

        rigidbody.freezeRotation = false; //resume physics
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space) && state == State.Alive)
        {
            rigidbody.AddRelativeForce(Vector3.up * mainThrust);
            if (!audioSource.isPlaying) audioSource.Play();
        }
        else audioSource.Stop();
    }
}
