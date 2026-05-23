using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;

public class Controller : NetworkBehaviour
{
    [SerializeField] PlayerInput CookieInput;
    [SerializeField] Rigidbody CookieBody;
    [SerializeField] NetworkAnimator CookieAnimator;
    [SerializeField] float MoveSpeed;
    [SerializeField] float RotateSpeed;
    [SerializeField] int JumpCount;
    [SerializeField] bool Grounded;
    [SerializeField] bool Idle;
    [SerializeField] bool Hop;

    // Update is called once per frame
    void Update()
    {
        if (isLocalPlayer)
        {
            float Movement = CookieInput.actions["Move"].ReadValue<Vector2>().y;
            float Rotation = CookieInput.actions["Move"].ReadValue<Vector2>().x;

            if (Movement != 0)
            {
                transform.Translate(0, 0, Movement * MoveSpeed * Time.deltaTime);

                if (!Hop && Grounded)
                {
                    CookieBody.AddForce(0, 3, 0, ForceMode.Impulse);

                    if (Movement > 0)
                    {
                        Animate("Hop");
                    }

                    if (Movement < 0)
                    {
                        Animate("ReverseHop");
                    }

                    Hop = true;
                }

                Idle = false;
            }

            if (Movement == 0 && Grounded && CookieAnimator.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && !Idle)
            {
                Animate("Idle");
                Idle = true;
            }

            if (Rotation != 0)
            {
                transform.Rotate(0, Rotation * RotateSpeed * Time.deltaTime, 0);
            }

            if (CookieInput.actions["Jump"].WasCompletedThisFrame() && JumpCount < 2)
            {
                JumpCount += 1;

                switch (JumpCount)
                {
                    case 1:
                        Animate("Jump");
                        break;
                    case 2:
                        Animate("Jump2");
                        break;
                }

                CookieBody.linearVelocity = Vector3.zero;
                CookieBody.AddForce(0, 5, 0, ForceMode.Impulse);
                Idle = false;
                Hop = false;
            }
        }
    }

    void Animate(string TriggerName)
    {
        CookieAnimator.SetTrigger(TriggerName);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isLocalPlayer)
        {
            Grounded = true;

            if (JumpCount > 0)
            {
                JumpCount = 0;
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (isLocalPlayer)
        {
            Grounded = false;

            if (Hop)
            {
                Hop = false;
            }
        }
    }
}
