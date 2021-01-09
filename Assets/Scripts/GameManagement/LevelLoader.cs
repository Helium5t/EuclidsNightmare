using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utility;
using Sensors.Door;

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

        [Header("Additive Level Loading Settings")] [SerializeField]
        private levelTag levelType = levelTag.Puzzle;

        [SerializeField] private bool useBuildIndex = false;
        [SerializeField] private Scene nextSceneToLoad;
        [SerializeField] private string nextSceneName;

        #region Internal Values and parameters for ALL

        public Vector3 levelStartingPoint { get; private set; }
        private Vector3 levelEndingPoint = Vector3.zero;

        private enum levelTag
        {
            Platform = 0,
            Puzzle = 1
        };

        private Vector3 loadedLevelStartingPoint = Vector3.zero;
        bool loaded = false;
        private AsyncOperation loadingStatus;
        public bool currentLevel { get; private set; }
        private bool firstLevel = false;
        private bool started = false;
        private int ownSceneIndex = 0;
        private LevelLoader nextLevelLoader;

        private string[] lastLevels ={"EndDemoRoom","FeedbackMenu"};
        private bool nextIsLast;

        #endregion


        private void Awake()
        {
            nextIsLast = false;
            currentLevel = false;
            _sceneName = gameObject.scene.name;
            _levelNameText = GameObject.FindGameObjectWithTag("LevelNameText").gameObject.GetComponent<Text>();
            if(useBuildIndex){
                nextSceneName = nextSceneToLoad.name;
                
            }
            foreach(string last in lastLevels){
                if(nextSceneName.Contains(last)){
                    nextIsLast = true;
                }
            }
            if(gameObject.scene == SceneManager.GetActiveScene()){
                currentLevel = true;
                firstLevel = true;
                Debug.LogError(gameObject.scene.name + " is first scene");
            }
            if(!firstLevel){
                foreach(DoorTrigger dt in GameObject.FindObjectsOfType<DoorTrigger>()){
                    if((int)dt.GetComponentInChildren<LevelTrigger>().mode == -1) dt.startOpen = true;
                }
                SceneAnimator.gameObject.SetActive(!useAdditiveLoading);
            }
            
        }

        private void Start()
        {
            if (!useAdditiveLoading) return;
            Transform endPosCube = transform.Find("EndingPoint");
            levelEndingPoint = transform.TransformVector(endPosCube.position);
            endPosCube.gameObject.SetActive(false);
            Transform startPos = transform.Find("StartingPoint");
            if (endPosCube)
            {
                levelEndingPoint = transform.TransformVector(endPosCube.position);
            }

            endPosCube.gameObject.SetActive(false);
            if (SceneManager.sceneCount < 3 && !loaded)
            {
                additiveLoadNextLevel();
            }

            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (SceneManager.GetSceneAt(i) == gameObject.scene)
                {
                    ownSceneIndex = i;
                    Debug.Log("My index is " + i + ": scene Name check is " +
                              SceneManager.GetSceneAt(ownSceneIndex).name);
                    break;
                }
            }
        }

        private void Update()
        {
            if (!useAdditiveLoading) return;

            #region additive loading section

            currentLevel = SceneManager.GetActiveScene() == gameObject.scene;
            if (Input.GetKeyDown(KeyCode.Q) && currentLevel)
            {
                startNextLevel();
            }

            #endregion
        }

        #region Single Level Loading

        public void LoadLevel(int levelBuildIndex) => StartCoroutine(LoadLevelRoutine(levelBuildIndex));

        public void LoadNextLevel()
        {
            if (!useAdditiveLoading) StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));
            else startNextLevel();
        }

        public void SkipLevel()
        {
            if (!useAdditiveLoading) StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex + 1));
            else StartCoroutine(LoadLevelRoutine(findNextPuzzle().gameObject.scene.buildIndex));
        }

        public void RestartCurrentLevel() => StartCoroutine(LoadLevelRoutine(SceneManager.GetActiveScene().buildIndex));

        public void DisplayLevelName()
        {
            if (_sceneName == Levels.MainMenu.ToString() || _sceneName == Levels.FeedbackMenu.ToString()) return;
            _levelNameText.text = SetLevelNameText("<:", ":>",true);
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
            //Just to be sure that everything is flowing as it should be
            Time.timeScale = 1f; 
            PauseMenu.GameIsPaused = false;
            
            if (useAdditiveLoading) SceneAnimator.gameObject.SetActive(true);

            SceneAnimator.SetTrigger(SceneTransitionTrigger);
            yield return new WaitForSeconds(sceneTransitionTime);
            SceneManager.LoadScene(levelBuildIndex, LoadSceneMode.Single);
            yield return null;
        }

        private string SetLevelNameText(string startDecoratorString, string endDecoratorString, bool splitOnCapital){
            if(splitOnCapital){
                List<string> words = new List<string>(3);
                string newWord = "" + (_sceneName[0]);
                for(int i =1;i<_sceneName.Length;i++){
                    if(Char.IsUpper(_sceneName[i])){
                        words.Add(newWord);
                        newWord = "";
                        newWord += _sceneName[i];
                    }
                    else{
                        newWord += _sceneName[i];
                    }
                }
                words.Add(newWord);
                string returned = startDecoratorString;
                Debug.Log(words.Count);
                for(int i =0; i<words.Count; i++){
                    returned += " ";
                    returned += words[i];
                }
                returned += " "+ endDecoratorString;
                return returned;
            }
            else return startDecoratorString + " " + _sceneName + " " + endDecoratorString;
        }
