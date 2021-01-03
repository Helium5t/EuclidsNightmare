using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AdditiveLevelLoader : MonoBehaviour
{

    private enum levelTag{
        Platform = 0
        ,Puzzle = 1
    };
    public Vector3 levelEndingPoint;
    private Vector3 loadedLevelStartingPoint = Vector3.zero;

    [SerializeField] private levelTag levelType = levelTag.Puzzle;
    [SerializeField] private bool useBuildIndex = false;
    [SerializeField] private Scene nextSceneToLoad;
    [SerializeField] private string nextSceneName;
    bool loaded = false;
    private AsyncOperation loadingStatus;
    private bool currentLevel = false;
    private int ownSceneIndex= 0;
    private AdditiveLevelLoader nextLevelLoader;

    private void Awake() {
        if(gameObject.scene == SceneManager.GetActiveScene()){
            currentLevel = true;
        }
        //nextSceneToLoad = SceneManager.GetSceneByPath("Assets/Scenes/DevelopmentScenes/NextLevelTest.unity");
    }
    // Start is called before the first frame update
    void Start()
    {
        
        Transform endPosCube = transform.Find("EndingPoint");
        Debug.Log(endPosCube.position);
        levelEndingPoint = transform.TransformVector(endPosCube.position);
        endPosCube.gameObject.SetActive(false);
        if(SceneManager.sceneCount < 3 && !loaded){
            loadNextLevel();
        }
        for(int i =0; i<SceneManager.sceneCount;i++){
            if(SceneManager.GetSceneAt(i) == gameObject.scene){
                ownSceneIndex = i;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentLevel = SceneManager.GetActiveScene() == gameObject.scene;
        if(Input.GetKeyDown(KeyCode.Q) && loaded && currentLevel){
            nextLevelLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"));
        }
    }

    private void loadNextLevel(){
        if(useBuildIndex){
            loadingStatus = SceneManager.LoadSceneAsync(nextSceneToLoad.buildIndex,LoadSceneMode.Additive);
            nextSceneName = nextSceneToLoad.name;
        }
        else{
            loadingStatus = SceneManager.LoadSceneAsync(nextSceneName,LoadSceneMode.Additive);
        }

        #region loading completed routine
        loadingStatus.completed += (AsyncOperation o) =>{
            Scene loadedScene= SceneManager.GetSceneAt(ownSceneIndex +1);
            for(int i =0; i<SceneManager.sceneCount;i++){
                if(SceneManager.GetSceneAt(i).name == nextSceneName){
                    loadedScene = SceneManager.GetSceneAt(i);
                    break;
                }
            }
            Debug.Log("I am "+gameObject.scene.name + " and am loading "+ loadedScene.name );
            foreach(GameObject g in loadedScene.GetRootGameObjects()){
                if(g.TryGetComponent<LevelStartingPoint>(out LevelStartingPoint p)){
                    loadedLevelStartingPoint = p.transform.position;
                    break;
                }
            }
            foreach(GameObject g in loadedScene.GetRootGameObjects()){
                if(g.TryGetComponent<AdditiveLevelLoader>(out AdditiveLevelLoader nextLoader)){
                    nextLevelLoader = nextLoader;
                }
                if(g.TryGetComponent<Light>(out Light l)){
                    if(l.type == LightType.Directional){
                        g.SetActive(false);
                        continue;
                    }
                }
                g.transform.position = g.transform.position + levelEndingPoint - loadedLevelStartingPoint;
                if(g.CompareTag("Player")) g.SetActive(false);
            }
        };
        #endregion

        loaded = true;
    }

    private void becomeActive(GameObject player){
        SceneManager.MoveGameObjectToScene(player,SceneManager.GetSceneAt(ownSceneIndex));
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(ownSceneIndex));
        if((int)levelType == 1){
            for(int i = ownSceneIndex-1; i>=0; i--){
                if(SceneManager.GetSceneAt(i).name !="DontDestroyOnLoad"){
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex).completed += (AsyncOperation o) =>{
                        ownSceneIndex -=1;
                        Resources.UnloadUnusedAssets();
                    };
                }
            }
        }
        currentLevel = true;
        reinitActiveObjects();
    }

    private void reinitActiveObjects(){
        Debug.Log("Active scene is "+SceneManager.GetActiveScene().name);
        if(GameObject.FindObjectsOfType<Portal>().Length == 0){
            Debug.Log("No portals found");
        }
        foreach(Portal p in GameObject.FindObjectsOfType<Portal>()){
            p.reinitPlayerCam(null);
        }
    }

    public Vector3 getStartingPoint(){
        return transform.TransformPoint(GetComponentInChildren<LevelStartingPoint>().transform.position);
    }

}
