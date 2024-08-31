using UnityEditor;
using UnityEngine.Assertions;

namespace Tests
{
    public class EditorModeTest
    {

        [NUnit.Framework.Test]
        public void EditorModeTestSimplePasses()
        {
            // Use the Assert class to test conditions.
            
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityEngine.TestTools.UnityTest]
        public System.Collections.IEnumerator EditorModeTestWithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // yield to skip a frame
            Assert.AreEqual(2, 2);
            yield return null;
        }
    }
}