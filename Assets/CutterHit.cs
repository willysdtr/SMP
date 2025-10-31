using UnityEngine;

public class CutterHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D collider)
    {

        if (collider.tag == "Player")
        {
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            player.cutCt++;
            gameObject.SetActive(false);

        }
    }
}
