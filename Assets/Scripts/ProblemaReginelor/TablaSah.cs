using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TablaSah : MonoBehaviour {

	public GameObject celula;
	public GameObject[] regina;
	public GameObject buton, loading, restart;
	public Text matrice;
	public int n = 4;
	int contor;
	//coordonate pentru crearea matricei
	public float x, y;

	[Header("Coloreaza tabla")]
	public Color[] culori;
	public Celula[] celule;

	// Use this for initialization
	void Start () {
		n = PlayerPrefs.GetInt("Regine");
		buton.SetActive(false);
		
		restart.SetActive(false);
		if(n <= 6){
			loading.SetActive(true);
			StartCoroutine(GenereazaTabla(x, y));
		}
		else
		{
			loading.SetActive(false);
			matrice.text = "Nu se pot plasa mai mult de 6 regine!";
		}
	}
	
//GENERAREA TABLEI DE SAH SI CREAREA OBIECTELOR DE TIP CELULA PENTRU COLORARE
	IEnumerator GenereazaTabla(float x, float y)
	{
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
        V = new int[nr,nr];

		for (int i = 0; i < nr; i++)
			{
				for (int j = 0; j < nr ; j++)
            	{
				if(celule[j].GetComponent<Celula>().transform.GetChild(0).position == celule[i].GetComponent<Celula>().transform.GetChild(1).position || celule[j].GetComponent<Celula>().transform.GetChild(2).position == celule[i].GetComponent<Celula>().transform.GetChild(3).position || celule[i].GetComponent<Celula>().transform.GetChild(0).position == celule[j].GetComponent<Celula>().transform.GetChild(1).position || celule[i].GetComponent<Celula>().transform.GetChild(2).position == celule[j].GetComponent<Celula>().transform.GetChild(3).position)
					V[i,j] = 1;
				else
					V[i,j] = 0;
            	}

			}
		Tabla(X, 0, V);
		
		buton.SetActive(true);
		loading.SetActive(false);
		
	}

//BACKTRACKING PENTRU COLORAREA TABLEI DE SAH
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
			if(v[K,i] == 1 && X[K] == X[i])
				return false;
		}
		return true;
	}

	void Tabla(int[] X, int K, int[,] V)
	{
		if(K == X.Length)
			RetSol(X);
		else
		{
			for (int i = 0; i < culori.Length; i++)
			{
				X[K] = i;
				if(Cont(X, K, V))
					Tabla(X, K + 1, V);
			}
		}
	}
#endregion


//BACKTRACKING PENTRU PLASAREA REGINELOR 1-6
#region Regine
	void Plaseaza(int[] X, int n)
    {
			
		contor++;
		matrice.text = "Nr solutii: " + contor;
			
		//SE CREAZA O MATRICE DIN COORDONATELE OBIECTELOR (AXA X SI AXA Z)
		//PLASAREA SE VA FACE IN FUNCTIE DE ACESTE COORDONATE
		Vector3 pozitie = new Vector3(x, 0, y);
        for (int i = 0; i < n; i++)
        {
			pozitie.x = i;
            for (int j = 0; j < n; j++)
            {
				pozitie.z = j;
				pozitie.y = .1f;
				GameObject reg = (j == X[i] ? Instantiate(regina[contor - 1], pozitie, Quaternion.identity) : null);	

			}

			//if(contor % 5 == 0)
			//	Restart();
		}	
	}
	public bool Continua(int[] X, int n, int k)
	{		
		for (int i = 0; i < k; i++)
		{
			if(X[k] == X[i] || Mathf.Abs(X[k] - X[i]) == k - i)
			{
				return false;
			}
		}
		
		return true;
	}

	public void Regine(int[] X, int n, int k)
	{
		if(k==n)
		Plaseaza(X, n);
		else
		{
			for (int i = 0; i < n; i++)
			{
				X[k] = i;
				if(Continua(X, n, k))
				{
					Regine(X, n, k + 1);
				}
			}
		}
	}

	public void Buton_Plaseaza()
	{
		if(n <= 6)
		{
			int[] x = new int[n];
	   		Regine(x, n, 0);
			buton.SetActive(false);
			restart.SetActive(true);

			GameObject[] vector = GameObject.FindGameObjectsWithTag("regina");
			if(contor > 4)
			{
				foreach (GameObject go in vector)
				{
					go.GetComponent<Animator>().SetBool("Blink", true);
				}
			}
			
		}
		else
			matrice.text = "Nu se pot plasa mai mult de 6 regine!";
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
