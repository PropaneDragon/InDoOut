using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace InDoOut_Desktop_Tests
{
    [TestClass]
    public class ActionHandlerTests
    {
        [TestMethod]
        public void DefaultAction()
        {
            var defaultAction = new TestActionMain();
            var actionHandler = new TestActionHandler(defaultAction);

            Assert.IsNotNull(actionHandler.DefaultAction);
            Assert.IsNotNull(actionHandler.CurrentAction);
            Assert.AreEqual(defaultAction, actionHandler.DefaultAction);
            Assert.AreEqual(defaultAction, actionHandler.CurrentAction);

            var alternateAction = new TestActionMain();

            defaultAction.NextAction = alternateAction;
            defaultAction.Returns = true;

            Assert.AreEqual(defaultAction, actionHandler.CurrentAction);
            Assert.IsTrue(actionHandler.MouseLeftDown(new Point(0, 0)));
            Assert.AreEqual(alternateAction, actionHandler.CurrentAction);

            alternateAction.NextAction = null;
            alternateAction.Returns = false;

            Assert.IsFalse(actionHandler.MouseLeftDown(new Point(0, 0)));
            Assert.AreEqual(defaultAction, actionHandler.CurrentAction);
        }

        [TestMethod]
        public void EventPassthrough()
        {
            var defaultAction = new TestActionMain();
            var actionHandler = new TestActionHandler(defaultAction);

            defaultAction.Returns = false;

            Assert.IsFalse(actionHandler.MouseLeftDown(new Point(0, 0)));
            Assert.IsFalse(actionHandler.MouseLeftMove(new Point(0, 0)));
            Assert.IsFalse(actionHandler.MouseLeftUp(new Point(0, 0)));
            Assert.IsFalse(actionHandler.MouseRightDown(new Point(0, 0)));
            Assert.IsFalse(actionHandler.MouseRightMove(new Point(0, 0)));
            Assert.IsFalse(actionHandler.MouseRightUp(new Point(0, 0)));

            defaultAction.Returns = true;

            Assert.IsTrue(actionHandler.MouseLeftDown(new Point(0, 0)));
            Assert.IsTrue(actionHandler.MouseLeftMove(new Point(0, 0)));
            Assert.IsTrue(actionHandler.MouseLeftUp(new Point(0, 0)));
            Assert.IsTrue(actionHandler.MouseRightDown(new Point(0, 0)));
            Assert.IsTrue(actionHandler.MouseRightMove(new Point(0, 0)));
            Assert.IsTrue(actionHandler.MouseRightUp(new Point(0, 0)));
        }
    }
}
