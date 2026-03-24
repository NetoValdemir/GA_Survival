using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PopulationManager : MonoBehaviour
{
    // Configurações iniciais da simulação
    public GameObject botPrefab;                                    // Prefab do agente
    public int populationSize = 50;                                 // Tamanho da população
    List<GameObject> population = new List<GameObject>();           // Lista para armazenar os indivíduos da população
    public static float elapsed = 0;                                // Variável de tempo de simulação por ciclo de vida
    public float trialTime = 5;                                     // Tempo total do ciclo de vida
    int generation = 1;                                             // Geração inicial

    GUIStyle guiStyle = new GUIStyle();                             // Estilo de GUI (Graphical User Interface)
    
    // Método para criação de GUI
    void OnGUI()
    {
        guiStyle.fontSize = 25;
        guiStyle.normal.textColor = Color.white;
        GUI.BeginGroup(new Rect(10, 10, 250, 150));
        GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
        GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
        GUI.Label(new Rect(10, 50, 200, 30), string.Format("Time: {0:0.00}", elapsed), guiStyle);
        GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
        GUI.EndGroup();
    }
    
    void Start()
    {
        // Criação da primeira geração
        for(int i = 0; i < populationSize; i++)
        {
            Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-4,4),
                                                this.transform.position.y,
                                                this.transform.position.z + Random.Range(-4,4));

            GameObject b = Instantiate(botPrefab, startingPos, this.transform.rotation);
            b.GetComponent<Brain>().Init();
            population.Add(b);
        }
    }

    // Método para distribuição de genes dos agentes pai
    GameObject Breed(GameObject parent1, GameObject parent2)
    {
        Vector3 startingPos = new Vector3(this.transform.position.x + Random.Range(-4, 4),
                                            this.transform.position.y,
                                            this.transform.position.z + Random.Range(-4, 4));

        GameObject offspring = Instantiate(botPrefab, startingPos, this.transform.rotation);
        Brain b = offspring.GetComponent<Brain>();

        if(Random.Range(0, 100) == 1)                                                                          // Taxa de Mutação ~ 1%
        {
            b.Init();
            b.dna.Mutate();
        }
        else                                                                                                  // Geração de agentes filho
        {
            b.Init();
            b.dna.Combine(parent1.GetComponent<Brain>().dna, parent2.GetComponent<Brain>().dna);
        }
        return offspring;
    }

    // Método para criação dos indivíduos filho
    void BreedNewPopulation()
    {
        List<GameObject> sortedList = population.OrderBy(o => o.GetComponent<Brain>().distanceTravelled).ToList(); // Ranqueia a Lista de acordo com a pontuação dos agentes

        population.Clear();

        // Estratégia de combinação dos genes dos pais (Metade dos genes do pai com metade dos genes da mãe)
        for(int i = (int) (sortedList.Count / 2.0f) - 1; i < sortedList.Count -1; i++)
        {
            population.Add(Breed(sortedList[i], sortedList[i + 1]));
            population.Add(Breed(sortedList[i + 1], sortedList[i]));
        }
        // Destrói os agentes pai e a população da lista temporária
        for(int i = 0; i < sortedList.Count; i++)
        {
            Destroy(sortedList[i]);
        }
        generation++;
    }

    // Método da Unity para atualização do jogo
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed >= trialTime)
        {
            BreedNewPopulation();
            elapsed = 0;
        }
    }
}
