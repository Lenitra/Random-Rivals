// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class Entity : MonoBehaviour
// {
//     public GameObject swordObj;
//     public Animator anim;
//     public bool isAI;

//     public string state = "";

//     public int posX = 0;
//     public int posY = 0;
//     public bool isDead = false;

//     public string skill = "";

//     public string[] actionsList = null;

//     // Faces : attribut_force
//     //  - att : attaque
//     //  - def : défense
//     public string[] faces = new string[6];
//     private string[] facesList = new string[6] {"att_1", "att_2", "att_3", "def_1", "def_2", "_"};


//     // Start is called before the first frame update
//     void Start()
//     {
//         // child 0 is the animation
//         anim = transform.GetChild(0).GetComponent<Animator>();
//         // pour chaque face du dé tirer une face aléatoire
//         for (int i = 0; i < faces.Length; i++)
//         {
//             faces[i] = facesList[Random.Range(0, facesList.Length)];
//         }

//         // Set la couleur en fonction de l'équipe
//         if (isAI)
//         {
//             GetComponent<Renderer>().material.color = Color.red;
//         }
//         // state take a random face
//         transform.Find(faces[Random.Range(0, faces.Length)]).gameObject.SetActive(true);
//     }

//     public void eventIsTurn(){
//         state = "wait";
//         if (!isAI) anim.SetBool("Selected", true);

//     }

//     public string getDetails(){
//         // retourne la liste des faces
//         string details = "";
//         foreach (string face in faces)
//         {
//             details += face + "\n";
//         }
//         return details;
//     }


//     // @param actions : tableau de string contenant les positions possibles
//     // @param dices : liste des dés
//     public void iaTurn(List<string> actions, List<Dice> dices){
//         int movetox, movetoy;

//         // trouver le dé qui n'est pas une IA le plus proche
//         Dice closest = null;
//         int dist = 100;
//         foreach (Dice d in dices)
//         {
//             if (d.isAI) continue;
//             int tmpDist = Mathf.Abs(d.posX - posX) + Mathf.Abs(d.posY - posY);
//             if (tmpDist < dist){
//                 dist = tmpDist;
//                 closest = d;
//             }
//         }

//         // pour chaque action possible, calculer la distance avec le dé le plus proche
//         dist = 100;
//         string bestAction = "";
//         foreach (string action in actions)
//         {
//             int newx = int.Parse(action.Split(';')[0]);
//             int newy = int.Parse(action.Split(';')[1]);
//             int tmpDist = Mathf.Abs(closest.posX - newx) + Mathf.Abs(closest.posY - newy);
//             if (tmpDist < dist){
//                 dist = tmpDist;
//                 bestAction = action;
//             }
//         }

//         if (bestAction == ""){
//             // take a random action
//             bestAction = actions[Random.Range(0, actions.Count)];
//         }

//         movetox = int.Parse(bestAction.Split(';')[0]);
//         movetoy = int.Parse(bestAction.Split(';')[1]);

//         state = "moving";
//         StartCoroutine(moving(movetox, movetoy));

//     }


//     public IEnumerator moving(int x, int y){
//         anim.SetBool("Selected", false);
//         skill = faces[Random.Range(0, faces.Length)];
//         state = "moving";
//         // make player move from posX;posY to x;y progressively
//         float duration = 0.25f; // seconds
//         float currentTime = 0;
//         Vector3 startPos = transform.position;
//         Vector3 endPos = new Vector3(x - 0.025f, y + 0.025f, -1);
//         while (currentTime < duration)
//         {
//             transform.position = Vector3.Lerp(startPos, endPos, currentTime / duration);
//             currentTime += Time.deltaTime;
//             yield return null;
//         }
//         transform.position = endPos;
//         posX = x;
//         posY = y;
//         state = "effect";
//     }



//     // Déplace le dé A UNE POSITION x;y
//     public void place(int x, int y){
//         posX = x;
//         posY = y;
//         transform.position = new Vector3(x  - 0.025f, y + 0.025f, -1);
//     }


//     // Déplace le dé dans une DIRRECTION x;y
//     private void move(int x, int y){
//         posX += x;
//         posY += y;
//         skill = faces[Random.Range(0, faces.Length)];
//         transform.position = new Vector3(posX - 0.025f, posY + 0.025f, -1);
//     }

    













//     // Fait le skill du dé
//     public void makeEffect(List<Dice> dices){
//         // activer l'enfant correspondant à l'état, l'enfant à le même nom que l'état
//         // désactiver les autres enfants sauf le premier
//         foreach (Transform c in transform)
//         {
//             if (c.name == skill)
//             {
//                 c.gameObject.SetActive(true);
//             } else {
//                 c.gameObject.SetActive(false);
//             }
//         }
//         // réactiver le premier enfant
//         transform.GetChild(0).gameObject.SetActive(true);

