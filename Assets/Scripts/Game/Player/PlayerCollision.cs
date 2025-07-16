using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class PlayerCollision : MonoBehaviour
{

    private PlayerStateMachine state_ma;

    private HashSet<GameObject> ground_obj = new HashSet<GameObject>();
    private HashSet<GameObject> wall_obj = new HashSet<GameObject>();

    private Rigidbody2D rb;

    [SerializeField] private Vector2 checkSize = new Vector2(0.5f, 1.0f);
    [SerializeField] private Vector2 checkOffset = new Vector2(0f, 0f);
    [SerializeField] private LayerMask climbLayer;

    public Vector2 hitobj_pos { get; private set; } = new Vector2(0.0f,0.0f);
    private int groundcount = 0;
    private bool moveFg;
    private bool jumpFg;
    private bool isground;
    private bool celling_hit;
    private bool ngFg;
    private bool climbFg;
    private bool ishit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        state_ma = GetComponent<PlayerStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector2 center = (Vector2)transform.position + checkOffset;

        Collider2D hit = Physics2D.OverlapBox(center, checkSize, 0f, climbLayer);
        
        ishit = hit;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 center = (Vector2)transform.position + checkOffset;
        Gizmos.DrawWireCube(center, checkSize);
    }

    public void JumpWithVelocity(Vector3 initialVelocity)
    {
        rb.linearVelocity = initialVelocity; // ← Rigidbodyのvelocityに直接代入でOK
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
        {
            if (collision.gameObject.tag == "Spring")
            {
                state_ma.SetJumpFg(true);
                state_ma.SetGimJumpFg(true);
                state_ma.SetMoveFg(false);
                JumpLine pad = collision.gameObject.GetComponent<JumpLine>();
                if (pad != null)
                {
                    //JumpWithVelocity(pad.GetInitialVelocity());
                    state_ma.SetInitVelocity(pad.GetInitialVelocity());
                }
                transform.position = new Vector3(collision.transform.position.x, transform.position.y, 0);
            }
            else
            {
                foreach (ContactPoint2D contact in collision.contacts)
                {

                    // 上向きに接触した場合のみカウント
                    if (contact.normal == Vector2.up)
                    {
                        state_ma.SetIsGround(true);
                        ground_obj.Add(collision.gameObject);

                    }
                    // 横向きに接触した場合のみカウント
                    if (contact.normal == Vector2.left || contact.normal == Vector2.right)
                    {
                        wall_obj.Add(collision.gameObject);
                        state_ma.SetMoveFg(false);
                    }
                }
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        ground_obj.Remove(collision.gameObject);
        wall_obj.Remove(collision.gameObject);
        if (ground_obj.Count == 0)
        {
           state_ma.SetIsGround(false);
        }

        if (wall_obj.Count == 0)
        {
           state_ma.SetMoveFg(true);
        }

    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "String")
        {
            state_ma.SetMoveFg(false);
            state_ma.SetClimbFg(true);
            state_ma.SetHitObjPos(collider.transform.position);
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (!ishit)
        {
            if (collider.gameObject.tag == "String")
            {
                state_ma.SetClimbFg(false);
                rb.bodyType = RigidbodyType2D.Dynamic;
                rb.linearVelocity = Vector2.zero;
            }
        }
        
    }

    //void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Wrinkles"))
    //    {
    //        //state_ma.SetDownFg(true);
    //    }
    //    if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
    //    {
    //        foreach (ContactPoint2D contact in collision.contacts)
    //        {
    //            Vector2 normal = contact.normal;

    //            if (normal == Vector2.left || normal == Vector2.right)
    //            {
    //                state_ma.SetMoveFg(true);
    //            }

    //            if (normal == Vector2.up)
    //            {
    //                state_ma.SetIsGround(true);
    //                if (collision.gameObject.CompareTag("Spring"))
    //                {
    //                    state_ma.SetGimJumpFg(true);
    //                }
    //            }

    //        }
    //    }
    //}


    //void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (((1 << collision.gameObject.layer) & state_ma.groundlayers) != 0)
    //    {
    //        foreach (ContactPoint2D contact in collision.contacts)
    //        {
    //            Vector2 normal = contact.normal;

    //            if (normal == Vector2.left || normal == Vector2.right)
    //            {
    //                state_ma.SetMoveFg(false);
    //            }

    //            if (Vector2.Dot(normal, Vector2.up) > 0.7f) //「上方向」判定
    //            {
    //                state_ma.SetIsGround(false);
    //                Debug.LogError("落下判定");
    //            }

    //        }
    //    }
    //}

}
