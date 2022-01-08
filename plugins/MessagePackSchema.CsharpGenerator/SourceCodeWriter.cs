using CsCodeGenerator;
using CsCodeGenerator.Enums;

namespace MessagePackSchema.CsharpGenerator
{
    /// <summary>
    /// Provides methods to write source code files.
    /// </summary>
    public class SourceCodeWriter
    {
        private readonly string mFileName;
        private readonly FileModel mFileModel;
        private readonly CsGenerator mGenerator;
        private readonly IList<ClassModel> mClasses;

        public SourceCodeWriter(string fileName)
        {
            mFileName = fileName;
            mGenerator = new();
            mFileModel = new(fileName);
            mClasses = new List<ClassModel>();
        }

        public void WriteUsingDirectives(List<string> directives)
        {
            mFileModel.LoadUsingDirectives(directives);
        }

        public ClassModel WriteClass(string name, KeyWord[] keywords, string? baseClass = null, string[]? interfaces = null)
        {
            ClassModel classModel = new(name);

            if (baseClass != null)
                classModel.BaseClass = baseClass;

            if (interfaces != null)
                classModel.Interfaces = interfaces.ToList();

            classModel.KeyWords = keywords.ToList();
            mClasses.Add(classModel);

            return classModel;
        }
    }
}