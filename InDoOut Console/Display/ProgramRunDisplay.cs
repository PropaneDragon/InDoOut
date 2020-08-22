using InDoOut_Core.Entities.Programs;
using System;
using System.Threading;

namespace InDoOut_Console.Display
{
    public class ProgramRunDisplay
    {
        public bool StartProgramDisplay(IProgram program)
        {
            if (program != null)
            {
                while (IsProgramRunning(program))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                return true;
            }

            return false;
        }

        private bool IsProgramRunning(IProgram program)
        {
            if (program != null)
            {
                var maxRetry = 100;

                for (var retryCount = 0; retryCount < maxRetry; ++retryCount)
                {
                    if (program.Running)
                    {
                        return true;
                    }
                    else
                    {
                        Thread.Sleep(TimeSpan.FromMilliseconds(5));
                    }
                }
            }

            return false;
        }
    }
}
