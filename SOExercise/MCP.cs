using System;
using System.Collections.Generic;

namespace SOExercise
{
    public class MCP
    {
        public struct Core
        {
            public Core(int coreId, double coreWCETFactor)
            {
                ID = coreId;
                WCETF = coreWCETFactor;
            }
            public int ID { get; set; }
            public double WCETF { get; set; }
        }
        public List<List<Core>> core;
        public MCP()
        {
            core = new List<List<Core>>();
        }
    }
}