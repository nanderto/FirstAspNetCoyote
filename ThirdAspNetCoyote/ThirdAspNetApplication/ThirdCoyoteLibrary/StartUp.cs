using System;
using System.Collections.Generic;
using System.Text;

namespace ThirdCoyoteLibrary
{
    public interface IStartUp
    {
        void Start();
    }

    public class StartUp : IStartUp
    {
        public StartUp(ICoyoteRuntime runtime)
        {
            Runtime = runtime;
        }

        public ICoyoteRuntime Runtime { get; }

        public void Start()
        {

        }
    }
}
