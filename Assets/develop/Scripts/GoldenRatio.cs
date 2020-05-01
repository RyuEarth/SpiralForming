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
	public float period=0f;
	public float offsetPeriod;
	public float dstArg=1f;
	[SerializeField]
	private float pow2;
	// Start is called before sthe first frame update
	void Start()
	{
		plotList = new List<GameObject>();
		array = new GameObject[numPoints];
		for (int i = 0; i < numPoints; i++)
		{
			plotList.Add(Instantiate(prefab, new Vector3(), Quaternion.identity, transform) as GameObject);
			plotList[i].transform.localScale = prefab.transform.localScale;
		}
	}

	// Update is called once per frame
	void Update()
	{
		currentTime += Time.deltaTime;
		bool calcFlag = false;
		if (currentTime > 0.016666f)
		{
			calcFlag = true;
			currentTime = 0f;

		}
		//Calc
		if (calcFlag)
		{
			turnFraction += speed;
			pow += powSpeed;

			pow2 = 0f;
			if (period > 0)
			{
				pow2 = SinWave() * pow;
			}else{
				pow2 = pow;
			}

			for (int i = 0; i < numPoints; i++)
			{
				float dst = Mathf.Clamp(Mathf.Pow(i / (numPoints - 1f) * dnum, pow2)*dstArg, 0f, 99999999999999f);
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
		plotList[plotNum].transform.localPosition = new Vector3(x,y);
	}

	private float SinWave(){
		float result = Mathf.Sin(2f*Mathf.PI * 1f/period * Time.time+Mathf.PI*offsetPeriod);	
		return result;
	}
}