#endregion
#region Additive Level Loading
        private void additiveLoadNextLevel(){
            if(nextIsLast) return;
            if(useBuildIndex){
                loadingStatus = SceneManager.LoadSceneAsync(nextSceneToLoad.buildIndex,LoadSceneMode.Additive);
            }   
            else{
                if(nextSceneName.Length == 0) { useAdditiveLoading = false; return;}
                loadingStatus = SceneManager.LoadSceneAsync(nextSceneName,LoadSceneMode.Additive);
            }
            Debug.LogError(gameObject.scene.name + " loading next Scene");
            #region loading completed routine

            loadingStatus.completed += (AsyncOperation o) =>
            {
                Scene loadedScene = SceneManager.GetSceneAt(ownSceneIndex + 1);
                for (int i = 0; i < SceneManager.sceneCount; i++)
                {
                    if (SceneManager.GetSceneAt(i).name == nextSceneName)
                    {
                        loadedScene = SceneManager.GetSceneAt(i);
                        break;
                    }
                }

                //Debug.Log("I am "+gameObject.scene.name + " and am loading "+ loadedScene.name );
                foreach (GameObject g in loadedScene.GetRootGameObjects())
                {
                    if (g.TryGetComponent<LevelLoader>(out nextLevelLoader))
                    {
                        loadedLevelStartingPoint = g.GetComponentInChildren<LevelStartingPoint>().transform.position;
                        break;
                    }
                }
                if(!nextLevelLoader){
                    Debug.LogError("Could not find next Level Loader");
                }

                foreach (GameObject g in loadedScene.GetRootGameObjects())
                {
                    if (g.TryGetComponent<Light>(out Light l))
                    {
                        if (l.type == LightType.Directional)
                        {
                            //g.SetActive(false);
                            Destroy(g);
                            continue;
                        }
                    }

                    g.transform.position = g.transform.position + levelEndingPoint - loadedLevelStartingPoint;
                    if (g.CompareTag("Player")) Destroy(g); //g.SetActive(false);
                }
            };

            #endregion

            loaded = true;
        }

        private void becomeActive(GameObject player, GameObject dirLight)
        {
            SceneManager.MoveGameObjectToScene(dirLight, SceneManager.GetSceneAt(ownSceneIndex));
            becomeActive(player);
        }

        private void becomeActive(GameObject player)
        {
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetSceneAt(ownSceneIndex));
            Debug.Log("Setting " + SceneManager.GetSceneAt(ownSceneIndex).name + " as active");
            SceneManager.SetActiveScene(SceneManager.GetSceneAt(ownSceneIndex));
            Debug.LogError(SceneManager.GetSceneAt(ownSceneIndex).name + " is now active");
            Debug.LogError(SceneManager.GetActiveScene().name + " double check");
            currentLevel = true;
            StartCoroutine("reinitActiveObjects", player);
        }

        private IEnumerator unloadPreviousScenes(){
            if(firstLevel){ 
                Debug.Log(gameObject.scene.name + "is the first level, not unloading");
                yield break;
            }
            yield return null;
            
            Debug.LogError(gameObject.scene.name + " deleting previous scenes");
            for (int i = ownSceneIndex - 1; i >= 0; i--)
            {
                if (SceneManager.GetSceneAt(i).name != "DontDestroyOnLoad")
                {
                    SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i).buildIndex).completed +=
                        (AsyncOperation o) =>
                        {
                            ownSceneIndex -= 1;
                            Resources.UnloadUnusedAssets();
                        };
                }

                yield return null;
            }
            yield return null;
            additiveLoadNextLevel();
        }

        private IEnumerator reinitActiveObjects(GameObject player)
        {
            yield return null;
            player.GetComponentInChildren<PauseMenu>().updateLoaderReference(this);
            List<Portal> refreshedPortals = PortalUtility.findPortalsInScenes();
            yield return null;
            if (refreshedPortals.Count == 0)
            {
                Debug.Log("No portals found");
            }
            else
            {
                foreach (Portal p in refreshedPortals)
                {
                    p.reinitPlayerCam(player.GetComponentInChildren<Camera>());
                    yield return null;
                }
            }

            foreach (NonEuclideanTunnel net in PortalUtility.findNETSInScenes())
            {
                net.reinitPlayer(player);
                yield return null;
            }

            yield return null;
            player.GetComponentInChildren<MainCamera>().resetCamera();
        }

        public void startLevel(){
            if(started) {
                Debug.Log("Level has already started");
                return;
            }
            started = true;
            if((int)levelType == 1){
                Debug.Log(gameObject.scene.name+ ":Unloading scenes");
                StartCoroutine(nameof(unloadPreviousScenes));
            }
        }

        public void startNextLevel(){
            if(nextIsLast){
                SceneAnimator.gameObject.SetActive(true);
                useAdditiveLoading = false;
                LoadNextLevel();
                return;
            }
            try{            
                Debug.LogError(gameObject.scene.name + ":starting "+ nextLevelLoader.gameObject.scene.name);
                }
            catch(MissingReferenceException e){
                Debug.LogError("Cannot start next level, next loader not found");
            }
            GameObject dirLight = null;
            foreach (Light l in GameObject.FindObjectsOfType<Light>())
            {
                if (l.type == LightType.Directional)
                {
                    l.transform.parent = null;
                    dirLight = l.gameObject;
                }
            }

            if (dirLight != null)
            {
                nextLevelLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"), dirLight);
            }
            else
            {
                nextLevelLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"));
            }

            currentLevel = false;
        }

        public void startNextLevel(LevelLoader nextLoader)
        {
            GameObject dirLight = null;
            foreach (Light l in GameObject.FindObjectsOfType<Light>())
            {
                if (l.type == LightType.Directional)
                {
                    dirLight = l.gameObject;
                }
            }

            if (dirLight != null)
            {
                nextLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"), dirLight);
            }
            else
            {
                nextLoader.becomeActive(GameObject.FindGameObjectWithTag("Player"));
            }

            currentLevel = false;
        }

        public void forceStartLevel()
        {
            Debug.LogError(gameObject.scene.name + " is forcing level transition");
            LevelLoader activeLoader = null;
            foreach (GameObject g in SceneManager.GetActiveScene().GetRootGameObjects())
            {
                if (g.TryGetComponent<LevelLoader>(out activeLoader))
                {
                    activeLoader.startNextLevel(this);
                    break;
                }
            }
        }

        private LevelLoader findNextPuzzle()
        {
            if (nextLevelLoader.levelType == levelTag.Puzzle)
            {
                return nextLevelLoader;
            }
            else
            {
                return nextLevelLoader.findNextPuzzle();
            }
        }
    }

    #endregion
}