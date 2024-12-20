using Feature.Interface;
using Core.Utilities;
using UnityEngine.SceneManagement;

// <auto-generated>
// This code was generated by SceneUtility.
// Do not modify this file manually.
// </auto-generated>

namespace Main.Scene.Generated
{
    public enum Scene
    {
        TitleScene,
        Stage,
        GameOverScene,
    }

    public static class SceneLoaderFeatures
    {
        public static SceneLoader TitleScene(ISceneDataModel sceneDataModel)
        {
            return new SceneLoader(Scene.TitleScene, "Assets/Scenes/OutGame/TitleScene.unity", sceneDataModel);
        }
        public static SceneLoader Stage(ISceneDataModel sceneDataModel)
        {
            return new SceneLoader(Scene.Stage, "Assets/Scenes/Template/Stage.unity", sceneDataModel);
        }
        public static SceneLoader GameOverScene(ISceneDataModel sceneDataModel)
        {
            return new SceneLoader(Scene.GameOverScene, "Assets/Scenes/OutGame/GameOverScene.unity", sceneDataModel);
        }
    }
}
