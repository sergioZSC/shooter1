
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float jumpForce = 3f;
    public float fireRange = 5f;
    public float playerDamage = 20f;
    public float rotationYSensitivity = 10f;
    public float rotationXSensitivity = 1f;
    public TMP_Text scoreLabel;

    int score;
    public GameObject explosion;
    public ParticleSystem muzzleFlashParticle;
    public Image faceImage;
    public Sprite[] faceStages;

    public float health = 100;

    private PlayerInputAction mInputAction;
    private InputAction mMovementAction;
    private InputAction mViewAction;
    private Rigidbody mRigidbody;
    private Transform mFirePoint;
    private Transform mCameraTransform;
    private float mRotationX = 0f;
    private bool jumpPressed = false;
    private bool onGround = true;


    private void Awake()
    {
        mInputAction = new PlayerInputAction();
        mRigidbody = GetComponent<Rigidbody>();
        mFirePoint = transform.Find("FirePoint");
        mCameraTransform = transform.Find("Main Camera");

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnEnable()
    {
        // Codigo que se ejecutara al habilitar un GO
        mInputAction.Player.Jump.performed += DoJump;
        mInputAction.Player.Jump.Enable();

        mInputAction.Player.Fire.performed += DoFire;
        mInputAction.Player.Fire.Enable();

        mViewAction = mInputAction.Player.View;
        mInputAction.Player.View.Enable();

        mMovementAction = mInputAction.Player.Movement;
        mMovementAction.Enable();

    }


    private void DoFire(InputAction.CallbackContext obj)
    {
        muzzleFlashParticle.Play(true);

        // Lanzar un raycast
        if (Physics.Raycast(
            mFirePoint.position,
            mCameraTransform.forward,
            out RaycastHit hit,
            fireRange
        ))
        {
            // Hubo una colision
            Debug.Log(hit.collider.name);
            GameObject nuevaExplosion =
                Instantiate(explosion, hit.point, transform.rotation);
            Destroy(nuevaExplosion, 1f);

            var enemyHit = hit.collider.GetComponent<EnemyController>();
            if (enemyHit != null)
            {
                enemyHit.health -= playerDamage / (enemyHit.data.defense + 1);
                if (enemyHit.health <= 0)
                    score += enemyHit.data.scoreYield;
            }
        }

        Debug.DrawRay(mFirePoint.position,
            transform.forward * fireRange,
            Color.red,
            .25f
        );
    }

    private void OnDisable()
    {
        // Codigo que se ejecutara al deshabilitar un GO
        mInputAction.Player.Jump.Disable();
        mMovementAction.Disable();
        mInputAction.Disable();
        //mInputAction.Player.View.Disable();
    }

    private void Update()
    {
        health = Mathf.Max(0, health);

        int imageToUse = Mathf.FloorToInt(health / (100f / (faceStages.Length - 1)));
        faceImage.sprite = faceStages[imageToUse];

        scoreLabel.text = $"Score: {score}";

        #region Rotacion
        Vector2 deltaPos = mViewAction.ReadValue<Vector2>();
        transform.Rotate(
            Vector3.up * deltaPos.x * Time.deltaTime * rotationYSensitivity
        );

        mRotationX -= deltaPos.y * rotationXSensitivity;
        mCameraTransform.localRotation = Quaternion.Euler(
            Mathf.Clamp(mRotationX, -90f, 90f),
            0f,
            0f
        );
        #endregion

        #region Movimiento
        Vector2 movement = Vector2.ClampMagnitude(
            mMovementAction.ReadValue<Vector2>(),
            1f
        );

        mRigidbody.velocity = movement.x * transform.right * moveSpeed +
            movement.y * transform.forward * moveSpeed +
            transform.up * mRigidbody.velocity.y;
        /*mRigidbody.velocity = new Vector3(
            movement.x * moveSpeed,
            mRigidbody.velocity.y,
            movement.y * moveSpeed
        );*/
        #endregion

        #region Salto
        if (jumpPressed && onGround)
        {
            mRigidbody.velocity += Vector3.up * jumpForce;
            jumpPressed = false;
            onGround = false;
        }
        #endregion
    }

    private void DoJump(InputAction.CallbackContext obj)
    {
        jumpPressed = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("collision");
        onGround = true;
        jumpPressed = false;
    }

}
