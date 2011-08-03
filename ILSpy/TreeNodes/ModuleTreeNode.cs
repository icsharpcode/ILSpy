using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.TreeNodes
{
    public sealed class ModuleTreeNode : ILSpyTreeNode
    {
        private ModuleDefinition module;
        private AssemblyTreeNode assemblyNode;

        #region .ctor
        public ModuleTreeNode(ModuleDefinition moduleDefinition, AssemblyTreeNode parentAssemblyNode)
        {
            if (moduleDefinition == null)
                throw new ArgumentNullException("moduleDefiniton");
            module = moduleDefinition;
            assemblyNode = parentAssemblyNode;

            this.LazyLoading = true;
        }
        #endregion

        #region Icon & Text

        public override object Icon
        {
            get
            {
                return Images.Library;
            }
        }

        public override object Text
        {
            get
            {
                return module.Name;
            }
        }

        #endregion

        #region Children

        Dictionary<string, NamespaceTreeNode> namespaces = new Dictionary<string, NamespaceTreeNode>();
        Dictionary<TypeDefinition, TypeTreeNode> typeDict = new Dictionary<TypeDefinition, TypeTreeNode>();

        protected override void LoadChildren()
        {
            this.Children.Add(new ReferenceFolderTreeNode(module, assemblyNode));
            if (module.HasResources)
                this.Children.Add(new ResourceListTreeNode(module));
            foreach (NamespaceTreeNode ns in namespaces.Values)
            {
                ns.Children.Clear();
            }
            foreach (TypeDefinition type in module.Types.OrderBy(t => t.FullName))
            {
                NamespaceTreeNode ns;
                if (!namespaces.TryGetValue(type.Namespace, out ns))
                {
                    ns = new NamespaceTreeNode(type.Namespace);
                    namespaces[type.Namespace] = ns;
                }
                TypeTreeNode node = new TypeTreeNode(type, assemblyNode);
                typeDict[type] = node;
                ns.Children.Add(node);
            }
            foreach (NamespaceTreeNode ns in namespaces.Values.OrderBy(n => n.Name))
            {
                if (ns.Children.Count > 0)
                    this.Children.Add(ns);
            }
        }

        /// <summary>
        /// Finds the node for a top-level type.
        /// </summary>
        public TypeTreeNode FindTypeNode(TypeDefinition def)
        {
            if (def == null)
                return null;
            EnsureLazyChildren();
            TypeTreeNode node;
            if (typeDict.TryGetValue(def, out node))
                return node;
            else
                return null;
        }

        /// <summary>
        /// Finds the node for a namespace.
        /// </summary>
        public NamespaceTreeNode FindNamespaceNode(string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
                return null;
            EnsureLazyChildren();
            NamespaceTreeNode node;
            if (namespaces.TryGetValue(namespaceName, out node))
                return node;
            else
                return null;
        }

        #endregion

        public override void Decompile(Language language, Decompiler.ITextOutput output, DecompilationOptions options)
        {
            language.WriteCommentLine(output, this.Text.ToString());
        }
    }
}
