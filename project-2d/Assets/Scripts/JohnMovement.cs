using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JohnMovement : MonoBehaviour
{
    public GameObject BulletPrefab;
    public float Speed;
    public float JumpForce;

    private Rigidbody2D Rigidbody2D;
    private Animator Animator;
    private float Horizontal;
    private bool Grounded;
    private int jumpCount;
    private float LastShoot;
    private int Health = 5;
    private bool isDead = false;

    void Start()
    {
        Rigidbody2D = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        jumpCount = 0;
    }

    void Update()
    {
        if (isDead) return; // Si el personaje est� muerto, no hacer nada

        Horizontal = Input.GetAxisRaw("Horizontal");

        // Cambiar la direcci�n del personaje
        if (Horizontal < 0.0f)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (Horizontal > 0.0f)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        // Activar animaci�n de correr
        if (Animator != null)
        {
            Animator.SetBool("running", Horizontal != 0.0f);
        }

        // Detectar si el personaje est� en el suelo
        Grounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f);

        if (Grounded)
        {
            jumpCount = 0;
            if (Animator != null)
            {
                Animator.SetBool("jumping", false);
                Animator.SetBool("falling", false);
            }
        }
        else if (Rigidbody2D.velocity.y < 0)
        {
            if (Animator != null)
            {
                Animator.SetBool("falling", true);
            }
        }

        // Salto
        if (Input.GetKeyDown(KeyCode.W))
        {
            Jump();
        }

        // Disparo
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > LastShoot + 0.25f)
        {
            Shoot();
            LastShoot = Time.time;
        }
    }

    private void Jump()
    {
        if (jumpCount < 1)
        {
            Rigidbody2D.AddForce(Vector2.up * JumpForce);
            jumpCount++;

            if (Animator != null)
            {
                Animator.SetBool("jumping", true);
                Animator.SetBool("falling", false);
            }
        }
    }

    private void Shoot()
    {
        Vector3 direction;
        if (transform.localScale.x == 1.0f) direction = Vector2.right;
        else direction = Vector2.left;

        GameObject bullet = Instantiate(BulletPrefab, transform.position + direction * 0.1f, Quaternion.identity);
        bullet.GetComponent<BulletScript>().SetDirection(direction);
    }

    private void FixedUpdate()
    {
        if (isDead) return;
        Rigidbody2D.velocity = new Vector2(Horizontal * Speed, Rigidbody2D.velocity.y);
    }

    public void Hit()
    {
        Health -= 1;
        if (Health <= 0 && !isDead)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        if (Animator != null)
        {
            Animator.SetTrigger("die");
        }

        Rigidbody2D.velocity = Vector2.zero;
        Rigidbody2D.gravityScale = 0f;

        GetComponent<Collider2D>().enabled = false;

        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(1.0f);
        Destroy(gameObject);
    }

}
