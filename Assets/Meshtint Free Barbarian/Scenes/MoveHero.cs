using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveHero : MonoBehaviour
{
    [SerializeField] private Camera _myCamera;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _sprintSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 15f;
    [SerializeField] private float _animationBlendSpeed = 2f;
    [SerializeField] private float _jumpSpeed = 15f;

    private CharacterController _heroController;
    private Animator _anim;

    private float _heroDesiredRotation = 0f;
    private float _heroDesiredAnimationSpeed = 0f;
    private bool _heroSprinting = false;

    private float _hSpeedY = 0;
    private float _hGravity = -9.81f;

    private bool _heroJumping = false;

    private void Start()
    {
        _heroController = GetComponent<CharacterController>();
        _anim = GetComponent<Animator>();
    }

    private void Update()
    {
        MovePlayer();

        if (Input.GetKeyDown(KeyCode.Mouse0))
            StartCoroutine(Attack());
    }

    public void MovePlayer()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump") && !_heroJumping)
        {
            _heroJumping = true;
            _anim.SetTrigger("Jump");

            _hSpeedY += _jumpSpeed;
        }
        if (!_heroController.isGrounded)
        {
            _hSpeedY += _hGravity * Time.deltaTime;
        }
        else if (_hSpeedY < 0)
        {
            _hSpeedY = 0;
        }
        _anim.SetBool("Jump", _heroJumping);

        if (_heroJumping && _hSpeedY < 0)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, .5f, LayerMask.GetMask("Default")))
            {
                _heroJumping = false;
                _anim.SetTrigger("Land");
            }
        }

        _heroSprinting = Input.GetKey(KeyCode.LeftShift);
        Vector3 movement = new Vector3(x, 0, z).normalized;

        Vector3 rotatedMovement = Quaternion.Euler(0, _myCamera.transform.rotation.eulerAngles.y, 0) * movement;
        Vector3 verticalMovement = Vector3.up * _hSpeedY;

        _heroController.Move((verticalMovement + (rotatedMovement * (_heroSprinting ? _sprintSpeed : _speed))) * Time.deltaTime);

        if (rotatedMovement.magnitude > 0)
        {
            _heroDesiredRotation = Mathf.Atan2(rotatedMovement.x, rotatedMovement.z) * Mathf.Rad2Deg;
            _heroDesiredAnimationSpeed = _heroSprinting ? 1 : .5f;
        }
        else
        {
            _heroDesiredAnimationSpeed = 0;
        }

        _anim.SetFloat("Speed", Mathf.Lerp(_anim.GetFloat("Speed"), _heroDesiredAnimationSpeed, _animationBlendSpeed * Time.deltaTime));

        Quaternion currentRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, _heroDesiredRotation, 0);
        transform.rotation = Quaternion.Lerp(currentRotation, targetRotation, _rotationSpeed * Time.deltaTime);

        //    float moveZ = Input.GetAxis("Vertical");
        //    float moveX = Input.GetAxis("Horizontal");

        //    // Update the move Deltas
        //    _moveVector.x = moveX;
        //    _moveVector.z = moveZ;
        //    _moveVector.Normalize();

        //    Vector3 direction = new Vector3(0, 0, moveZ);
        //    direction = transform.TransformDirection(direction);

        //    if (_isGrounded)
        //    {
        //        if (direction != Vector3.zero && !Input.GetKey(KeyCode.LeftShift))
        //            Walk();
        //        else if (direction != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        //            Run();
        //        else if (direction == Vector3.zero)
        //            Idle();

        //        direction *= _moveSpeed;

        //        if (Input.GetKey(KeyCode.Space))
        //            Jump();
        //    }

        //    _controller.Move(direction * Time.deltaTime);

        //    _velocity.y += _gravity * Time.deltaTime;
        //    _controller.Move(_velocity * Time.deltaTime);
    }

    private IEnumerator Attack()
    {
        _anim.SetLayerWeight(_anim.GetLayerIndex("Attack Layer"), 1);
        _anim.SetTrigger("Attack");

        yield return new WaitForSeconds(0.9f);
        _anim.SetLayerWeight(_anim.GetLayerIndex("Attack Layer"), 0);
    }
    //private void Idle()
    //{
    //    _anim.SetFloat("Speed", 0, 0.1f, Time.deltaTime);
    //}

    //private void Walk()
    //{
    //    _moveSpeed = _walkSpeed;
    //    _anim.SetFloat("Speed", 0.5f, 0.1f, Time.deltaTime);
    //}

    //private void Run()
    //{
    //    _moveSpeed = _runSpeed;
    //    _anim.SetFloat("Speed", 1, 0.1f, Time.deltaTime);
    //}

    //private void Jump()
    //{
    //    _velocity.y = Mathf.Sqrt(_jumpHeight * -2 * _gravity);
    //}
}
