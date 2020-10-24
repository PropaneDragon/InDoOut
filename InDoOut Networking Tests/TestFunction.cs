using InDoOut_Core.Entities.Functions;
using System;

namespace InDoOut_Networking_Tests
{
    internal class TestFunction : Function
    {
        public override string Description => "ABCDEFGHIJKLMNOPQRSTUVWXYZ /0123456789" +
                                               "abcdefghijklmnopqrstuvwxyz £©µÀÆÖÞßéöÿ" +
                                               "–—‘“”„†•…‰™œŠŸž€ ΑΒΓΔΩαβγδω АБВГДабвгд" +
                                               "∀∂∈ℝ∧∪≡∞ ↑↗↨↻⇣ ┐┼╔╘░►☺♀ ﬁ�⑀₂ἠḂӥẄɐː⍎אԱა";

        public override string Name => "Hello world, Καλημέρα κόσμε, コンニチハ";

        public override string Group => "Testing";

        public override string[] Keywords => new[] { "Nah" };

        protected override IOutput Started(IInput triggeredBy) => throw new NotImplementedException();
    }
}
