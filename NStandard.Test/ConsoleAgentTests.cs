﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace NStandard.Test
{
    public class ConsoleAgentTests
    {
        [Fact]
        public void Test1()
        {
            using (var agent = new ConsoleAgent())
            {
                Console.WriteLine(123);
                Console.WriteLine(456);

                var output = agent.ReadAllText();
                Assert.Equal($@"123{Environment.NewLine}456{Environment.NewLine}", output);
            }

        }

    }
}
