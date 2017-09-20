using System;
using System.Collections.Generic;
using SolutionMerger.Parsers;

namespace SolutionMerger.Models
{
    public class ProjectDirectory : BaseProject
    {
        private const string DirPackageGuid = "{2150E333-8FDC-42A3-9474-1A3956D46DE8}";

        public ProjectDirectory(string name, string guid = null, string packageGuid = null)
        {
            Guid = guid ?? "{" + System.Guid.NewGuid().ToString().ToUpper() + "}";
            Name = name;
            ProjectInfo = new ProjectInfo(null, packageGuid ?? DirPackageGuid, Environment.NewLine) { Project = this };
            NestedProjects = new List<ProjectRelationInfo>();
        }

        public override string Location { get { return Name; } }
        public NestedProjectsInfo NestedProjectsInfo { get; set; }
        public List<ProjectRelationInfo> NestedProjects { get; private set; }
    }
}