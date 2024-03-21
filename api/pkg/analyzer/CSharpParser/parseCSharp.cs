// // 

// using System;
// using System.IO;
// using System.Linq;
// using Microsoft.CodeAnalysis;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Syntax;
// using System.Collections.Generic;
// using Newtonsoft.Json;

// public class Program
// {
//     public class Node
//     {
//         public string ID { get; set; }
//         public string Type { get; set; }
//     }

//     public class Edge
//     {
//         public string Source { get; set; }
//         public string Target { get; set; }
//     }

//     public class Graph
//     {
//         public List<Node> Nodes { get; set; } = new List<Node>();
//         public List<Edge> Edges { get; set; } = new List<Edge>();

//         public void AddNode(Node node)
//         {
//             if (!Nodes.Any(n => n.ID == node.ID))
//             {
//                 Nodes.Add(node);
//                 Console.WriteLine($"Added node: {node.ID} of type {node.Type}");
//             }
//         }

//         public void AddEdge(Edge edge)
//         {
//             if (!Edges.Any(e => e.Source == edge.Source && e.Target == edge.Target))
//             {
//                 Edges.Add(edge);
//                 Console.WriteLine($"Added edge from {edge.Source} to {edge.Target}");
//             }
//         }
//     }

//     public static void Main(string[] args)
//     {
//         Console.WriteLine("Starting C# file parsing...");
//         if (args.Length == 0)
//         {
//             Console.WriteLine("No directory provided");
//             return;
//         }
//         string directory = args[0];
//         var graph = new Graph();
//         Console.WriteLine($"Parsing C# files in directory: {directory}");
//         ParseCSharpFiles(directory, graph);
//         Console.WriteLine("Parsing completed. Generating JSON output...");
//         Console.WriteLine(JsonConvert.SerializeObject(graph, Formatting.Indented));
//     }

//     public static void ParseCSharpFiles(string directory, Graph graph)
//     {
//         Console.WriteLine($"Enumerating .cs files in {directory}");
//         foreach (var file in Directory.EnumerateFiles(directory, "*.cs", SearchOption.AllDirectories))
//         {
//             Console.WriteLine($"Processing file: {file}");
//             var code = File.ReadAllText(file);
//             var syntaxTree = CSharpSyntaxTree.ParseText(code);
//             var root = syntaxTree.GetRoot();

//             var namespaceNodes = root.DescendantNodes().OfType<NamespaceDeclarationSyntax>();
//             foreach (var namespaceNode in namespaceNodes)
//             {
//                 string namespaceID = namespaceNode.Name.ToString();
//                 graph.AddNode(new Node { ID = namespaceID, Type = "namespace" });
//                 Console.WriteLine($"Added namespace node: {namespaceID}");

//                 var classNodes = namespaceNode.DescendantNodes().OfType<ClassDeclarationSyntax>();
//                 foreach (var classNode in classNodes)
//                 {
//                     string classID = namespaceID + "." + classNode.Identifier.ValueText;
//                     graph.AddNode(new Node { ID = classID, Type = "class" });
//                     graph.AddEdge(new Edge { Source = namespaceID, Target = classID });
//                     Console.WriteLine($"Added class node: {classID}, with edge from {namespaceID}");

//                     var methodNodes = classNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
//                     foreach (var methodNode in methodNodes)
//                     {
//                         string methodID = classID + "." + methodNode.Identifier.ValueText;
//                         graph.AddNode(new Node { ID = methodID, Type = "method" });
//                         graph.AddEdge(new Edge { Source = classID, Target = methodID });
//                         Console.WriteLine($"Added method node: {methodID}, with edge from {classID}");
//                     }
//                 }

//                 var interfaceNodes = namespaceNode.DescendantNodes().OfType<InterfaceDeclarationSyntax>();
//                 foreach (var interfaceNode in interfaceNodes)
//                 {
//                     string interfaceID = namespaceID + "." + interfaceNode.Identifier.ValueText;
//                     graph.AddNode(new Node { ID = interfaceID, Type = "interface" });
//                     graph.AddEdge(new Edge { Source = namespaceID, Target = interfaceID });
//                     Console.WriteLine($"Added interface node: {interfaceID}, with edge from {namespaceID}");

//                     var methodNodes = interfaceNode.DescendantNodes().OfType<MethodDeclarationSyntax>();
//                     foreach (var methodNode in methodNodes)
//                     {
//                         string methodID = interfaceID + "." + methodNode.Identifier.ValueText;
//                         graph.AddNode(new Node { ID = methodID, Type = "method" });
//                         graph.AddEdge(new Edge { Source = interfaceID, Target = methodID });
//                         Console.WriteLine($"Added method node: {methodID}, with edge from {interfaceID}");
//                     }
//                 }
//             }
//         }
//     }
// }
