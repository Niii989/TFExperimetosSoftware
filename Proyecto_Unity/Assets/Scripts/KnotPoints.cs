using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using System;

public class KnotPoints : MonoBehaviour {

    Quaternion[] theta1Array = new Quaternion[5];
    Quaternion[] theta2Array = new Quaternion[5];
    Quaternion[] theta3Array = new Quaternion[5];
    Vector3[] positionsArm = new Vector3[5];
    

	public InputField time;
    public GameObject[] checks = new GameObject[5];
	public Transform BaseRotation;
	public Transform Link1Rotation;
	public Transform Link2Rotation;
    public GameObject jointPos;

	int knotPoints = 0;
	bool nextPoint;
    string formatoNombre = "_" + DateTime.Today.Year.ToString() + "_" + DateTime.Today.Month.ToString() + "_" + DateTime.Today.Day.ToString();

    //hide all icons
    void Start () 
	{
		for (int i = 0; i < checks.Length; i++)
		{
			checks[i].SetActive(false);
		}
	}

	//executed when "save position" is clicked
	public void SavePoint()
	{
		//saves position, up to a maximum of 5
        if (knotPoints < 5)
		{
			theta1Array[knotPoints] = BaseRotation.localRotation;
			theta2Array[knotPoints] = Link1Rotation.localRotation;
			theta3Array[knotPoints] = Link2Rotation.localRotation;
            positionsArm[knotPoints] = jointPos.transform.position;

			checks[knotPoints].SetActive(true);

			knotPoints++;
		}
	}

	//clear all saved points
	public void ClearPoints()
	{
		knotPoints = 0;
		for (int i = 0; i < checks.Length; i++)
		{
			checks[i].SetActive(false);
		}
						
	}

	//runs when "go to saved" is pressed
	public void GoThroughPoints()
	{
		if(time.text != null)
		{
			StartCoroutine(RotateMe(float.Parse(time.text))) ;
		}
	}

	//takes robot through all knot points over desired time
	IEnumerator RotateMe(float inTime)
	{
		DHParameters.setMoveSlider (true);
		for (int i = 0; i < knotPoints; i++)
		{
			Quaternion baseFromAngle = BaseRotation.localRotation;
			Quaternion link1FromAngle = Link1Rotation.localRotation;
			Quaternion link2FromAngle = Link2Rotation.localRotation;

			for(float t = 0f ; t < 1f ; t += Time.deltaTime/inTime)
			{
				BaseRotation.localRotation = Quaternion.Lerp(baseFromAngle, theta1Array[i], t);
				Link1Rotation.localRotation = Quaternion.Lerp(link1FromAngle, theta2Array[i], t);
				Link2Rotation.localRotation = Quaternion.Lerp(link2FromAngle, theta3Array[i], t);

				yield return null ;
			}
		}
		DHParameters.setMoveSlider (false);
	}

    public void DownloadCodeMELFA()
    {
        Vector3 positionAux;
        if (checks.Length > 0)
        {
            string filename = "C:/Users/BP2549/Downloads/Program_Code_" + formatoNombre+".txt";
   
            StreamWriter sw = File.CreateText(filename);

            sw.WriteLine("MELFA CODE");
            sw.WriteLine("OVRD " + Convert.ToString(Decimal.Round(1/Decimal.Parse(time.text),2)));
            for (int i = 0; i < knotPoints; i++)
            {
                positionAux = positionsArm[i];
                sw.WriteLine("MOV " + positionAux.x.ToString() + ", " + positionAux.y.ToString() + ", " + positionAux.z.ToString());
            }

            sw.Close();               
            
        }
    }

}
