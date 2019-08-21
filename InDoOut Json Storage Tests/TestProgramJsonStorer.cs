﻿using InDoOut_Core.Functions;
using InDoOut_Json_Storage;
using InDoOut_Plugins.Loaders;

namespace InDoOut_Json_Storage_Tests
{
    internal class TestProgramJsonStorer : ProgramJsonStorer
    {
        public TestProgramJsonStorer(string path, IFunctionBuilder builder, ILoadedPlugins loadedPlugins) : base(builder, loadedPlugins, path)
        {
        }

        public bool SavePublic(JsonProgram program)
        {
            return Save(program);
        }

        public JsonProgram LoadPublic()
        {
            return Load();
        }
    }
}
