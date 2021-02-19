using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsEnd : MonoBehaviour
{
    // Start is called before the first frame update
    public void End(){
        SceneManager.LoadScene("FeedbackMenu",LoadSceneMode.Single);
    }
}
