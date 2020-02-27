using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CsDamageText : MonoBehaviour
{
    float m_moveSpeed = 5f;
    float m_destroyTime = 5f;

    [SerializeField]
    Text m_TextDamage;


    // Start is called before the first frame update
    void Start()
    {
        m_moveSpeed = 3f;
        m_destroyTime = 3f;
        m_TextDamage = gameObject.GetComponent<Text>();
    }

    public void SetText(int nDamage)
    {
        Debug.Log(nDamage);
        m_TextDamage.text = nDamage.ToString();
        m_TextDamage.transform.position = new Vector3(m_TextDamage.transform.position.x + UnityEngine.Random.Range(-20, 20),
            m_TextDamage.transform.position.y + UnityEngine.Random.Range(-20, 20),
            m_TextDamage.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_destroyTime >= 0)
        {
            Vector3 vectorUpText = new Vector3(m_TextDamage.transform.position.x, m_TextDamage.transform.position.y
            + (m_moveSpeed + Time.deltaTime), m_TextDamage.transform.position.z);

            m_TextDamage.transform.position = vectorUpText;

            m_destroyTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
