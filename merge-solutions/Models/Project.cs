using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SolutionMerger.Parsers;
using SolutionMerger.Utils;

namespace SolutionMerger.Models
{
    public class Project : BaseProject
    {
        public override string Location { get { return AbsolutePath; } }

        public string AbsolutePath { get; set; }

        public static void GenerateProjectDirs(NestedProjectsInfo nestedSection, List<BaseProject> projects)
        {
            Func<BaseProject, string> getActualSolutionName = p =>
                p is ProjectDirectory || p.Location.IsWebSiteUrl() || p.Location.StartsWith(p.SolutionDir)//Means it is a project that is located inside solution base folder or a project directory or its a website
                    ? p.SolutionName
                    : PathHelpers.GetDirName(Path.GetDirectoryName(Path.GetDirectoryName(p.Location)));

            var groupedSoltions = projects.ToArray().GroupBy(getActualSolutionName);
            foreach (var group in groupedSoltions)
            {
                var dir = new ProjectDirectory(group.Key);
                projects.Add(dir);

                dir.NestedProjectsInfo = nestedSection;
                nestedSection.Dirs.Add(dir);
                dir.NestedProjects.AddRange(group.Select(pr => new ProjectRelationInfo(pr, dir)));
            }
        }
    }
}