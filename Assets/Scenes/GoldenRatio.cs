using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldenRatio : MonoBehaviour
{
	public int numPoints = 100;
	public float turnFraction = 0.5f;
	public GameObject prefab;
	private List<GameObject> plotList;
	private GameObject[] array;
	float currentTime = 0f;
	public float dnum = 1f;
	public float pow = 1f;
	public float speed = 0.0001f;
	public float powSpeed = 0f;
	// Start is called before sthe first frame update
	void Start()
	{
		plotList = new List<GameObject>();
		array = new GameObject[numPoints];
		for (int i = 0; i < numPoints; i++)
		{
			plotList.Add(Instantiate(prefab, new Vector3(), Quaternion.identity, transform) as GameObject);
		}
	}

	// Update is called once per frame
	void Update()
	{
		currentTime += Time.deltaTime;
		bool calcFlag = false;
		if (currentTime > 0.01f)
		{
			calcFlag = true;
			currentTime = 0f;
		}
		//Calc
		if (calcFlag)
		{
			turnFraction += speed;
			pow += powSpeed;
			for (int i = 0; i < numPoints; i++)
			{
				float dst = Mathf.Clamp(Mathf.Pow(i / (numPoints - 1f) * dnum, pow), 0f, 99999999999999f);
				float angle = 2 * Mathf.PI * turnFraction * i;

				float x = dst * Mathf.Cos(angle);
				float y = dst * Mathf.Sin(angle);

				PlotPoint(x, y, i);
			}
		}
	}

	private void PlotPoint(float x, float y, int plotNum)
	{
		Vector3 vec = plotList[plotNum].transform.position;
		vec.x = x;
		vec.y = y;
		plotList[plotNum].transform.position = vec;
	}
}
