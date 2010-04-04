using System.Collections.Generic;
using StoryTeller.Workspace;

namespace StoryTeller.Domain
{
    public class Hierarchy : Suite
    {
        public Hierarchy(IProject project)
            : base(project.Name)
        {
        }

        public Hierarchy(string name)
            : base(name)
        {
        }

        public Test AddTest(string testPath)
        {
            var path = new TPath(testPath);

            var test = new Test(path.Name);
            Suite suite = path.IsEnd ? this : findOrCreateSuite(path.ToSuite());
            suite.AddTest(test);

            return test;
        }

        public override TPath GetPath()
        {
            return new TPath("");
        }

        public void ClearResults()
        {
            GetAllTests().Each(x => x.Reset());
        }
    }
}