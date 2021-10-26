using System.Collections.Generic;

namespace SOExercise
{
    public class Task
    {
        public List<int> Deadline;
        public List<int> Id;
        public List<int> Period;
        public List<int> WCET;
        public Task()
        {
            Deadline = new List<int>();
            Id = new List<int>();
            Period = new List<int>();
            WCET = new List<int>();
        }
    }
}