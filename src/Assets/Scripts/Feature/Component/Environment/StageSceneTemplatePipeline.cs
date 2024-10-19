#region

using UnityEditor.SceneTemplate;
using UnityEngine.SceneManagement;

#endregion

namespace Feature.Component.Environment
{
    public class StageSceneTemplatePipeline : ISceneTemplatePipeline
    {
        public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset) => true;

        public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive,
            string sceneName)
        {
        }

        public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene,
            bool isAdditive, string sceneName)
        {
        }
    }
}