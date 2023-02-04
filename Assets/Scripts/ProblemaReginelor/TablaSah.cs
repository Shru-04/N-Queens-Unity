using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class TablaSah : MonoBehaviour
{

	public GameObject celula;
	public GameObject[] regina, sol;
	public GameObject buton, loading, restart, buton2, check, ins;
	public Text matrice;
	public int n = 4;
	public bool done = false, startplace = false;
	int contor;
	int diy_contor;
	//coordonate pentru crearea matricei
	public float x, y;

	[Header("Coloreaza tabla")]
	public Color[] culori;
	public Celula[] celule;

	// Use this for initialization
	void Start()
	{
		diy_contor = 0; done = false; startplace = false;
		n = PlayerPrefs.GetInt("Regine");
		buton.SetActive(false);
		buton2.SetActive(false);
		ins.SetActive(false);

		restart.SetActive(false);
		check.SetActive(false);
		if (n <= 8)
		{
			loading.SetActive(true);
			StartCoroutine(GenereazaTabla(x, y));
		}
		else
		{
			loading.SetActive(false);
			matrice.text = "No more than 8 queens can be placed";
		}
	}

	void Update()
    {
		if (diy_contor < n && done && Input.GetMouseButtonDown(0) && startplace)
        {
			/*// First Destroy all pawns 
			GameObject[] enemies = GameObject.FindGameObjectsWithTag("regina");
			for (int i = 0; i < enemies.Length; i++)
			{
				Destroy(enemies[i]);
			}*/

			//Main Logic

			if (!Camera.main) return;

			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f))
            {
				float x = hit.point.x, z = hit.point.z;
				if ((x - (int)x) > 0.5) x += 1;
				if ((z - (int)z) > 0.5) z += 1;
				Debug.Log(hit.point);

				Vector3 pozitie = new Vector3((int)x, 0.1f, (int)z);
				GameObject go = Instantiate(regina[0], pozitie, Quaternion.identity);
				go.name = "Queen";
				go.tag = "queen";

				diy_contor++;
			}

		}
		else if (diy_contor >= n && Input.GetMouseButtonDown(0))
		{
			matrice.text = "Cannot place more than " + n.ToString() + " queens\n Hit Restart for more";
		}

		if (diy_contor >= 0 && Input.GetMouseButtonDown(1) && startplace)
        {
			if (!Camera.main) return;
			RaycastHit hit;
			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 50.0f) && hit.transform.gameObject != null)
            {
				//Debug.Log("OK " + hit.transform.gameObject);
				if (hit.transform.gameObject.tag == "queen")
                {
					Destroy(hit.transform.gameObject);
					diy_contor--;
					matrice.text = "";
				}
            }
		}
    }

	//GENERAREA TABLEI DE SAH SI CREAREA OBIECTELOR DE TIP CELULA PENTRU COLORARE
	IEnumerator GenereazaTabla(float x, float y)
	{
		done = false;
		GameObject go = new GameObject();

		Vector3 pozitie = new Vector3(x, 0, y);


		for (int i = 0; i < n; i++)
		{
			pozitie.x = i;
			for (int j = 0; j < n; j++)
			{
				pozitie.z = j;
				yield return new WaitForSeconds(.1f);
				go = Instantiate(celula, pozitie, Quaternion.identity);
				go.transform.SetParent(transform);

			}

		}

		yield return new WaitForSeconds(1);

		GameObject[] vector = GameObject.FindGameObjectsWithTag("celula");
		celule = new Celula[vector.Length];

		for (int i = 0; i < vector.Length; i++)
		{
			celule[i] = vector[i].GetComponent<Celula>();
		}
		//coloreaza



		int[] X;
		int[,] V;
		int nr = celule.Length;

		X = new int[nr];
		V = new int[nr, nr];

		for (int i = 0; i < nr; i++)
		{
			for (int j = 0; j < nr; j++)
			{
				if (celule[j].GetComponent<Celula>().transform.GetChild(0).position == celule[i].GetComponent<Celula>().transform.GetChild(1).position || celule[j].GetComponent<Celula>().transform.GetChild(2).position == celule[i].GetComponent<Celula>().transform.GetChild(3).position || celule[i].GetComponent<Celula>().transform.GetChild(0).position == celule[j].GetComponent<Celula>().transform.GetChild(1).position || celule[i].GetComponent<Celula>().transform.GetChild(2).position == celule[j].GetComponent<Celula>().transform.GetChild(3).position)
					V[i, j] = 1;
				else
					V[i, j] = 0;
			}

		}
		Tabla(X, 0, V);

		buton.SetActive(true);
		buton2.SetActive(true);
		loading.SetActive(false);
		done = true;

	}

	//BACKTRACKING FOR COLORING THE CHESS BOARD
	#region coloreaza
	void RetSol(int[] x)
	{
		for (int i = 0; i < x.Length; i++)
		{
			celule[i].GetComponent<Renderer>().material.color = culori[x[i]];
		}
	}

	bool Cont(int[] X, int K, int[,] v)
	{
		for (int i = 0; i < K; i++)
		{
			if (v[K, i] == 1 && X[K] == X[i])
				return false;
		}
		return true;
	}

	void Tabla(int[] X, int K, int[,] V)
	{
		if (K == X.Length)
			RetSol(X);
		else
		{
			for (int i = 0; i < culori.Length; i++)
			{
				X[K] = i;
				if (Cont(X, K, V))
					Tabla(X, K + 1, V);
			}
		}
	}
	#endregion


	//BACKTRACKING PENTRU PLASAREA REGINELOR 1-6
	#region Regine


	IEnumerator DisplaySoln()
	{
		GameObject[] vector = GameObject.FindGameObjectsWithTag("regina");
		for (int i = 0; i < 1000; i++)
		{
			foreach (GameObject go in vector)
			{
				if (go.name == ("Hello_" + (i % contor + 1)))
				{
					go.SetActive(true);
				}
				else
					go.SetActive(false);
			}
			yield return new WaitForSeconds(10);
		}
	}

	void Plaseaza(int[] X, int n)
	{

		contor++;
		matrice.text = "No. of Solutions = " + contor;
		String s = "";

		//A MATRIX IS CREATED FROM THE COORDINATES OF THE OBJECTS (X AXIS AND Z AXIS)
		//THE PLACEMENT WILL BE DONE ACCORDING TO THESE COORDINATES
		Vector3 pozitie = new Vector3(x, 0, y);
		for (int i = 0; i < n; i++)
		{
			pozitie.x = i;
			for (int j = 0; j < n; j++)
			{
				pozitie.z = j;
				pozitie.y = .1f;
				s += (j == X[i]) ? "X " : "_ ";
				GameObject reg = (j == X[i]) ? Instantiate(regina[(int)(contor - 1) % (int)10], pozitie, Quaternion.identity) : null;
				if (reg != null)
					reg.name = "Hello_" + contor;

			}
			s += "\n";

		}
		Debug.Log(s);
		return;
	}
	public bool Continua(int[] X, int n, int k)
	{
		for (int i = 0; i < k; i++)
		{
			if (X[k] == X[i] || Mathf.Abs(X[k] - X[i]) == k - i)
			{
				return false;
			}
		}

		return true;
	}

	public void Regine(int[] X, int n, int k)
	{
		if (k == n)
		{
			Plaseaza(X, n);     //Placing Part
			return;
		}
		else
		{
			for (int i = 0; i < n; i++)
			{
				X[k] = i;
				if (Continua(X, n, k))
				{
					Regine(X, n, k + 1);
				}
			}
		}
	}

	public void Buton_Plaseaza()
	{
		if (n <= 8)
		{
			int[] x = new int[n];
			Regine(x, n, 0);            // Logic Part
			buton.SetActive(false);
			buton2.SetActive(false);
			restart.SetActive(true);
			check.SetActive(false);
			ins.SetActive(false);
			if (n == 2 || n == 3)
            {
				matrice.text = "Sorry, No Solutions :( ";
				return;
			}
			GameObject[] vector = GameObject.FindGameObjectsWithTag("regina");
			for (int i = 0; i < 1000; i++)
				StartCoroutine(DisplaySoln());
		}
		else
			matrice.text = "Cannot place more than 8 queens";
	}

	public void Buton_DIY()
    {
		if (n <= 8)
		{
			startplace = true;
			buton.SetActive(false);
			buton2.SetActive(false);
			restart.SetActive(true);
			check.SetActive(true);
			ins.SetActive(true);
			if (n == 2 || n == 3)
			{
				matrice.text = "Sorry, No Solutions :( ";
				check.SetActive(false);
				ins.SetActive(false);
				startplace = false;
				return;
			}
			int[] x = new int[n];
			Regine(x, n, 0);
			GameObject[] vector = GameObject.FindGameObjectsWithTag("regina");
			sol = vector;
			foreach (GameObject go in vector)
				go.SetActive(false);
			matrice.text = "";
		}
		else
			matrice.text = "Cannot place more than 8 queens";
	}

	public void Buton_Check()
    {
		//GameObject[] vector = GameObject.FindGameObjectsWithTag("regina");
		GameObject[] vector = sol;
		Debug.Log(vector.Length);
		GameObject[] res = GameObject.FindGameObjectsWithTag("queen");
		bool cmp = false;

		foreach (GameObject go in vector)
			go.SetActive(false);

		for (int i = 0; i < contor; i++)
		{
			List<Vector3> list = new List<Vector3>(), list2 = new List<Vector3>();
			foreach (GameObject go in vector)
			{
				if (go.name == ("Hello_" + (i + 1)))
                {
					list.Add(go.transform.position);
				}
			}
			foreach (GameObject go in res)
			{
				list2.Add(go.transform.position);
			}

			list.Sort((a, b) => a.x.CompareTo(b.x));
			list2.Sort((a, b) => a.x.CompareTo(b.x));

			String s = "";
			foreach (Vector3 x in list)
				s = s + x + "\n";
			Debug.Log("Done \n " + s);
			s = "";
			foreach (Vector3 x in list2)
				s = s + x + "\n";
			Debug.Log("Done \n " + s);

			// Checking
			Debug.Log(list.Except(list2).ToList().Count);
			if (list.Except(list2).ToList().Count == 0)
			{
				matrice.text = "Correct !!";
				cmp = true;
				return;
			}
		}

		if (!cmp)
			matrice.text = "Incorrect !!";
	}

	public void Restart()
	{
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
	}

	public void Menu()
	{
		UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1);
	}
	#endregion
}
