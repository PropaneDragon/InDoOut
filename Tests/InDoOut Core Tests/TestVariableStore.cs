using InDoOut_Core.Variables;
using System.Collections.Generic;

namespace InDoOut_Core_Tests
{
    public class TestVariableStore : VariableStore
    {
        public List<IVariable> PublicVariables => Variables;
    }
}
