using System;
using UnityEngine;
using UnityEngine.UI;

public class DebugData : MonoBehaviour
{
    [SerializeField]
    private Text iterationText, calculationTime;

    private readonly string iterString = "Number of iterations: ";
    private readonly string calcTString = "Calculation time: ";

    public void SetActiveTo(bool b)
    {
        iterationText.gameObject.SetActive(b);
        calculationTime.gameObject.SetActive(b);
    }

    public void UpdateData(float calcT, int iter)
    {
        //Debug.Log("calc time = " + calcT + ". Iterations: " + iter);
        calcT = (float)Math.Round(calcT, 3);
        iterationText.text = iterString + iter;
        calculationTime.text = calcTString + calcT + "s";
    }
}
