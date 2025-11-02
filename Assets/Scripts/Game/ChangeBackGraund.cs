using UnityEngine;

public class ChangeBackGraund : MonoBehaviour
{
    [SerializeField] private GameObject m_BaackGround_1;
    [SerializeField] private GameObject m_BaackGround_2;
    [SerializeField] private GameObject m_BaackGround_3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int i= SMPState.CURRENT_STAGE/5;
        switch (i)
        { 
            case 0:
                m_BaackGround_1.SetActive(true);
                break;
            case 1:
                m_BaackGround_2.SetActive(true);
                break;
            case 2:
                m_BaackGround_3.SetActive(true);
                break;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
