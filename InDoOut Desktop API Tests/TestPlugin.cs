﻿using InDoOut_Plugins.Core;

namespace InDoOut_Desktop_API_Tests
{
    public class TestPlugin : Plugin
    {
        public string PublicName { get; set; } = "Default name";
        public string PublicDescription { get; set; } = null;
        public string PublicAuthor { get; set; } = null;

        public override string Name => PublicName;

        public override string Description => PublicDescription;

        public override string Author => PublicAuthor;
    }
}
