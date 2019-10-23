using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 회사 문의 연락처 보여주기.
/// </summary>
public class ComTelManager : MonoBehaviour
{
    public GameObject standardDiscripsionLabel;
    public GameObject companyLabel;
    public bool isStandardLabel;

    private void Start()
    {
        isStandardLabel = true;
    }
    void OnClick()
    {
        isStandardLabel = !isStandardLabel;
    }

    void Update()
    {
        if (isStandardLabel)
        {
            standardDiscripsionLabel.gameObject.SetActive(true);
            companyLabel.gameObject.SetActive(false);
        }
        else
        {
            standardDiscripsionLabel.gameObject.SetActive(false);
            companyLabel.gameObject.SetActive(true);
        }
    }
}