//         if (skill.Contains("att")){
//             int x = posX;
//             int y = posY;
//             int attPow = int.Parse(skill.Split('_')[1]);
//             foreach (Dice d in dices)
//             {   
//                 // si il est adjacent
//                 bool isAdjacent = false;
//                 if (d.posX == x && d.posY == y+1) isAdjacent = true;
//                 if (d.posX == x+1 && d.posY == y+1) isAdjacent = true;
//                 if (d.posX == x+1 && d.posY == y) isAdjacent = true;
//                 if (d.posX == x+1 && d.posY == y-1) isAdjacent = true;
//                 if (d.posX == x && d.posY == y-1) isAdjacent = true;
//                 if (d.posX == x-1 && d.posY == y-1) isAdjacent = true;
//                 if (d.posX == x-1 && d.posY == y) isAdjacent = true;
//                 if (d.posX == x-1 && d.posY == y+1) isAdjacent = true;

//                 if (isAdjacent && d.isAI == isAI) continue;

//                 if (isAdjacent){
//                     Debug.Log(gameObject.name + " : " + skill + " combat " + d.name + " : " + d.skill );
//                     // si il a un shield 
//                     if (d.skill.Contains("def")){
//                         int defPow = int.Parse(d.skill.Split('_')[1]);
//                         if (attPow > defPow){
//                             int tmx = posX - d.posX;
//                             int tmy = posY - d.posY;
//                             // StartCoroutine(swordAttk(tmx, tmy));
//                             d.isDead = true;
//                             d.death();
//                         }
//                     } else {
//                         int tmx = posX - d.posX;
//                         int tmy = posY - d.posY;
//                         // StartCoroutine(swordAttk(tmx, tmy));
//                         d.isDead = true;
//                         d.death();
//                     }
//                     state = "end";
//                     return;
//                 }
//             }
//         }
//         state = "end";
//     }


//     public void death(){
//         isDead = true;
//         StartCoroutine(annimDeath());
//     }
    


//     IEnumerator annimDeath(){
//         // loop 10 times
//         while (transform.localScale.x > 0.01f)
//         {
//             // reduce the size
//             transform.localScale -= new Vector3(0.05f, 0.05f, 0);
//             yield return new WaitForSeconds(0.01f);
//         }
//         Destroy(gameObject);
//     }

//     // IEnumerator playable(){
//     //     float blueFactor = 0;
//     //     bool up = false;
//     //     Color blue = new Color(blueFactor, blueFactor, 1, 1);
//     //     while (!hasPlayed && isTurn)
//     //     {   
//     //         if(up){
//     //             blueFactor += 0.01f;
//     //         } else {
//     //             blueFactor -= 0.01f;
//     //         }
//     //         if (blueFactor > 1) up = false;
//     //         if (blueFactor < 0) up = true;
//     //         blue = new Color(blueFactor, blueFactor, 1, 1);
//     //         GetComponent<Renderer>().material.color = blue;
//     //         yield return new WaitForSeconds(0.01f);
//     //     } 
//     //     GetComponent<Renderer>().material.color = Color.blue;
//     //     yield return null;
//     // }

//     // IEnumerator swordAttk(int x, int y){
//     //     if(x == 0 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, 45);
        
//     //     if(x == 1 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, 0);

//     //     if(x == 1 && y == 0)swordObj.transform.rotation = Quaternion.Euler(0, 0, -45);

//     //     if(x == 1 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -90);

//     //     if(x == 0 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -135);

//     //     if(x == -1 && y == -1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -180);

//     //     if(x == -1 && y == 0)swordObj.transform.rotation = Quaternion.Euler(0, 0, -225);

//     //     if(x == -1 && y == 1)swordObj.transform.rotation = Quaternion.Euler(0, 0, -270);

//     //     swordObj.SetActive(true);
//     //     yield return new WaitForSeconds(0.5f);
//     //     swordObj.SetActive(false);

//     // }


//     // IEnumerator iaDelay(int x, int y){
//     //     float redFactor = 0;
//     //     bool up = false;
//     //     Color red = new Color(1, redFactor, redFactor, 1);
//     //     // loop 17 times
//     //     for (int i = 0; i < 80; i++)
//     //     {   
//     //         if(up){
//     //             redFactor += 0.02f;
//     //         } else {
//     //             redFactor -= 0.02f;
//     //         }
//     //         if (redFactor > 1) up = false;
//     //         if (redFactor < 0) up = true;
//     //         red = new Color(1, redFactor, redFactor, 1);
//     //         GetComponent<Renderer>().material.color = red;
//     //         yield return new WaitForSeconds(0.01f);
//     //     }
//     //     GetComponent<Renderer>().material.color = Color.red;
//     //     move(x, y);
//     //     isTurn = false;
//     //     hasPlayed = true;
//     // }




// }