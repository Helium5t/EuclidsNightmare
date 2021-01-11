using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utility;

public static class SceneUtility
{
    public static int levelsNum = 25;
    public static string getSceneName(int buildIndex){
        return ((Levels) buildIndex).ToString();
    }
    public static int getSceneBuildIndex(string sceneName){
        for(int i =0; i<levelsNum;i++){
            if(sceneName == ((Levels)i).ToString()){
                return i;
            }
        }
        throw new System.Exception("No level in build with such name");
    }
}
