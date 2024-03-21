// api/cmd/repoviz/main.go

package main

import (
	"log"
	"repoviz/api/pkg/analyzer"
	"repoviz/api/pkg/repository"
)

func main() {
	log.Println("Starting RepoViz execution...")

	url := ""                     // Updated URL
	directory := "./clonedRepo"   // Ensure this directory can be created or already exists but is empty
	username := "AndrewLazare443" // Use your actual username or an empty string
	token := ""                   // Use your actual token or an empty string

	log.Printf("Cloning repository from URL: %s into directory: %s\n", url, directory)
	err := repository.CloneRepository(url, directory, username, token) // Pass username and token variables correctly
	if err != nil {
		log.Fatalf("Error cloning repository: %v", err)
	} else {
		log.Println("Repository successfully cloned.")
	}

	log.Printf("Detecting languages in repository located at: %s\n", directory)
	languages, err := analyzer.DetectLanguagesInRepo(directory)
	if err != nil {
		log.Fatalf("Error detecting languages: %v", err)
	} else {
		log.Printf("Detected languages: %v\n", languages)
	}

	log.Println("Starting integration workflow...")
	analyzer.IntegrationWorkflow(directory)
	log.Println("Integration workflow completed successfully.")

	log.Println("RepoViz execution completed.")
}
