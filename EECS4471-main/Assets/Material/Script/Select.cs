using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Select : MonoBehaviour
{
    // Start is called before the first frame update


    private int currentIndex;
    public GameObject selected;

    public GameObject[] selectable;

    void Start()
    {

       //selectable = GameObject.FindGameObjectsWithTag("selectable");
        currentIndex = 0;
    }

    // Update is called once per frame
    void Update()
    {

       if(Input.GetButtonDown("select")){
            selectable = GameObject.FindGameObjectsWithTag("selectable");
            if(selected == null){
                if(currentIndex < selectable.Length-1){
                    selected = selectable[currentIndex];
                    }
                    else if(selectable.Length == 0){
                    return;}
            }

            if(selected!=null){
                selected.GetComponent<Outline>().enabled = true;
                if(currentIndex < selectable.Length-1){
                selected.GetComponent<Outline>().enabled = false;
                currentIndex += 1;
                selected = selectable[currentIndex];
                selected.GetComponent<Outline>().enabled = true;
            }else{
                selected.GetComponent<Outline>().enabled = false;
                currentIndex = 0;
                selected = selectable[currentIndex];
                selected.GetComponent<Outline>().enabled = true;
            }
            }
           
        }
        
       if(Input.GetButtonDown("create")){
            if(selected!=null){
            {selected.GetComponent<Outline>().enabled = false;
            Instantiate(selected, new Vector3(Random.value*10, Random.value*10, Random.value*10), Quaternion.identity);
            selected.GetComponent<Outline>().enabled = true;
            }}}
        if(Input.GetButtonDown("kill")){
            if(selected != null){
                Destroy(selected); 
                currentIndex = 0;
                selectable = GameObject.FindGameObjectsWithTag("selectable");}
                }
    }

}
