using System.Threading.Tasks;
using Core.Utilities;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Main.Controller
{
    public class RootManager: IStartable
    {
        [Inject]
        public RootManager()
        {
        }
        
        public void Start()
        {
            _ = InitializeCloudService();
        }
        
        private static async Task InitializeCloudService()
        {
            await CloudService.Instance.Initialize();
        }
    }
}