using InDoOut_Core.Variables;
using System.Collections.Generic;

namespace InDoOut_Core_Tests
{
    internal class TestVariableStore : VariableStore
    {
        public List<IVariable> PublicVariables => Variables;
    }
}
