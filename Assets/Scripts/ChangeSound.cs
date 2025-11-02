using UnityEngine;

public class ChangeSound : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // AudioSource をここにアタッチ
    [SerializeField] private AudioClip gameMusic_1;
    [SerializeField] private AudioClip gameMusic_2;
    [SerializeField] private AudioClip gameMusic_3;

    void Start()
    {
        int i = SMPState.CURRENT_STAGE / 5;

        switch (i)
        {
            case 0:
                audioSource.clip = gameMusic_1;
                break;
            case 1:
                audioSource.clip = gameMusic_2;
                break;
            case 2:
                audioSource.clip = gameMusic_3;
                break;
        }

       audioSource.Play(); // 新しいクリップを再生
    }
}