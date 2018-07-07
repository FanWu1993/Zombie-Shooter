using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{

    public GameObject bullet_to_spawn;

    public float fire_Rate = 1f;
    private float nextFire = 0.0f;
    private float reSetTime = 0.0f;
    public float movement_speed = 6f;
    public float rotation_speed = 50f;

    private AudioSource audioSource;
    Animator anim;
    private GameObject bullet;
    private bool reSetGame=false;
    private bool alive = true;

    //private GameObject bullet;

    void Start()
    {
        this.gameObject.transform.position = new Vector3(0f, 0f, 0f);
        anim = gameObject.GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        anim.SetBool("Walk", false);
        anim.SetBool("Shoot", false);

        if (alive) { 
        // MOVEMENT
        float horizontal_input = -Input.GetAxis("Horizontal");
        float vertical_input = Input.GetAxis("Vertical");

        // Horizontal is rotation
        this.transform.Rotate(new Vector3(0, 0, horizontal_input * rotation_speed * Time.deltaTime));

        // Vertical is forwards/backwards
        if (vertical_input!=0 || horizontal_input!=0)
        {
            anim.SetBool("Walk", true);
            this.transform.Translate(Vector2.up * vertical_input * movement_speed * Time.deltaTime);
        }

        // SHOOTING
        bool is_shooting = Input.GetButton("Fire1");

        
        // Is the user holding down LEFT CLICK?
        if (is_shooting && Time.time > nextFire)
        {
            // Create the bullet
            bullet = (GameObject)Instantiate(bullet_to_spawn, transform.position, Quaternion.Euler(new Vector3(0, 0, 1)));
            bullet.transform.rotation = transform.rotation;

            // Give it a velocity
            //bullet.GetComponent<Rigidbody2D>().AddForce(200 * new Vector2(Mathf.Cos((transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad)
            //, Mathf.Sin((transform.rotation.eulerAngles.z + 90) * Mathf.Deg2Rad)));
            Rigidbody2D bullet_phsics = bullet.GetComponent<Rigidbody2D>();
            bullet_phsics.velocity = 7 * this.transform.up;
            //Destroy(bullet, 10);

            // Set the shooting cooldown 

            nextFire = Time.time + fire_Rate;

            anim.SetBool("Shoot", true);
        }
        }
        if (reSetGame && reSetTime + 3f < Time.time)
        {

            //oGameObject.Find("UIManager").SendMessage("TimerReset");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);

            reSetGame = false;
        }

    }
    

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag == "Zombie")
        {
            audioSource.Play();
            GetComponent<Collider2D>().enabled = false;
            alive = false;
            reSetTime = Time.time;
            reSetGame = true;
        }
    }
}