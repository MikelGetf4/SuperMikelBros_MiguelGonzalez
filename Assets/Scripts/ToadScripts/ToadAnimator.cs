using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToadAnimator : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator animator;
    private float direction;
    private ToadController toadController;

    private void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.animator = GetComponent<Animator>();
        toadController = GetComponent<ToadController>();
    }

    private void Update()
    {
        this.direction = Input.GetAxisRaw("Horizontal");
    }

    private void LateUpdate()
    {
        this.animator.SetFloat("VelocidadX", Mathf.Abs(this.rb.velocity.x));
        this.animator.SetFloat("VelocidadY", this.rb.velocity.y);
        animator.SetBool("EnSuelo", toadController.estaEnSuelo);
        animator.SetBool("Agachado", toadController.agachado);
        animator.SetBool("Muerto", toadController.muerto);

        if (direction != 0 && Mathf.Sign(direction) != Mathf.Sign(rb.velocity.x))
        {
            this.animator.SetBool("Sliding", true);
        }
        else
        {
            this.animator.SetBool("Sliding", false);
        }
    }
}
