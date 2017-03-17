using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ltracker.Data;
using ltracker.Data.Repositories;
using ltracker.Data.Entities;

namespace ltracker.Tests.Repositories
{
    /// <summary>
    /// Descripción resumida de RepositoriesTest
    /// </summary>
    [TestClass]
    public class RepositoriesTest
    {
        public RepositoriesTest()
        {
            //
            // TODO: Agregar aquí la lógica del constructor
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Obtiene o establece el contexto de las pruebas que proporciona
        ///información y funcionalidad para la serie de pruebas actual.
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

        #region Atributos de prueba adicionales
        //
        // Puede usar los siguientes atributos adicionales conforme escribe las pruebas:
        //
        // Use ClassInitialize para ejecutar el código antes de ejecutar la primera prueba en la clase
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup para ejecutar el código una vez ejecutadas todas las pruebas en una clase
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Usar TestInitialize para ejecutar el código antes de ejecutar cada prueba 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup para ejecutar el código una vez ejecutadas todas las pruebas
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void Topic_ShouldInsertOK()
        {
            try
            {
                var context = new LearningContext();
                var topicRepository = new TopicRepository(context);
                topicRepository.Insert(new Topic { Name = "Entity Framework ", Description = "Applying EF6 Data Access Strategy" });
                context.SaveChanges();
                Assert.IsFalse(false);
            }
            catch (Exception ex) {
                Assert.IsFalse(true);
            }
        }
    }
}
