using GeneratorSubsystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace TestGenerator
{
    
    
    /// <summary>
    ///This is a test class for GeneratorTest and is intended
    ///to contain all GeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GeneratorTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for generateRequests
        ///</summary>
        [TestMethod()]
        public void generateRequestsTest()
        {
            Request[] actual;
            UniformGen gen1=new UniformGen(1,10);
            UniformGen gen2=new UniformGen(1,10);
            UniformGen gen3=new UniformGen(1,10);
            ExponentialGen timeGen = new ExponentialGen(30);
            Generator target = new Generator(timeGen, gen1, gen2, gen3);
            DateTime dt = new DateTime(2010,10,13,8,0,0);
            actual = target.generateRequests(dt);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Generator Constructor
        ///</summary>
        [TestMethod()]
        public void GeneratorConstructorTest()
        {
            RayleighGen gen1 = new RayleighGen(8);
            NormalGen gen2 = new NormalGen(7, 2);
            GammaGen gen3 = new GammaGen(0.6, 6);
            GammaGen timeGen = new GammaGen(8,30);
            Generator target = new Generator(timeGen, gen1, gen2, gen3);
            Assert.IsNotNull(target);
        }
    }
}
