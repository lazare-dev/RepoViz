// api/pkg/repository/cloneRepo.go
package repository

import (
	"log"
	"os"

	"github.com/go-git/go-git/v5"
	"github.com/go-git/go-git/v5/plumbing/transport/http"
)

func CloneRepository(url, directory, username, token string) error {
	// Check if the repository already exists
	if _, err := os.Stat(directory + "/.git"); err == nil {
		log.Printf("Repository already exists in %s, skipping clone.\n", directory)
		return nil // Return early if repository already cloned
	} else if !os.IsNotExist(err) {
		// An error other than "not exists", like a permission issue
		log.Printf("Error checking if repository exists: %v\n", err)
		return err
	}

	log.Printf("Starting clone operation for URL: %s into directory: %s\n", url, directory)

	cloneOptions := &git.CloneOptions{
		URL:      url,
		Progress: os.Stdout,
	}

	// Set authentication if username and token are provided
	if username != "" && token != "" {
		log.Println("Authentication provided, setting up basic auth")
		cloneOptions.Auth = &http.BasicAuth{
			Username: username, // Correctly use variables
			Password: token,
		}
	} else {
		log.Println("No authentication provided, proceeding without auth")
	}

	_, err := git.PlainClone(directory, false, cloneOptions)
	if err != nil {
		log.Printf("Error occurred during clone: %v\n", err)
		return err
	}

	log.Println("Clone operation completed successfully")
	return nil
}
