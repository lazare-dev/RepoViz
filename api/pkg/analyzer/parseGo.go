// api/pkg/analyzer/parseGo.go

package analyzer

import (
	"go/ast"
	"go/parser"
	"go/token"
	"log"
	"os"
	"path/filepath"
	"strings"
)

func ParseGoFiles(directory string, graph *Graph) error {
	log.Printf("Starting to parse Go files in directory: %s\n", directory)
	fset := token.NewFileSet()

	err := filepath.Walk(directory, func(path string, info os.FileInfo, err error) error {
		if err != nil {
			log.Printf("Error accessing path %q: %v\n", path, err)
			return err
		}
		if info.IsDir() || filepath.Ext(path) != ".go" {
			return nil // Skip directories and non-Go files
		}

		fileNodeID := strings.TrimPrefix(path, directory+"/")
		graph.AddNode(Node{ID: fileNodeID, Type: "file"})
		log.Printf("Added file node: %s\n", fileNodeID)

		node, err := parser.ParseFile(fset, path, nil, parser.ParseComments)
		if err != nil {
			log.Printf("Error parsing Go file %q: %v\n", path, err)
			return err
		}

		ast.Inspect(node, func(n ast.Node) bool {
			switch x := n.(type) {
			case *ast.FuncDecl:
				funcID := fileNodeID + ":" + x.Name.Name
				graph.AddNode(Node{ID: funcID, Type: "function"})
				graph.AddEdge(Edge{Source: fileNodeID, Target: funcID})
				log.Printf("Added function node and edge for %q\n", funcID)
			case *ast.GenDecl:
				if x.Tok == token.TYPE {
					for _, spec := range x.Specs {
						ts, ok := spec.(*ast.TypeSpec)
						if !ok {
							continue
						}
						typeName := ts.Name.Name
						typeID := fileNodeID + ":" + typeName
						graph.AddNode(Node{ID: typeID, Type: "type"})
						graph.AddEdge(Edge{Source: fileNodeID, Target: typeID})
						log.Printf("Added type node and edge for %q\n", typeID)
					}
				}
			}
			return true
		})

		return nil
	})

	if err != nil {
		log.Printf("Failed to complete parsing Go files: %v\n", err)
		return err
	}

	log.Println("Completed parsing Go files.")
	return nil
}
