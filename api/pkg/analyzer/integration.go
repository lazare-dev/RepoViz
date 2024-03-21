// api/pkg/analyzer/integration.go

package analyzer

import (
	"bytes"
	"encoding/json"
	"log"
	"os/exec"
	"strconv"
)

func ExecuteExternalParser(parserCommand string, args []string) (*Graph, error) {
	cmd := exec.Command(parserCommand, args...)
	var stderr bytes.Buffer
	cmd.Stderr = &stderr
	output, err := cmd.Output()
	if err != nil {
		log.Printf("Error executing %s: %v, stderr: %s\n", parserCommand, err, stderr.String())
		return nil, err
	}

	var graph Graph
	err = json.Unmarshal(output, &graph)
	if err != nil {
		log.Printf("Error unmarshalling output from %s: %v\n", parserCommand, err)
		return nil, err
	}

	return &graph, nil
}

func IntegrationWorkflow(directory string) {
	log.Println("Starting integration workflow...")

	finalGraph := NewGraph()

	// Adjust these paths as per your actual project structure
	log.Println("Parsing Go files...")
	if err := ParseGoFiles(directory, finalGraph); err != nil {
		log.Fatalf("Failed to parse Go files: %v\n", err)
	}

	log.Println("Parsing JavaScript files...")
	jsGraph, err := ExecuteExternalParser("node", []string{"/Users/andrewlazare/Projects/golang/RepoViz/api/pkg/analyzer/parseJS.js", directory}) // Adjusted path
	if err != nil {
		log.Fatalf("Failed to execute JavaScript parser: %v\n", err)
	}
	finalGraph.Integrate(jsGraph)

	log.Println("Parsing C# files...")
	csGraph, err := ExecuteExternalParser("dotnet", []string{"/Users/andrewlazare/Projects/golang/RepoViz/api/pkg/analyzer/CSharpParser/bin/Release/net8.0/CSharpParser.dll", directory}) // Adjusted path
	if err != nil {
		log.Fatalf("Failed to execute C# parser: %v\n", err)
	}
	finalGraph.Integrate(csGraph)

	makeIDsUnique(finalGraph)

	log.Println("Saving final integrated graph...")
	if err = finalGraph.SaveToFile("/Users/andrewlazare/Projects/golang/RepoViz/web/repoviz/src/final_graph.json"); err != nil { // Adjusted path
		log.Fatalf("Failed to save final graph: %v\n", err)
	}

	log.Println("Integration workflow completed successfully.")
}

func makeIDsUnique(graph *Graph) {
	idMap := make(map[string]int)

	for i, node := range graph.Nodes {
		if _, exists := idMap[node.ID]; exists {
			idMap[node.ID]++
			graph.Nodes[i].ID = node.ID + "_" + strconv.Itoa(idMap[node.ID])
		} else {
			idMap[node.ID] = 0
		}
	}

	uniqueEdges := make(map[string]bool)
	newEdges := []Edge{}

	for _, edge := range graph.Edges {
		edgeID := edge.Source + "_" + edge.Target
		if !uniqueEdges[edgeID] {
			uniqueEdges[edgeID] = true
			newEdges = append(newEdges, edge)
		}
	}
	graph.Edges = newEdges
}
