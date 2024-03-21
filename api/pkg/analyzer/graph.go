// api/pkg/analyzer/graph.go

package analyzer

import (
	"encoding/json"
	"log"
	"os"
)

type Node struct {
	ID   string `json:"id"`
	Type string `json:"type"`
}

type Edge struct {
	Source string `json:"source"`
	Target string `json:"target"`
}

type Graph struct {
	Nodes []Node `json:"nodes"`
	Edges []Edge `json:"edges"`
}

func NewGraph() *Graph {
	log.Println("Creating a new graph")
	return &Graph{}
}

func (g *Graph) AddNode(node Node) {
	log.Printf("Adding node: ID=%s, Type=%s\n", node.ID, node.Type)
	g.Nodes = append(g.Nodes, node)
}

func (g *Graph) AddEdge(edge Edge) {
	log.Printf("Adding edge: Source=%s, Target=%s\n", edge.Source, edge.Target)
	g.Edges = append(g.Edges, edge)
}

func (g *Graph) Integrate(other *Graph) {
	log.Printf("Integrating another graph into the current one. Nodes before: %d, Edges before: %d\n", len(g.Nodes), len(g.Edges))
	g.Nodes = append(g.Nodes, other.Nodes...)
	g.Edges = append(g.Edges, other.Edges...)
	log.Printf("Integration complete. Nodes after: %d, Edges after: %d\n", len(g.Nodes), len(g.Edges))
}

func (g *Graph) SaveToFile(filename string) error {
	log.Printf("Saving graph to file: %s\n", filename)
	file, err := json.MarshalIndent(g, "", " ")
	if err != nil {
		log.Printf("Error marshalling graph to JSON: %v\n", err)
		return err
	}

	err = os.WriteFile(filename, file, 0644)
	if err != nil {
		log.Printf("Error writing graph to file: %s, Error: %v\n", filename, err)
		return err
	}

	log.Printf("Graph successfully saved to file: %s\n", filename)
	return nil
}
