using Moq;
using System;

namespace Store.Tests.MockData.MockIdentity
{
    public class FakeRoleManagerBuilder
    {

        private Mock<FakeRoleManager> _mock = new Mock<FakeRoleManager>();

        public FakeRoleManagerBuilder With(Action<Mock<FakeRoleManager>> mock)
        {
            mock(_mock);
            return this;
        }
        public Mock<FakeRoleManager> Build()
        {
            return _mock;
        }


    }
}
