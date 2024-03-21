// api/pkg/analyzer/CSharpParser/Program.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;

public class Program
{
    public class Node
    {
        public string ?ID { get; set; }
        public string ?Label { get; set; }
        public string ?Type { get; set; }
        public string ?FilePath { get; set; }
        public string Language { get; set; } = "C#";
    }

    public class Edge
    {
        public string ?Source { get; set; }
        public string ?Target { get; set; }
        public string ?Label { get; set; }
    }

    public class Graph
    {
        public List<Node> Nodes { get; set; } = new List<Node>();
        public List<Edge> Edges { get; set; } = new List<Edge>();

        public void AddNode(Node node)
        {
            if (!Nodes.Any(n => n.ID == node.ID))
            {
                Nodes.Add(node);
            }
        }

        public void AddEdge(Edge edge)
        {
            var existingEdge = Edges.FirstOrDefault(e => e.Source == edge.Source && e.Target == edge.Target && e.Label == edge.Label);
            if (existingEdge == null)
            {
                Edges.Add(edge);
            }
        }
    }

    private static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Directory not provided.");
            return;
        }

        string directoryPath = args[0];
        Graph graph = new Graph();

        ProcessDirectory(directoryPath, graph);

        string jsonOutput = JsonConvert.SerializeObject(graph, Formatting.Indented);
        Console.WriteLine(jsonOutput);
    }

    private static void ProcessDirectory(string directoryPath, Graph graph)
    {
        var csFiles = Directory.GetFiles(directoryPath, "*.cs", SearchOption.AllDirectories);
        foreach (var file in csFiles)
        {
            ParseCSharpFile(file, graph);
        }
    }

    private static void ParseCSharpFile(string filePath, Graph graph)
    {
        var code = File.ReadAllText(filePath);
        var tree = CSharpSyntaxTree.ParseText(code);
        var root = (CompilationUnitSyntax)tree.GetRoot();

        var namespaceDeclarations = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
        foreach (var ns in namespaceDeclarations)
        {
            foreach (var member in ns.Members)
            {
                ParseMember(member, graph, ns.Name.ToString(), filePath);
            }
        }
    }

    private static void ParseMember(SyntaxNode member, Graph graph, string namespaceName, string filePath)
    {
        switch (member)
        {
            case ClassDeclarationSyntax classDecl:
                AddClassNodeAndMembers(classDecl, graph, namespaceName, filePath);
                break;
            case InterfaceDeclarationSyntax interfaceDecl:
                AddInterfaceNodeAndMembers(interfaceDecl, graph, namespaceName, filePath);
                break;
            case EnumDeclarationSyntax enumDecl:
                AddEnumNodeAndMembers(enumDecl, graph, namespaceName, filePath);
                break;
        }
    }

    private static void AddClassNodeAndMembers(ClassDeclarationSyntax classDecl, Graph graph, string namespaceName, string filePath)
    {
        var classNodeID = $"{namespaceName}.{classDecl.Identifier.Text}:{Path.GetFileNameWithoutExtension(filePath)}";
        graph.AddNode(new Node { ID = classNodeID, Label = classDecl.Identifier.Text, Type = "class", FilePath = filePath });

        foreach (var member in classDecl.Members)
        {
            switch (member)
            {
                case MethodDeclarationSyntax methodDecl:
                    var methodNodeID = $"{classNodeID}.{methodDecl.Identifier.Text}";
                    graph.AddNode(new Node { ID = methodNodeID, Label = methodDecl.Identifier.Text, Type = "method", FilePath = filePath });
                    graph.AddEdge(new Edge { Source = classNodeID, Target = methodNodeID, Label = "contains" });
                    break;
                case PropertyDeclarationSyntax propertyDecl:
                    var propertyNodeID = $"{classNodeID}.{propertyDecl.Identifier.Text}";
                    graph.AddNode(new Node { ID = propertyNodeID, Label = propertyDecl.Identifier.Text, Type = "property", FilePath = filePath });
                    graph.AddEdge(new Edge { Source = classNodeID, Target = propertyNodeID, Label = "contains" });
                    break;
            }
        }
    }

    private static void AddInterfaceNodeAndMembers(InterfaceDeclarationSyntax interfaceDecl, Graph graph, string namespaceName, string filePath)
    {
        var interfaceNodeID = $"{namespaceName}.{interfaceDecl.Identifier.Text}:{Path.GetFileNameWithoutExtension(filePath)}";
        graph.AddNode(new Node { ID = interfaceNodeID, Label = interfaceDecl.Identifier.Text, Type = "interface", FilePath = filePath });

        foreach (var member in interfaceDecl.Members)
        {
            switch (member)
            {
                case MethodDeclarationSyntax methodDecl:
                    var methodNodeID = $"{interfaceNodeID}.{methodDecl.Identifier.Text}";
                    graph.AddNode(new Node { ID = methodNodeID, Label = methodDecl.Identifier.Text, Type = "method", FilePath = filePath });
                    graph.AddEdge(new Edge { Source = interfaceNodeID, Target = methodNodeID, Label = "contains_method" });
                    break;
                case PropertyDeclarationSyntax propertyDecl:
                    var propertyNodeID = $"{interfaceNodeID}.{propertyDecl.Identifier.Text}";
                    graph.AddNode(new Node { ID = propertyNodeID, Label = propertyDecl.Identifier.Text, Type = "property", FilePath = filePath });
                    graph.AddEdge(new Edge { Source = interfaceNodeID, Target = propertyNodeID, Label = "contains_property" });
                    break;
            }
        }
    }

    private static void AddEnumNodeAndMembers(EnumDeclarationSyntax enumDecl, Graph graph, string namespaceName, string filePath)
    {
        var enumNodeID = $"{namespaceName}.{enumDecl.Identifier.Text}:{Path.GetFileNameWithoutExtension(filePath)}";
        graph.AddNode(new Node { ID = enumNodeID, Label = enumDecl.Identifier.Text, Type = "enum", FilePath = filePath });

        foreach (var member in enumDecl.Members)
        {
            var memberNodeID = $"{enumNodeID}.{member.Identifier.Text}";
            graph.AddNode(new Node { ID = memberNodeID, Label = member.Identifier.Text, Type = "enumMember", FilePath = filePath });
            graph.AddEdge(new Edge { Source = enumNodeID, Target = memberNodeID, Label = "contains" });
        }
    }
}
