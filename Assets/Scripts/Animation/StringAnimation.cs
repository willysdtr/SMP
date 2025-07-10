using UnityEngine;
using UnityEngine.EventSystems;

public class StringAnimation : MonoBehaviour
{
    private InputSystem_Actions inputActions;
    void Awake()
    {
        inputActions.Stirng.nami.performed += ctx =>
        {
            PlayAnimarion();
        };
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    void PlayAnimarion()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Play"); // アニメーションを再生
        }
    }
    void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Enable();  

    }
    void OnDisable()
    {
        if (inputActions == null)
        {
            inputActions = new InputSystem_Actions();
        }
        inputActions.Stirng.Disable();
    }
}
