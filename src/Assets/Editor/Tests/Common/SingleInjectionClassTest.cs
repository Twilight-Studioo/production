#region

using NUnit.Framework;
using VContainer;
using Assert = UnityEngine.Assertions.Assert;

#endregion

namespace Editor.Tests.Common
{
    public abstract class SingleInjectionClassTest<T> where T : class
    {
        protected IObjectResolver Container; // DIコンテナ
        protected T TestInstance; // テスト対象のインスタンス

        [SetUp]
        public virtual void SetUp()
        {
            // ContainerBuilderの作成
            var builder = new ContainerBuilder();

            // 必要な依存関係を登録
            RegisterDependencies(builder);

            // テスト対象のクラスを登録
            builder.Register<T>(Lifetime.Transient);

            // コンテナのビルド
            Container = builder.Build();

            // テスト対象のインスタンスを解決
            TestInstance = Container.Resolve<T>();
        }

        // 必要な依存関係を登録するためのメソッド（派生クラスで実装）
        protected virtual void RegisterDependencies(IContainerBuilder builder)
        {
            // 依存関係を登録するロジック
        }

        [Test]
        public void インスタンス作成が成功しているか()
        {
            Assert.IsNotNull(TestInstance);
        }
    }
}