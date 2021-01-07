using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;

namespace GameManagement
{
    public class LevelLoader : MonoBehaviour
    {

        [Header("Exposed Parameters")] [SerializeField]
        private float sceneTransitionTime;

        [SerializeField] private float levelNameTransitionTime = 1f;

        [Space] [Header("Animators")] public Animator SceneAnimator;
        public Animator LevelNameAnimator;

        private static readonly int SceneTransitionTrigger = Animator.StringToHash("Start");
        private static readonly int LevelNameTransitionTrigger = Animator.StringToHash("NameFadeStart");

        private Text _levelNameText;
        private string _sceneName;
        
        [SerializeField] private bool useAdditiveLoading = true;
    
        [Header("Additive Level Loading Settings")]
        [SerializeField] private levelTag levelType = levelTag.Puzzle;
        [SerializeField] private bool useBuildIndex = false;
        [SerializeField] private Scene nextSceneToLoad;
        [SerializeField] private string nextSceneName;

        #region Internal Values and parameters for ALL
        
        public Vector3 levelStartingPoint{get;private set;}
        private Vector3 levelEndingPoint = Vector3.zero;
        private enum levelTag{Platform = 0,Puzzle = 1};
        private Vector3 loadedLevelStartingPoint = Vector3.zero;
        bool loaded = false;
        private AsyncOperation loadingStatus;
        public bool currentLevel{get;private set;}
        private bool firstLevel = false;
        private bool started = false;
        private int ownSceneIndex= 0;
        private LevelLoader nextLevelLoader;



        #endregion



        private void Awake()
        {
            currentLevel = false;
            _sceneName = gameObject.scene.name;
            _levelNameText = GameObject.FindGameObjectWithTag("LevelNameText").gameObject.GetComponent<Text>();
            if(gameObject.scene == SceneManager.GetActiveScene()){
                currentLevel = true;
                firstLevel = true;
            }
            if(useAdditiveLoading && !firstLevel){
                SceneAnimator.gameObject.SetActive(false);
            }

        }
        private void Start() {
            if(!useAdditiveLoading) return;
            Transform endPosCube = transform.Find("EndingPoint");
            levelEndingPoint = transform.TransformVector(endPosCube.position);
            endPosCube.gameObject.SetActive(false);
            Transform startPos = transform.Find("StartingPoint");
            if(endPosCube){
                levelEndingPoint = transform.TransformVector(endPosCube.position);
            }
            endPosCube.gameObject.SetActive(false);
            if(SceneManager.sceneCount < 3 && !loaded){
                additiveLoadNextLevel();
            }
            for(int i =0; i<SceneManager.sceneCount;i++){
                if(SceneManager.GetSceneAt(i) == gameObject.scene){
                    ownSceneIndex = i;
                    Debug.Log("My index is "+ i+ ": scene Name check is " + SceneManager.GetSceneAt(ownSceneIndex).name);
                    break;
                }
            }
        }

        private void Update() { 


            if(!useAdditiveLoading) return;
            #region additive loading section
            currentLevel = SceneManager.GetActiveScene() == gameObject.scene;
            if(Input.GetKeyDown(KeyCode.Q) && currentLevel){
                startNextLevel();
            }
            #endregion
        }
#region Single Level Loading
        public void LoadLevel(int levelBuildIndex) => StartCoroutine(LoadLevelRoutine(levelBuildIndex));

        public void LoadNextLevel(){ 
            if(!useAdditiveLoading) StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));
            else startNextLevel();
        }

        public void SkipLevel(){
            if(!useAdditiveLoading) StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));
            else StartCoroutine(LoadLevelRoutine(findNextPuzzle().gameObject.scene.buildIndex));
        }

        public void RestartCurrentLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));

        public void DisplayLevelName()
        {
            if (_sceneName == Levels.MainMenu.ToString() || _sceneName == Levels.FeedbackMenu.ToString()) return;
            //_levelNameText.text = _sceneBuildIndex + ") " + _sceneName;
            _levelNameText.text = SetLevelNameText("<:", ":>");
            StartCoroutine(LevelNameFade());
        }

        private IEnumerator LevelNameFade()
        {
            LevelNameAnimator.SetTrigger(LevelNameTransitionTrigger);
            yield return new WaitForSeconds(levelNameTransitionTime);
            LevelNameAnimator.SetTrigger(LevelNameTransitionTrigger);
            yield return null;
        }

        private IEnumerator LoadLevelRoutine(int levelBuildIndex)
        {
            Time.timeScale = 1f; //Just to be sure that everything is flowing as it should be
            if(useAdditiveLoading){
                SceneAnimator.gameObject.SetActive(true);
            }
            SceneAnimator.SetTrigger(SceneTransitionTrigger);
            yield return new WaitForSeconds(sceneTransitionTime);
            SceneManager.LoadScene(levelBuildIndex,LoadSceneMode.Single);
            yield return null;
        }

        private string SetLevelNameText(string startDecoratorString, string endDecoratorString) =>
            startDecoratorString + " " + _sceneName + " " + endDecoratorString;
