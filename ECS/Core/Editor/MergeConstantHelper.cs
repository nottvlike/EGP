namespace CoreEditor
{
    using UnityEditor;
    using UnityEngine;
    using Mono.Cecil;
    using System.Linq;
    using System.IO;
    using ECS.Common;

    public class MergeConstantHelper
    {
        const string CONSTANT_CLASS_NAME = "Constant";
        const string MAIN_ASSEMBLY_PATH = "Assets/EGP/Plugins/libECSCore.dll";

        static string[] AssemblyPathList =
        {
            "Assets/EGP/Plugins/libECSUI.dll",
            "Assets/EGP/Plugins/libECSObject.dll",
            "Assets/EGP/Plugins/libAssetManager.dll"
        };

        [MenuItem("Tools/Merge Constant")]
        public static void BuildStandaloneAssetBundle()
        {
            foreach (var path in AssemblyPathList)
            {
                var assembly = AssemblyDefinition.ReadAssembly(GetAssemblyFullPath(path));
                if (assembly == null)
                {
                    Debug.LogError(string.Format("Load assembly failed: {0}", path));
                    return;
                }

                MergeAssemblyConstant(assembly);
                assembly.Dispose();
            }

            SaveMainConstantClass();
        }

        static string GetAssemblyFullPath(string path)
        {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), path);
        }

        static AssemblyDefinition mainConstantDefinition;
        static TypeDefinition GetMainConstantClass()
        {
            if (mainConstantDefinition == null)
            {
                mainConstantDefinition = AssemblyDefinition.ReadAssembly(GetAssemblyFullPath(MAIN_ASSEMBLY_PATH));
            }
            return mainConstantDefinition.MainModule.Types.Where(_ => _.Name == CONSTANT_CLASS_NAME).First(); ;
        }

        static void SaveMainConstantClass()
        {
            mainConstantDefinition.Write(GetAssemblyFullPath(MAIN_ASSEMBLY_PATH));
            mainConstantDefinition.Dispose();
            mainConstantDefinition = null;
        }

        static TypeDefinition[] GetConstantClassList(AssemblyDefinition assembly)
        {

            return assembly.Modules.SelectMany(_ => _.Types)
                .Where(_ => !_.IsAbstract && _.IsClass && !_.IsInterface)
                .Where(n => n.HasCustomAttributes && n.CustomAttributes.Any(y =>
                {
                    return y.Constructor.DeclaringType.ToString() == typeof(MergeConstant).ToString();
                })
                ).ToArray();
        }

        static void MergeAssemblyConstant(AssemblyDefinition assembly)
        {
            var mainConstantClass = GetMainConstantClass();
            var constantClassList = GetConstantClassList(assembly);

            foreach (var constantClass in constantClassList)
            {
                foreach (var filed in constantClass.Fields)
                {
                    if (!filed.IsPublic || !filed.IsStatic)
                    {
                        continue;
                    }

                    var hasSameField = false;
                    foreach (var tmpField in mainConstantClass.Fields)
                    {
                        if (tmpField.Name == filed.Name)
                        {
                            hasSameField = true;
                            break;
                        }
                    }

                    if (hasSameField)
                    {
                        continue;
                    }

                    var newFiled = new FieldDefinition(filed.Name, filed.Attributes, filed.FieldType);
                    newFiled.IsPublic = true;
                    newFiled.IsStatic = true;
                    newFiled.IsInitOnly = true;
                    newFiled.InitialValue = filed.InitialValue;
                    newFiled.Constant = filed.Constant;
                    newFiled.HasConstant = filed.HasConstant;

                    mainConstantClass.Fields.Add(newFiled);
                }
                
            }
        }
    }
}