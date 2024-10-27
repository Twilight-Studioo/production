#region

using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

#endregion

namespace Tests
{
    public class PlayModeTest
    {
        [Test]
        public void PlayModeTestSimplePasses()
        {
            // Use the Assert class to test conditions.
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator PlayModeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            yield return null;
        }
    }
}