#endregion
#region Additive Level Loading
        private void additiveLoadNextLevel(){
            if(useBuildIndex){
                loadingStatus = SceneManager.LoadSceneAsync(nextSceneToLoad.buildIndex,LoadSceneMode.Additive);
                nextSceneName = nextSceneToLoad.name;
            }   
            else{
                if(nextSceneName.Length == 0) { useAdditiveLoading = false; return;}
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
                //Debug.Log("I am "+gameObject.scene.name + " and am loading "+ loadedScene.name );
                foreach(GameObject g in loadedScene.GetRootGameObjects()){
                    if(g.TryGetComponent<LevelLoader>(out nextLevelLoader)){
                        loadedLevelStartingPoint = g.GetComponentInChildren<LevelStartingPoint>().transform.position;
                        break;
                    }
                    
                }
                foreach(GameObject g in loadedScene.GetRootGameObjects()){
                    if(g.TryGetComponent<Light>(out Light l)){
                        if(l.type == LightType.Directional){
                            //g.SetActive(false);
                            Destroy(g);
                            continue;
                        }
                    }
                    g.transform.position = g.transform.position + levelEndingPoint - loadedLevelStartingPoint;
                    if(g.CompareTag("Player")) Destroy(g);//g.SetActive(false);
                }
            };
            #endregion

            loaded = true;
        }

        private void becomeActive(GameObject player,GameObject dirLight){
            SceneManager.MoveGameObjectToScene(dirLight,SceneManager.GetSceneAt(ownSceneIndex));
            becomeActive(player);
        }
        private void becomeActive(GameObject player){
            SceneManager.MoveGameObjectToScene(player,SceneManager.GetSceneAt(ownSceneIndex));
            Debug.Log("Setting "+SceneManager.GetSceneAt(ownSceneIndex).name + " as active");
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(ownSceneIndex));
            currentLevel = true;
            StartCoroutine("reinitActiveObjects",player);
        }

        private IEnumerator unloadPreviousScenes(){
            if(firstLevel) yield break;
            yield return null;
            for(int i = ownSceneIndex-1; i>=0; i--){
                    if(SceneManager.GetSceneAt(i).name !="DontDestroyOnLoad"){
                        SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex).completed += (AsyncOperation o) =>{
                            ownSceneIndex -=1;
                            Resources.UnloadUnusedAssets();
                        };
                    }
                    yield return null;
                }
            yield return null;
            additiveLoadNextLevel();
        }

        private IEnumerator reinitActiveObjects(GameObject player){
            yield return null;
            List<Portal> refreshedPortals = PortalUtility.findPortalsInScenes();
            yield return null;
            if(refreshedPortals.Count == 0){
                Debug.Log("No portals found");
            }
            else{
                foreach(Portal p in refreshedPortals){
                    p.reinitPlayerCam(player.GetComponentInChildren<Camera>());
                    yield return null;
                }
            }
            foreach(NonEuclideanTunnel net in PortalUtility.findNETSInScenes()){
                net.reinitPlayer(player);
                yield return null;
            }
            yield return null;
            player.GetComponentInChildren<MainCamera>().resetCamera();
        }

        public void startLevel(){
            if(started) return;
            started = true;
            if((int)levelType == 1){
                StartCoroutine("unloadPreviousScenes");
            }
        }

        public void startNextLevel(){
            Debug.Log("I am "+ gameObject.scene.name + " and am starting "+ nextLevelLoader.gameObject.scene.name);
            GameObject dirLight = null;
            foreach( Light l in GameObject.FindObjectsOfType<Light>()){
                if(l.type == LightType.Directional){
                    l.transform.parent = null;
                    dirLight = l.gameObject;
                }
            }
            if(dirLight!=null){
                nextLevelLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"),dirLight);
            }
            else{
                nextLevelLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"));
            }
            currentLevel = false;
        }
        public void startNextLevel(LevelLoader nextLoader){
            GameObject dirLight = null;
            foreach( Light l in GameObject.FindObjectsOfType<Light>()){
                if(l.type == LightType.Directional){
                    dirLight = l.gameObject;
                }
            }
            if(dirLight!=null){
                nextLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"),dirLight);
            }
            else{
                nextLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"));
            }
            currentLevel = false;
        }

        public void forceStartLevel(){
            Debug.LogError(gameObject.scene.name + " is forcing level transition");
            LevelLoader activeLoader = null;
            foreach(GameObject g in SceneManager.GetActiveScene().GetRootGameObjects()){
                if(g.TryGetComponent<LevelLoader>(out activeLoader)){
                    activeLoader.startNextLevel(this);
                    break;
                }
            }
        }

        private LevelLoader findNextPuzzle(){
            if(nextLevelLoader.levelType==levelTag.Puzzle){
                return nextLevelLoader;
            }
            else{
                return nextLevelLoader.findNextPuzzle();
            }
        }
    }
#endregion
}