using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{

    public Animator anim;
    public bool isAI;

    public string state = "loading";

    public int posX = 0;
    public int posY = 0;

    public bool isDead = false;

    public string skill = "";               // skill actif sur le dé
    public string[] skills = new string[6]; // liste des skills

    public float cooldown = 3; // temps entre chaque mouvement
    public float timer = 0;    // temps actuel

    // Liste des faces possibles pour l'ia (pour tirer aléatoirement les faces)
    private string[] facesList = new string[6] {"att_1", "att_2", "att_3", "def_1", "def_2", "_"};



    void Start(){
        if (isAI) {
            for (int i = 0; i < skills.Length; i++) {
                skills[i] = facesList[Random.Range(0, facesList.Length)];
            }
            GetComponent<Renderer>().material.color = Color.red;
        }
    }



    void Update(){
        if (timer < cooldown) {
            timer += Time.deltaTime;
            state = "loading";
            return;
        } else {
            state = "waiting";
        }
    }



    public void moveTo(int x, int y, List<Dice> dices){
        StartCoroutine(moving(x, y));
        posX = x;
        posY = y;
        timer = 0;
        roll();
        makeAction(dices);
    }


    // Fonction qui tire aléatoirement un skill et l'affiche
    private void roll(){
        skill = skills[Random.Range(0, skills.Length)];
        Transform skillsTransform = transform.Find("Skills");
        // activate the child with the same name as the skill
        foreach (Transform child in skillsTransform){
            if (child.name == skill){
                child.gameObject.SetActive(true);
            } else {
                child.gameObject.SetActive(false);
            }
        }
    }


    private void makeAction(List<Dice> dices){
        string action = skill.Split('_')[0];
        if (action == "") return;
        int power = int.Parse(skill.Split('_')[1]);
        if (action == "att"){
            Dice bestTarget = null;
            int bestTargetPower = 0;
            // loop through all dices and if there is an adjacent dice, attack it
            foreach (Dice d in dices){
                if (d.isDead) continue;
                if (d == this) continue;
                if (Mathf.Abs(d.posX - posX) <= 1 && Mathf.Abs(d.posY - posY) <= 1){
                    if(d.isAI != isAI){
                        // TODO: ordre de priorité
                        // 1 - Selectionner un joueur qui as rien
                        // 2 - Selectionner un joueur qui as le moins de power et qui n'est pas en mode def
                        // 3 - Selectionner un joueur qui as le moins de def 
                        if (d.skill.Split('_')[0] == "def"){
                            if (int.Parse(d.skill.Split('_')[1]) < power){
                                bestTarget = d;
                            }
                        }
                        else {
                            bestTarget = d;
                        }
                    }
                }
            }
            if (bestTarget != null) {
                bestTarget.death();
                // give money
                GameData gd = SaveSystem.LoadGameData();
                gd.money += 10;
                SaveSystem.SaveGameData(gd);
            }
        }
        // if (action == "sei"){
        //     // Séisme : détruit tous les dés adjacents avec une défense inférieure à la puissance 
        // }
    }


    public void playIA(List<Dice> dices, List<string> actions){
        int movetox, movetoy;
        Dice closest = null;
        int dist = int.MaxValue;

        if (actions.Count == 0){
            StartCoroutine(moving(posX, posY));
            return;
        }

        foreach (Dice d in dices)
        {
            if (d.isAI) continue;
            int tmpDist = Mathf.Abs(d.posX - posX) + Mathf.Abs(d.posY - posY);
            if (tmpDist < dist){
                dist = tmpDist;
                closest = d;
            }
        }

        // pour chaque action possible, calculer la distance avec le dé le plus proche
        dist = int.MaxValue;
        string bestAction = "";
        foreach (string action in actions)
        {
            int newx = int.Parse(action.Split(';')[0]);
            int newy = int.Parse(action.Split(';')[1]);
            int tmpDist = Mathf.Abs(closest.posX - newx) + Mathf.Abs(closest.posY - newy);
            if (tmpDist < dist){
                dist = tmpDist;
                bestAction = action;
            }
        }

        if (bestAction == ""){
            // take a random action
            bestAction = actions[Random.Range(0, actions.Count-1)];
        }

        movetox = int.Parse(bestAction.Split(';')[0]);
        movetoy = int.Parse(bestAction.Split(';')[1]);

        state = "playing";
        moveTo(movetox, movetoy, dices);

    }


    
    public IEnumerator moving(int x, int y){
        state = "playing";
        float duration = 0.25f; // seconds
        float currentTime = 0;
        Vector3 startPos = transform.position;
        Vector3 endPos = new Vector3(x - 0.025f, y + 0.025f, -1);
        while (currentTime < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;
        state = "loading";
    }



    public void death(){
        isDead = true;
        StartCoroutine(annimDeath());
    }

    IEnumerator annimDeath(){
        while (transform.localScale.x > 0.01f)
        {
            // reduce the size
            transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }



}
