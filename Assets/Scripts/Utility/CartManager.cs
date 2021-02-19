using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CartManager : MonoBehaviour
{
    private CinemachineDollyCart cart;
    [SerializeField] private bool active = false;
    [SerializeField] private bool constantSpeed = true;
    [SerializeField] private List<string> indexSpeedList;
    Dictionary<int ,float> speedDict;


    void Start()
    {
        cart = GetComponent<CinemachineDollyCart>();
        cart.enabled = active;
        speedDict = new Dictionary<int, float>();
        foreach(string s in indexSpeedList){
            int index = int.Parse(s.Split('-')[0]);
            float speed = float.Parse(s.Split('-')[1]);
            Debug.Log(index + " - " + speed);
            speedDict.Add(index,speed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(constantSpeed) return;
        else{
            int posIdx = (int)cart.m_Position;
            Debug.Log("Index is" + posIdx);
            float originalSpeed = cart.m_Speed;
            try{
                cart.m_Speed = speedDict[posIdx];
                Debug.Log("Set speed to "+ cart.m_Speed);
            }
            catch{
                cart.m_Speed = originalSpeed;
                return;
            }
        }
    }

    public void activateCart(){
        cart.enabled = true;
        active = true;
    }
    public void deactivateCart(){
        cart.enabled = false;
        active = false;
    }

    public bool isCartActive(){
        return active;
    }
}
