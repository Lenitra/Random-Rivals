using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dice : MonoBehaviour
{
    public GameObject swordObj;
    public bool isAI;
    public bool isTurn = false;
    public bool hasPlayed = false;
    public int posX = 0;
    public int posY = 0;
    public GameManager gameManager;
    public bool isDead = false;

    public string state = "";

    public string[] actionsList = null;

    // Faces : attribut_force
    //  - att : attaque
    //  - def : défense
    public string[] faces = new string[6];
    public string[] facesList = new string[6] {"att_1", "att_2", "att_3", "def_1", "def_2", "def_3"};


    // Start is called before the first frame update
    void Start()
    {
        // pour chaque face du dé tirer une face aléatoire
        for (int i = 0; i < faces.Length; i++)
        {
            faces[i] = facesList[Random.Range(0, facesList.Length)];
        }

        // Set la couleur en fonction de l'équipe
        if (isAI)
        {
            GetComponent<Renderer>().material.color = Color.red;
        } else {
            GetComponent<Renderer>().material.color = Color.blue;
        }
        // state take a random face
        state = faces[Random.Range(0, faces.Length)];
        transform.Find(state).gameObject.SetActive(true);

    }

    // appelée par le game Manager
    public void turn(string[] actions, List<Dice> dices = null){

        if(isAI && isTurn == true && hasPlayed == false){

            // trouver le dé qui n'est pas une IA le plus proche
            Dice closest = null;
            int dist = 100;
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
            dist = 100;
            string bestAction = "";
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i] == "") continue;
                int tmpx = int.Parse(actions[i].Split(';')[0]);
                int tmpy = int.Parse(actions[i].Split(';')[1]);
                int tmpDist = Mathf.Abs(closest.posX - (posX + tmpx)) + Mathf.Abs(closest.posY - (posY + tmpy));
                if (tmpDist < dist){
                    bestAction = actions[i];
                    dist = tmpDist;
                }
            }


            int x, y;
            if (bestAction == ""){
                // déplacement random
                x = Random.Range(-1, 2);
                y = Random.Range(-1, 2);
                bestAction = x + ";" + y;
                // while x;y is not in actions
                hasPlayed = false;
                while (!System.Array.Exists(actions, element => element == bestAction))
                {
                    x = Random.Range(-1, 2);
                    y = Random.Range(-1, 2);
                    bestAction = x + ";" + y;
                }
            }
            x = int.Parse(bestAction.Split(';')[0]);
            y = int.Parse(bestAction.Split(';')[1]);

            StartCoroutine(iaDelay(x, y));
        } else {

            // compare actions with actionsList
            bool same = true;
            if (actionsList == null)
            {
                same = false;
            } else if (actions.Length != actionsList.Length)
            {
                same = false;
            } else {
                for (int i = 0; i < actions.Length; i++)
                {
                    if (actions[i] != actionsList[i])
                    {
                        same = false;
                    }
                }
            }

            if (isTurn == true && hasPlayed == false && same == false) {
                StartCoroutine(playable());
                actionsList = actions;
                return;
            }
            // if (string.Join(",", actions) == string.Join(",", actionsList)) return;

        }
        
    }

    // appelée par le PlayerControler
    public void lisenInput(int x, int y){
        if (isTurn)
        {
            x = x - posX;
            y = y - posY;
            string tmp = x + ";" + y;
            // while x;y is not in actions
            if (System.Array.Exists(actionsList, element => element == tmp))
            {
                isTurn = false;
                hasPlayed = true;

                move(x, y);
            }
        }
    }

    // Déplace le dé, change son état et fait l'effet 
    private void move(int x, int y){
        posX += x;
        posY += y;
        state = faces[Random.Range(0, faces.Length)];
    }

    // Fait l'effet du dé
    public void makeEffect(List<Dice> dices){
        // activer l'enfant correspondant à l'état, l'enfant à le même nom que l'état
        // désactiver les autres enfants
        foreach (Transform c in transform)
        {
            if (c.name == state)
            {
                c.gameObject.SetActive(true);
            } else {
                c.gameObject.SetActive(false);
            }
        }

        if (state.Contains("att")){
            int x = posX;
            int y = posY;
            int attPow = int.Parse(state.Split('_')[1]);
            foreach (Dice d in dices)
            {   
                // si il est adjacent
                bool isAdjacent = false;
                if (d.posX == x && d.posY == y+1) isAdjacent = true;
                if (d.posX == x+1 && d.posY == y+1) isAdjacent = true;
                if (d.posX == x+1 && d.posY == y) isAdjacent = true;
                if (d.posX == x+1 && d.posY == y-1) isAdjacent = true;
                if (d.posX == x && d.posY == y-1) isAdjacent = true;
                if (d.posX == x-1 && d.posY == y-1) isAdjacent = true;
                if (d.posX == x-1 && d.posY == y) isAdjacent = true;
                if (d.posX == x-1 && d.posY == y+1) isAdjacent = true;

                if (isAdjacent && d.isAI == isAI) continue;

                if (isAdjacent){
                    Debug.Log(gameObject.name + " : " + state + " combat " + d.name + " : " + d.state );
                    // si il a un shield 
                    if (d.state.Contains("def")){
                        int defPow = int.Parse(d.state.Split('_')[1]);
                        if (attPow > defPow){
                            int tmx = posX - d.posX;
                            int tmy = posY - d.posY;
                            // StartCoroutine(swordAttk(tmx, tmy));
                            d.isDead = true;
                            d.death();
                        }
                    } else {
                        int tmx = posX - d.posX;
                        int tmy = posY - d.posY;
                        // StartCoroutine(swordAttk(tmx, tmy));
                        d.isDead = true;
                        d.death();
                    }
                    return;
                }
            }
        }
    }


    public void death(){
        isDead = true;
        StartCoroutine(annimDeath());
    }
    


    IEnumerator annimDeath(){
        // loop 10 times
        while (transform.localScale.x > 0.01f)
        {
            // reduce the size
            transform.localScale -= new Vector3(0.05f, 0.05f, 0);
            yield return new WaitForSeconds(0.01f);
        }
        Destroy(gameObject);
    }

    IEnumerator playable(){
        float blueFactor = 0;
        bool up = false;
        Color blue = new Color(blueFactor, blueFactor, 1, 1);
        while (!hasPlayed && isTurn)
        {   
            if(up){
                blueFactor += 0.01f;
            } else {
                blueFactor -= 0.01f;
            }
            if (blueFactor > 1) up = false;
            if (blueFactor < 0) up = true;
            blue = new Color(blueFactor, blueFactor, 1, 1);
            GetComponent<Renderer>().material.color = blue;
            yield return new WaitForSeconds(0.01f);
        } 
        GetComponent<Renderer>().material.color = Color.blue;
        yield return null;
    }

    IEnumerator swordAttk(int x, int y){
        if(x == 0 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, 45);
        
        if(x == 1 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, 0);

        if(x == 1 && y == 0)swordObj.transform.rotation = Quaternion.Euler(0, 0, -45);

        if(x == 1 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -90);

        if(x == 0 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -135);

        if(x == -1 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -180);

        if(x == -1 && y == 0)swordObj.transform.rotation = Quaternion.Euler(0, 0, -225);

        if(x == -1 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -270);

        swordObj.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        swordObj.SetActive(false);

    }


    IEnumerator iaDelay(int x, int y){
        float redFactor = 0;
        bool up = false;
        Color red = new Color(1, redFactor, redFactor, 1);
        // loop 17 times
        for (int i = 0; i < 80; i++)
        {   
            if(up){
                redFactor += 0.02f;
            } else {
                redFactor -= 0.02f;
            }
            if (redFactor > 1) up = false;
            if (redFactor < 0) up = true;
            red = new Color(1, redFactor, redFactor, 1);
            GetComponent<Renderer>().material.color = red;
            yield return new WaitForSeconds(0.01f);
        }
        GetComponent<Renderer>().material.color = Color.red;
        move(x, y);
        isTurn = false;
        hasPlayed = true;
    }

}
